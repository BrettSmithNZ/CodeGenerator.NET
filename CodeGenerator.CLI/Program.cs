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
            string baseDirectory = Console.ReadLine();

            string[] projects = Directory.GetDirectories(baseDirectory);
            foreach (var project in projects)
            {
                string templateDirectory = Path.Combine(project, "_generator", "templates");
                string buildDefinitionPath = Path.Combine(project, "_generator", "project-build.yaml");
                if (File.Exists(buildDefinitionPath) && !File.Exists(Path.Combine(project, "_generator", ".exclude")))
                {
                    string yamlContent = File.ReadAllText(buildDefinitionPath);
                    var buildDefinition = Yaml.ParseYaml(yamlContent);
                    if (!string.IsNullOrWhiteSpace(buildDefinition.TemplatesDefinition))
                    {
                        string templateDefinitionYaml = File.ReadAllText(buildDefinition.TemplatesDefinition.Replace("%baseDirectory%", baseDirectory));
                        var templateDefinition = Yaml.ParseYaml(templateDefinitionYaml);
                        buildDefinition.Templates = templateDefinition.Templates;
                    }

                    if (File.Exists(Path.Combine(project, "class1.cs")))
                    {
                        File.Delete(Path.Combine(project, "class1.cs"));
                    }
                    if (!string.IsNullOrWhiteSpace(buildDefinition.TemplateDirectory))
                    {
                        templateDirectory = buildDefinition.TemplateDirectory.Replace("%baseDirectory%", baseDirectory);
                    }

                    var folders = buildDefinition.Templates.Where(o => o.UseProjectDirectory != true).Select(o => o.Name);

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
                            //should consolidate typeName & resource
                            string fileName = template.FileName.Replace("%resource%", typeName).Globals(nameSpace, typeName);
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
                                string savePath = template.UseProjectDirectory == true ?
                                    Path.Combine(project, fileName) : Path.Combine(project, templateName, fileName);
                                File.WriteAllText(savePath,
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

                                        string fileName = templatePart.FileName.Model(classModel).Globals(nameSpace, typeName);
                                        string savePath = template.UseProjectDirectory == true ?
                                                Path.Combine(project, fileName) : Path.Combine(project, templateName, fileName);

                                        File.WriteAllText(savePath, templatePartContent.Content(classModel).Globals(nameSpace, typeName));
                                    }
                                }
                                else
                                {
                                    if (string.IsNullOrWhiteSpace(template.FileName))
                                    {
                                        throw new Exception($"Template parts are not supported for combined models, please specify filename for template {templateName}");
                                    }
                                    string templateContent = File.ReadAllText(Path.Combine(templateDirectory, $"{templateName}.class.template"));
                                    string fileName = template.FileName.Model(classModel).Globals(nameSpace, typeName);

                                    string savePath = template.UseProjectDirectory == true ?
                                            Path.Combine(project, fileName) : Path.Combine(project, templateName, fileName);
                                            
                                    File.WriteAllText(savePath, templateContent.Content(classModel).Globals(nameSpace, typeName));

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
