using System;
using System.Collections.Generic;
using System.Text;

namespace Blog.Core.Entities
{   
    public class Article: Entity
    {
        public string Title { get; set; }
        public AdminUser Author { get; set; }
        public Category Category { get; set; }
    }
}
