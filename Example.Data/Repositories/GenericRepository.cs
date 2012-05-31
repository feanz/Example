using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Example.Data.Repositories.Interfaces;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using Ninject;
using Utilities.Extensions;

namespace Example.Data.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {  
        [Inject]
        public ISession Session { get; set; }

        public GenericRepository(ISession session)
        {
            Session = session;
        }

        public virtual T GetById(int id)
        {
            return Session.Get<T>(id);
        }

        public virtual IEnumerable<T> GetBy(System.Linq.Expressions.Expression<System.Func<T, bool>> expression)
        {
            return Session.Query<T>().Where(expression).ToList();
        }

        public virtual IEnumerable<T> GetAll()
        {
            return Session.CreateCriteria<T>().List<T>();            
        }

        public virtual IEnumerable<T> GetPaged(int rows, int page, string sortExpression, string sortDirection)
        {
            var results = Session.CreateCriteria(typeof(T))
                   .SetFirstResult((page - 1) * rows)
                   .SetMaxResults(page * rows);

            if (sortDirection.IsNotNull())
            results.AddOrder(sortDirection.ToUpperInvariant().Equals("ASC")
                                 ? Order.Asc(sortExpression)
                                 : Order.Desc(sortExpression));

            return results.List<T>();
        }

        public virtual int GetCount()
        {
            return Session.Query<T>().Count();
        }

        public virtual bool InsertOrUpdate(T item)
        {
            Session.SaveOrUpdate(item);
            return true;
        }

        public virtual bool InsertOrUpdate(IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                Session.SaveOrUpdate(item);
            }
            return true;
        }

        public virtual bool Delete(T item)
        {
            Session.Delete(item);
            return true;
        }

        public virtual bool Delete(int id)
        {
            var queryString = string.Format(CultureInfo.CurrentCulture, "delete {0} where id = :id",
                                         typeof(T));
            Session.CreateQuery(queryString)
                   .SetParameter("id", id)
                   .ExecuteUpdate();
            return true;
        }

        public virtual bool Delete(IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                Session.Delete(item);
            }
            return true;
        }
    }
}
