using System;
using System.Drawing;
using System.Windows.Forms;

namespace xAcademics
{
    public class BarChartVisualizationStrategy : IVisualizationStrategy
    {
        private Panel visualizationPanel;
        private int yPosition;
        private long maxSize = 1;

        public void BeginVisualization(Panel panel)
        {
            this.visualizationPanel = panel;
            this.visualizationPanel.Controls.Clear();
            yPosition = 20;
            maxSize = 1;
        }

        public void VisualizeFolder(Folder folder, int depth)
        {
            maxSize = Math.Max(maxSize, folder.Size);

            string indent = new string(' ', depth * 2);
            string text = $"{indent}📁 {folder.Name}";

            Label label = new Label
            {
                Text = text,
                Location = new Point(20, yPosition),
                Size = new Size(200, 24),
                Font = new Font("Arial", 10, FontStyle.Bold),
                ForeColor = Color.DarkBlue,
                TextAlign = ContentAlignment.MiddleLeft
            };

            visualizationPanel.Controls.Add(label);

            if (folder.Size > 0)
            {
                int barWidth = (int)((folder.Size * 400) / maxSize);
                barWidth = Math.Max(barWidth, 50);

                ProgressBar bar = new ProgressBar
                {
                    Minimum = 0,
                    Maximum = 100,
                    Value = (int)((folder.Size * 100) / maxSize),
                    Location = new Point(230, yPosition),
                    Size = new Size(barWidth, 20),
                    Style = ProgressBarStyle.Continuous,
                    ForeColor = Color.RoyalBlue
                };

                Label sizeLabel = new Label
                {
                    Text = FormatSize(folder.Size),
                    Location = new Point(240 + barWidth, yPosition),
                    Size = new Size(80, 20),
                    Font = new Font("Arial", 9),
                    ForeColor = Color.DarkGreen,
                    TextAlign = ContentAlignment.MiddleLeft
                };

                visualizationPanel.Controls.Add(bar);
                visualizationPanel.Controls.Add(sizeLabel);
            }

            yPosition += 30;
        }

        public void VisualizeFile(FileItem file, int depth)
        {
            maxSize = Math.Max(maxSize, file.Size);

            string indent = new string(' ', (depth + 1) * 2);
            string text = $"{indent}📄 {file.Name}";

            Label label = new Label
            {
                Text = text,
                Location = new Point(20, yPosition),
                Size = new Size(200, 22),
                Font = new Font("Arial", 9),
                ForeColor = Color.DimGray,
                TextAlign = ContentAlignment.MiddleLeft
            };

            visualizationPanel.Controls.Add(label);

            if (file.Size > 0)
            {
                int barWidth = (int)((file.Size * 400) / maxSize);
                barWidth = Math.Max(barWidth, 30);

                ProgressBar bar = new ProgressBar
                {
                    Minimum = 0,
                    Maximum = 100,
                    Value = (int)((file.Size * 100) / maxSize),
                    Location = new Point(230, yPosition),
                    Size = new Size(barWidth, 16),
                    Style = ProgressBarStyle.Continuous,
                    ForeColor = Color.SeaGreen
                };

                string info = $"{FormatSize(file.Size)} | .{file.Extension}";
                Label infoLabel = new Label
                {
                    Text = info,
                    Location = new Point(240 + barWidth, yPosition),
                    Size = new Size(100, 16),
                    Font = new Font("Arial", 8),
                    ForeColor = Color.Gray,
                    TextAlign = ContentAlignment.MiddleLeft
                };

                visualizationPanel.Controls.Add(bar);
                visualizationPanel.Controls.Add(infoLabel);
            }

            yPosition += 28;
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