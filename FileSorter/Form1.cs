using System.Text;

namespace FileSorter
{
    public partial class FileSorter : Form
    {
        StringBuilder builtString = new StringBuilder();
        List<string> allFiles;
        string path;

        public FileSorter()
        {
            InitializeComponent();
            allFiles = new List<string>();
            TextBox.CheckForIllegalCrossThreadCalls = false;
        }

        public void Start()
        {
            using (var fbd = new FolderBrowserDialog())
            {
                var result = fbd.ShowDialog();
                path = fbd.SelectedPath;

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(path))
                {
                    var folders = Directory.GetDirectories(path);

                    for (var i = 0; i < folders.Length; i++)
                    {
                        var folder = folders[i];
                        var files = Directory.GetFiles(folder);

                        foreach (var file in files)
                        {
                            allFiles.Add(file);
                        }
                    }
                }
            }
        }

        public void CreateDirs()
        {
            foreach (var file in allFiles)
            {
                var nextPath = path.Replace("DCIM", "") + "output/";
                Directory.CreateDirectory(nextPath);
                var modifyTime = File.GetLastWriteTime(file).ToString().Split(' ')[0].Replace('/','-');
                string newModifyTime;

                var reversedTime = Reverse(modifyTime);
                var timeArray = reversedTime.Split('-');
                for (var i = 0; i < timeArray.Length; i++)
                {
                    builtString.Append(Reverse(timeArray[i]));
                    builtString.Append('-');
                }
                newModifyTime = builtString.ToString().Remove(builtString.Length - 1);
                var fileDirectory = nextPath + newModifyTime;
                builtString.Clear();

                Directory.CreateDirectory(fileDirectory);

                CopyFile(file, fileDirectory);
            }
        }

        private void CopyFile(string file, string path)
        {
            textBox1.Text = file;
            var name = Path.GetFileName(file);
            var dest = Path.Combine(path, name);
            var counter = 1;

            if (File.Exists(dest))
            {
                string existingFile;
                if (dest.Contains("HEIC"))
                {
                    existingFile = dest.Insert(dest.Length - 5, counter.ToString());
                } else
                {
                    existingFile = dest.Insert(dest.Length - 4, counter.ToString());
                }
                File.Copy(file, existingFile);
                counter++;
            } else
            {
                File.Copy(file, dest);
            }
        }

        public string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        private async Task Starter()
        {
            Start();
            Loading.Visible = true;

            StartButton.Enabled = false;
            StartButton.Image = Properties.Resources.Start2;
            await Task.Run(() => CreateDirs());

            StartButton.Image = Properties.Resources.Done1;
            Loading.Visible = false;
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            _ = Starter();
        }
    }
}
