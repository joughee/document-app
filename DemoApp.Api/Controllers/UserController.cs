using DemoApp.Api.Models;
using DemoApp.Api.Services.Interfaces;
using DemoApp.Core.Entities;
using DemoApp.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static DemoApp.Api.Statics;

namespace DemoApp.Api.Controllers
{
    [Authorize(Roles = Roles.Administrator)]
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("authenticate")]
        [AllowAnonymous]
        public async Task<AuthenticateResponse> Authenticate([FromBody] AuthenticateRequest authRequest)
        {
            return await _userService.Authenticate(authRequest);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var user = await _userService.GetUserById(id);

            return Ok(user);
        }
        
        [HttpGet("{username}")]
        public async Task<IActionResult> Get(string username)
        {
            var user = await _userService.GetUserByUserName(username);

            return Ok(user);
        }
        
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] User user)
        {
            var userId = await _userService.CreateUser(user);

            return Ok(userId);
        }
        
        [HttpPut]
        public async Task<IActionResult> Put([FromBody] User user)
        {
            var success = await _userService.UpdateUser(user);

            return Ok(success);
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _userService.DeleteUser(id);

            return Ok(success);
        }
        
        [HttpDelete("{userId, role}")]
        public async Task<IActionResult> DeleteUserRole(int userId, string role)
        {
            var success = await _userService.RemoveUserFromRole(userId, role);

            return Ok(success);
        }
    }
}
