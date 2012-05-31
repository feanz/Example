using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using Example.Core.Model;
using Example.Data.Repositories.Interfaces;
using Utilities.Extensions;
using Web.Helpers;
using Web.ViewModels;

namespace Web.Controllers
{
    [CustomAuthorize(Roles = "SuperUser")]
    public class AccountController : BaseController
    {
        private readonly IAccountRepository _repository;

        public AccountController(IAccountRepository repository)
        {
            ViewBag.MenuItem = "Admin";
            _repository = repository;
        }

        public virtual ActionResult Create()
        {
            ViewBag.PossibleRoles = _repository.GetRoles();

            return View(new User());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Create(User user)
        {
            if (ModelState.IsValid)
            {
                if (Request.Form["RoleIds"].IsNotNull())
                {
                    UpdateUserRoles(Request.Form["RoleIds"].ToArray(), user);
                }

                _repository.InsertOrUpdate(user);
                this.FlashInfo("User Created");
                return RedirectToAction("Index");
            }
            this.FlashWarning("Error creating account");
            ViewBag.PossibleRoles = _repository.GetRoles();
            return View(user);
        }

        public virtual ActionResult Edit(int id)
        {
            var user = _repository.GetById(id);

            if (user.IsNull())
                return new ResourceNotFoundResult();
            PopulateAssignedRolesData(user);
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Edit(int id, FormCollection formCollection, string[] selectedRoles)
        {
            var userToUpdate = _repository.GetById(id);

            if (TryUpdateModel(userToUpdate, "", null, new[] {"Roles"}))
            {
                UpdateUserRoles(selectedRoles, userToUpdate);

                _repository.InsertOrUpdate(userToUpdate);
                this.FlashInfo("User Updated");
                return RedirectToAction("Index");
            }
            var user = _repository.GetById(id);
            PopulateAssignedRolesData(user);
            return View(userToUpdate);
        }

        public virtual ViewResult Index(int? page)
        {
            var users = _repository.GetAll();

            return View(users);
        }

        [CustomAuthorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            var user = _repository.GetById(id);

            if (user.UserName.ToUpperInvariant() !=
                Environment.UserDomainName.ToLower() + "\\" + Environment.UserName.ToUpperInvariant())
            {
                _repository.Delete(user);
                this.FlashWarning("User deleted");
                return RedirectToAction("Index");
            }
            this.FlashWarning("You can't delete you own account");
            return RedirectToAction("Index");
        }

        [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
        public JsonResult IsUserNameAvailable(string userName, string id)
        {
            var user = _repository.GetBy(x => x.UserName == userName);

            if (!id.IsNullOrEmpty())
            {
                if (user.Count() == 0)
                    return Json(true, JsonRequestBehavior.AllowGet);
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        private void PopulateAssignedRolesData(User user)
        {
            var allRoles = _repository.GetRoles();
            var userRoles = new HashSet<int>(user.Roles.Select(c => c.Id));
            var viewModel = allRoles.Select(role => new AssignedRole
                                                        {
                                                            RoleID = role.Id,
                                                            RoleName = role.RoleName,
                                                            Assigned = userRoles.Contains(role.Id)
                                                        }).ToList();
            ViewBag.PossibleRoles = viewModel;
        }

        private void UpdateUserRoles(IEnumerable<string> selectedRoles, User userToUpdate)
        {
            if (selectedRoles.IsNull())
            {
                userToUpdate.Roles.Clear();
                return;
            }

            var selectedRolesHS = new HashSet<string>(selectedRoles);
            var userRoles = new HashSet<int>(userToUpdate.Roles.Select(c => c.Id));
            var allRoles = _repository.GetRoles();
            foreach (var role in allRoles)
            {
                if (selectedRolesHS.Contains(role.Id.ToStringInvariantCulture()))
                {
                    if (!userRoles.Contains(role.Id))
                    {
                        userToUpdate.AddRole(role);
                    }
                }
                else
                {
                    if (userRoles.Contains(role.Id))
                    {
                        userToUpdate.RemoveRole(role);
                    }
                }
            }
        }
    }
}