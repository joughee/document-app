﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace DemoApp.Core.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        [JsonIgnore]
        public string PasswordHash { get; set; }
        public bool Active { get; set; }
        public string[] Roles { get; set; }
        public IEnumerable<UserGroup> Groups { get; set; }
    }
}
