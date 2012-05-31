using FluentNHibernate.Mapping;
using Example.Core.Model;

namespace Web.Persistence.DataMapping
{
    public class RoleMap : ClassMap<Role>
    {
        public RoleMap()
        {
            Id(x => x.Id);
            Map(x => x.RoleName);
            HasManyToMany(x => x.Users)
                .Cascade.All()
                .Inverse()
                .Table("UserRoles");
        }
    }
}