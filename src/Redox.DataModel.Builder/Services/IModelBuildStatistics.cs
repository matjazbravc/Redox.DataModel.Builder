namespace Redox.DataModel.Builder.Services
{
    public interface IModelBuildStatistics
    {
        void Initialize(long totalFiles);

        string Update(string fileName);
    }
}