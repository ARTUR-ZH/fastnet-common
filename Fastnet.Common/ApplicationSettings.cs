using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Fastnet.Common
{
    public static class ApplicationSettings
    {

        public static T Key<T>(string name, T defaultValue)
        {
            string valueSetting = ConfigurationManager.AppSettings[name];
            if (valueSetting == null)
            {
                return defaultValue;
            }
            return TConverter.ChangeType<T>(valueSetting);
        }
    }
}
