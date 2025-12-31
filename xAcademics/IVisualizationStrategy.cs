using System.Windows.Forms;

namespace xAcademics
{
    public interface IVisualizationStrategy
    {
        void BeginVisualization(Panel panel);
        void VisualizeFolder(Folder folder, int depth);
        void VisualizeFile(FileItem file, int depth);
        void EndVisualization();
    }
}