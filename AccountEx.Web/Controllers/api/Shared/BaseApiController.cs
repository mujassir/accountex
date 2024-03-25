using ImpromptuInterface;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Net.Http;
using System.Web.Http;
using System.Collections.Specialized;
using System.Web.Script.Serialization;
using AccountEx.Web.Filters;
using System.Web.Mvc;

namespace AccountEx.Web
{
    [SessionManagementAttribute]
    [SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    public class BaseApiController : ApiController
    {
        public NameValueCollection QueryString
        {
            get
            {
                return Request.RequestUri.ParseQueryString();
            }
        }
        protected string WrapToolTip(string content)
        {
            return "<span title='" + content + "'>" + content + "</span>";
        }
        protected string Option(string value, string text, params KeyValuePair<string, string>[] attributes)
        {
            var html = "";

            foreach (var item in attributes)
            {
                html += " " + item.Key + "=\"" + item.Value + "\""; ;
            }
            html = "<option value=\"" + value + "\" " + html + ">" + text + "</option>";
            return html;
        }
        protected JsonSerializerSettings GetJsonSetting()
        {
            var setting = new JsonSerializerSettings();
            setting.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            var dateConverter = new IsoDateTimeConverter
            {
                DateTimeFormat = "dd/MM/yyyy"
            };

            setting.Converters.Add(dateConverter);
            return setting;
        }
        protected string JsonResult(bool success, string data)
        {
            return new JavaScriptSerializer().Serialize(new
            {
                Result = success,
                Data = data,
            });
        }

        protected string Option(string value, string text, bool isSelected, params KeyValuePair<string, string>[] attributes)
        {
            var html = "";

            foreach (var item in attributes)
            {
                html += " " + item.Key + "=\"" + item.Value + "\""; ;
            }
            var selected = "selected=\"selected\"";
            if (!isSelected) selected = "";
            html = "<option value=\"" + value + "\" " + selected + " " + html + ">" + text + "</option>";
            return html;
        }

        //public string LoadCity()
        //{
        //    var list = new CityCodeRepository().Get();
        //    var html = list.Aggregate(Option("", "Please Select"), (current, item) => current + (Option(item.Code, item.Code, new KeyValuePair<string, string>("mytag", item.Id + ""))));
        //    return html;
        //}
        protected static IList CreateGenericList(Type collectionType)
        {
            var listType = typeof(List<>).MakeGenericType(new[] { collectionType });
            return (IList)Activator.CreateInstance(listType);
        }
        protected object SetJsonValue(object record, Dictionary<string, bool> AllowedChid, params string[] ignoreProperty)
        {
            var t = record.GetType();
            var src = Activator.CreateInstance(t.BaseType);
            var NewRecord = new ExpandoObject();
            try
            {

                var colNames = record.GetType().GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).Select(a => new { a.Name, a.PropertyType }).ToList();
                colNames = colNames.Where(p => !ignoreProperty.Contains(p.Name) || !ignoreProperty.Contains(p.PropertyType.Name)).ToList();

                foreach (var item in colNames)
                {

                    var columnName = item.Name;
                    if (ignoreProperty.Contains(columnName))
                        continue;
                    var type = item.PropertyType + "";
                    var propertyInfo = src.GetType().GetProperty(item.Name);
                    var srcchild = AllowedChid.ContainsKey(columnName);
                    if (srcchild)
                    {

                        if (AllowedChid[columnName])
                        {
                            var childtype = (GetPropValue(record, item.Name)).GetType();
                            dynamic chidlrecord = GetPropValue(record, item.Name);
                            dynamic inputData = CreateGenericList(childtype);
                            var list = new List<object>();
                            foreach (var child in chidlrecord)
                            {
                                var filterchildobj = GetChild(child, ignoreProperty);
                                list.Add(filterchildobj);

                            }

                            if (inputData != null)
                            {
                                Impromptu.InvokeSet(NewRecord, columnName, list);
                                // propertyInfo.SetValue(src, inputData, null);
                            }
                        }
                        else
                        {
                            var filterchildobj = GetChild(GetPropValue(record, item.Name), ignoreProperty.ToArray());
                            Impromptu.InvokeSet(NewRecord, columnName, filterchildobj);
                        }
                    }
                    else
                    {

                        var newval = GetPropValue(record, item.Name);
                        if (newval != null)
                        {
                            //  propertyInfo.SetValue(src, newval, null);
                            Impromptu.InvokeSet(NewRecord, columnName, newval);
                        }
                    }
                }
                return NewRecord;
            }
            catch (Exception)
            {
                return NewRecord;
            }

        }
        //public object SetJsonValue(object record, params string[] ignoreProperty)
        //{
        //    Type t = record.GetType();
        //    var src = Activator.CreateInstance(t.BaseType);
        //    ExpandoObject NewRecord = new ExpandoObject();
        //    try
        //    {

        //        var colNames = record.GetType().GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).Select(a => new { a.Name, a.PropertyType }).ToList();
        //        colNames = colNames.Where(p => !ignoreProperty.Contains(p.Name) || !ignoreProperty.Contains(p.PropertyType.Name)).ToList();

        //        foreach (var item in colNames)
        //        {
        //            if (ignoreProperty.Contains(item.Name))
        //                continue;
        //            string columnName = item.Name;
        //            string type = item.PropertyType + "";
        //            PropertyInfo propertyInfo = src.GetType().GetProperty(item.Name);
        //            var newval = GetPropValue(record, item.Name);
        //            Impromptu.InvokeSet(NewRecord, columnName, newval);

        //        }

        //        return NewRecord;
        //    }
        //    catch (Exception)
        //    {
        //        return NewRecord;
        //    }

        //}
        protected object GetChild(object record, params string[] ignoreProperty)
        {

            try
            {




                var colNames = record.GetType().GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).Select(a => new { a.Name, a.PropertyType }).ToList();
                colNames = colNames.Where(p => !ignoreProperty.Contains(p.Name) || !ignoreProperty.Contains(p.PropertyType.Name)).ToList();
                var t = record.GetType();
                var src = Activator.CreateInstance(t.BaseType);


                foreach (var item in colNames)
                {
                    if (ignoreProperty.Contains(item.Name))
                        continue;

                    var columnName = item.Name;
                    var type = item.PropertyType + "";
                    var propertyInfo = src.GetType().GetProperty(item.Name);
                    var newval = GetPropValue(record, item.Name);
                    if (newval != null)
                        propertyInfo.SetValue(src, newval, null);

                }
                return src;
            }
            catch (Exception)
            {

                return null;
            }

        }
        protected object GetPropValue(object src, string propName)
        {
            return src.GetType().GetProperty(propName).GetValue(src, null);
        }
        public static string GetEditIcon(object id)
        {

            return "<i class='fa fa-edit' onclick=\"Base.Edit(" + id.ToString() + ")\" title='Edit' ></i>";

        }
        public static string GetEditIcon(object id, string classsname)
        {
            return "<i class='fa fa-edit' onclick=\"" + classsname + ".Edit(" + id.ToString() + ")\" title='Edit' ></i>";
        }
        public static string GetDeleteIcon(object id)
        {
            return "<i class='fa fa-trash-o' onclick=\"Base.Delete(" + id.ToString() + ")\" title='Delete' ></i>";
        }
        public static string GetDeleteIcon(object id, string classsname)
        {
            return "<i class='fa fa-trash-o' onclick=\"" + classsname + ".Delete(" + id.ToString() + ")\" title='Delete' ></i>";
        }
        public static string GetPrintIcon(object id, string classsname)
        {
            return "<i class='fa fa-print' onclick=\"" + classsname + ".GetPrintDetail(" + id.ToString() + ")\" title='Print' ></i>";
        }
    }
}