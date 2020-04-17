using System;
using System.Collections.Generic;
using System.Text;

namespace Blog.Core.Data
{
    public class UnitOfWork: IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }
        public bool Save()
        {
            return _context.SaveChanges() > 0;
        }
    }
}
