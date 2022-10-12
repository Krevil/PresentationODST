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

namespace PresentationODST.Controls
{
    /// <summary>
    /// Interaction logic for TagFieldEnumControl.xaml
    /// </summary>
    public partial class TagFieldBlockIndexControl : UserControl
    {
        public TagFieldBlockIndexControl()
        {
            InitializeComponent();
            DataContext = this;
        }

        private Bungie.Tags.TagFieldBlockIndex _TagField;
        public Bungie.Tags.TagFieldBlockIndex TagField
        {
            get
            {
                return _TagField;
            }
            set
            {
                _TagField = value;
                foreach (Bungie.Tags.TagFieldBlockIndex.TagFieldBlockIndexItem IndexItem in value.Items)
                {
                    ValueComboBox.Items.Add(IndexItem.BlockIndexName);

                    //ValueComboBox.Items.Add(EnumItem.EnumIndex + ". " + EnumItem.EnumName);
                    // Preference setting should change whether to display the enumindex
                }
                ValueComboBox.SelectedIndex = value.Value + 1; // I do not like this one bit
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

        private void ValueComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded) return;
            _TagField.Value = ValueComboBox.SelectedIndex - 1;
        }
    }
}
