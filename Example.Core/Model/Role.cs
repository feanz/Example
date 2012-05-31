using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Example.Core.Model
{
    public class Role
    {
        [Key]
        public virtual int Id { get; set; }

        [Required]
        public virtual string RoleName { get; set; }

        public virtual IList<User> Users { get; set; }

        public Role()
        {
            Users = new List<User>();
        }

        public override string ToString()
        {
            return RoleName.ToString();
        }
    }
}