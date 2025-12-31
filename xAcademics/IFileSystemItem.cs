using System;

namespace xAcademics
{
    public interface IFileSystemItem
    {
        string Name { get; }
        long Size { get; }
        long CalculateSize();
        void Visualize(IVisualizationStrategy strategy, int depth);
        string Extension { get; }
    }
}