using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DemoApp.Infrastructure.DTOs
{
    public class DocumentDTO
    {
        public int Id { get; set; }
        [MaxLength(500)]
        public string Name { get; set; }
        public string Description { get; set; }
        public string[] Categories { get; set; }
    }
}
