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
            else if (!Directory.Exists(Properties.Settings.Default.ODSTEKPath + @"\tags"))
                return false;
            else
            {
                TagExplorer.DataContext = new TagDirectory(Utilities.Path.ODSTEKTagsPath);
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
            if (File.Exists(Utilities.Path.ODSTEKTagsPath + SaveTagView.TagFile.Path.RelativePathWithExtension))
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

        private void TagExplorerContextMenu_Open_Click(object sender, RoutedEventArgs e)
        {
            ManagedBlam.Tags.OpenTag(((TagDirectoryItem)((MenuItem)sender).DataContext).FullPath);
        }

        private void TagExplorerContextMenu_Duplicate_Click(object sender, RoutedEventArgs e)
        {
            
            string FileName = ((TagDirectoryItem)((MenuItem)sender).DataContext).FullPath;
            Bungie.Tags.TagPath TagName = Bungie.Tags.TagPath.FromFilename(FileName);
            string CopyPath = Utilities.Path.ODSTEKTagsPath + TagName.RelativePath + " - Copy." + TagName.Extension;
            if (File.Exists(CopyPath))
            {
                for (int i = 2; i < 250; i++) // No one is going to make over 250 copies of the same file, right? Consider this your free sanity check
                {
                    if (File.Exists(Utilities.Path.ODSTEKTagsPath + TagName.RelativePath + " - Copy (" + i + ")." + TagName.Extension))
                        continue;
                    else
                    {
                        CopyPath = Utilities.Path.ODSTEKTagsPath + TagName.RelativePath + " - Copy (" + i + ")." + TagName.Extension;
                        break;
                    }
                }
            }
            File.Copy(FileName, CopyPath);
        }

        private void TagExplorerContextMenu_FileExplorer_Click(object sender, RoutedEventArgs e)
        {
            string FolderPath = System.IO.Path.GetDirectoryName(((TagDirectoryItem)((MenuItem)sender).DataContext).FullPath);
            if (FolderPath == null || FolderPath == "") return;
            Process.Start("explorer.exe", FolderPath);
        }

        private void TagExplorerContextMenu_Delete_Click(object sender, RoutedEventArgs e)
        {
            TagDirectoryItem DeletedFile = (TagDirectoryItem)((MenuItem)sender).DataContext;
            if (!File.Exists(DeletedFile.FullPath)) return;
            File.Delete(DeletedFile.FullPath);
            //DeletedFile.ParentItem.SubFolders.Remove(DeletedFile); // should not need this as the watcher will handle it
        }

        private void TagExplorer_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.F5) return;
            ((TagDirectory)TagExplorer.DataContext).TagDirectories.Clear(); // maybe change this to a new TagDirectory()?
            TagExplorer.DataContext = new TagDirectory(Utilities.Path.ODSTEKTagsPath);
        }

        private void TagExplorerContextMenu_Rename_Click(object sender, RoutedEventArgs e)
        {
            Dialogs.TagRenamer Renamer = new Dialogs.TagRenamer();
            TagDirectoryItem ItemToRename = (TagDirectoryItem)((MenuItem)sender).DataContext;
            string OriginalPath = ItemToRename.FullPath;
            Renamer.NameTextBox.Text = ItemToRename.Name;
            if (Renamer.ShowDialog() == true)
            {
                ItemToRename.FullPath = ItemToRename.FullPath.Replace(ItemToRename.Name, Renamer.NameTextBox.Text);
                ItemToRename.Name = Renamer.NameTextBox.Text;
                File.Move(OriginalPath, ItemToRename.FullPath);
            }
        }

        public static void NewTag()
        {
            GroupSelector = new Dialogs.TagGroupSelector();
            if (GroupSelector.ShowDialog() == true)
            {
                LayoutDocumentPane ldp = Main_Window.TagDock.Layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();
                Bungie.Tags.TagFile NewTag = new Bungie.Tags.TagFile();
                Bungie.Tags.TagGroupType SelectedItem = (Bungie.Tags.TagGroupType)GroupSelector.TagListBox.SelectedItem;
                Bungie.Tags.TagPath NewPath = Bungie.Tags.TagPath.FromPathAndExtension("tag" + NewTagCount, SelectedItem.Extension);
                NewTag.New(NewPath);
                LayoutDocument TagTab = new LayoutDocument
                {
                    Title = "tag" + NewTagCount + "." + SelectedItem.Extension,
                    Content = new TagView()
                };
                TagView NewTagView = (TagView)TagTab.Content;
                NewTagView.TagFile = NewTag;
                foreach (Bungie.Tags.TagField field in NewTag.Fields)
                {
                    ManagedBlam.Tags.AddFieldValues(NewTagView.TagGrid, field);
                }
                ldp.Children.Add(TagTab);
                ldp.SelectedContentIndex = ldp.IndexOfChild(TagTab);
                NewTagCount++;
            }
        }

        /*
        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!IsLoaded) return;
            foreach (UIElement item in TagExplorer.Items)
            {
                if (SearchBar.Text == "")
                {
                    item.Visibility = Visibility.Visible;
                    continue;
                }
                if (!item.ToString().Contains(SearchBar.Text))
                {
                    item.Visibility = Visibility.Collapsed;
                }
                else
                {
                    item.Visibility = Visibility.Visible;
                }
            }
        }
        */
    }

    #region TreeView Malarky

    public class TagDirectory
    {
        public TagDirectory(string dir)
        {
            FileSystemWatcher watcher = new FileSystemWatcher
            {
                Path = Utilities.Path.ODSTEKTagsPath,
                IncludeSubdirectories = true,
                NotifyFilter = (NotifyFilters)95,
                Filter = "*.*"
            };
            watcher.Changed += new FileSystemEventHandler(OnFileChanged);
            watcher.Created += new FileSystemEventHandler(OnFileChanged);
            watcher.Deleted += new FileSystemEventHandler(OnFileChanged);
            watcher.Renamed += new RenamedEventHandler(OnFileRenamed);
            watcher.EnableRaisingEvents = true;
            DirectoryInfo TagsInfo = new DirectoryInfo(Properties.Settings.Default.ODSTEKPath + @"\tags");
            TagDirectories.Add(new TagDirectoryItem(TagsInfo));

            AllTagDirectories.AddRange(TagDirectories);
        }

        public ObservableCollection<TagDirectoryItem> TagDirectories { get; set; } = new ObservableCollection<TagDirectoryItem>();
        public static List<TagDirectoryItem> AllTagDirectories { get; set; } = new List<TagDirectoryItem>();

        private void OnFileChanged(object source, FileSystemEventArgs e)
        {
            Debug.WriteLine("{0}, with path {1} has been {2}", e.Name, e.FullPath, e.ChangeType);
            switch (e.ChangeType)
            {
                case WatcherChangeTypes.Deleted:
                    TagDirectoryItem DeletedItem = AllTagDirectories.Find(x => x.FullPath == e.FullPath);
                    if (DeletedItem == null)
                        return;
                    if (DeletedItem.ParentItem != null)
                        Application.Current.Dispatcher.BeginInvoke(new Action(() => DeletedItem.ParentItem.SubFolders.Remove(DeletedItem)));
                    AllTagDirectories.Remove(DeletedItem);
                    break;
                case WatcherChangeTypes.Created:
                    TagDirectoryItem ParentDir = AllTagDirectories.Find(x => x.FullPath == new DirectoryInfo(e.FullPath).Parent.FullName);
                    if (ParentDir == null)
                        return;
                    TagDirectoryItem CreatedDir = new TagDirectoryItem(new DirectoryInfo(e.FullPath), ParentDir);
                    Application.Current.Dispatcher.BeginInvoke(new Action(() => ParentDir.SubFolders.Add(CreatedDir)));
                    AllTagDirectories.Add(CreatedDir);
                    break;
                default:
                    break;
            }
            
        }

        private void OnFileRenamed(object source, RenamedEventArgs e)
        {
            Debug.WriteLine(" {0} renamed to {1}", e.OldFullPath, e.FullPath);
            TagDirectoryItem RenamedItem = AllTagDirectories.Find(x => x.FullPath == e.OldFullPath);
            if (RenamedItem == null)
                return;
            RenamedItem.FullPath = e.FullPath;
            RenamedItem.Name = e.Name;
        }
    }
    // search hack idea: totally bypass everything and just hide the regular tag browser items and only show items using tagsinfo.getfilesysteminfos(user_search_string, SearchOptions.AllDirectories)
    // treeviews may support filtering as a built in thing? look into that
    public class TagDirectoryItem : INotifyPropertyChanged
    {
        public TagDirectoryItem(FileSystemInfo info, TagDirectoryItem parent)
        {
            FullPath = info.FullName;
            Name = info.Name;
            IsFile = !info.Attributes.HasFlag(FileAttributes.Directory);
            DirectoryInfo dirinfo = new DirectoryInfo(info.FullName);
            ParentItem = parent;
            if (!IsFile)
            {
                try
                {
                    foreach (FileSystemInfo fsinfo in dirinfo.GetFileSystemInfos())
                    {
                        SubFolders.Add(new TagDirectoryItem(fsinfo, this));
                    }
                }
                catch
                {
                    Debug.WriteLine("Failed to add files to TreeView - was a file removed?"); // Change to use a messagebox? GetFileSystemInfos can sometimes throw an exception when a file gets deleted.
                }
                TagDirectory.AllTagDirectories.AddRange(SubFolders); // Big collection of every item for matching - remember to add to this whenever a file is created
            }
        }

        public TagDirectoryItem(FileSystemInfo info)
        {
            FullPath = info.FullName;
            Name = info.Name;
            IsFile = !info.Attributes.HasFlag(FileAttributes.Directory);
            DirectoryInfo dirinfo = new DirectoryInfo(info.FullName);
            if (!IsFile)
            {

                foreach (FileSystemInfo fsinfo in dirinfo.GetFileSystemInfos())
                {
                    SubFolders.Add(new TagDirectoryItem(fsinfo, this));
                }
                TagDirectory.AllTagDirectories.AddRange(SubFolders); // Big collection of every item for matching - remember to add to this whenever a file is created
            }
        }

        public void RefreshSubFolders()
        {
            SubFolders.Clear();
            DirectoryInfo dirinfo = new DirectoryInfo(FullPath);
            foreach (FileSystemInfo fsinfo in dirinfo.GetFileSystemInfos())
            {
                SubFolders.Add(new TagDirectoryItem(fsinfo, this));
            }
        }

        public bool IsFile { get; set; }
        private string _FullPath;
        public string FullPath
        {
            get
            {
                return _FullPath;
            }
            set
            {
                _FullPath = value;
                OnPropertyChanged("FullPath");
            }
        }
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
        public TagDirectoryItem ParentItem { get; set; }

        public ObservableCollection<TagDirectoryItem> SubFolders { get; set; } = new ObservableCollection<TagDirectoryItem>();

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string property)
        {
            if (PropertyChanged == null) return;
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(property));
        }

        public override string ToString()
        {
            return Name;
        }
    }
    #endregion
}
