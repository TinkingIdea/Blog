using System;
using System.Collections.Generic;
using System.Text;

namespace Blog.Core.Data
{
    public interface IUnitOfWork
    {
        bool Save();
    }
}
