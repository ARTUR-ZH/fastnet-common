using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;

namespace Fastnet.Web.Common
{
    public class TemplateLibrary //: CustomFactory !! add this when customisinig for DWH
    {
        private class Templates
        {
            // key = template name, value fullpath
            private Dictionary<string, string> dict = new Dictionary<string, string>();
            public void Add(string name, string path)
            {
                dict.Add(name, path);
            }
            public string Get(string name)
            {
                if (dict.ContainsKey(name))
                {
                    return dict[name];
                }
                return null;
            }
        }
        public static void ScanForTemplates()
        {
            var mainTemplateFolder = new System.IO.DirectoryInfo(HostingEnvironment.MapPath("~/Templates"));
            if (System.IO.Directory.Exists(mainTemplateFolder.FullName))
            {
                LoadTemplateInfo(mainTemplateFolder);
            }
            var areasDi = new System.IO.DirectoryInfo(HostingEnvironment.MapPath("~/Areas"));
            foreach (System.IO.DirectoryInfo di in areasDi.GetDirectories())
            {
                //Debug.Print("area {0} found", di.Name);
                var tf = System.IO.Path.Combine(di.FullName, "Templates");
                if (System.IO.Directory.Exists(tf))
                {
                    LoadTemplateInfo(new System.IO.DirectoryInfo(tf));
                }
            }
        }
        public static TemplateLibrary GetInstance()
        {
            var app = HttpContext.Current.Application;
            if (app.Get("template-library") == null)
            {
                app.Set("template-library", new TemplateLibrary());
            }
            return app.Get("template-library") as TemplateLibrary;
        }
        // key = location, value = list of templates
        private Dictionary<string, Templates> templatesByLocation = new Dictionary<string, Templates>();
        private TemplateLibrary()
        {

        }
        public void AddTemplate(string location, string name, string path)
        {
            location = location.ToLower();
            name = name.ToLower();
            if (!templatesByLocation.ContainsKey(location))
            {
                templatesByLocation.Add(location, new Templates());
            }
            Templates templates = templatesByLocation[location];
            templates.Add(name, path);
        }
        public string GetTemplate(string location, string name)
        {
            FileInfo file;
            return GetTemplate(location, name, out file);
        }
        public string GetTemplate(string location, string name, out FileInfo file)
        {
            location = location.ToLower();
            name = name.ToLower();
            if (templatesByLocation.ContainsKey(location))
            {
                Templates templates = templatesByLocation[location];
                string text = ReadText(templates.Get(name), out file);
                return text;
            }
            file = null;
            return null;
        }
        private static string ReadText(string fn, out FileInfo file)
        {
            if (fn != null)
            {
                file = new FileInfo(fn);
                return File.ReadAllText(file.FullName);
            }
            else
            {
                file = null;
                return string.Empty;
            }
        }
        private static void LoadTemplateInfo(System.IO.DirectoryInfo templateFolder)
        {
            var templateLibrary = TemplateLibrary.GetInstance();
            Action<string, System.IO.DirectoryInfo> findHtmlFiles = (location, di) =>
            {
                var files = di.EnumerateFiles("*.html");
                foreach (System.IO.FileInfo file in files)
                {
                    //Debug.Print("Add location {0}, file {1}", location, System.IO.Path.GetFileNameWithoutExtension(file.Name));
                    templateLibrary.AddTemplate(location, System.IO.Path.GetFileNameWithoutExtension(file.Name), file.FullName);
                }
            };
            string appName = "main";
            if (string.Compare(templateFolder.Parent.Parent.Name, "Areas", true) == 0)
            {
                appName = templateFolder.Parent.Name.ToLower();
            }
            Debug.Print("loading templates for {0}", appName);
            findHtmlFiles(appName, templateFolder);
            var directories = templateFolder.EnumerateDirectories("*", System.IO.SearchOption.AllDirectories);
            foreach (System.IO.DirectoryInfo dir in directories)
            {
                string location = appName + "-" + dir.FullName.Substring(dir.FullName.ToLower().IndexOf("templates\\") + 10);
                findHtmlFiles(location.Replace("\\", "-").ToLower(), dir);
            }
            //Application["td"] = templateLibrary;
        }
    }
}
