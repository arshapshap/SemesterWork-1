using HttpServer.Models;
using Scriban;
using Scriban.Parsing;
using Scriban.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.TemplateService
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

        public View(string templateFileName) : this(templateFileName, new { }) { }

        public string GetHTML(string path)
        {
            var template = File.ReadAllText($"{path}/{templateFileName}.sbnhtml");

            var parsedTemplate = Template.Parse(template);
            var result = Render(parsedTemplate, model, path);

            return result;
        }

        private string Render(Template template, object? model, string path)
        {
            var context = new TemplateContext();

            context.TemplateLoader = new TemplateLoader(path);
            var scriptObject = new ScriptObject();

            if (model != null)
                scriptObject.Import(model);

            context.PushGlobal(scriptObject);

            return template.Render(context);
        }
    }
}
