namespace Redox.DataModel.Builder.Services
{
    public class ModelBuildStatistics : IModelBuildStatistics
    {
        private readonly object _lock = new object();
        private long _createdFiles;
        private long _totalFiles;

        public void Initialize(long totalFiles)
        {
            _totalFiles = totalFiles;
        }

        public string Update(string fileName)
        {
            lock (_lock)
            {
                ++_createdFiles;
            }

            return $"Creating data model file {fileName} ({Progress(_createdFiles, _totalFiles)})";
        }

        private static string Progress(long current, long total)
        {
            return $"{(int)(100.0 * current / total)}%";
        }
    }
}