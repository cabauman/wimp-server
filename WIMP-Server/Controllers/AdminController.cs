using System;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WIMP_Server.Data.Users;
using WIMP_Server.Dtos.Users;
using WIMP_Server.Models.Users;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using WIMP_Server.Dtos.Admin;
using WIMP_Server.Auth.Policies;
using WIMP_Server.Auth.Roles;

namespace WIMP_Server.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = Policy.OnlyAdmins)]
public class AdminController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IMapper _mapper;
    private readonly ILogger<AdminController> _logger;

    public AdminController(IUserRepository userRepository, UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IMapper mapper, ILogger<AdminController> logger)
    {
        _userRepository = userRepository;
        _userManager = userManager;
        _roleManager = roleManager;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpPost("invite")]
    public ActionResult<ReadInvitationKeyDto> GenerateInvitationKey()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var invitationKey = new InvitationKey
        {
            GeneratedByUserId = userId,
            ExpiresAt = DateTime.Now.AddMinutes(30),
            Key = Guid.NewGuid().ToString(),
        };

        _userRepository.CreateInvitationKey(invitationKey);
        _userRepository.SaveChanges();

        return Ok(_mapper.Map<ReadInvitationKeyDto>(invitationKey));
    }

    [HttpGet("invite/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public ActionResult<ReadInvitationKeyDto> GetInvitationKey(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAdmin = User.IsInRole("Admin");

        var invitationKey = _userRepository.GetInvitationKeyWithId(id);

        if (invitationKey == null)
        {
            return NotFound($"No invitations found with ID: {id}");
        }

        if (invitationKey.GeneratedByUserId != userId && !isAdmin)
        {
            return NotFound($"No invitations found with ID: {id}");
        }

        return Ok(_mapper.Map<ReadInvitationKeyDto>(invitationKey));
    }

    [HttpGet("invite")]
    public ActionResult<IEnumerable<ReadInvitationKeyDto>> GetInvitationKeys()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAdmin = User.IsInRole(Role.Admin);

        var invitationKeys = _userRepository.GetAllInvitationKeys();

        if (!isAdmin)
        {
            invitationKeys = invitationKeys.Where(ik => ik.GeneratedByUserId == userId);
        }

        return Ok(_mapper.Map<IEnumerable<ReadInvitationKeyDto>>(invitationKeys));
    }

    [HttpGet("users", Name = "GetUserList")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ReadUsersDto>> GetUserList([FromQuery] int page)
    {
        var users = _userRepository.GetUsers();

        const int perPage = 10;
        var total = users.Count();
        var totalPages = (int)Math.Ceiling((double)total / perPage);

        var paginatedUsers = users
            .Skip(page * perPage)
            .Take(perPage);

        var readUsers = new List<ReadUserDto>();
        foreach (var user in paginatedUsers)
        {
            var readUser = _mapper.Map<ReadUserDto>(user);
            readUser.Roles = await _userManager.GetRolesAsync(user)
                .ConfigureAwait(true);

            readUsers.Add(readUser);
        }

        var result = new ReadUsersDto
        {
            Page = page,
            PerPage = perPage,
            TotalPages = totalPages,
            Total = total,
            Users = readUsers
        };

        return Ok(result);
    }

    [HttpDelete("users/delete", Name = "DeleteUserWithId")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteUserWithId([FromQuery] string userId)
    {
        var myUserId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
        if (myUserId.Value == userId)
        {
            return Forbid("Can't delete own user");
        }

        var user = await _userManager.FindByIdAsync(userId)
            .ConfigureAwait(true);

        if (user == null)
        {
            return NotFound($"Couldn't find user with id: {nameof(userId)}");
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

    [HttpPost("users/role", Name = "ChangeRole")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> ChangeRole([FromBody] ChangeUserRoleDto changeUserRoleDto)
    {
        var user = await _userManager.FindByIdAsync(changeUserRoleDto.UserId)
            .ConfigureAwait(true);

        if (user == null)
        {
            return NotFound($"Couldn't find user with id: {nameof(changeUserRoleDto.UserId)}");
        }

        var role = await _roleManager.FindByNameAsync(changeUserRoleDto.NewRole)
            .ConfigureAwait(true);
        if (role == null)
        {
            return NotFound($"Couldn't find role: {changeUserRoleDto.NewRole}");
        }

        var currentUserRoles = await _userManager.GetRolesAsync(user)
            .ConfigureAwait(true);

        await _userManager.RemoveFromRolesAsync(user, currentUserRoles)
            .ConfigureAwait(true);

        var result = await _userManager.AddToRoleAsync(user, changeUserRoleDto.NewRole)
            .ConfigureAwait(true);
        if (!result.Succeeded)
        {
            return UnprocessableEntity(string.Join('\n', result.Errors.Select(e => e.Description)));
        }

        _logger.LogInformation($"Updated users role: {user.Id}, {changeUserRoleDto.NewRole}");

        return Ok();
    }
}
