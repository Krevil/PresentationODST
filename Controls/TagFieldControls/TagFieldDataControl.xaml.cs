using PresentationODST.Utilities;
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

namespace PresentationODST.Controls.TagFieldControls
{
    // This is a WIP but you can copy and paste the Base64 to/from Guerilla
    public partial class TagFieldDataControl : UserControl, ITagFieldControlBase
    {
        public TagFieldDataControl()
        {
            InitializeComponent();
            DataContext = this;
            
        }

        public Bungie.Tags.TagField GetTagField()
        {
            return _TagField;
        }

        private Bungie.Tags.TagFieldData _TagField;
        public Bungie.Tags.TagFieldData TagField
        {
            get
            {
                return _TagField;
            }
            set
            {
                _TagField = value;
                if (value.IsEditableAsText)
                {
                    ValueTextBox.Text = value.DataAsText;
                }
                else
                {
                    ValueTextBox.Text = Convert.ToBase64String(value.GetData());
                    //WPF.Log("Test {0}", ((Bungie.Tags.FunctionEditorParameter)value).ValueAsString);
                }      
                NameTextBlock.Text = value.FieldName;
                if (value.Description.Length > 0)
                {
                    HintTextBlock.Visibility = Visibility.Visible;
                    ((ToolTip)ToolTipService.GetToolTip(HintTextBlock)).Content = value.Description;
                }
            }
        }

        private void ValueTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!IsLoaded) return;
            if (_TagField.IsEditableAsText)
            {
                _TagField.SetData(Encoding.UTF8.GetBytes(ValueTextBox.Text));
            }
            else
            {
                byte[] newValue;
                try
                {
                    newValue = Convert.FromBase64String(ValueTextBox.Text);
                }
                catch
                {
                    MessageBox.Show("Could not parse string as Base64", "Error");
                    return;
                }
                _TagField.SetData(newValue);
            }
        }
    }
}
