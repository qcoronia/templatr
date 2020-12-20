using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace templater
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0 && (new string[] { "?", "-h", "--help" }).Contains(args[0]))
            {
                Console.WriteLine("Usage:");
                Console.WriteLine("\ttemplater <template name>");
                Console.WriteLine("Description:");
                Console.WriteLine("\tThe templates are found beside templater.exe with .template as extension");
                Console.WriteLine("Template Tokens:");
                Console.WriteLine("\t{{fileName}} — replaces the token with file name without extension");
                return;
            }

            var workingDir = Directory.GetCurrentDirectory();
            var files = Directory.GetFiles(workingDir, "*.ts");
            foreach (var file in files)
            {
                Console.WriteLine($"\tFound:\t'{Path.GetExtension(file)} {Path.GetFileName(file)}'");
            }

            var templateFile = args.Length > 0 ? args[0] : string.Empty;
            var template = GetItemTemplate(templateFile);

            Console.WriteLine("Generating...");
            var filesToList = files.Where(e => Path.GetExtension(e) == ".ts" && Path.GetFileNameWithoutExtension(e) != "index");
            var output = string.Empty;
            foreach (var file in filesToList)
            {
                Console.WriteLine($"\tWriting:\t'{Path.GetFileNameWithoutExtension(file)}'");
                var templateOutput = template.Replace("{{fileName}}", Path.GetFileNameWithoutExtension(file));
                output += templateOutput;
            }

            var outputFile = Path.Combine(workingDir, "index.ts");
            File.WriteAllText(outputFile, output);
            Console.WriteLine("Completed");
            Console.WriteLine("Press any key to continue...");
            Console.ReadLine();
        }

        static string GetItemTemplate(string templateFile)
        {
            if (string.IsNullOrEmpty(templateFile))
            {
                return "{{fileName}}\r\n";
            }

            var locationFullPath = Assembly.GetEntryAssembly().Location;
            var locationFolder = Path.GetDirectoryName(locationFullPath);
            var templatePath = Path.Combine(locationFolder, $"{templateFile}.template");
            var lineTemplate = File.ReadAllText(templatePath);

            return lineTemplate;
        }
    }
}
