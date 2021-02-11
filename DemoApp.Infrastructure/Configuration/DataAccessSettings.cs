using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoApp.Infrastructure.Configuration
{
    public class DataAccessSettings
    {
        public string DatabaseProvider { get; set; }
        public string MainDbConnectionString { get; set; }
        public string FileDbConnectionString { get; set; }
    }
}
