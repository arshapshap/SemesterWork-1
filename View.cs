using Scriban;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer
{
    internal class View
    {
        private string templateFileName;
        private object model;
        public User? CurrentUser { get; set; }

        public View(string templateFileName, object model)
        {
            this.templateFileName = templateFileName;
            this.model = model;
        }

        public string GetHTML(string path)
        {
            var page = GetHTMLFromFile($"{path}/{templateFileName}.html", model);
            var header = GetHTMLFromFile($"{path}/header.html", new { User = CurrentUser });
            page = page.Replace("*HEADER*", header);

            return page;
        }

        private string GetHTMLFromFile(string path, object model)
        {
            if (!File.Exists(path))
                return "";

            return GetHTML(File.ReadAllText(path), model);
        }

        private string GetHTML(string template, object model)
        {
            var parsedTemplate = Template.Parse(template);
            var result = parsedTemplate.Render(model);

            return result;
        }
    }
}
