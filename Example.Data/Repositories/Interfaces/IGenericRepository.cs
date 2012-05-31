using System;
using System.Collections.Generic;

namespace Example.Data.Repositories.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        bool InsertOrUpdate(IEnumerable<T> items);
        bool InsertOrUpdate(T item);
        bool Delete(IEnumerable<T> items);
        bool Delete(T item);
        bool Delete(int id);
        IEnumerable<T> GetBy(System.Linq.Expressions.Expression<Func<T, bool>> expression);
        IEnumerable<T> GetAll();
        IEnumerable<T> GetPaged(int rows, int page, string sortExpression, string sortDirection);
        int GetCount();
        T GetById(int id);
    }
}
