using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blog.Core.Data
{
    public interface IRepository<T>
    {
        void Add(T item);
        void AddRange(IEnumerable<T> items);
        void Delete(T item);
        void DeleteRange(IEnumerable<T> items);
        void Update(T item);
        void UpdateRange(IEnumerable<T> items);
        IQueryable<T> All();
        T Get(long id);
        bool Save();
    }
}
