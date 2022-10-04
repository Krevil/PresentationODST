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
        public static bool _BlamInitialized = false;
        public enum InputMode
        {
            Initial = 0,
            TagPath = 1,
            TagEditing = 2
        }
        public InputMode CurrentInputMode = InputMode.Initial;
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
        Bungie.Tags.TagFile OpenTag;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            WPFConsole.Text = "Type a tag path and extension to begin:\n";
            InputField.Focus();
            if (Properties.Settings.Default.ODSTEKPath.Length > 0)
            {
                Bungie.ManagedBlamSystem.InitializeProject(Bungie.InitializationType.TagsOnly, Properties.Settings.Default.ODSTEKPath);
                StatusBarText.Text = BlamInitialized;
            }
            else
            {
                MessageBox.Show("Please navigate to your ODSTEK install folder.", "Startup", MessageBoxButton.OK);
                SetODSTEKPath();
            }
        }


        public static void SetODSTEKPath()
        {
            System.Windows.Forms.FolderBrowserDialog fbg = new System.Windows.Forms.FolderBrowserDialog();
            if (fbg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Properties.Settings.Default.ODSTEKPath = fbg.SelectedPath;
                Properties.Settings.Default.Save();
            }
            else if (fbg.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
            {

            }
            else
            {
                MessageBox.Show("Path not found, please try again.", "Error");
            }
        }

        public static void WPFConsoleWriteLine(object text)
        {
            WPFConsole.Text += text + Environment.NewLine;
        }

        public static void WPFConsoleWrite(object text)
        {
            WPFConsole.Text += text;
        }

        public static string WPFConsoleReadLine()
        {
            string[] ConsoleText = WPFConsole.Text.Split('\n');
            return ConsoleText[ConsoleText.Length - 1];
        }

        public static void WPFConsoleAllowWrite(bool AllowWrite)
        {
            WPFConsole.IsReadOnly = AllowWrite;
        }

        public static string GetTagsRelativePath(string path)
        {
            return path.Substring(Properties.Settings.Default.ODSTEKPath.Length + 6);
        }

        public static void PrintTagStuff(string[] TagPath)
        {
            
            
            Bungie.Tags.TagPath test_path = Bungie.Tags.TagPath.FromPathAndExtension(TagPath[0], TagPath[1]);
            using (var tagFile = new Bungie.Tags.TagFile(test_path))
            {
                WPFConsoleWriteLine("Opened " + tagFile.Path);
                foreach (var field in tagFile.Fields)
                {
                    PrintFieldValues(field);
                }
                //Bungie.Tags.TagFieldEnum usage = (Bungie.Tags.TagFieldEnum)tagFile.Fields[2];
                //WPFConsoleWriteLine(usage.Items[usage.Value]);
                //WPFConsoleWriteLine($"Name: {((Bungie.Tags.TagFieldElement)tagFile.Fields[0]).GetStringData()}");
                //WPFConsoleWriteLine($"Flags: {((Bungie.Tags.TagFieldFlags)tagFile.Fields[3]).RawValue}");
            }
        }

        public static void PrintFieldValues(Bungie.Tags.TagField field)
        {
            WPFConsoleWriteLine("Field name: " + field.DisplayName);
            WPFConsoleWriteLine("Field type: " + field.FieldType);
            switch (field.FieldType)
            {
                case Bungie.Tags.TagFieldType.CharInteger:
                case Bungie.Tags.TagFieldType.ShortInteger:
                case Bungie.Tags.TagFieldType.LongInteger:
                case Bungie.Tags.TagFieldType.Int64Integer:
                case Bungie.Tags.TagFieldType.Real:
                    Bungie.Tags.TagFieldElement FieldInteger = (Bungie.Tags.TagFieldElement)field;
                    WPFConsoleWriteLine("Field value: " + FieldInteger.GetStringData());
                    break;
                case Bungie.Tags.TagFieldType.CharEnum:
                case Bungie.Tags.TagFieldType.ShortEnum:
                case Bungie.Tags.TagFieldType.LongEnum:
                    Bungie.Tags.TagFieldEnum FieldEnum = (Bungie.Tags.TagFieldEnum)field;
                    WPFConsoleWriteLine("Field value: " + FieldEnum.Items[FieldEnum.Value]);
                    break;
                case Bungie.Tags.TagFieldType.Reference:
                    Bungie.Tags.TagFieldReference TagRef = (Bungie.Tags.TagFieldReference)field;
                    WPFConsoleWriteLine("Field value: " + TagRef.Path.RelativePath + "." + TagRef.Path.Extension);
                    break;
                case Bungie.Tags.TagFieldType.Block:
                    Bungie.Tags.TagFieldBlock TagBlock = (Bungie.Tags.TagFieldBlock)field;
                    WPFConsoleWriteLine("Block elements: " + TagBlock.Elements.Count);
                    foreach (Bungie.Tags.TagElement BlockElement in TagBlock.Elements)
                    {
                        //Debug.WriteLine(BlockElement.Fields.Length);
                        for (long l = 0; l < BlockElement.Fields.Length; l++)
                        {
                            PrintFieldValues(BlockElement.Fields[l]);
                        }
                    }
                    break;
                default:
                    break;
            }
            WPFConsoleWriteLine("");
        }

        private void InputField_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && InputField.Text.Length > 0)
            {
                switch (CurrentInputMode)
                {
                    case InputMode.Initial:
                        if (InputField.Text.ToLower() == "edittag")
                        {
                            CurrentInputMode = InputMode.TagPath;
                        }
                        break;
                    case InputMode.TagPath:
                        string[] FieldText = InputField.Text.Split('.');
                        InputField.Text = "";
                        if (FieldText.Length != 2)
                        {
                            MessageBox.Show("Bad tag path", "Error");
                        }
                        else
                        {
                            WPFConsoleWriteLine(InputField.Text);
                            PrintTagStuff(FieldText);
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        private void CommandBinding_Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == true)
            {
                if (!ofd.FileName.Contains(Properties.Settings.Default.ODSTEKPath + @"\tags"))
                {
                    MessageBox.Show("You tried to open a tag outside of your working directory. Bad!", "Oops...");
                }
                else
                {
                    string[] OpenPath = GetTagsRelativePath(ofd.FileName).Split('.');
                    OpenTag = new Bungie.Tags.TagFile(Bungie.Tags.TagPath.FromPathAndExtension(OpenPath[0], OpenPath[1]));
                    WPFConsoleWriteLine("Opened " + OpenTag.Path);
                    foreach (var field in OpenTag.Fields)
                    {
                        PrintFieldValues(field);
                    }

                    //Debug.WriteLine(ofd.FileName.Substring(Properties.Settings.Default.ODSTEKPath.Length + 1)); // Do not want the \
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
            }
        }

        private void CommandBinding_New_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenTag = new Bungie.Tags.TagFile();
            SaveFileDialog sfd = new SaveFileDialog
            {
                InitialDirectory = Properties.Settings.Default.ODSTEKPath + @"\tags"
            };
            if (sfd.ShowDialog() == true)
            {
                string[] SavePath = GetTagsRelativePath(sfd.FileName).Split('.');
                //Debug.WriteLine(SavePath[0] + " " + SavePath[1]);
                System.IO.File.Create(sfd.FileName);
                OpenTag.Load(Bungie.Tags.TagPath.FromPathAndExtension(SavePath[0], SavePath[1]));
            }
            
            foreach (var field in OpenTag.Fields)
            {
                PrintFieldValues(field);
            }
        }

        private void CommandBinding_Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog
            {
                InitialDirectory = Properties.Settings.Default.ODSTEKPath + @"\tags"
            };
            if (sfd.ShowDialog() == true)
            {
                string[] SavePath = GetTagsRelativePath(sfd.FileName).Split('.');
                //Debug.WriteLine(SavePath[0] + " " + SavePath[1]);
                OpenTag.SaveAs(Bungie.Tags.TagPath.FromPathAndExtension(SavePath[0], SavePath[1]));
            }
        }
    }
}
