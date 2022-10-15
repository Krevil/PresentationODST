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
    /// <summary>
    /// Interaction logic for TagFieldElement.xaml
    /// </summary>
    public partial class TagFieldElementControl : UserControl
    {
        public TagFieldElementControl()
        {
            InitializeComponent();
            DataContext = this;
            
        }

        private Bungie.Tags.TagFieldElement _TagField;
        public Bungie.Tags.TagFieldElement TagField
        {
            get
            {
                return _TagField;
            }
            set
            {
                _TagField = value;
                ValueTextBox.Text = value.GetStringData();
                NameTextBlock.Text = value.FieldName;
                TypeTextBlock.Text = value.FieldType.ToString().ToLower();
                TypeTextBlock.Visibility = Properties.Settings.Default.FieldTypes ? Visibility.Visible : Visibility.Hidden;
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
            if (double.TryParse(ValueTextBox.Text, out _))
                _TagField.SetStringData(ValueTextBox.Text);
        }
    }
}
