using FluentNHibernate.Mapping;
using Example.Core.Model;

namespace Web.Persistence.DataMapping
{
    public class UserMap : ClassMap<User>
    {
        public UserMap()
        {
            Table("UserTable");
            Id(x => x.Id);
            Map(x => x.UserName);
            Map(x => x.FirstName);
            Map(x => x.LastName);            
            Map(x => x.Email);
            Map(x => x.CreatedOn);
            Map(x => x.LastLogin);
            HasManyToMany(x => x.Roles)
               .Cascade.All()
               .Table("UserRoles");
        }
    }
}