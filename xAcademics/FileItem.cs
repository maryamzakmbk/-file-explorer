namespace xAcademics
{
    public class FileItem : IFileSystemItem
    {
        private string name;
        private long size;
        private string extension;

        public string Name => name;
        public long Size => size;
        public string Extension => extension;

        public FileItem(string name, long size)
        {
            this.name = name;
            this.size = size;

            int dotIndex = name.LastIndexOf('.');
            this.extension = (dotIndex > 0 && dotIndex < name.Length - 1) ?
                name.Substring(dotIndex + 1) : "";
        }

        public long CalculateSize() => size;

        public void Visualize(IVisualizationStrategy strategy, int depth)
        {
            strategy.VisualizeFile(this, depth);
        }
    }
}
