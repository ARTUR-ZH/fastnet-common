using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.IO;
using Fastnet.Common;

namespace Fastnet.Web.Common
{
    public enum FactoryName
    {
        None,
        DonWhillansHut
    }
    public abstract class CustomFactory
    {
        public FactoryName FactoryName { get; private set; }
        public dynamic Settings { get; set; }
        public CustomFactory()
        {
            //string setting = ApplicationSettings.Key("Customisation:Factory", "None");
            Settings = GetSettings();
            if (Settings != null)
            {
                string factory = Settings.factory ?? "None";
                FactoryName = (FactoryName)Enum.Parse(typeof(FactoryName), factory, true);
            } else
            {
                FactoryName = FactoryName.None;
            }
        }
        private dynamic GetSettings()
        {
            var customisationFile = HostingEnvironment.MapPath("~/customisation.json");
            if (File.Exists(customisationFile))
            {
                string text = File.ReadAllText(customisationFile);
                return text.ToJsonDynamic();
            }
            else
            {
                return null;
            }
        }
    }

}
