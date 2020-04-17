using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Blog.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Blog.Core.Data
{
    public class EfRepository<T> : IRepository<T> where T : Entity
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> _entities;

        public EfRepository(ApplicationDbContext context)
        {
            _context = context;
            _entities = _context.Set<T>();
        }
        public void Add(T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (item.CreatedAtUtc == Entity.EntityInitialDate)
            {
                item.CreatedAtUtc = DateTime.Now;
            }

            _entities.Add(item);
        }

        public void AddRange(IEnumerable<T> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            foreach (var item in items)
            {
                Add(item);
            }
        }

        public void Delete(T item)
        {
            if (item == null)
            {
                throw new ArgumentException(nameof(item));
            }

            _entities.Remove(item);
        }

        public void DeleteRange(IEnumerable<T> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            foreach (var item in items)
            {
                Delete(item);
            }
        }

        public void Update(T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (item.ModifiedAtUtc == Entity.EntityInitialDate)
            {
                item.ModifiedAtUtc = DateTime.Now;
            }

            _entities.Update(item);
        }

        public void UpdateRange(IEnumerable<T> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            foreach (var item in items)
            {
                Update(item);
            }
        }

        public IQueryable<T> All()
        {
            return _entities;
        }

        public T Get(long id)
        {
            return _entities.Find(id);
        }

        public bool Save()
        {
            return _context.SaveChanges() > 0;
        }
    }
}
