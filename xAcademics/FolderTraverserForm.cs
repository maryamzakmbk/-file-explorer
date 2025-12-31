using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace xAcademics
{
    public partial class FolderTraverserForm : Form
    {
        private Folder rootFolder;
        private IVisualizationStrategy currentStrategy;
        private Panel filesPanel;
        private Panel visualizationPanel;
        private Panel scrollPanel;
        private SplitContainer splitContainer;
        private Label statusLabel;

        private List<string> allFiles = new List<string>();
        private List<string> allFolders = new List<string>();

        public FolderTraverserForm()
        {
            InitializeComponent();
            InitializeCustomComponents();
        }

        private void InitializeCustomComponents()
        {
            Text = "Folder Traverser - File/Folder Visualization";
            Size = new Size(1200, 700);

            splitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical,
                SplitterDistance = 300, 
                FixedPanel = FixedPanel.Panel1
            };

            Panel leftPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.WhiteSmoke
            };

            Label leftTitle = new Label
            {
                Text = "📁 Files & Folders Area",
                Font = new Font("Arial", 12, FontStyle.Bold),
                ForeColor = Color.DarkSlateBlue,
                Dock = DockStyle.Top,
                Height = 40,
                TextAlign = ContentAlignment.MiddleCenter,
                BorderStyle = BorderStyle.FixedSingle
            };

            Panel filesScrollPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(5)
            };

            filesPanel = new Panel
            {
                AutoSize = true,
                Width = filesScrollPanel.ClientRectangle.Width - 20,
                BackColor = Color.White
            };

            filesScrollPanel.Controls.Add(filesPanel);

            leftPanel.Controls.Add(filesScrollPanel);
            leftPanel.Controls.Add(leftTitle);

            Panel rightPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };

            Panel controlPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.LightGray,
                BorderStyle = BorderStyle.FixedSingle
            };

            Button browseButton = new Button
            {
                Text = "📂 Browse Folder",
                Size = new Size(120, 35),
                Location = new Point(20, 12),
                BackColor = Color.SteelBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 9, FontStyle.Bold)
            };
            browseButton.Click += BrowseButton_Click;

            Button treeButton = new Button
            {
                Text = "🌳 Tree View",
                Size = new Size(100, 35),
                Location = new Point(150, 12),
                BackColor = Color.ForestGreen,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 9, FontStyle.Bold)
            };
            treeButton.Click += TreeButton_Click;

            Button barButton = new Button
            {
                Text = "📊 Bar Chart",
                Size = new Size(100, 35),
                Location = new Point(260, 12),
                BackColor = Color.Orange,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 9, FontStyle.Bold)
            };
            barButton.Click += BarButton_Click;

            statusLabel = new Label
            {
                Text = "Select a folder to begin...",
                Location = new Point(380, 18),
                Size = new Size(300, 25),
                Font = new Font("Arial", 9),
                ForeColor = Color.DarkSlateGray
            };

            controlPanel.Controls.Add(browseButton);
            controlPanel.Controls.Add(treeButton);
            controlPanel.Controls.Add(barButton);
            controlPanel.Controls.Add(statusLabel);

            Label rightTitle = new Label
            {
                Text = "📊 Visualization Area",
                Font = new Font("Arial", 12, FontStyle.Bold),
                ForeColor = Color.DarkSlateBlue,
                Dock = DockStyle.Top,
                Height = 40,
                TextAlign = ContentAlignment.MiddleCenter,
                BorderStyle = BorderStyle.FixedSingle
            };

            scrollPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };

            visualizationPanel = new Panel
            {
                AutoSize = true,
                Width = scrollPanel.ClientRectangle.Width - 20,
                BackColor = Color.White
            };

            scrollPanel.Controls.Add(visualizationPanel);

            rightPanel.Controls.Add(scrollPanel);
            rightPanel.Controls.Add(rightTitle);
            rightPanel.Controls.Add(controlPanel);

            splitContainer.Panel1.Controls.Add(leftPanel);
            splitContainer.Panel2.Controls.Add(rightPanel);

            Controls.Add(splitContainer);

            currentStrategy = new TreeVisualizationStrategy();

        }

        private void BrowseButton_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Select a folder to analyze";
                dialog.ShowNewFolderButton = false;
                dialog.RootFolder = Environment.SpecialFolder.Desktop;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    TraverseFolder(new DirectoryInfo(dialog.SelectedPath));
                }
            }
        }

        private void TreeButton_Click(object sender, EventArgs e)
        {
            currentStrategy = new TreeVisualizationStrategy();
            if (rootFolder != null)
                VisualizeFolder();
        }

        private void BarButton_Click(object sender, EventArgs e)
        {
            currentStrategy = new BarChartVisualizationStrategy();
            if (rootFolder != null)
                VisualizeFolder();
        }

        private void TraverseFolder(DirectoryInfo directory)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                statusLabel.Text = "Processing folder...";
                Refresh();

                allFiles.Clear();
                allFolders.Clear();
                filesPanel.Controls.Clear();

                rootFolder = new Folder(directory.Name);
                BuildCompositeStructure(directory, rootFolder);

                long totalSize = rootFolder.CalculateSize();
                statusLabel.Text = $"Total: {FormatSize(totalSize)} | Files: {allFiles.Count} | Folders: {allFolders.Count}";

                DisplayFilesAndFolders();

                VisualizeFolder();
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show($"Access denied: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void BuildCompositeStructure(DirectoryInfo currentDir, Folder currentFolder)
        {
            try
            {
 
                allFolders.Add(currentDir.FullName);


                foreach (var file in currentDir.GetFiles())
                {
                    allFiles.Add(file.FullName);
                    currentFolder.AddChild(new FileItem(file.Name, file.Length));
                }

                foreach (var subDir in currentDir.GetDirectories())
                {
                    Folder subFolder = new Folder(subDir.Name);
                    currentFolder.AddChild(subFolder);
                    BuildCompositeStructure(subDir, subFolder);
                }
            }
            catch (UnauthorizedAccessException)
            {

            }
        }

        private void DisplayFilesAndFolders()
        {
            int yPos = 10;

            Label filesTitle = new Label
            {
                Text = "📄 FILES",
                Location = new Point(10, yPos),
                Size = new Size(250, 25),
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = Color.DarkSlateBlue
            };
            filesPanel.Controls.Add(filesTitle);
            yPos += 30;

            foreach (var filePath in allFiles)
            {
                FileInfo file = new FileInfo(filePath);

                Label fileLabel = new Label
                {
                    Text = $"  📄 {file.Name}",
                    Location = new Point(10, yPos),
                    Size = new Size(200, 20),
                    Font = new Font("Arial", 9),
                    ForeColor = Color.DimGray,
                    Cursor = Cursors.Hand
                };
                fileLabel.Click += (s, e) => ShowFileInfo(file);

                Label sizeLabel = new Label
                {
                    Text = FormatSize(file.Length),
                    Location = new Point(220, yPos),
                    Size = new Size(70, 20),
                    Font = new Font("Arial", 8),
                    ForeColor = Color.Gray,
                    TextAlign = ContentAlignment.MiddleRight
                };

                filesPanel.Controls.Add(fileLabel);
                filesPanel.Controls.Add(sizeLabel);
                yPos += 22;
            }

            yPos += 10;

            Label foldersTitle = new Label
            {
                Text = "📁 FOLDERS",
                Location = new Point(10, yPos),
                Size = new Size(250, 25),
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = Color.DarkSlateBlue
            };
            filesPanel.Controls.Add(foldersTitle);
            yPos += 30;

            foreach (var folderPath in allFolders)
            {
                DirectoryInfo dir = new DirectoryInfo(folderPath);

                Label folderLabel = new Label
                {
                    Text = $"  📁 {dir.Name}",
                    Location = new Point(10, yPos),
                    Size = new Size(250, 20),
                    Font = new Font("Arial", 9),
                    ForeColor = Color.DarkBlue,
                    Cursor = Cursors.Hand
                };
                folderLabel.Click += (s, e) => ShowFolderInfo(dir);

                filesPanel.Controls.Add(folderLabel);
                yPos += 22;
            }

            filesPanel.Height = yPos + 10;
        }

        private void ShowFileInfo(FileInfo file)
        {
            MessageBox.Show(
                $"File: {file.Name}\n" +
                $"Size: {FormatSize(file.Length)}\n" +
                $"Extension: {file.Extension}\n" +
                $"Path: {file.DirectoryName}\n" +
                $"Created: {file.CreationTime}",
                "File Information",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        private void ShowFolderInfo(DirectoryInfo dir)
        {
            try
            {
                int fileCount = dir.GetFiles().Length;
                int folderCount = dir.GetDirectories().Length;

                MessageBox.Show(
                    $"Folder: {dir.Name}\n" +
                    $"Path: {dir.FullName}\n" +
                    $"Contains: {fileCount} files, {folderCount} subfolders\n" +
                    $"Created: {dir.CreationTime}\n" +
                    $"Last Modified: {dir.LastWriteTime}",
                    "Folder Information",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch
            {
                MessageBox.Show("Cannot access folder information.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void VisualizeFolder()
        {
            if (rootFolder == null) return;


            visualizationPanel.Width = scrollPanel.ClientRectangle.Width - 20;

            currentStrategy.BeginVisualization(visualizationPanel);

            rootFolder.Visualize(currentStrategy, 0);

            currentStrategy.EndVisualization();

            visualizationPanel.Refresh();
            scrollPanel.Refresh();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (visualizationPanel != null && scrollPanel != null)
            {
                visualizationPanel.Width = scrollPanel.ClientRectangle.Width - 20;
                if (rootFolder != null)
                    VisualizeFolder();
            }
        }

        private string FormatSize(long size)
        {
            if (size < 1024) return $"{size} B";
            else if (size < 1024 * 1024) return $"{(size / 1024.0):F1} KB";
            else if (size < 1024 * 1024 * 1024) return $"{(size / (1024.0 * 1024.0)):F1} MB";
            else return $"{(size / (1024.0 * 1024.0 * 1024.0)):F2} GB";
        }
    }
}