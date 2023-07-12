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
    public partial class TagFieldBlockIndexControl : UserControl, ITagFieldControlBase
    {
        public TagFieldBlockIndexControl()
        {
            InitializeComponent();
            DataContext = this;
        }

        public Bungie.Tags.TagField GetTagField()
        {
            return _TagField;
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
                    ValueComboBox.Items.Add(new TextBlock { Text = IndexItem.BlockIndexName });
                }
                ValueComboBox.SelectedIndex = TagField.Value + 1; // I do not like this one bit
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
            }
        }

        private void ValueComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded) return;
            _TagField.Value = ValueComboBox.SelectedIndex - 1;
        }
    }
}
