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
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Save();
        }

        private void OutputWindowVisibility_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!((ComboBox)sender).IsLoaded) return;

            switch (((ComboBox)sender).SelectedIndex)
            {
                case 0:
                    MainWindow.Main_Window.MainGrid.RowDefinitions[3].Height = new GridLength(150);
                    Properties.Settings.Default.OutputWindowHeight = 150;
                    Properties.Settings.Default.ShowOutputWindow = Visibility.Visible;
                    break;
                case 1:
                    MainWindow.Main_Window.MainGrid.RowDefinitions[3].Height = new GridLength(0);
                    Properties.Settings.Default.OutputWindowHeight = 0;
                    Properties.Settings.Default.ShowOutputWindow = Visibility.Collapsed;
                    break;
                default:
                    break;
            }

            Properties.Settings.Default.Save();
        }

        private void OutputWindowVisibility_Initialized(object sender, EventArgs e)
        {
            ((ComboBox)sender).SelectedIndex = Properties.Settings.Default.ShowOutputWindow == Visibility.Visible ? 0 : 1;
        }
    }
}
