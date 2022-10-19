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
    /// Interaction logic for TagFieldFlagsControl.xaml
    /// </summary>
    public partial class TagFieldBlockFlagsControl : UserControl
    {
        public TagFieldBlockFlagsControl()
        {
            InitializeComponent();
            DataContext = this;
        }

        private Bungie.Tags.TagFieldBlockFlags _TagField;
        public Bungie.Tags.TagFieldBlockFlags TagField
        {
            get
            {
                return _TagField;
            }
            set
            {
                _TagField = value;
                foreach (Bungie.Tags.TagFieldBlockFlags.TagFieldBlockFlagsItem flag in _TagField.Items)
                {
                    CheckBox box = new CheckBox
                    {
                        Content = flag.FlagName,
                        IsChecked = flag.IsSet
                    };
                    box.Click += new RoutedEventHandler(CheckBox_Click);
                    ValueListBox.Items.Add(box);
                }
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

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            int Index = ValueListBox.Items.IndexOf((CheckBox)sender);
            _TagField.Items[Index].IsSet = (bool)((CheckBox)sender).IsChecked;
            e.Handled = true;
        }

        private void ValueListBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;
        }
    }
}
