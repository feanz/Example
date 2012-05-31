using System;
using System.Configuration;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using Web.Persistence.DataMapping;

namespace Example.Data
{
    public static class NHibernateConfiguration
    {
        public static ISessionFactory BuildSessionFactory()
        {
            var cfg = OracleClientConfiguration.Oracle9
                .ConnectionString(c => c.Is(ConfigurationManager.ConnectionStrings["Database"].ToString()))
                .Driver<ProfiledClientDriver>();

            return Fluently.Configure()
                    .Database(cfg)
                    .Mappings(m => m.FluentMappings.AddFromAssemblyOf<UserMap>().ExportTo(@".\"))
                    .ExposeConfiguration(BuildSchema)
            .BuildSessionFactory();
        }

        private static void BuildSchema(NHibernate.Cfg.Configuration config)
        {
            // this NHibernate tool takes a configuration (with mapping info in)
            // and exports a database schema from it.  You can edit the directory location to save migration files to another location            
            Action<string> updateExport = x =>
            {
                //var directory = @"C:\Migrations\" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
                
                //if(!Directory.Exists(directory))
                //    Directory.CreateDirectory(directory);

                //var path = directory + DateTime.Now.ToMigrationString() + ".sql";

                //using (var file = new FileStream(path, FileMode.Append, FileAccess.Write))
                //using (var sw = new StreamWriter(file))
                //{
                //    sw.Write(x);
                //    sw.Close();
                //}
            };

            new SchemaUpdate(config)
              .Execute(updateExport, false);
        }
    }
}