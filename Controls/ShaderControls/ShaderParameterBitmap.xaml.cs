using PresentationODST.Utilities;
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
using Microsoft.Win32;

namespace PresentationODST.Controls.ShaderControls
{
    public partial class ShaderParameterBitmap : UserControl
    {
        public ShaderParameterBitmap()
        {
            InitializeComponent();
        }

        public ShaderAnimatedParameter BaseMapScaleUniform { get; set; }
        public ShaderAnimatedParameter BaseMapScaleX { get; set; }
        public ShaderAnimatedParameter BaseMapScaleY { get; set; }
        public ShaderAnimatedParameter BaseMapTranslationX { get; set; }
        public ShaderAnimatedParameter BaseMapTranslationY { get; set; }
        public ShaderAnimatedParameter BaseMapFrameIndex { get; set; }
        public ShaderParameterComboBox BaseMapWrapMode { get; set; }
        public ShaderParameterComboBox BaseMapWrapModeX { get; set; }
        public ShaderParameterComboBox BaseMapWrapModeY { get; set; }
        public ShaderParameterComboBox BaseMapFilterMode { get; set; }
        public ShaderParameterComboBox BaseMapExternMode { get; set; }

        private Bungie.Tags.TagFieldBlock _TagField;
        public Bungie.Tags.TagFieldBlock TagField
        {
            get
            {
                return _TagField;
            }
            set
            {
                _TagField = value;
            }
        }

        public Bungie.Tags.TagFieldReference Bitmap;
        public Bungie.Tags.TagElement Parameter;

        public string FieldName
        {
            get
            {
                return FieldNameTextBox.Text;
            }
            set
            {
                FieldNameTextBox.Text = value;
            }
        }

        public string DefaultValue
        {
            get
            {
                return DefaultValueTextBox.Text;
            }
            set
            {
                DefaultValueTextBox.Text = value;
            }
        }

        public string HelpText
        {
            get
            {
                return ValueTextBox.ToolTip.ToString();
            }
            set
            {
                ValueTextBox.ToolTip = value;
            }
        }

        public void FindParameterValues()
        {
            // look for a bitmap matching the field name in the shader tag's parameters, if it exists fill in the value field
            foreach (Bungie.Tags.TagElement tfe in TagField.Elements)
            {
                if (((Bungie.Tags.TagFieldElementStringID)tfe.Fields[0]).GetStringData() == FieldName)
                {
                    Parameter = tfe;
                    Bitmap = (Bungie.Tags.TagFieldReference)tfe.Fields[2];
                    // Add animated parameters and other values, wrap mode = address mode
                }

            }
        }

        // Everything below here is lifted from TagFieldReferenceControl and could probably be moved into a common class

        private void ValidateValueText()
        {
            bool PointsSomewhere = Bitmap.Reference.ReferencePointsSomewhere();
            bool ValidFile = Bitmap.Reference.ReferencePointsToValidTagFile();
            ValueTextBox.Text = PointsSomewhere ? Bitmap.Reference.Path.RelativePathWithExtension : "";
            ValueTextBox.Foreground = ValidFile ? Utilities.WPF.BlackBrush : Utilities.WPF.RedBrush;
        }

        private void GetValidGroups()
        {
            IEnumerable<Bungie.Tags.TagGroupType> ValidGroups = Bitmap.Definition.GetAllowedGroupTypes();
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
                FileName = Bitmap.Reference.ReferencePointsSomewhere() ? Bitmap.Reference.Path.ShortName : ""
            };
            if (ofd.ShowDialog() == true)
            {
                if (Bitmap == null)
                {
                    AddParameter();
                }
                ValueTextBox.Text = Utilities.Path.GetTagsRelativePath(ofd.FileName);
                Bitmap.Reference.Path = Bungie.Tags.TagPath.FromPathAndExtension(ValueTextBox.Text.Split('.')[0], ValueTextBox.Text.Split('.')[1]);
                ValidateValueText();
            }
        }

        private void AddParameter()
        {
            Parameter = TagField.AddElement();
            ((Bungie.Tags.TagFieldElementStringID)Parameter.Fields[0]).SetStringData(FieldName);
            Bitmap = (Bungie.Tags.TagFieldReference)Parameter.Fields[2];
        }

        private void OpenTag()
        {
            ManagedBlam.Tags.OpenTag(ValueTextBox.Text);
        }

        private void ClearBitmap()
        {
            // We don't delete the parameter afterwards. Probably could, but not worth the effort I think since Guerilla doesn't bother to cleanup.
            ValueTextBox.Text = "";
            Bitmap.Reference.Path = null;
        }

        private void CopyPath()
        {
            if (!Bitmap.Reference.ReferencePointsSomewhere()) return;
            Clipboard.SetText(Bitmap.Reference.Path.RelativePathWithExtension);
        }

        private void PastePath()
        {
            Bungie.Tags.TagPath newPath;
            string[] ClipboardRef = Clipboard.GetText().Split('.');
            if (ClipboardRef.Length > 2)
            {
                WPF.Log("Could not get group type from clipboard");
                return;
            }
            if (ClipboardRef.Length < 2)
            {
                ClipboardRef = Clipboard.GetText().Split(',');
                if (ClipboardRef.Length < 2 || ClipboardRef.Length > 2)
                {
                    WPF.Log("Could not get group type from clipboard");
                    return;
                }
                try
                {
                    ClipboardRef[1] = Bungie.Tags.TagGroupType.GetExtensionFromGroupType(ClipboardRef[1]);
                }
                catch
                {
                    WPF.Log("Could not get group type");
                }
            }
            if (!ValidGroupTypes.Contains(ClipboardRef[1]))
            {
                WPF.Log("This is not a valid group type for this field");
                return;
            }
            try
            {
                newPath = Bungie.Tags.TagPath.FromPathAndExtension(ClipboardRef[0], ClipboardRef[1]);
            }
            catch
            {
                WPF.Log("Could not validate tag path");
                return;
            }
            // Bitmap can be null since this is a Shader and it could be using the default value. If so, let's make it not null.
            if (Bitmap == null)
            {
                AddParameter();
            }
            ValueTextBox.Text = Clipboard.GetText();
            Bitmap.Reference.Path = newPath;
            ValidateValueText();
        }

        private void ValueTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (Bitmap == null) return;
            if (e.KeyboardDevice.Modifiers == ModifierKeys.Control)
            {
                switch (e.Key)
                {
                    case Key.C:
                        CopyPath();
                        break;
                    case Key.V:
                        PastePath();
                        break;
                }
            }         
            else if (e.Key == Key.Delete)
            {
                ClearBitmap();
            }
        }

        private void ValueTextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (Bitmap == null) return;
            if (Bitmap.Reference.ReferencePointsSomewhere() && Bitmap.Reference.ReferencePointsToValidTagFile())
            {
                OpenTag();
            }
        }

        private void ScaleUniform_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
