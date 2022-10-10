using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using Microsoft.Win32;

namespace PresentationODST
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static Xceed.Wpf.AvalonDock.Layout.LayoutDocumentPane TagTabs;
        public static bool _BlamInitialized = false;
        public string BlamInitialized
        {
            get
            {
                if (Bungie.ManagedBlamSystem.IsInitialized)
                    return "Ready";
                else
                    return "Disconnected";
            }
        }
        public static Dialogs.TagGroupSelector GroupSelector;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            TagTabs = TagDocuments;
            if (!InitializeProject())
            {
                MessageBox.Show("Please navigate to your ODSTEK install folder.", "Startup", MessageBoxButton.OK);
                Utilities.Path.SetODSTEKPath();
            }
        }

        private bool InitializeProject()
        {
            if (Properties.Settings.Default.ODSTEKPath.Length <= 0) return false;

            Bungie.ManagedBlamSystem.InitializeProject(Bungie.InitializationType.TagsOnly, Properties.Settings.Default.ODSTEKPath);
            StatusBarText.Text = BlamInitialized;
            return true;
        }

        private void CommandBinding_Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                //InitialDirectory = Properties.Settings.Default.ODSTEKPath
            };
            if (ofd.ShowDialog() == true)
            {
                if (!ofd.FileName.Contains(Properties.Settings.Default.ODSTEKPath + @"\tags"))
                {
                    MessageBox.Show("You tried to open a tag outside of your working directory. Bad!", "Oops...");
                }
                else
                {
                    ManagedBlam.Tags.OpenTag(ofd.FileName);
                }
            }
        }

        private void ODSTEKPath_Click(object sender, RoutedEventArgs e)
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
                InitializeProject();
            }
        }

        private void CommandBinding_New_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            GroupSelector = new Dialogs.TagGroupSelector();
            if (GroupSelector.ShowDialog() == true)
            {
                Bungie.Tags.TagFile NewTag = new Bungie.Tags.TagFile();
                Bungie.Tags.TagGroupType SelectedItem = (Bungie.Tags.TagGroupType)GroupSelector.TagListBox.SelectedItem;
                NewTag.New(Bungie.Tags.TagPath.FromPathAndType("tag" + TagDocuments.Children.Count, SelectedItem.Extension));
                Xceed.Wpf.AvalonDock.Layout.LayoutDocument TagTab = new Xceed.Wpf.AvalonDock.Layout.LayoutDocument
                {
                    Title = "tag" + TagDocuments.Children.Count + "." + SelectedItem.Extension,
                    Content = new Controls.TagView()
                };
                Controls.TagView NewTagView = (Controls.TagView)TagTab.Content;
                NewTagView.TagFile = NewTag;
                foreach (Bungie.Tags.TagField field in NewTag.Fields)
                {
                    ManagedBlam.Tags.AddFieldValues(NewTagView.TagGrid, field);
                }
                TagDocuments.Children.Add(TagTab);
                TagDocuments.SelectedContentIndex = TagDocuments.Children.IndexOf(TagTab);
            }
        }

        private void CommandBinding_Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Controls.TagView SaveTagView = (Controls.TagView)TagDocuments.Children[TagDocuments.SelectedContentIndex].Content;
            SaveTagView.Save();
        }

        private void SaveFileAs_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog
            {
                InitialDirectory = Properties.Settings.Default.ODSTEKPath + @"\tags"
            };
            if (sfd.ShowDialog() == true)
            {
                string[] SavePath = Utilities.Path.GetTagsRelativePath(sfd.FileName).Split('.');
                //Debug.WriteLine(SavePath[0] + " " + SavePath[1]);
                Controls.TagView SaveAsTagView = (Controls.TagView)TagDocuments.Children[TagDocuments.SelectedContentIndex].Content;
                SaveAsTagView.SaveAs(Bungie.Tags.TagPath.FromPathAndExtension(SavePath[0], SavePath[1]));
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void LayoutRoot_ElementRemoved(object sender, Xceed.Wpf.AvalonDock.Layout.LayoutElementEventArgs e)
        {
        }
    }
}
