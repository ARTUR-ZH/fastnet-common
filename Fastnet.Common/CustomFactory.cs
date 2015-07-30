using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;
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
            string factory = Settings.factory ?? "None";
            FactoryName = (FactoryName)Enum.Parse(typeof(FactoryName), factory, true);
        }
        private dynamic GetSettings()
        {
            var customisationFile = HostingEnvironment.MapPath("~/customisation.json");
            string text = System.IO.File.ReadAllText(customisationFile);
            return text.ToJsonDynamic();
        }
    }

}
