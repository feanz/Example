using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Utilities;

namespace Example.Core.Model
{
    public class User
    {        
        [Key]
        public virtual int Id { get; set; }

        [Required]
        [Display(Name = "User name")]
        //[Remote("IsUserNameAvailable", "Account", AdditionalFields = "Id", ErrorMessage = "User name is already taken")]
        public virtual string UserName { get; set; }

        [Required]
        [Display(Name = "First name")]
        [StringLength(255)]
        public virtual string FirstName { get; set; }

        [Required]
        [Display(Name = "Last name")]
        [StringLength(255)]
        public virtual string LastName { get; set; }

        [Required]        
        [Display(Name = "Email address")]
        [DataType(DataType.EmailAddress), RegularExpression(RegexPattern.EMAIL, ErrorMessage = "Not a valid email")]
        public virtual string Email { get; set; }

        public virtual string RoleList {
            get {
                return Roles.ToString();
            }
        }

        public virtual IList<Role> Roles { get; set; }        

        [Display(Name = "Created On")]
        public virtual DateTime CreatedOn { get; set; }

        [Display(Name = "Last Login")]        
        public virtual DateTime LastLogin { get; set; }

        public virtual void AddRole(Role role)
        {
            role.Users.Add(this);
            Roles.Add(role);            
        }

        public virtual void RemoveRole(Role role)
        {
            role.Users.Remove(this);
            Roles.Remove(role);
        }

        public User()
        {
            CreatedOn = DateTime.UtcNow;
            LastLogin = DateTime.UtcNow;
            Roles = new List<Role>();
        }
    }
}
