using System.Collections.Generic;

namespace xAcademics
{
    public class Folder : IFileSystemItem
    {
        private string name;
        private List<IFileSystemItem> children;
        private long? cachedSize = null;

        public string Name => name;
        public long Size
        {
            get
            {
                if (!cachedSize.HasValue)
                    CalculateSize();
                return cachedSize.Value;
            }
        }
        public string Extension => "";
        public List<IFileSystemItem> Children => children;

        public Folder(string name)
        {
            this.name = name;
            this.children = new List<IFileSystemItem>();
        }

        public void AddChild(IFileSystemItem item)
        {
            children.Add(item);
            cachedSize = null;
        }

        public long CalculateSize()
        {
            cachedSize = 0;
            foreach (var child in children)
            {
                cachedSize += child.CalculateSize();
            }
            return cachedSize.Value;
        }

        public void Visualize(IVisualizationStrategy strategy, int depth)
        {
            strategy.VisualizeFolder(this, depth);
            foreach (var child in children)
            {
                child.Visualize(strategy, depth + 1);
            }
        }
    }
}
