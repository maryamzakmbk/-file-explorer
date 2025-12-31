using System;
using System.Drawing;
using System.Windows.Forms;

namespace xAcademics
{
    public class TreeVisualizationStrategy : IVisualizationStrategy
    {
        private Panel visualizationPanel;
        private int yPosition;

        public void BeginVisualization(Panel panel)
        {
            this.visualizationPanel = panel;
            this.visualizationPanel.Controls.Clear();
            yPosition = 20;
        }

        public void VisualizeFolder(Folder folder, int depth)
        {
            string indent = new string(' ', depth * 4);
            string prefix = depth == 0 ? "•" : "└──";

            string text = $"{indent}{prefix} {folder.Name}";

            Label label = new Label
            {
                Text = text,
                Location = new Point(20, yPosition),
                Size = new Size(350, 24),
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = Color.DarkBlue,
                TextAlign = ContentAlignment.MiddleLeft
            };

            Label sizeLabel = new Label
            {
                Text = $"({FormatSize(folder.Size)})",
                Location = new Point(380, yPosition),
                Size = new Size(120, 24),
                Font = new Font("Arial", 10),
                ForeColor = Color.DarkGreen,
                TextAlign = ContentAlignment.MiddleRight
            };

            visualizationPanel.Controls.Add(label);
            visualizationPanel.Controls.Add(sizeLabel);
            yPosition += 26;
        }

        public void VisualizeFile(FileItem file, int depth)
        {
            string indent = new string(' ', (depth + 1) * 4);
            string text = $"{indent}- {file.Name}";

            Label label = new Label
            {
                Text = text,
                Location = new Point(20, yPosition),
                Size = new Size(350, 22),
                Font = new Font("Arial", 10),
                ForeColor = Color.DimGray,
                TextAlign = ContentAlignment.MiddleLeft
            };

            string info = $"[{FormatSize(file.Size)} | .{file.Extension}]";
            Label infoLabel = new Label
            {
                Text = info,
                Location = new Point(380, yPosition),
                Size = new Size(120, 22),
                Font = new Font("Courier New", 9),
                ForeColor = Color.Gray,
                TextAlign = ContentAlignment.MiddleRight
            };

            visualizationPanel.Controls.Add(label);
            visualizationPanel.Controls.Add(infoLabel);
            yPosition += 24;
        }

        public void EndVisualization()
        {
            visualizationPanel.Height = Math.Max(yPosition + 20, visualizationPanel.Parent.Height);
        }

        private string FormatSize(long size)
        {
            if (size < 1024) return $"{size} B";
            else if (size < 1024 * 1024) return $"{(size / 1024.0):F1} KB";
            else return $"{(size / (1024.0 * 1024.0)):F1} MB";
        }
    }
}