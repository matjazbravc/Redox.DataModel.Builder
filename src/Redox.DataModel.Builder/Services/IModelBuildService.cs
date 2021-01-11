namespace Redox.DataModel.Builder.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IModelBuildService
    {
        Task ProcessAsync(Dictionary<string, byte[]> jsonSchemaFiles, int maxThreads = 4);
    }
}