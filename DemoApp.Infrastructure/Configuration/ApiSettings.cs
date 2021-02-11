using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoApp.Infrastructure.Configuration
{
    public class ApiSettings
    {
        public string AuthKey { get; set; }
        public int HashIterations { get; set; }
    }
}
