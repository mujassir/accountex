using System.Configuration;
using System.Web.Configuration;
using System.Xml;

namespace AccountEx.Common
{
    public partial class SiteConfig : IConfigurationSectionHandler
    {
        #region Fields
        private static string _connectionString = string.Empty;
        private static bool _initialized = false;
        private static int _cookieExpires = 128;
        private static bool _cacheEnabled = false;
        private static XmlNode _scheduleTasks;
        #endregion

        #region Methods
        /// <summary>
        /// Creates a configuration section handler.
        /// </summary>
        /// <param name="parent">Parent object.</param>
        /// <param name="configContext">Configuration context object.</param>
        /// <param name="section">Section XML node.</param>
        /// <returns>The created section handler object.</returns>
        public object Create(object parent, object configContext, XmlNode section)
        {
            var sqlServerNode = section.SelectSingleNode("SqlServer");
            if (sqlServerNode != null)
            {
                if (sqlServerNode.Attributes != null)
                {
                    var attribute = sqlServerNode.Attributes["ConnectionStringName"];
                    if ((attribute != null) && (WebConfigurationManager.ConnectionStrings[attribute.Value] != null))
                    {
                        _connectionString = WebConfigurationManager.ConnectionStrings[attribute.Value].ConnectionString;
                    }
                }
            }

            var cacheNode = section.SelectSingleNode("Cache");
            if (cacheNode != null)
            {
                if (cacheNode.Attributes != null)
                {
                    var attribute = cacheNode.Attributes["Enabled"];
                    if (attribute != null && attribute.Value != null)
                    {
                        var str1 = attribute.Value.ToUpperInvariant();
                        _cacheEnabled = (str1 == "TRUE" || str1 == "YES" || str1 == "1");
                    }
                }
            }

            _scheduleTasks = section.SelectSingleNode("ScheduleTasks");

            return null;
        }

        /// <summary>
        /// Initializes the SiteConfig object
        /// </summary>
        public static void Init()
        {
            if (!_initialized)
            {
                ConfigurationManager.GetSection("SiteConfig");
                _initialized = true;
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the connection string that is used to connect to the storage
        /// </summary>
        public static string ConnectionString
        {
            get
            {
                return _connectionString;
            }
            set
            {
                _connectionString = value;
            }
        }

        /// <summary>
        /// Gets or sets the expiration date and time for the Cookie in hours
        /// </summary>
        public static int CookieExpires
        {
            get
            {
                return _cookieExpires;
            }
            set
            {
                _cookieExpires = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the cache is enabled
        /// </summary>
        public static bool CacheEnabled
        {
            get
            {
                return true;
                // return _cacheEnabled;
            }
            set
            {
                _cacheEnabled = value;
            }
        }

        /// <summary>
        /// Gets or sets a schedule tasks section
        /// </summary>
        public static XmlNode ScheduleTasks
        {
            get
            {
                return _scheduleTasks;
            }
            set
            {
                _scheduleTasks = value;
            }
        }
        #endregion
    }

}
