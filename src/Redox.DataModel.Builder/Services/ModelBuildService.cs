namespace Redox.DataModel.Builder.Services
{
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using NJsonSchema;
    using NJsonSchema.CodeGeneration.CSharp;
    using Redox.DataModel.Builder.Configuration;
    using Redox.DataModel.Builder.Extensions;
    using Redox.DataModel.Builder.Services.Helpers;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;

    public class ModelBuildService : IModelBuildService
    {
        private readonly IModelBuildStatistics _statistics;
        private readonly AppConfig _config;
        private readonly ITaskHelper _enumerableHelper;

        public ModelBuildService(IOptions<AppConfig> config, IModelBuildStatistics statistics,
            ITaskHelper enumerableHelper)
        {
            _config = config.Value;
            _statistics = statistics;
            _enumerableHelper = enumerableHelper;
        }

        public async Task ProcessAsync(Dictionary<string, byte[]> jsonSchemaFiles, int maxThreads = 4)
        {
            _statistics.Initialize(jsonSchemaFiles.Count);
            try
            {
                async Task BuildFileAsync(KeyValuePair<string, byte[]> jsonFile, IModelBuildStatistics statistics)
                {
                    var modelFileName = Path.Combine(_config.DataModelFolder, jsonFile.Key).Replace("/", "\\").Replace(".json", ".cs");
                    modelFileName = NormalizeFilePath(modelFileName);
                    await BuildModelFileAsync(modelFileName, jsonFile.Value).ConfigureAwait(false);
                    Program.Logger.LogInformation(statistics.Update(modelFileName));
                }

                await _enumerableHelper.ForEachAsync(
                    source: jsonSchemaFiles,
                    partitionCount: maxThreads,
                    body: jsonFile => BuildFileAsync(jsonFile, _statistics)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Program.Logger.LogError(ex.Message);
            }
        }

        private async Task BuildModelFileAsync(string fileName, byte[] fileContent)
        {
            var newFileDirectory = new FileInfo(fileName).Directory;
            newFileDirectory.Create();
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
            var className = Path.GetFileNameWithoutExtension(fileName).FirstCharToUpper();
            var nameSpace = string.Join(".", _config.DefaultNamespace, newFileDirectory.Name.FirstCharToUpper(), className);
            var jsonFile = Encoding.UTF8.GetString(fileContent);
            var schema = JsonSchema.FromJsonAsync(jsonFile).Result;
            var settings = new CSharpGeneratorSettings
            {
                ClassStyle = CSharpClassStyle.Poco,
                HandleReferences = true,
                Namespace = nameSpace,
                SchemaType = SchemaType.JsonSchema
            };
            var generator = new CSharpGenerator(schema, settings);
            var fileContents = generator.GenerateFile(className);
            if (!string.IsNullOrEmpty(fileContents))
            {
                await File.WriteAllTextAsync(fileName, fileContents).ConfigureAwait(false);
            }
            else
            {
                Console.WriteLine($"Something went wrong with {fileName}");
            }
        }

        private static string NormalizeFilePath(string fileName)
        {
            var result = string.Empty;
            var pathParts = fileName.Split(Path.DirectorySeparatorChar);
            foreach (var part in pathParts)
            {
                result = Path.Combine(result, part.FirstCharToUpper());
            }
            return result;
        }
    }
}
