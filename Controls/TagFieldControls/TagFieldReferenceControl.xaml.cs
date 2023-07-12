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
using PresentationODST.Utilities;

namespace PresentationODST.Controls.TagFieldControls
{
    public partial class TagFieldReferenceControl : UserControl, ITagFieldControlBase
    {
        public TagFieldReferenceControl()
        {
            InitializeComponent();
        }

        public Bungie.Tags.TagField GetTagField()
        {
            return _TagField;
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
                GetValidGroups();
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

        private void GetValidGroups()
        {
            IEnumerable<Bungie.Tags.TagGroupType> ValidGroups = TagField.Definition.GetAllowedGroupTypes();
            if (ValidGroups.Count() <= 0) return;
            TagReferenceFilter = "tags (";
            ValidGroupTypes = new List<string>();
            foreach (Bungie.Tags.TagGroupType GroupType in ValidGroups) 
            {
                ValidGroupTypes.Add(GroupType.Extension);
            }
            foreach (string s in ValidGroupTypes)
            {
                TagReferenceFilter += "*." + s + ";";
            }
            TagReferenceFilter = TagReferenceFilter.Substring(0, TagReferenceFilter.Length - 1);
            // cut off the last ; because whilst there is probably a better way I cannot be bothered right now
            TagReferenceFilter += ")|";
            foreach (string s in ValidGroupTypes)
            {
                TagReferenceFilter += "*." + s + ";";
            }
            TagReferenceFilter = TagReferenceFilter.Substring(0, TagReferenceFilter.Length - 1);
            if (Properties.Settings.Default.ExpertMode)
                TagReferenceFilter += "|all tags (*.*)|*.*";
            // I am bad at building strings but it works and doesn't cause lag
        }

        private List<string> ValidGroupTypes = new List<string>();
        private string TagReferenceFilter = "all tags (*.*)|*.*";

        private void SelectTagButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                InitialDirectory = ValueTextBox.Text.Length > 0 ? System.IO.Path.GetDirectoryName(Utilities.Path.ODSTEKTagsPath + ValueTextBox.Text.Split('.')[0]) : Utilities.Path.ODSTEKTagsPath,
                Filter = TagReferenceFilter,
                FileName = TagField.Reference.ReferencePointsSomewhere() ? TagField.Reference.Path.ShortName : ""
            };
            if (ofd.ShowDialog() == true)
            {
                ValueTextBox.Text = Utilities.Path.GetTagsRelativePath(ofd.FileName);
                TagField.Reference.Path = Bungie.Tags.TagPath.FromPathAndExtension(ValueTextBox.Text.Split('.')[0], ValueTextBox.Text.Split('.')[1]);
                ValidateValueText();
            }
        }

        private void OpenTagButton_Click(object sender, RoutedEventArgs e)
        {
            ManagedBlam.Tags.OpenTag(ValueTextBox.Text);
        }

        private void ClearTagButton_Click(object sender, RoutedEventArgs e)
        {
            ValueTextBox.Text = "";
            TagField.Reference.Path = null;
        }

        private void CopyPath_Click(object sender, RoutedEventArgs e)
        {
            if (!TagField.Reference.ReferencePointsSomewhere()) return;
            Clipboard.SetText(TagField.Reference.Path.RelativePathWithExtension);
        }

        private void PastePath_Click(object sender, RoutedEventArgs e)
        {
            Bungie.Tags.TagPath newPath;
            string[] ClipboardRef = Clipboard.GetText().Split('.');
            if (ClipboardRef.Length > 2)
            {
                WPF.Log("{0}: Could not get group type from clipboard", TagField.FieldPath);
                return;
            }
            if (ClipboardRef.Length < 2)
            {
                ClipboardRef = Clipboard.GetText().Split(',');
                if (ClipboardRef.Length < 2 || ClipboardRef.Length > 2)
                {
                    WPF.Log("{0}: Could not get group type from clipboard", TagField.FieldPath);
                    return;
                }
                try
                {
                    ClipboardRef[1] = Bungie.Tags.TagGroupType.GetExtensionFromGroupType(ClipboardRef[1]);
                }    
                catch
                {
                    WPF.Log("{0}: Could not get group type from {1}", TagField.FieldPath, Clipboard.GetText().Split('.')[1]);
                }
            }
            if (!ValidGroupTypes.Contains(ClipboardRef[1]))
            {
                WPF.Log("{0}: {1} is not a valid group type for this field", TagField.FieldPath, ClipboardRef[1]);
                return;
            }
            try
            {
                newPath = Bungie.Tags.TagPath.FromPathAndExtension(ClipboardRef[0], ClipboardRef[1]);
            }
            catch 
            {
                WPF.Log("{0}: Could not validate tag path \"{1}\"", TagField.FieldPath, Clipboard.GetText());
                return;
            }
            ValueTextBox.Text = Clipboard.GetText();
            TagField.Reference.Path = newPath;
            ValidateValueText();
        }
    }
}
