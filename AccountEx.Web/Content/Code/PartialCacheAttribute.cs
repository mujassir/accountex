using System.Web.Configuration;
using System.Web.Mvc;

namespace AccountEx.Web.Code
{
    public class PartialCacheAttribute : OutputCacheAttribute
    {
        public PartialCacheAttribute(string cacheProfileName)
        {
            var cacheSection = (OutputCacheSettingsSection)WebConfigurationManager
                                 .GetSection("system.web/caching/outputCacheSettings");

            var cacheProfile = cacheSection.OutputCacheProfiles[cacheProfileName];

            Duration = cacheProfile.Duration;
            VaryByParam = cacheProfile.VaryByParam;
        }
    }
}