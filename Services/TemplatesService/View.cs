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

namespace HttpServer.TemplatesService
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

        public async Task<string> GetHTMLAsync(string path)
        {
            var template = await File.ReadAllTextAsync($"{path}/{templateFileName}.sbnhtml");

            var parsedTemplate = Template.Parse(template);
            var result = await RenderAsync(parsedTemplate, model, path);

            return result;
        }

        private async Task<string> RenderAsync(Template template, object? model, string path)
        {
            var context = new TemplateContext();

            context.TemplateLoader = new TemplateLoader(path);
            var scriptObject = new ScriptObject();

            if (model is not null)
                scriptObject.Import(model);

            context.PushGlobal(scriptObject);

            return await template.RenderAsync(context);
        }
    }
}
