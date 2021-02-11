using DemoApp.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoApp.Api.Models
{
    public class AuthenticateResponse
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Token { get; set; }
        public AuthenticateResponse(User user, string token)
        {
            Id = user.Id;
            Username = user.EmailAddress;
            Token = token;
        }
    }
}
