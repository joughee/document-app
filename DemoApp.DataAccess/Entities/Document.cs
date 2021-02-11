using System;
using System.Collections.Generic;
using System.Text;

namespace DemoApp.Core.Entities
{
    public class Document
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string[] Categories { get; set; }
        public int InsertUserId { get; set; }
        public DateTime InsertDateTime { get; set; }
        public int ModifyUserId { get; set; }
        public DateTime ModifyDateTime { get; set; }
    }
}
