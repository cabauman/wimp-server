using System;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WIMP_Server.Data.Users;
using WIMP_Server.Dtos.Users;
using WIMP_Server.Models.Users;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Collections.Generic;

namespace WIMP_Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin,Manager")]
    public class AdminController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public AdminController(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
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
            var isAdmin = User.IsInRole("Admin");

            var invitationKeys = _userRepository.GetAllInvitationKeys();

            if (!isAdmin)
            {
                invitationKeys = invitationKeys.Where(ik => ik.GeneratedByUserId == userId);
            }

            return Ok(_mapper.Map<IEnumerable<ReadInvitationKeyDto>>(invitationKeys));
        }
    }
}