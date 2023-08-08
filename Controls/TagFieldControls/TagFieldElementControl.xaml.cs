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
    public partial class TagFieldElementControl : UserControl, ITagFieldControlBase
    {
        public TagFieldElementControl()
        {
            InitializeComponent();
            DataContext = this;
            
        }

        public Bungie.Tags.TagField GetTagField()
        {
            return _TagField;
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
                Utilities.WPF.AddFieldContextMenu(GridContextMenu, _TagField);
            }
        }

        private void ValueTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!IsLoaded) return;
            if (double.TryParse(ValueTextBox.Text, out _))
            {
                _TagField.SetStringData(ValueTextBox.Text);
                ((TextBlock)((Button)GridContextMenu.Items[1]).Content).Text = "Field Checksum: " + _TagField.CalculateFieldChecksum().ToString();
            }
        }
    }
}
