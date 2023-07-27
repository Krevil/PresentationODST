using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using PresentationODST.Dialogs;

namespace PresentationODST.Utilities
{
    public static class Path
    {
        public static string GetTagsRelativePath(string path)
        {
            if (path.Contains(Properties.Settings.Default.ODSTEKPath))
                return path.Substring(Properties.Settings.Default.ODSTEKPath.Length + 6);
            else if (path.Contains("tags"))
            {
                return path.Substring(path.IndexOf("tags") + 5);
            }
            else
            {
                return path;
            }                
        }

        public static string[] GetTagsRelativePathAndExtension(string path)
        {
            List<string> result = new List<string>();
           
            if (path.Contains(Properties.Settings.Default.ODSTEKPath))
            {
                result = new List<string>(path.Substring(Properties.Settings.Default.ODSTEKPath.Length + 6).Split('.'));
            }
            else if (path.Contains("tags"))
            {
                result = new List<string>(path.Substring(path.IndexOf("tags") + 5).Split('.'));
            }
            else // Assume the path is already relative
            {
                result = new List<string>(path.Split('.'));
            }
            // If for some ungodly reason a user thinks it's a great idea to have multiple periods in their file names
            if (result.Count > 2)
            {
                string newFileName = "";
                for (int i = 0; i < result.Count - 1; i++)
                {
                    newFileName += result[i];
                }
                result = new List<string> { newFileName, result[result.Count - 1] };
            }
            return result.ToArray();
        }

        public static string ODSTEKTagsPath = Properties.Settings.Default.ODSTEKPath + @"\tags\";

        public static void SetODSTEKPath()
        {
            System.Windows.Forms.FolderBrowserDialog fbg = new System.Windows.Forms.FolderBrowserDialog();
            if (fbg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (!Directory.Exists(fbg.SelectedPath + @"\tags"))
                {
                    CustomMessageBox.Show("Please select a folder that contains a tags folder"); // Change this later after redoing the tag explorer
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
                CustomMessageBox.Show("Path not found, please try again.", "Error");
            }
        }
    }
}