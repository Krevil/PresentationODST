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
using Xceed.Wpf.AvalonDock.Layout;
using PresentationODST.Controls;
using PresentationODST.Controls.LayoutDocuments;
using PresentationODST.Utilities;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.IO;

namespace PresentationODST
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow Main_Window;
        public static int NewTagCount = 1;
        public static Dialogs.TagGroupSelector GroupSelector;

        public MainWindow()
        {
            InitializeComponent();
            Main_Window = this;
            if (!InitializeProject())
            {
                MessageBox.Show("Please navigate to your ODSTEK install folder.", "Startup", MessageBoxButton.OK);
                Utilities.Path.SetODSTEKPath();
            }
        }

        public bool InitializeProject()
        {
            if (Properties.Settings.Default.ODSTEKPath.Length <= 0)
                return false;
            else
            {
                DataContext = new TagDirectory(Utilities.Path.ODSTEKTagsPath);
                Bungie.ManagedBlamSystem.InitializeProject(Bungie.InitializationType.TagsOnly, Properties.Settings.Default.ODSTEKPath);
                return true;
            }
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

        private void Preferences_Click(object sender, RoutedEventArgs e)
        {
            LayoutDocument PreferencesTab = new LayoutDocument
            {
                Title = "Preferences",
                Content = new Preferences()
            };
            LayoutDocumentPane ldp = TagDock.Layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();
            ldp.Children.Add(PreferencesTab);
            ldp.SelectedContentIndex = ldp.IndexOfChild(PreferencesTab);
        }

        private void CommandBinding_New_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ManagedBlam.Tags.NewTag();
        }

        private void CommandBinding_Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (TagDocuments.Children[TagDocuments.SelectedContentIndex].Content.GetType() != typeof(TagView)) return;
            TagView SaveTagView = (TagView)TagDocuments.Children[TagDocuments.SelectedContentIndex].Content;
            if (System.IO.File.Exists(Utilities.Path.ODSTEKTagsPath + SaveTagView.TagFile.Path.RelativePathWithExtension))
            {
                SaveTagView.Save();
            }
            else
            {
                SaveFileDialog sfd = new SaveFileDialog
                {
                    InitialDirectory = Utilities.Path.ODSTEKTagsPath
                };
                if (sfd.ShowDialog() == true)
                {
                    string[] SavePath = Utilities.Path.GetTagsRelativePath(sfd.FileName).Split('.');
                    SaveTagView.SaveAs(Bungie.Tags.TagPath.FromPathAndExtension(SavePath[0], SavePath[1]));
                }
            }
        }

        private void SaveFileAs_Click(object sender, RoutedEventArgs e)
        {
            if (TagDocuments.Children[TagDocuments.SelectedContentIndex].Content.GetType() != typeof(TagView)) return;
            SaveFileDialog sfd = new SaveFileDialog
            {
                InitialDirectory = Utilities.Path.ODSTEKTagsPath
            };
            if (sfd.ShowDialog() == true)
            {
                string[] SavePath = Utilities.Path.GetTagsRelativePath(sfd.FileName).Split('.');
                TagView SaveAsTagView = (TagView)TagDocuments.Children[TagDocuments.SelectedContentIndex].Content;
                SaveAsTagView.SaveAs(Bungie.Tags.TagPath.FromPathAndExtension(SavePath[0], SavePath[1]));
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Uri iconUri = new Uri("pack://application:,,,/PresentationODST.ico", UriKind.RelativeOrAbsolute);
            Icon = BitmapFrame.Create(iconUri, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
        }

        private void TagExplorerButton_DoubleClick(object sender, RoutedEventArgs e)
        {
            ManagedBlam.Tags.OpenTag(((TagDirectoryItem)((Button)sender).DataContext).FullPath);
            //I feel like there must be a better way than doing all this casting?
        }

        private void TagExplorerButton_Click(object sender, RoutedEventArgs e)
        {
            //((TreeViewItem)((Button)sender)).IsSelected = true;
        }

        private void GridSplitter_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            Properties.Settings.Default.TagExplorerWidth = MainGrid.ColumnDefinitions[0].Width.Value;
            Properties.Settings.Default.Save();
        }
    }

    #region TreeView Malarky

    public class TagDirectory
    {
        public TagDirectory(string dir)
        {
            TagDirectories.Add(new TagDirectoryItem(dir));
        }

        public ObservableCollection<TagDirectoryItem> TagDirectories { get; set; } = new ObservableCollection<TagDirectoryItem>();
    }

    public class TagDirectoryItem : INotifyPropertyChanged
    {
        public TagDirectoryItem(string fullpath)
        {
            FullPath = fullpath;
            DirectoryInfo DirInfo = new DirectoryInfo(fullpath);
            _Name = DirInfo.Name;
            IsFile = !DirInfo.Attributes.HasFlag(FileAttributes.Directory);

            if (!IsFile)
            {
                foreach (string file in Directory.GetFiles(FullPath))
                {
                    SubFolders.Add(new TagDirectoryItem(file));
                }
                foreach (string folder in Directory.GetDirectories(FullPath))
                {
                    SubFolders.Add(new TagDirectoryItem(folder));
                }
            }
        }

        public bool IsFile { get; set; }
        public string FullPath { get; set; }

        public ObservableCollection<TagDirectoryItem> SubFolders { get; set; } = new ObservableCollection<TagDirectoryItem>();

        private string _Name;
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
                OnPropertyChanged("Name");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string property)
        {
            if (PropertyChanged == null) return;
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(property));
        }

        public override string ToString()
        {
            return _Name;
        }
    }
    #endregion
}
