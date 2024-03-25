using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AccountEx.Common;
using System.Data.Entity;
using System.Data;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Net;
using System.Net.Sockets;
using AccountEx.CodeFirst.Context;
using AccountEx.CodeFirst.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Text.RegularExpressions;


namespace AccountEx.Repositories
{
    /// <summary>
    /// Generic base class for all repository classes contains CRUD methods for all entity classes
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    public class GlobalGenericRepository<T> where T : class
    {
        protected AccountExContext Db = null;
        public GlobalGenericRepository()
        {
            Db = Connection.GetContext();
            var contextAdapter = ((IObjectContextAdapter)Db);
            contextAdapter.ObjectContext.SavingChanges += ObjectContext_SavingChanges;
        }
        void ObjectContext_SavingChanges(object sender, EventArgs e)
        {
            ObjectContext context = sender as ObjectContext;
            if (context != null)
            {
                foreach (ObjectStateEntry entry in context.ObjectStateManager.GetObjectStateEntries(EntityState.Added))
                {
                    var entity = (GlobalBaseEntity)entry.Entity;
                    if (entity != null)
                    {
                        entity.CreatedAt = DateTime.Now;
                        entity.CreatedBy = SiteContext.Current.User.Id;
                       // entity.CompanyId = SiteContext.Current.User.CompanyId;
                    }
                }

                foreach (ObjectStateEntry entry in context.ObjectStateManager.GetObjectStateEntries(EntityState.Modified))
                {
                    if (entry.Entity == null) continue;
                    var type = entry.Entity.GetType();
                    var properties = type.GetProperties();
                    var entity = (GlobalBaseEntity)entry.Entity;
                    entity.ModifiedAt = DateTime.Now;
                    entity.ModifiedBy = SiteContext.Current.User.Id;
                    foreach (var property in properties)
                    {
                        //if (property.PropertyType.Name.Contains("ICollection"))
                        //{
                        //    var items = (dynamic)type.GetProperty(property.Name).GetValue(entry.Entity);
                        //    foreach (var item in items)
                        //    {
                        //        item.CompanyId = entity.CompanyId;

                        //        item.CreatedBy = entity.CreatedBy;
                        //        item.CreatedAt = entity.CreatedAt;

                        //        item.ModifiedAt = DateTime.Now;
                        //        item.ModifiedBy = SiteContext.Current.User.Id;
                        //    }
                        //}
                        var attributes = property.GetCustomAttributes(typeof(NeverUpdateAttribute), false);
                        if (attributes.Length > 0)
                            entry.RejectPropertyChanges(property.Name);
                    }
                }
            }
        }
        private DbSet<T> _objectset;
        protected DbSet<T> ObjectSet
        {
            get { return _objectset ?? (_objectset = Db.Set<T>()); }
        }
        protected IQueryable<T> Collection
        {
            get
            {
                return AsQueryable();
            }
        }
        public virtual void Add(T entity)
        {
            ObjectSet.Add(entity);
            SaveChanges();
            //SaveLog(entity, "Added");
        }
        public virtual void Update(T entity)
        {

            ObjectSet.Attach(entity);
            var entry = Db.Entry(entity);
            entry.State = EntityState.Modified;
            SaveChanges();
            //SaveLog(entity, "Updated");
        }
        //public virtual void Delete(T entity)
        //{
        //    if (entity == null) return;

        //    ObjectSet.Remove(entity);
        //    var entry = Db.Entry(entity);
        //    entry.State = EntityState.Deleted;
        //    var orignalentity = entry.GetDatabaseValues().ToObject();
        //    SaveChanges();
        //    SaveLog(orignalentity, "Deleted");
        //}
        public virtual void Delete(int id)
        {
            
            var entity = ObjectSet.Where("it.Id=" + id).FirstOrDefault();
            if (entity == null) return;
            ((dynamic)entity).IsDeleted = true;
            var entry = Db.Entry(entity);
            entry.State = EntityState.Modified;
            SaveChanges();
            //SaveLog(entity, "Deleted");
        }
        public virtual void Add(List<T> entities)
        {
            foreach (var entity in entities)
            {
                Add(entity);
            }
            SaveChanges();
        }
        public virtual void Save(List<T> entities)
        {
            foreach (var entity in entities)
            {
                Save(entity);
            }
        }
        public virtual void Save(T entity)
        {
            if (((dynamic)entity).Id == 0)
            {
                Add(entity);
            }
            else
            {
                Update(entity);
            }
        }
        public virtual T GetById(int id)
        {
            return ObjectSet.Where("it.IsDeleted !=" + true).Where("it.Id=" + id).FirstOrDefault();
        }
        public virtual List<IdName> GetNames()
        {
            return GetNames("Name", GetClassName());
        }
        public virtual List<IdName> GetNames(string nameColumn, string tableName)
        {
            var sqlQuery = "Select Id, " + nameColumn + " as Name From " + tableName + " Where IsDeleted=0 ";
            //if (SiteContext.Current.User != null && SiteContext.Current.User.CompanyId > 0)
            //    sqlQuery += " AND CompanyId = " + SiteContext.Current.User.CompanyId;
            return Db.Database.SqlQuery<IdName>(sqlQuery).ToList();
        }
        private string Pluralize(string s)
        {
            PluralizationService plural =
                PluralizationService.CreateService(
                    CultureInfo.GetCultureInfo("en-us"));
            return plural.Pluralize(s);
        }
        private string GetClassName()
        {
            var v = typeof(T);
            return Pluralize(v.Name);
        }
        public virtual T FirstOrDefault()
        {
            return Collection.FirstOrDefault();
        }
        public virtual T FirstOrDefault(Expression<Func<T, bool>> predicate)
        {
            return Collection.FirstOrDefault(predicate);
        }
        public virtual List<T> GetAll(Expression<Func<T, bool>> predicate)
        {
            return Collection.ToList();
        }
        public virtual List<T> GetAll()
        {
            return Collection.ToList();
        }
        public virtual IQueryable<T> AsQueryable()
        {
            //var companyFilter = "";
            //if (SiteContext.Current.User != null && SiteContext.Current.User.CompanyId > 0)
            //    companyFilter = " AND it.CompanyId == " + SiteContext.Current.User.CompanyId;
            return !typeof(T).Name.Contains("vw_") ? ObjectSet.Where("it.IsDeleted == false").AsQueryable() : ObjectSet.AsQueryable();
        }
        public virtual void SaveChanges()
        {
            Db.SaveChanges();
        }
        public virtual void SaveLog(object record, ActionType actionType)
        {
            string jsondata = "";
            try
            {
                //var type=record.GetType();
                //var obj=Activator.CreateInstance(type);
                //var table = Db.GetTableName<record.GetType().IsClass>();
                var tableName = ContextExtensions.GetTableName<T>();
                var tableWithSchema = tableName.Split('.').ToArray();
                if (!string.IsNullOrWhiteSpace(tableWithSchema[1]))
                    tableName = (tableWithSchema[1]).Replace("[", "").Replace("]", "");
                tableName = tableName.ToLower();
               

                jsondata = JsonConvert.SerializeObject(record, GetJsonSetting());
                var action = Enum.GetName(typeof(ActionType),actionType);
                var documentNo = 0;
                var documentCode = "";
                var moduleType = 0;
                var description = "";
                if (SiteContext.Current.LogMappings.Any(p => p.TableName.ToLower() == tableName))
                {
                    var mapping = SiteContext.Current.LogMappings.FirstOrDefault(p => p.TableName.ToLower() == tableName);
                    if (!string.IsNullOrWhiteSpace(mapping.ModuleKey))
                        moduleType =Numerics.GetInt(record.GetType().GetProperty(mapping.ModuleKey).GetValue(record, null));
                    if (mapping.LogType == (byte)LogType.Transaction || mapping.LogType == (byte)LogType.Service)
                    {
                        documentNo = Numerics.GetInt(record.GetType().GetProperty(mapping.LogKey).GetValue(record, null));
                        description = Enum.GetName(typeof(VoucherType), moduleType);
                      
                    }
                    else if (mapping.LogType == (byte)LogType.ChartOfAccount)
                    {
                        documentCode = (string)record.GetType().GetProperty(mapping.LogKey).GetValue(record, null);
                        description = Enum.GetName(typeof(AccountDetailFormType), moduleType);
                    }
                    if (string.IsNullOrWhiteSpace(mapping.ModuleKey))
                        description = mapping.Description;
                  
                   

                }

                var log = new SystemLog();
              //  var tablename = record.GetType().BaseType.Name == "Object" ? record.GetType().Name : record.GetType().BaseType.Name;
                log.TableName = tableName;
                log.DocumentId = (int?)record.GetType().GetProperty("Id").GetValue(record, null);
                log.DocumentCode =documentCode;
                log.DocumentType = tableName;
                log.ModuleType = moduleType;
                log.Description = description;
                //log.DocumentNo = (int?)record.GetType().GetProperty(DocumentKey).GetValue(record, null); ;
                log.DocumentNo =documentNo;
                log.Action = action;
                log.CreatedAt = DateTime.Now;
                log.CreatedBy = SiteContext.Current.User.Id;
                log.UserName = SiteContext.Current.User.Username;
                log.DocumentYear = DateTime.Now.Year.ToString();
                log.ComputerIp = LocalIpAddress();
                log.ComputerUser = Environment.UserName;
                log.ComputerName = Environment.MachineName;
                log.LogData.Add(new LogData() { Data = jsondata });
                Db.SystemLogs.Add(log);
                Db.SaveChanges();

            }

            catch (DbEntityValidationException e)
            {

            }


        }
        private string LocalIpAddress()
        {
            IPHostEntry host;
            string localIp = "";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIp = ip.ToString();
                    break;
                }
            }
            return localIp;
        }
        protected JsonSerializerSettings GetJsonSetting()
        {
            var setting = new JsonSerializerSettings();
            setting.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            IsoDateTimeConverter dateConverter = new IsoDateTimeConverter
            {
                DateTimeFormat = "dd/MM/yyyy"
            };

            setting.Converters.Add(dateConverter);
            return setting;
        }
      

    }
    
}