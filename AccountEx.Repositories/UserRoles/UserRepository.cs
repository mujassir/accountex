using AccountEx.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using AccountEx.CodeFirst.Models;
using AccountEx.CodeFirst.Models.CRM;

namespace AccountEx.Repositories
{
    public class UserRepository : GenericRepository<User>
    {

        public UserRepository() : base() { }
        public UserRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }
        public Dictionary<int, string> GetUserNames(List<int> ids)
        {
            return Collection.Where(p => ids.Contains(p.Id)).ToDictionary(p => p.Id, q => q.Username);
        }
        public IList<User> GetAllUsersByCompanyIds(List<int> ids)
        {
            return Db.Users.Where(p => ids.Contains((int)p.CompanyId)).ToList();
        }
        public Dictionary<int, string> GetFullNames(List<int> ids)
        {
            return Collection.Where(p => ids.Contains(p.Id)).ToDictionary(p => p.Id, q => (q.FirstName + " " + q.LastName).Trim());
        }
        public List<IdName> GetIdNames(List<int> ids)
        {
            return Collection.Where(p => ids.Contains(p.Id)).Select(p => new IdName()
            {
                Id = p.Id,
                Name = p.FirstName + " " + p.LastName
            }).ToList();
        }
        public vw_CRMUser GetCRMUserById(int id)
        {
            return new GenericRepository<vw_CRMUser>().FirstOrDefault(p => p.Id == id);
        }
        public List<IdName> LoadCRMUserByType(CRMUserType type)
        {
            var query = Collection.AsQueryable();
            if (type == CRMUserType.SalesExecutive)
            {
                List<CRMUserType> types = new List<CRMUserType>() { CRMUserType.Admin, CRMUserType.CEO, CRMUserType.RSM, CRMUserType.SalesExecutive };
                if (SiteContext.Current.UserTypeId == CRMUserType.RSM)
                {
                    query = query.Where(p => (p.RSMId == SiteContext.Current.User.Id && p.UserTypeId == type) || p.Id == SiteContext.Current.User.Id);
                }
                else if (types.Contains(SiteContext.Current.UserTypeId))
                {
                    List<CRMUserType> spTypes = new List<CRMUserType>() { CRMUserType.RSM, CRMUserType.SalesExecutive };

                    query = query.Where(p => p.UserTypeId.HasValue && spTypes.Contains(p.UserTypeId.Value));
                }
                else if (SiteContext.Current.UserTypeId == CRMUserType.SalesExecutive)
                {
                    query = query.Where(p => p.Id == SiteContext.Current.User.Id);
                }
                else if (SiteContext.Current.UserTypeId == CRMUserType.DivisionalHead)
                {
                    List<CRMUserType> spTypes = new List<CRMUserType>() { CRMUserType.RSM, CRMUserType.SalesExecutive };
                    query = query.Where(p => p.UserTypeId.HasValue && spTypes.Contains(p.UserTypeId.Value));
                }
                else
                {
                    query = query.Where(p => p.Id == SiteContext.Current.User.Id);
                }
            }
            else
            {
                query = query.Where(p => p.UserTypeId == type);
            }

            return query.Select(p => new IdName()
            {
                Id = p.Id,
                Name = p.FirstName + " " + p.LastName
            }).ToList();
        }
        public int GetDefaultUserByTypeId(CRMUserType type)
        {
            if (Collection.Any(p => p.UserTypeId == type))
                return Collection.FirstOrDefault(p => p.UserTypeId == type).Id;
            else return 0;
        }
        public List<IdName> LoadCRMUserByType(List<CRMUserType> types)
        {
            return Collection.Where(p => p.UserTypeId != null && types.Contains(p.UserTypeId.Value)).Select(p => new IdName()
            {
                Id = p.Id,
                Name = p.FirstName + " " + p.LastName
            }).ToList();
        }
        public List<IdName> LoadCRMUserByType(CRMUserType type, int id)
        {
            return LoadCRMUserByType(type, new List<int>() { id });
        }
        public List<IdName> LoadCRMUserByType(CRMUserType type, List<int> Ids)
        {
            return Collection.Where(p => p.UserTypeId == type && Ids.Contains(p.Id)).Select(p => new IdName()
            {
                Id = p.Id,
                Name = p.FirstName + " " + p.LastName
            }).ToList();
        }
        public bool CheckUserType(CRMUserType type, int id)
        {
            return Collection.Any(p => p.Id == id && p.UserTypeId == type);
        }
        public CRMUserType GetUserType(int id)
        {
            if (Collection.Any(p => p.Id == id))
                return Collection.FirstOrDefault(p => p.Id == id).UserTypeId.Value;
            else return CRMUserType.LabUser;


        }
        public void ChangePassword(int userId, string oldPassword, string newPassword)
        {
            var user = Collection.First(p => p.Id == userId);
            if (user.Hash == Sha1Sign.Hash(user.Username + oldPassword))
            {
                user.Hash = Sha1Sign.Hash(user.Username + newPassword);
                Db.SaveChanges();
            }
            else
                throw new OwnException("Old password is not correct");

        }

        public User GetByUsername(string username)
        {

            username = username.ToLower();
            return Db.Users.FirstOrDefault(p => p.Username.ToLower() == username.ToLower());
        }
        public bool IsUserExist(string username, int id)
        {

            username = username.ToLower();
            return Db.Users.Any(p => p.Username.ToLower() == username.ToLower() && p.Id != id);
        }

        public void Create(string username, string password, bool isLive, bool isSystemUser)
        {
            var user = new User
            {
                Username = username,
                Hash = Sha1Sign.Hash(username + password),
                IsLive = isLive,
                IsSystemUser = isSystemUser,
            };
            Add(user);
        }

        public override void Update(User entity)
        {
            var query = "Delete from UserRoles where UserId=" + entity.Id;
            Db.Database.ExecuteSqlCommand(query);
            foreach (var item in entity.UserRoles)
            {
                item.ModifiedAt = DateTime.Now;
                item.ModifiedBy = SiteContext.Current.User.Id;
                Db.UserRoles.Add(item);
            }
            entity.UserRoles = null;
            if (string.IsNullOrWhiteSpace(entity.Hash))
            {
                var user = Collection.AsNoTracking().FirstOrDefault(p => p.Id == entity.Id);
                if (user != null) entity.Hash = user.Hash;
            }
            base.Update(entity);
        }

        public void UpdateCRMUser(User entity)
        {

            if (string.IsNullOrWhiteSpace(entity.Hash))
            {
                var user = Collection.AsNoTracking().FirstOrDefault(p => p.Id == entity.Id);
                if (user != null) entity.Hash = user.Hash;
            }
            else
            {
                entity.Hash = Sha1Sign.Hash(entity.Username + entity.Hash);
            }
            Update(entity);
        }
        public void Save(User entity, string hash)
        {
            var user = entity;
            var username = user.Username.ToLower();
            //user.Company = null;
            var repo = new UserRepository();
            if (user.Id == 0)
            {
                user.Hash = Sha1Sign.Hash(username + user.Hash);
                Guid.NewGuid().ToString().Substring(0, 12);
                repo.Add(user);
            }
            else
            {
                user.Hash = !string.IsNullOrWhiteSpace(user.Hash) ? Sha1Sign.Hash(username + user.Hash) : hash;
                repo.Update(user);
            }

            //new UserRepository().Save(User);

        }
        public void SaveCRMUser(User entity)
        {
            var user = entity;
            var username = user.Username.ToLower();
            //user.Company = null;
            user.UserRoles.Add(new UserRole()
            {
                UserId = entity.Id,
                RoleId = entity.RoleId
            });
            var repo = new UserRepository();
            if (user.Id == 0)
            {

                user.Hash = Sha1Sign.Hash(username + user.Hash);
                Guid.NewGuid().ToString().Substring(0, 12);
                repo.Add(user);
            }
            else
            {
                repo.UpdateCRMUser(user);
            }

            //new UserRepository().Save(User);

        }
        public bool CheckIfAdminUserExist(int id)
        {
            return Collection.Any(p => p.IsAdmin && p.Id != id);
        }



    }
}
