using System;
using System.Collections.Generic;
using System.Text;
using Blog.Core.Enums;

namespace Blog.Core.Entities
{
    public class Category: Entity
    {
        public string Name { get; set; }
        public CustomerEnums.Status Status { get; set; }
    }
}
