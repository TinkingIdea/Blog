using System;
using System.Collections.Generic;
using System.Text;
using Blog.Core.Enums;

namespace Blog.Core.Entities
{
    public class AdminUser: Entity
    {
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string HashedPassword { get; set; }
        public CustomerEnums.Status Status { get; set; }
    }
}
