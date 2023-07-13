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
using System.Globalization;
using PresentationODST.Dialogs;

namespace PresentationODST
{
    // TODO:
    // Remember to change Chief back to the ODST when using that MB version
    // Update block element titles when possible (ie when a name field gets added to an added block element)
    // Add more info to block context menu - fix double entries?
    // Implement tag mover/copyer
    // Fix tag explorer adding new items
    // Make empty tag blocks viewable
    // More logging? Add clear logger button
    // Run Tool menu
    // Unsaved tags warning before shutdown
    // Add something to show those annoying struct fields
    // Launch scenario UI. Should be some stuff for this in ManagedBlam

    public partial class MainWindow : Window
    {
        public static MainWindow Main_Window = (MainWindow)Application.Current.MainWindow;
        public static int NewTagCount = 1;
        public static TagGroupSelector GroupSelector;
        public static TagReopener Reopener;
        public static ObservableCollection<TagSearchFile> AllTagFiles = new ObservableCollection<TagSearchFile>();

        public MainWindow()
        {
            InitializeComponent();
            Main_Window = this;
        }

        public bool InitializeProject()
        {
            if (Properties.Settings.Default.ODSTEKPath.Length <= 0)
                return false;
            else if (!Directory.Exists(Properties.Settings.Default.ODSTEKPath + @"\tags"))
                return false;
            else
            {
                TagExplorer.DataContext = new TagDirectory();
                DirectoryInfo dirInfo = new DirectoryInfo(Utilities.Path.ODSTEKTagsPath);
                List<TagSearchFile> tsfList = new List<TagSearchFile>();
                foreach (FileInfo fi in dirInfo.GetFiles("*.*", SearchOption.AllDirectories).ToList())
                {
                    tsfList.Add(new TagSearchFile(fi));
                }
                AllTagFiles = new ObservableCollection<TagSearchFile>(tsfList);
                TagSearchListView.ItemsSource = new ObservableCollection<TagSearchFile>(tsfList);
                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(TagSearchListView.ItemsSource);
                view.Filter = TagSearchFilter;
                
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
            TagDocuments.Children.Add(PreferencesTab);
            TagDocuments.SelectedContentIndex = TagDocuments.IndexOfChild(PreferencesTab);
        }

        private void CommandBinding_New_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //ManagedBlam.Shaders.NewShader();
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
            if (TagDocuments.Children.Count <= 0) return;
            if (TagDocuments.Children[TagDocuments.SelectedContentIndex].Content.GetType() != typeof(TagView)) return;
            string fileExt = ((TagView)TagDocuments.Children[TagDocuments.SelectedContentIndex].Content).TagFile.Path.Extension;
            SaveFileDialog sfd = new SaveFileDialog
            {
                InitialDirectory = Utilities.Path.ODSTEKTagsPath,
                AddExtension = true,
                DefaultExt = fileExt,
                Filter = fileExt + "|*." + fileExt
            };
            if (sfd.ShowDialog() == true)
            {
                string[] SavePath = Utilities.Path.GetTagsRelativePath(sfd.FileName).Split('.');
                TagView SaveAsTagView = (TagView)TagDocuments.Children[TagDocuments.SelectedContentIndex].Content;
                Bungie.Tags.TagPath saveTagPath = Bungie.Tags.TagPath.FromPathAndExtension(SavePath[0], SavePath[1]);
                SaveAsTagView.SaveAs(saveTagPath);
                TagDocuments.Children[TagDocuments.SelectedContentIndex].Close();
                ManagedBlam.Tags.OpenTag(sfd.FileName);
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Why is this here? Because without it the application gets a low res version of the icon. Why does it do this? Who knows.
            Uri iconUri = new Uri("pack://application:,,,/Images/ODSTIcon.ico", UriKind.RelativeOrAbsolute);
            Icon = BitmapFrame.Create(iconUri, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
        }

        private void TagExplorerButton_DoubleClick(object sender, RoutedEventArgs e)
        {
            if (((TagDirectoryItem)((Button)sender).DataContext).IsFile)
                ManagedBlam.Tags.OpenTag(((TagDirectoryItem)((Button)sender).DataContext).FullPath);
            //I feel like there must be a better way than doing all this casting?
        }

        private void TagExplorerButton_Click(object sender, RoutedEventArgs e)
        {
            //((TreeViewItem)((Button)sender).TemplatedParent).IsSelected = true;
        }

        private void GridSplitter_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            Properties.Settings.Default.TagExplorerWidth = MainGrid.ColumnDefinitions[0].Width.Value;
            Properties.Settings.Default.OutputWindowHeight = MainGrid.RowDefinitions[3].Height.Value;
            Properties.Settings.Default.Save();
        }

        private void TagExplorerContextMenu_Open_Click(object sender, RoutedEventArgs e)
        {
            if (((TagDirectoryItem)((MenuItem)sender).DataContext).IsFile)
                ManagedBlam.Tags.OpenTag(((TagDirectoryItem)((MenuItem)sender).DataContext).FullPath);
        }

        private void TagExplorerContextMenu_Duplicate_Click(object sender, RoutedEventArgs e)
        {
            if (((TagDirectoryItem)((MenuItem)sender).DataContext).IsFile)
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
            else
            {
                // logic for folders
            }
        }

        private void TagExplorerContextMenu_FileExplorer_Click(object sender, RoutedEventArgs e)
        {
            string FolderPath;
            if (((TagDirectoryItem)((MenuItem)sender).DataContext).IsFile)
            {
                FolderPath = System.IO.Path.GetDirectoryName(((TagDirectoryItem)((MenuItem)sender).DataContext).FullPath);
            }
            else
            {
                FolderPath = System.IO.Path.GetFullPath(((TagDirectoryItem)((MenuItem)sender).DataContext).FullPath);
            }
            if (FolderPath == null || FolderPath == "") return;
            Process.Start("explorer.exe", FolderPath);
        }

        private void TagExplorerContextMenu_Delete_Click(object sender, RoutedEventArgs e)
        {
            if (((TagDirectoryItem)((MenuItem)sender).DataContext).IsFile)
            {
                TagDirectoryItem DeletedFile = (TagDirectoryItem)((MenuItem)sender).DataContext;
                if (!File.Exists(DeletedFile.FullPath)) return;
                File.Delete(DeletedFile.FullPath);
                //DeletedFile.ParentItem.SubFolders.Remove(DeletedFile); // should not need this as the watcher will handle it
            }
            else
            {
                // logic for folders
            }
        }

        private void TagExplorer_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.F5) return;
            ((TagDirectory)TagExplorer.DataContext).TagDirectories.Clear(); // maybe change this to a new TagDirectory()?
            TagExplorer.DataContext = new TagDirectory();
        }

        private void TagExplorerContextMenu_Rename_Click(object sender, RoutedEventArgs e)
        {
            if (((TagDirectoryItem)((MenuItem)sender).DataContext).IsFile)
            {
                TagRenamer Renamer = new TagRenamer();
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
            else
            {

            }
        }

        private void RefreshExplorer_Click(object sender, RoutedEventArgs e)
        {
            ((TagDirectory)TagExplorer.DataContext).TagDirectories.Clear();
            TagExplorer.DataContext = new TagDirectory();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            if (!InitializeProject())
            {
                MessageBox.Show("Please navigate to your editing kit root folder.", "Startup", MessageBoxButton.OK);
                Utilities.Path.SetODSTEKPath();
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            // Replace code here to check if there are unsaved documents
            if (TagDocuments.Children.Count <= 0)
            {
                e.Cancel = false;
                return;
            }
            // Consider making a custom MessageBox control instead of using the default
            MessageBoxResult cancelMsg = MessageBox.Show("Are you sure you want to exit the program?\nAny unsaved changes will be lost", "Exit", MessageBoxButton.OKCancel);
            if (cancelMsg != MessageBoxResult.OK)
            {
                e.Cancel = true;
            }
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.OpenTags == null) return;
            if (Properties.Settings.Default.OpenTags.Count <= 0) return;
            Reopener = new TagReopener();
            Reopener.Owner = Main_Window;
            if (Reopener.ShowDialog() == true)
            {
                foreach (string tagPath in Reopener.TagsToOpen)
                {
                    try
                    {
                        ManagedBlam.Tags.OpenTag(tagPath);
                    }
                    catch
                    {
                        WPF.Log("Tag Reopener: Could not open tag {0}", tagPath);
                    }
                }
            }
        }

        private void TagDock_DocumentClosed(object sender, Xceed.Wpf.AvalonDock.DocumentClosedEventArgs e)
        {
            if (e.Document.Content is TagView view)
            {
                ManagedBlam.Tags.OpenTags.Remove(view.TagFile);
            }
        }

        private bool TagSearchFilter(object item)
        {
            if (SearchBar.Text.Length <= 0)
                return false;
            else
                return (((TagSearchFile)item).Name + "." + ((TagSearchFile)item).TagInfo.Extension).Contains(SearchBar.Text);
        }

        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(TagSearchListView.ItemsSource).Refresh();
        }

        private void TagSearchButton_Click(object sender, RoutedEventArgs e)
        {
            ManagedBlam.Tags.OpenTag(((Button)sender).ToolTip.ToString());
        }

        private void SearchBar_GotFocus(object sender, RoutedEventArgs e)
        {
            TagExplorerPane.SelectedContentIndex = 1;
        }

        private void TagSearchExplorer_Click(object sender, RoutedEventArgs e)
        {
            string FolderPath = System.IO.Path.GetFullPath(((TagSearchFile)((MenuItem)sender).DataContext).TagInfo.DirectoryName);
            if (FolderPath == null || FolderPath == "") return;
            Process.Start("explorer.exe", FolderPath);
        }
    }

    public class TagSearchFile
    {
        public TagSearchFile(FileInfo fileInfo)
        {
            TagInfo = fileInfo;
            TagPath = Utilities.Path.GetTagsRelativePath(TagInfo.FullName);
            Name = fileInfo.Name;
        }

        public string Name { get; set; }
        public FileInfo TagInfo { get; set; }
        public string TagPath { get; set; }
    }

    public class TagDirectory
    {
        public TagDirectory()
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
            Debug.WriteLine("Tag Explorer: {0}, with path {1} has been {2}", e.Name, e.FullPath, e.ChangeType);
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
                    {
                        Debug.WriteLine("Tag Explorer: Could not find parent of " + e.FullPath);
                        return;
                    }
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
            Debug.WriteLine("Tag Explorer: {0} renamed to {1}", e.OldFullPath, e.FullPath);
            TagDirectoryItem RenamedItem = AllTagDirectories.Find(x => x.FullPath == e.OldFullPath);
            if (RenamedItem == null)
                return;
            RenamedItem.FullPath = e.FullPath;
            RenamedItem.Name = e.Name;
        }
    }

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
                    Debug.WriteLine("Tag Explorer: Failed to add files to TreeView - was a file removed?"); // Change to use a messagebox? GetFileSystemInfos can sometimes throw an exception when a file gets deleted.
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

        public Visibility IsVisible
        {
            get
            {
                return IsFile == true ? Visibility.Visible : Visibility.Collapsed;
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

    // I copied most of this from stackexchange, need to research converters to properly understand how this functions
    public class TagDirectoryConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            System.Collections.IList collection = value as System.Collections.IList;
            ListCollectionView view = new ListCollectionView(collection);
            SortDescription sd1 = new SortDescription(parameter.ToString(), ListSortDirection.Ascending);
            SortDescription sd2 = new SortDescription("Name", ListSortDirection.Ascending);
            view.SortDescriptions.Add(sd1);
            view.SortDescriptions.Add(sd2);

            return view;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
