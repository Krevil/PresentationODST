using System.IO;
using System.Windows;

namespace PresentationODST.Utilities
{
    public static class Path
    {
        public static string GetTagsRelativePath(string path)
        {
            if (path.Contains(Properties.Settings.Default.ODSTEKPath))
                return path.Substring(Properties.Settings.Default.ODSTEKPath.Length + 6);
            else
            {
                return path;
            }                
        }

        public static string ODSTEKTagsPath = Properties.Settings.Default.ODSTEKPath + @"\tags\";

        public static void SetODSTEKPath()
        {
            System.Windows.Forms.FolderBrowserDialog fbg = new System.Windows.Forms.FolderBrowserDialog();
            if (fbg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (!Directory.Exists(fbg.SelectedPath + @"\tags"))
                {
                    MessageBox.Show("Please select a folder that contains a tags folder"); // Change this later after redoing the tag explorer
                    return;
                }
                Properties.Settings.Default.ODSTEKPath = fbg.SelectedPath;
                Properties.Settings.Default.Save();
                // Ugly hack but it works
                System.Windows.Forms.Application.Restart();
                Application.Current.Shutdown();
            }
            else if (fbg.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
            {

            }
            else
            {
                MessageBox.Show("Path not found, please try again.", "Error");
            }
        }
    }
}
