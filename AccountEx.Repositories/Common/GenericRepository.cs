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
using System.Web;

namespace AccountEx.Repositories
{
    /// <summary>
    /// Generic base class for all repository classes contains CRUD methods for all entity classes
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    public class GenericRepository<T> : BaseRepository where T : class
    {
        protected bool IsGlobalEntity { get; set; }
        public GenericRepository()
        {
            Db = Connection.GetContext();
            var contextAdapter = ((IObjectContextAdapter)Db);
            contextAdapter.ObjectContext.SavingChanges += ObjectContext_SavingChanges;
        }
        void ObjectContext_SavingChanges(object sender, EventArgs e)
        {
            if (IsGlobalEntity) return;
            ObjectContext context = sender as ObjectContext;
            if (context != null)
            {
                foreach (ObjectStateEntry entry in context.ObjectStateManager.GetObjectStateEntries(EntityState.Added))
                {
                    var entity = entry.Entity as BaseEntity;
                    if (entity != null)
                    {
                        entity.CreatedAt = DateTime.Now;
                        entity.CreatedBy = SiteContext.Current.User.Id;
                        entity.CompanyId = SiteContext.Current.User.CompanyId;
                    }
                }

                foreach (ObjectStateEntry entry in context.ObjectStateManager.GetObjectStateEntries(EntityState.Modified))
                {
                    if (entry.Entity == null) continue;
                    var type = entry.Entity.GetType();
                    var properties = type.GetProperties();
                    var entity = (BaseEntity)entry.Entity;
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
        protected IQueryable<T> FiscalCollection
        {
            get
            {
                return AsQueryable(true);
            }
        }
        public virtual void Add(T entity)
        {
            Add(entity, true, true);
        }
        public virtual void Add(T entity, bool saveLog)
        {
            Add(entity, saveLog, true);
        }
        public virtual void Add(bool saveChanges, T entity)
        {
            Add(entity, true, saveChanges);
        }

        public virtual void Add(T entity, bool saveLog, bool saveChanges)
        {
            ObjectSet.Add(entity);
            if (saveLog)
                Db.SystemLogs.Add(SaveLog(entity, ActionType.Added, false));
            if (saveChanges)
                SaveChanges();
        }

        public virtual void Update(T entity)
        {
            Update(entity, true, true);
        }
        public virtual void Update(T entity, bool saveLog)
        {
            Update(entity, saveLog, true);
        }
        public virtual void Update(bool saveChanges, T entity)
        {
            Update(entity, true, saveChanges);
        }
        public virtual void Update(T entity, bool saveLog, bool saveChanges)
        {

            ObjectSet.Attach(entity);
            var entry = Db.Entry(entity);
            entry.State = EntityState.Modified;
            if (saveLog)
                Db.SystemLogs.Add(SaveLog(entity, ActionType.Updated, false));
            if (saveChanges)
                SaveChanges();
        }
        public virtual void Delete(int id)
        {

            Delete(new List<int> { id }, "Id", false);
        }
        public virtual void Delete(int id, bool addFiscalCheck)
        {

            Delete(new List<int> { id }, "Id", addFiscalCheck);
        }
        public virtual void Delete(int id, string deleteOnColumn)
        {

            Delete(new List<int> { id }, deleteOnColumn, false);
        }
        public virtual void Delete(int id, string deleteOnColumn, bool addFiscalCheck)
        {

            Delete(new List<int> { id }, deleteOnColumn, addFiscalCheck);
        }


        public virtual void Delete(List<int> ids)
        {

            Delete(ids, "Id", false);
        }
        public virtual void Delete(List<int> ids, bool addFiscalCheck)
        {

            Delete(ids, "Id", addFiscalCheck);
        }
        public virtual void Delete(List<int> ids, string deleteOnColumn)
        {

            Delete(ids, deleteOnColumn, false);
        }

        public virtual void Delete(List<int> ids, string deleteOnColumn, bool addFiscalCheck)
        {
            if (ids.Count() > 0)
            {
                var tableName = ContextExtensions.GetTableName<T>();
                var query = string.Format("DELETE FROM {0} WHERE {1} IN ({2}) AND CompanyId={3}", tableName, deleteOnColumn, string.Join(",", ids), SiteContext.Current.User.CompanyId);
                if (addFiscalCheck)
                    query += " AND FiscalId=" + SiteContext.Current.Fiscal.Id;
                Db.Database.ExecuteSqlCommand(query);
            }
        }
        public virtual void Add(List<T> entities)
        {
            Add(entities, false, false);
        }
        public virtual void Add(List<T> entities, bool saveLog, bool saveChanges)
        {
            foreach (var entity in entities)
            {
                Add(entity, false, false);
            }
            if (saveChanges)
                SaveChanges();
        }
        public virtual void Save(List<T> entities)
        {
            Save(entities, false, false);
        }
        public virtual void Save(bool saveChanges, List<T> entities)
        {
            Save(entities, false, saveChanges);
        }
        public virtual void Save(List<T> entities, bool saveLog, bool saveChanges)
        {
            foreach (var entity in entities)
            {
                Save(entity, false, false);
            }
            if (saveChanges)
                SaveChanges();
        }
        public virtual void Save(T entity)
        {
            Save(entity, true, true);
        }
        public virtual void Save(T entity, bool saveLog, bool saveChanges)
        {
            if (((dynamic)entity).Id == 0)
            {
                Add(entity, saveLog, saveChanges);
            }
            else
            {
                Update(entity, saveLog, saveChanges);
            }
        }
        public virtual T GetById(int id)
        {
            return GetById(id, false, false);
        }
        public virtual T GetById(int id, bool noTracking)
        {
            return GetById(id, noTracking, false);
        }
        public virtual T GetById(bool excludeCompanyCheck, int id)
        {
            return GetById(id, false, excludeCompanyCheck);
        }
        public virtual T GetById(bool excludeCompanyCheck, int id, bool noTracking)
        {
            return GetById(id, noTracking, excludeCompanyCheck);
        }
        public virtual T GetById(int id, bool noTracking, bool excludeCompanyCheck)
        {
            var query = ObjectSet.Where("it.IsDeleted ==" + false + " And it.Id=" + id);
            if (!excludeCompanyCheck)
                query = query.Where("it.CompanyId == " + SiteContext.Current.User.CompanyId);
            if (noTracking)
                return query.AsNoTracking().FirstOrDefault();
            else
                return query.FirstOrDefault();
        }

        public bool IsExist(string name, int id, string columnName = "Name")
        {
            if (string.IsNullOrWhiteSpace(name))
                name = "";
            return ObjectSet.Where("it.CompanyId == " + SiteContext.Current.User.CompanyId + " AND it." + columnName + ".ToLower() ==\"" + name.ToLower() + "\" And it.Id!=" + id).Any();
        }
        //public string GetIdByName(string name, string columnName = "Name")
        //{
        //    var record = ObjectSet.Where("it.CompanyId == " + SiteContext.Current.User.CompanyId + " AND it." + columnName + ".ToLower() ==\"" + name.ToLower() + "\"").Select("new(Id)");
        //    return Numerics.GetInt(record) + "";
        //}

        public virtual List<IdName> GetNames()
        {
            return GetNames("Name", GetClassName());
        }
        public virtual List<IdName> GetNames(string nameColumn)
        {
            return GetNames(nameColumn, GetClassName());
        }
        public virtual List<IdName> GetNames(string nameColumn, string tableName)
        {
            var sqlQuery = "Select Id, " + nameColumn + " as Name From " + tableName + " Where IsDeleted=0 ";
            if (SiteContext.Current.User != null && SiteContext.Current.User.CompanyId > 0)
                sqlQuery += " AND CompanyId = " + SiteContext.Current.User.CompanyId;
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
        public virtual T FirstOrDefault(Expression<Func<T, bool>> predicate, bool noTracking)
        {
            if (noTracking)
                return Collection.AsNoTracking().FirstOrDefault(predicate);
            else
                return Collection.FirstOrDefault(predicate);
        }
        public virtual List<T> GetAll(Expression<Func<T, bool>> predicate)
        {
            return Collection.Where(predicate).ToList();
        }
        public virtual IEnumerable<T> Get(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = "")
        {
            IQueryable<T> query = Collection;

            if (filter != null)
            {
                query = Collection.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            else
            {
                return query.ToList();
            }
        }
        public virtual List<T> GetAll()
        {
            return Collection.ToList();
        }
        public virtual IQueryable GetAll<P>() where P : class
        {
            var obj = Activator.CreateInstance(typeof(P));
            var proprties = obj.GetType().GetProperties().Select(p => p.Name);
            var seelctFilter = string.Join(",", proprties.ToList());
            return Collection.Select("new(" + seelctFilter + ")");

        }
        public virtual IQueryable GetAll<P>(Byte type) where P : class
        {
            var obj = Activator.CreateInstance(typeof(P));
            var proprties = obj.GetType().GetProperties().Select(p => p.Name);
            var seelctFilter = string.Join(",", proprties.ToList());
            return Collection.Where("it.AccountDetailFormId ==" + type).Select("new(" + seelctFilter + ")");

        }
        public virtual IQueryable GetAll<P>(List<byte> types) where P : class
        {
            var obj = Activator.CreateInstance(typeof(P));
            var proprties = obj.GetType().GetProperties().Select(p => p.Name);
            var seelctFilter = string.Join(",", proprties.ToList());
            var whereFilter = "";
            foreach (var type in types)
            {
                whereFilter += "it.AccountDetailFormId=" + type + " or ";
            }
            whereFilter = whereFilter.Trim();
            whereFilter = whereFilter.EndsWith("or") ? whereFilter.Substring(0, whereFilter.Length - 2) : whereFilter;
            return Collection.Where(whereFilter).Select("new(" + seelctFilter + ")");

        }
        public virtual IQueryable<T> AsQueryable()
        {
            return AsQueryable(false);
        }
        public virtual IQueryable<T> AsQueryable(bool IsFiscal)
        {
            if (typeof(T).Name.Contains("Nexus_"))
            {
                return ObjectSet.AsQueryable();
            }
            else
            {
                var filter = "AND it.CompanyId=0";
                if (SiteContext.Current.User != null && SiteContext.Current.User.CompanyId > 0)
                    filter = " AND it.CompanyId == " + SiteContext.Current.User.CompanyId;
                if (IsFiscal)
                    filter += " AND it.FiscalId == " + SiteContext.Current.Fiscal.Id;
                //filter = " AND it.Date between " + SiteContext.Current.Fiscal.FromDate + " AND " + SiteContext.Current.Fiscal.ToDate;
                return !typeof(T).Name.Contains("vw_")
                    ? ObjectSet.Where("it.IsDeleted == false " + filter).AsQueryable()
                    : ObjectSet.AsQueryable().Where("it.IsDeleted == false " + filter);
            }
        }

        public virtual IQueryable<P> AsQueryable<P>() where P : class
        {
            return AsQueryable<P>(false);
        }
        public virtual IQueryable<P> AsQueryable<P>(bool IsFiscal) where P : class
        {
            var filter = "";
            if (SiteContext.Current.User != null && SiteContext.Current.User.CompanyId > 0)
                filter = " AND it.CompanyId == " + SiteContext.Current.User.CompanyId;
            if (IsFiscal)
                filter += " AND it.FiscalId == " + SiteContext.Current.Fiscal.Id;
            // filter = " AND it.Date between " + SiteContext.Current.Fiscal.FromDate + " AND " + SiteContext.Current.Fiscal.ToDate;
            return !typeof(P).Name.Contains("vw_")
                ? Db.Set<P>().Where("it.IsDeleted == false " + filter).AsQueryable()
                : Db.Set<P>().AsQueryable().Where("it.IsDeleted == false " + filter);
        }

        public virtual void SaveChanges()
        {
            Db.SaveChanges();
        }
        public virtual void SaveLog(object record, ActionType actionType)
        {
            SaveLog(record, actionType, true);
        }
        public virtual SystemLog SaveLog(object record, ActionType actionType, bool saveChanges)
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
                var action = Enum.GetName(typeof(ActionType), actionType);
                var documentNo = 0;
                var documentCode = "";
                var moduleType = 0;
                var description = "";
                if (SiteContext.Current.LogMappings.Any(p => p.TableName.ToLower() == tableName))
                {
                    var mapping = SiteContext.Current.LogMappings.FirstOrDefault(p => p.TableName.ToLower() == tableName);
                    if (!string.IsNullOrWhiteSpace(mapping.ModuleKey))
                        moduleType = Numerics.GetInt(record.GetType().GetProperty(mapping.ModuleKey).GetValue(record, null));
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
                var clientIPAddress = "";
                if (HttpContext.Current.Request.Cookies["ClientIpAddess"] != null && !string.IsNullOrWhiteSpace(HttpContext.Current.Request.Cookies["ClientIpAddess"].Value))
                {
                    clientIPAddress = HttpContext.Current.Request.Cookies["ClientIpAddess"].Value;
                }
                //var ip2 = LocalIpAddress();


                var log = new SystemLog();
                //  var tablename = record.GetType().BaseType.Name == "Object" ? record.GetType().Name : record.GetType().BaseType.Name;
                log.TableName = tableName;
                log.DocumentId = (int?)record.GetType().GetProperty("Id").GetValue(record, null);
                log.DocumentCode = documentCode;
                log.DocumentType = tableName;
                log.ModuleType = moduleType;
                log.Description = description;
                //log.DocumentNo = (int?)record.GetType().GetProperty(DocumentKey).GetValue(record, null); ;
                log.DocumentNo = documentNo;
                log.Action = action;
                log.CreatedAt = DateTime.Now;
                log.CreatedBy = SiteContext.Current.User.Id;
                log.UserName = SiteContext.Current.User.Username;
                log.DocumentYear = DateTime.Now.Year.ToString();
                log.ComputerIp = clientIPAddress;
                log.ComputerUser = Environment.UserName;
                log.ComputerName = Environment.MachineName;
                log.LogData.Add(new LogData() { Data = jsondata });
                if (saveChanges)
                {
                    Db.SystemLogs.Add(log);
                    Db.SaveChanges();
                }
                return log;

            }

            catch (DbEntityValidationException)
            {
                return null;
            }


        }
        //public virtual string MakeSqlQuery()
        //{
        //    string jsondata = "";
        //    try
        //    {


        //    }

        //    catch (DbEntityValidationException e)
        //    {

        //    }


        //}
        public void ExecuteQuery(string query)
        {
            Db.Database.ExecuteSqlCommand(query);
        }
        //private string LocalIpAddress()
        //{
        //    IPHostEntry host;
        //    string localIp = "";
        //    host = Dns.GetHostEntry(Dns.GetHostName());
        //    foreach (IPAddress ip in host.AddressList)
        //    {
        //        if (ip.AddressFamily == AddressFamily.InterNetwork)
        //        {
        //            localIp = ip.ToString();
        //            break;
        //        }
        //    }
        //    return localIp;
        //}
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
    public static class ContextExtensions
    {
        public static string GetTableName<T>() where T : class
        {
            ObjectContext objectContext = ((IObjectContextAdapter)Connection.GetContext()).ObjectContext;

            return objectContext.GetTableName<T>();
        }

        public static string GetTableName<T>(this ObjectContext context) where T : class
        {
            string sql = context.CreateObjectSet<T>().ToTraceString();
            Regex regex = new Regex("FROM (?<table>.*) AS");
            Match match = regex.Match(sql);

            string table = match.Groups["table"].Value;
            return table;
        }
    }
}