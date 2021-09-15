using eProjectSemester3_1.Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace eProjectSemester3_1.Application.Providers
{
    public class RoleProvider : System.Web.Security.RoleProvider
    {
        public MembershipService MembershipService
        {
            get
            {
                return ServiceFactory.Get<MembershipService>();
            }
        }

        public override string[] GetRolesForUser(string username)
        {
            // get user login
            var user = MembershipService.GetUser(username, true);

            var roles = new List<string>();

            if (user != null)
            {
                roles.AddRange(user.Roles.Select(role => role.RoleName));
            }

            // retturn array role for user 
            return roles.ToArray();
        }

        #region NOT IMPLEMENTED - not required

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new System.NotImplementedException();
        }

        public override string ApplicationName
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
                throw new System.NotImplementedException();
            }
        }

        public override void CreateRole(string roleName)
        {
            throw new System.NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new System.NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new System.NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new System.NotImplementedException();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new System.NotImplementedException();
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            throw new System.NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new System.NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}