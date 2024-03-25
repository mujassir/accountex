using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Web.Http;

namespace AccountEx.Web
{
    public static class WebApiConfig
    {
        public static string UrlPrefix { get { return "api"; } }
        public static string UrlPrefixRelative { get { return "~/api"; } }
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            //config.Formatters.Add(new BrowserJsonFormatter());
            //config.Formatters.Remove(config.Formatters.XmlFormatter);
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver =
            new DefaultContractResolver { IgnoreSerializableAttribute = true };
        }
        public class BrowserJsonFormatter : JsonMediaTypeFormatter
        {
            public BrowserJsonFormatter()
            {
                SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
                SerializerSettings.Formatting = Formatting.Indented;
            }

            public override void SetDefaultContentHeaders(Type type, HttpContentHeaders headers, MediaTypeHeaderValue mediaType)
            {
                base.SetDefaultContentHeaders(type, headers, mediaType);
                headers.ContentType = new MediaTypeHeaderValue("application/json");
            }
        }
    }
}
