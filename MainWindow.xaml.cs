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


        // I do it this way so that only the folders and files the user has visible are loaded. 
        // This also means the user can add new files and folders and have it refresh, I just realised that
        private void TagExplorer_Expanded(object sender, RoutedEventArgs e)
        {
            foreach (TagDirectoryItem item in ((TagDirectoryItem)((TreeViewItem)e.OriginalSource).DataContext).SubFolders)
            {
                if (!item.IsFile)
                {
                    foreach (string file in Directory.GetFiles(item.FullPath))
                    {
                        item.SubFolders.Add(new TagDirectoryItem(file) { ParentItem = item });
                    }
                    foreach (string folder in Directory.GetDirectories(item.FullPath))
                    {
                        item.SubFolders.Add(new TagDirectoryItem(folder));
                    }
                }
            }

        }

        private void TagExplorer_Collapsed(object sender, RoutedEventArgs e)
        {
            foreach (TagDirectoryItem item in ((TagDirectoryItem)((TreeViewItem)e.OriginalSource).DataContext).SubFolders)
            {
                if (!item.IsFile)
                    item.SubFolders.Clear();
            }
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
                for (int i = 2; i < 250; i++) // No one is going to make over 250 copies of the same file, right?
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
            TagDirectoryItem Parent = ((TagDirectoryItem)((MenuItem)sender).DataContext).ParentItem;
            Parent.SubFolders.Add(new TagDirectoryItem(CopyPath) { ParentItem = Parent });
        }

        private void TagExplorerContextMenu_FileExplorer_Click(object sender, RoutedEventArgs e)
        {
            string FolderPath = System.IO.Path.GetDirectoryName(((TagDirectoryItem)((MenuItem)sender).DataContext).FullPath);
            if (FolderPath == null || FolderPath == "") return;
            Process.Start("explorer.exe", FolderPath);
        }

        private void TagExplorerContextMenu_Delete_Click(object sender, RoutedEventArgs e)
        {
            TagDirectoryItem DeletedFile = ((TagDirectoryItem)((MenuItem)sender).DataContext);
            if (!File.Exists(DeletedFile.FullPath)) return;
            File.Delete(DeletedFile.FullPath);
            DeletedFile.ParentItem.SubFolders.Remove(DeletedFile);
        }

        private void TagExplorer_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.F5) return;
            ((TagDirectory)TagExplorer.DataContext).TagDirectories.Clear();
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
            MainWindow.GroupSelector = new Dialogs.TagGroupSelector();
            if (MainWindow.GroupSelector.ShowDialog() == true)
            {
                LayoutDocumentPane ldp = MainWindow.Main_Window.TagDock.Layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();
                Bungie.Tags.TagFile NewTag = new Bungie.Tags.TagFile();
                Bungie.Tags.TagGroupType SelectedItem = (Bungie.Tags.TagGroupType)MainWindow.GroupSelector.TagListBox.SelectedItem;
                Bungie.Tags.TagPath NewPath = Bungie.Tags.TagPath.FromPathAndExtension("tag" + MainWindow.NewTagCount, SelectedItem.Extension);
                NewTag.New(NewPath);
                LayoutDocument TagTab = new LayoutDocument
                {
                    Title = "tag" + MainWindow.NewTagCount + "." + SelectedItem.Extension,
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
                MainWindow.NewTagCount++;
            }
        }
    }

    #region TreeView Malarky

    public class TagDirectory : INotifyPropertyChanged
    {
        public TagDirectory(string dir)
        {
            TagDirectoryItem TagDir = new TagDirectoryItem(dir);
            foreach (string file in Directory.GetFiles(dir))
            {
                TagDir.SubFolders.Add(new TagDirectoryItem(file) { ParentItem = TagDir });
            }
            foreach (string folder in Directory.GetDirectories(dir))
            {
                TagDir.SubFolders.Add(new TagDirectoryItem(folder));
            }
            // Hack to get the initial files and folders
            _TagDirectories.Add(TagDir);
        }

        // Look into FileSystemWatcher as a replacement for this
        private ObservableCollection<TagDirectoryItem> _TagDirectories = new ObservableCollection<TagDirectoryItem>();
        public ObservableCollection<TagDirectoryItem> TagDirectories
        {
            get
            {
                return _TagDirectories;
            }
            set
            {
                _TagDirectories = value;
                OnPropertyChanged("TagDirectories");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string property)
        {
            if (PropertyChanged == null) return;
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }

    public class TagDirectoryItem : INotifyPropertyChanged
    {
        public TagDirectoryItem(string fullpath)
        {
            FullPath = fullpath;
            DirectoryInfo DirInfo = new DirectoryInfo(fullpath);
            _Name = DirInfo.Name;
            IsFile = !DirInfo.Attributes.HasFlag(FileAttributes.Directory);

            /*
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
            */
        }

        public bool IsFile { get; set; }
        public string FullPath { get; set; }
        public TagDirectoryItem ParentItem { get; set; }
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
