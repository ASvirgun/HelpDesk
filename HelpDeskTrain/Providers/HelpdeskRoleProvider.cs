using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using HelpDeskTrain.Models;

namespace HelpDeskTrain.Providers
{
    public class HelpdeskRoleProvider : RoleProvider
    {
        public override bool IsUserInRole(string username, string roleName)
        {
            bool outputResult = false;
            using (var db = new HelpdeskContext())
            {
                try
                {
                    var user = db.Users.FirstOrDefault(x => x.Login == username);
                    if (user != null)
                    {
                        var role = db.Roles.Find(user.RoleId);
                        if (role != null && role.Name == roleName)
                            outputResult = true;
                    }
                }
                catch
                {
                    outputResult = false;
                }
            }
            return outputResult;
        }

        public override string[] GetRolesForUser(string login)
        {
            var role = new string[]{};
            using (var db = new HelpdeskContext())
            {
                try
                {
                    var user = db.Users.FirstOrDefault(x => x.Login == login);
                    if (user != null)
                    {
                        var userRole = db.Roles.Find(user.RoleId);
                        if (userRole != null)
                        {
                            role = new string[]{userRole.Name};
                        }
                    }
                }
                catch
                {
                    role = new string[] { };
                }
            }
            return role;
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string ApplicationName { get; set; }
    }
}