using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PresentationODST.Controls.LayoutDocuments
{
    /// <summary>
    /// Interaction logic for Preferences.xaml
    /// </summary>
    public partial class Preferences : UserControl
    {
        public Preferences()
        {
            InitializeComponent();
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog fbg = new System.Windows.Forms.FolderBrowserDialog();
            if (Properties.Settings.Default.ODSTEKPath.Length > 0)
            {
                try
                {
                    fbg.SelectedPath = Properties.Settings.Default.ODSTEKPath;
                    fbg.RootFolder = Environment.SpecialFolder.MyComputer;
                }
                catch
                {

                }
            }
            if (fbg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Properties.Settings.Default.ODSTEKPath = fbg.SelectedPath;
                Properties.Settings.Default.Save();
                MainWindow.Main_Window.InitializeProject();
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Save();
        }
    }
}
