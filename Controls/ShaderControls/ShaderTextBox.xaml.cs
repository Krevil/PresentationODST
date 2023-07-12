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

namespace PresentationODST.Controls.ShaderControls
{
    public partial class ShaderTextBox : UserControl
    {
        public ShaderTextBox()
        {
            InitializeComponent();
        }

        private Bungie.Tags.TagFieldElementStringID _TagField;
        public Bungie.Tags.TagFieldElementStringID TagField
        {
            get
            {
                return _TagField;
            }
            set
            {
                _TagField = value;
                ValueTextBox.Text = value.GetStringData();
            }
        }

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

        private void ValueTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!IsLoaded) return;
            if (ValueTextBox.Text.Length > TagField.MaxLength)
            {
                ValueTextBox.Foreground = Utilities.WPF.RedBrush;
            }
            ValueTextBox.Foreground = Utilities.WPF.BlackBrush;
            _TagField.SetStringData(ValueTextBox.Text);
        }
    }
}
