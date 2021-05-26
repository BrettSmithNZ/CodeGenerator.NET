using System;
using System.IO;
using System.Linq;
using CodeGenerator.Content;
using CodeGenerator.Settings;

namespace CodeGenerator.CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Specify your solution directory");
            string dir =  Console.ReadLine() ;
            string[] projects = Directory.GetDirectories(dir);
            foreach (var project in projects)
            {
                string templateDirectory = Path.Combine(project, "_generator", "templates");
                string buildDefinitionPath = Path.Combine(project, "_generator", "project-build.yaml");
                if (File.Exists(buildDefinitionPath))
                {
                    string yamlContent = File.ReadAllText(buildDefinitionPath);
                    var buildDefinition = Yaml.ParseYaml(yamlContent);

                    if (File.Exists(Path.Combine(project, "class1.cs")))
                    {
                        File.Delete(Path.Combine(project, "class1.cs"));
                    }


                    var folders = buildDefinition.Templates.Select(o => o.Name);

                    foreach (var folder in folders)
                    {
                        if (Directory.Exists(Path.Combine(project, folder)))
                        {
                            Directory.Delete(Path.Combine(project, folder), true);
                        }
                        Directory.CreateDirectory(Path.Combine(project, folder));

                    }

                    string typeName = buildDefinition.Resource.Singular();
                    string nameSpace = new DirectoryInfo(project).Name;

                    foreach (var template in buildDefinition.Templates)
                    {
                        string templateName = template.Name;

                        if (template.Consolidated == true)
                        {
                            if (string.IsNullOrWhiteSpace(template.FileName))
                            {
                                throw new Exception($"Template parts are not supported for combined models, please specify filename for template {templateName}");
                            }
                            string fileName = template.FileName.Replace("%resource%", typeName);
                            string[] templateFiles = Directory.GetFiles(templateDirectory, $"{templateName}.*");
                            foreach (var templateFile in templateFiles.Where(o => o.EndsWith(".model.template") == false))
                            {
                                string modelContent = string.Join("\n\t\t", buildDefinition.Models.Select(classModel =>
                                   {
                                       string classModelTemplate = File.ReadAllText(templateFile.Replace(".template", ".model.template"));
                                       return classModelTemplate.Content(classModel);
                                   }));
                                if (templateFile.Contains(".interface.") == true)
                                {
                                    fileName = $"I{fileName}";
                                }
                                string templateContent = File.ReadAllText(templateFile);
                                File.WriteAllText(Path.Combine(project, templateName, fileName),
                                    templateContent.Replace("%modelContent%", modelContent).Globals(nameSpace, typeName));
                            }

                        }
                        else
                        {
                            foreach (var classModel in buildDefinition.Models)
                            {
                                if (template.Parts?.Any() == true)
                                {
                                    foreach (var templatePart in template.Parts)
                                    {
                                        string templatePartContent = File.ReadAllText(Path.Combine(templateDirectory, $"{templateName}.{templatePart.Name}.template"));
                                        string fileName = Path.Combine(project, templateName, templatePart.FileName.Replace("%model%", classModel.Model));
                                        File.WriteAllText(fileName, templatePartContent.Content(classModel).Globals(nameSpace, typeName));
                                    }
                                }
                                else
                                {
                                    if (string.IsNullOrWhiteSpace(template.FileName))
                                    {
                                        throw new Exception($"Template parts are not supported for combined models, please specify filename for template {templateName}");
                                    }
                                    string templateContent = File.ReadAllText(Path.Combine(templateDirectory, $"{templateName}.class.template"));
                                    string fileName = Path.Combine(project, templateName, template.FileName.Replace("%model%", classModel.Model));
                                    File.WriteAllText(fileName, templateContent.Content(classModel).Globals(nameSpace, typeName));

                                }
                            }

                        }
                    }



                    Console.WriteLine(buildDefinitionPath);
                }

            }
            Console.WriteLine("Finsihed Writing code");
        }
    }
}
