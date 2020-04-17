using System;
using System.Collections.Generic;
using System.Text;
using Blog.Core.Enums;

namespace Blog.Core.Entities
{
    public class Menu: Entity
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public int Order { get; set; }
        public int Level { get; set; }
        public string Icon { get; set; }
        public CustomerEnums.Status Status { get; set; }
    }
}
