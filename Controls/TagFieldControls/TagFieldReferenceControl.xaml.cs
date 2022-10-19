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
using Microsoft.Win32;

namespace PresentationODST.Controls.TagFieldControls
{
    /// <summary>
    /// Interaction logic for TagFieldReferenceControl.xaml
    /// </summary>
    public partial class TagFieldReferenceControl : UserControl
    {
        public TagFieldReferenceControl()
        {
            InitializeComponent();
        }

        private Bungie.Tags.TagFieldReference _TagField;
        public Bungie.Tags.TagFieldReference TagField
        {
            get
            {
                return _TagField;
            }
            set
            {
                _TagField = value;
                NameTextBlock.Text = value.FieldName;
                if (value.Units.Length > 0)
                {
                    TypeTextBlock.Visibility = Visibility.Visible;
                    if (Properties.Settings.Default.FieldTypes)
                    {
                        TypeTextBlock.Text = value.Units + " (" + value.FieldType.ToString().ToLower() + ")";
                    }
                    else
                    {
                        TypeTextBlock.Text = value.Units;
                    }
                }
                else
                {
                    TypeTextBlock.Text = "(" + value.FieldType.ToString().ToLower() + ")";
                    TypeTextBlock.Visibility = Properties.Settings.Default.FieldTypes ? Visibility.Visible : Visibility.Hidden;
                }
                if (value.Description.Length > 0)
                {
                    HintTextBlock.Visibility = Visibility.Visible;
                    ((ToolTip)ToolTipService.GetToolTip(HintTextBlock)).Content = value.Description;
                }
                ValidateValueText();
            }
        }

        private void ValidateValueText()
        {
            bool PointsSomewhere = _TagField.Reference.ReferencePointsSomewhere();
            bool ValidFile = _TagField.Reference.ReferencePointsToValidTagFile();
            ValueTextBox.Text = PointsSomewhere ? TagField.Reference.Path.RelativePathWithExtension : "";
            ValueTextBox.Foreground = ValidFile ? Utilities.WPF.BlackBrush : Utilities.WPF.RedBrush;
            OpenTagButton.IsEnabled = ValidFile;
            ClearTagButton.IsEnabled = PointsSomewhere;
        }

        // future me please write some code to handle this it will not be that hard I promise
        public string[] ValidGroupTypes = { "none" };

        private void SelectTagButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                InitialDirectory = ValueTextBox.Text.Length > 0 ? System.IO.Path.GetDirectoryName(Utilities.Path.ODSTEKTagsPath + ValueTextBox.Text.Split('.')[0]) : Utilities.Path.ODSTEKTagsPath,
                Filter = ValidGroupTypes[0] != "none" ? String.Join("|", ValidGroupTypes) : "All Tags (*.*)|*.*" // I am not adding all tag types because that makes Foundation lag to hell
            };
            if (ofd.ShowDialog() == true)
            {
                ValueTextBox.Text = Utilities.Path.GetTagsRelativePath(ofd.FileName);
                TagField.Reference.Path = Bungie.Tags.TagPath.FromPathAndExtension(ValueTextBox.Text.Split('.')[0], ValueTextBox.Text.Split('.')[1]);
                ValidateValueText();
                // need to limit this to only tags
            }
        }

        private void OpenTagButton_Click(object sender, RoutedEventArgs e)
        {
            ManagedBlam.Tags.OpenTag(ValueTextBox.Text);
        }

        private void ClearTagButton_Click(object sender, RoutedEventArgs e)
        {
            ValueTextBox.Text = "";
            _TagField.Reference.Path = null;
        }
    }
}
