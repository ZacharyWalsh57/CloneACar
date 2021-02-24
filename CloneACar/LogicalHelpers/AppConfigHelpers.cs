using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace CloneACar.LogicalHelpers
{
    public static class AppConfigHelper
    {
        public static string ReturnConfigItem(string NameOfItem)
        {
            var ValueOfItem = ConfigurationManager.AppSettings[NameOfItem];
            if (ValueOfItem == null) { return ""; }

            return ValueOfItem;
        }

        public static List<Tuple<string, string>> ReturnDebugPaths()
        {
            string BasePath = AppConfigHelper.ReturnConfigItem("BaseLogPath");
            var PathsNamed = new List<Tuple<string, string>>();
            var AWSPaths = ConfigurationManager.GetSection("DebugLogDirectoryInformation") as NameValueCollection;
            for (var Index = 0; Index < AWSPaths.AllKeys.Length; Index++)
            {
                string PathForMake = AWSPaths.Get(Index);
                PathForMake = PathForMake.Replace("$BASELOG$", BasePath);

                PathsNamed.Add(new Tuple<string, string>(AWSPaths.GetKey(Index), PathForMake));
            }

            return PathsNamed;
        }
    }
}
