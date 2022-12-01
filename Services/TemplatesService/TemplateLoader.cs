using Scriban;
using Scriban.Parsing;
using Scriban.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.TemplatesService
{
    internal class TemplateLoader : ITemplateLoader
    {
        private string path;
        public TemplateLoader(string path)
        {
            this.path = path;
        }

        public string GetPath(TemplateContext context, SourceSpan callerSpan, string templateName)
        {
            return Path.Combine(Environment.CurrentDirectory, path, templateName);
        }

        public string Load(TemplateContext context, SourceSpan callerSpan, string templatePath)
        {
            return File.ReadAllText(templatePath);
        }

        public ValueTask<string> LoadAsync(TemplateContext context, SourceSpan callerSpan, string templatePath)
        {
            return new ValueTask<string>(File.ReadAllTextAsync(templatePath));
        }
    }
}
