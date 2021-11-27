using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WIMP_Server.Auth.Policies;
using WIMP_Server.Auth.Roles;
using WIMP_Server.Data.Auth;
using WIMP_Server.Data.Users;
using WIMP_Server.Dtos.Users;
using WIMP_Server.Models.Auth;
using WIMP_Server.Models.Users;
using WIMP_Server.Options;

namespace WIMP_Server.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = Policy.OnlyUsers)]
public class UserController : ControllerBase
{
    public UserController(IUserRepository userRepository, IApiKeyRepository apiKeyRepository, UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IOptions<JwtOptions> jwt, IMapper mapper, ILogger<UserController> logger)
    {
        _userRepository = userRepository;
        _apiKeyRepository = apiKeyRepository;
        _userManager = userManager;
        _roleManager = roleManager;
        _jwt = jwt.Value;
        _mapper = mapper;
        _logger = logger;
    }

    private readonly IUserRepository _userRepository;
    private readonly IApiKeyRepository _apiKeyRepository;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly JwtOptions _jwt;
    private readonly IMapper _mapper;
    private readonly ILogger<UserController> _logger;

    [HttpPost("register")]
    [ProducesResponseType(typeof(ReadUserDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
    [AllowAnonymous]
    public async Task<ActionResult<ReadUserDto>> Register([FromBody] RegisterUserDto registerUserDto)
    {
        var existingUser = await _userManager
            .FindByNameAsync(registerUserDto.Username)
            .ConfigureAwait(true);
        if (existingUser != null)
        {
            return Conflict($"User with {registerUserDto.Username} already exists");
        }

        if (registerUserDto.InviteKey == null)
        {
            return Unauthorized("Invalid invite key");
        }

        var invitationKey = _userRepository.FindInvitationKey(registerUserDto.InviteKey);
        if (invitationKey == null || invitationKey.ExpiresAt < DateTime.Now)
        {
            return Unauthorized("Invalid or expired invite key");
        }

        var user = _mapper.Map<User>(registerUserDto);
        var createUserResult = await _userManager
            .CreateAsync(user, registerUserDto.Password)
            .ConfigureAwait(true);
        if (!createUserResult.Succeeded)
        {
            return UnprocessableEntity(string.Join('\n', createUserResult.Errors.Select(e => e.Description)));
        }

        var addRoleResult = await _userManager.AddToRoleAsync(user, Role.User)
                .ConfigureAwait(true);
        if (!addRoleResult.Succeeded)
        {
            return UnprocessableEntity(string.Join('\n', addRoleResult.Errors.Select(e => e.Description)));
        }

        _logger.LogInformation($"Registered user with id {user.Id} and username {user.UserName}");

        var roles = await _userManager.GetRolesAsync(user)
            .ConfigureAwait(true);

        var readUserDto = _mapper.Map<ReadUserDto>(user);
        readUserDto.Roles = roles;

        return CreatedAtRoute(nameof(GetUserById), new { readUserDto.Id }, readUserDto);
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [AllowAnonymous]
    public async Task<ActionResult<ReadTokenDto>> Login([FromBody] LoginUserDto loginUserDto)
    {
        var user = await _userManager.FindByNameAsync(loginUserDto.Username)
            .ConfigureAwait(true);
        if (user != null && await _userManager.CheckPasswordAsync(user, loginUserDto.Password)
            .ConfigureAwait(true))
        {
            var userRoles = await _userManager.GetRolesAsync(user)
                .ConfigureAwait(true);

            var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

            authClaims.AddRange(userRoles.Select(r => new Claim(ClaimTypes.Role, r)));

            var signingKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwt.Secret));

            var expires = DateTime.Now.AddMinutes(_jwt.ExpiresAfterMinutes);

            var token = new JwtSecurityToken(
                issuer: _jwt.ValidIssuer,
                audience: _jwt.ValidAudience,
                expires: expires,
                claims: authClaims,
                signingCredentials: new SigningCredentials(signingKey,
                    SecurityAlgorithms.HmacSha256)
            );

            return Ok(new ReadTokenDto
            {
                Token = new JwtSecurityTokenHandler()
                    .WriteToken(token),
                Expiration = token.ValidTo
            });
        }

        return Unauthorized();
    }

    [HttpGet("{id}", Name = "GetUserById")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReadUserDto>> GetUserById(string id)
    {
        var user = await _userManager.FindByIdAsync(id)
            .ConfigureAwait(true);

        if (user == null)
        {
            return NotFound($"Couldn't find user with {nameof(id)}");
        }

        var roles = await _userManager.GetRolesAsync(user)
            .ConfigureAwait(true);

        var readUser = _mapper.Map<ReadUserDto>(user);
        readUser.Roles = roles;

        return Ok(readUser);
    }

    [HttpGet(Name = "GetUser")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReadUserDto>> GetUser()
    {
        var userIdentity = HttpContext.User.Identity;
        var user = await _userManager.FindByNameAsync(userIdentity.Name)
            .ConfigureAwait(true);

        if (user == null)
        {
            return NotFound("Couldn't find user.");
        }

        var roles = await _userManager.GetRolesAsync(user)
            .ConfigureAwait(true);

        var readUser = _mapper.Map<ReadUserDto>(user);
        readUser.Roles = roles;

        return Ok(readUser);
    }

    [HttpPost("changePassword", Name = "ChangePassword")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
    {
        var userIdentity = HttpContext.User.Identity;

        var user = await _userManager.FindByNameAsync(userIdentity.Name)
            .ConfigureAwait(true);

        if (user == null)
        {
            return NotFound("Couldn't find user");
        }

        var changePasswordResult = await _userManager.ChangePasswordAsync(user, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword)
            .ConfigureAwait(true);
        if (!changePasswordResult.Succeeded)
        {
            return UnprocessableEntity(string.Join('\n', changePasswordResult.Errors.Select(e => e.Description)));
        }

        return Ok();
    }

    [HttpDelete(Name = "DeleteUser")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteUser()
    {
        var userIdentity = HttpContext.User.Identity;

        var user = await _userManager.FindByNameAsync(userIdentity.Name)
            .ConfigureAwait(true);

        if (user == null)
        {
            return NotFound("Couldn't find user");
        }

        var result = await _userManager.DeleteAsync(user)
            .ConfigureAwait(true);
        if (!result.Succeeded)
        {
            return UnprocessableEntity(string.Join('\n', result.Errors.Select(e => e.Description)));
        }

        _logger.LogInformation($"Deleted user: {user.Id}");

        return Ok();
    }

    [HttpGet("key", Name = nameof(GetApiKeys))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<ActionResult<IEnumerable<ReadApiKeyDto>>> GetApiKeys()
    {
        var owner = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
        var apiKey = _apiKeyRepository.GetByOwner(owner);
        if (apiKey == null)
        {
            return NotFound();
        }

        return Ok(_mapper.Map<IEnumerable<ReadApiKeyDto>>(apiKey));
    }

    [HttpGet("key/{key}", Name = nameof(GetApiKey))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<ActionResult<ReadApiKeyDto>> GetApiKey(string key)
    {
        var owner = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
        var apiKey = _apiKeyRepository
            .GetByOwner(owner)
            .FirstOrDefault(k => k.Key == key);
        if (apiKey == null)
        {
            return NotFound();
        }

        return Ok(_mapper.Map<ReadApiKeyDto>(apiKey));
    }

    [HttpPost("key")]
    [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public ActionResult<ReadApiKeyDto> CreateApiKey()
    {
        var owner = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
        var existingApiKey = _apiKeyRepository.GetByOwner(owner).FirstOrDefault();
        if (existingApiKey != null)
        {
            return Conflict("User already have a generated API key");
        }

        var roles = new List<ApiKeyRole> {
                new ApiKeyRole { Role = Role.IntelReport }
            };

        var apiKey = new ApiKey
        {
            Created = DateTime.UtcNow,
            Key = Nanoid.Nanoid.Generate(),
            Owner = owner,
            Roles = roles,
        };

        _apiKeyRepository.Add(apiKey);
        _apiKeyRepository.Save();

        var readApiKeyDto = _mapper.Map<ReadApiKeyDto>(apiKey);

        return CreatedAtRoute(nameof(GetApiKey), new { readApiKeyDto.Key }, readApiKeyDto);
    }

    [HttpDelete("key/{key}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult DeleteApiKey(string key)
    {
        var owner = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
        var existingApiKey = _apiKeyRepository
            .GetByOwner(owner)
            .FirstOrDefault(apiKey => apiKey.Key == key);
        if (existingApiKey == null)
        {
            return NotFound();
        }

        _apiKeyRepository.Delete(existingApiKey);
        _apiKeyRepository.Save();

        return Ok();
    }
}
