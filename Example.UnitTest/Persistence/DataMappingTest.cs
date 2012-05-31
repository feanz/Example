using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Web.Persistence;
using NHibernate;
using System.Collections;
using Example.Data;

namespace Example.UnitTest.Persistence
{
    [TestClass]
    public class DataMappingTest
    {
        /// <summary>
        /// A simple test that issues a select on all the tables in the current mapping if any fail the database is not in an expected satte
        /// </summary>
        [TestMethod]
        public void AllNHibernateMappingAreOkay()
        {
            var sessionFactory = NHibernateConfiguration.BuildSessionFactory();
            using (ISession session = sessionFactory.OpenSession())
            {
                var allClassMetadata = sessionFactory.GetAllClassMetadata();
                foreach (var entry in allClassMetadata)
                {
                    session.CreateCriteria(entry.Key)
                         .SetMaxResults(0).List();
                }
            }
        }
    }
}
