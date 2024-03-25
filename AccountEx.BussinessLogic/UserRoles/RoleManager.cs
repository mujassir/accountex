using AccountEx.CodeFirst.Models;
using AccountEx.Common;
using AccountEx.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AccountEx.BussinessLogic
{
    public static class RoleManager
    {
        public static void Save(Role role)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {
                var repo = new RoleRepository();
                repo.Save(role, repo);
                repo.SaveChanges();
                scope.Complete();

            }
        }

        public static ApiResponse Delete(int id)
        {
            ApiResponse response;
            var err = ServerValidateDelete(id);
            if (err == "")
            {
                new RoleRepository().Delete(id);
                response = new ApiResponse { Success = true };
            }
            else
            {
                response = new ApiResponse() { Success = false, Error = err };
            }


            return response;
        }

        private static string ServerValidateDelete(int id)
        {
            var err = ",";
            try
            {

                if (new UserRoleRepository().CheckIfRoleAssigned(id))
                {
                    err += "Role is assigned to user and can't be deleted.";
                }
            }
            catch (Exception)
            {

                err = "error in data validation";
            }
            err = err.Trim(',');
            return err;


        }
    }
}
