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
    public partial class TagFieldElement4dControl : UserControl
    {
        public TagFieldElement4dControl()
        {
            InitializeComponent();
            DataContext = this;

        }

        private Bungie.Tags.TagFieldElementArray _TagField;
        public Bungie.Tags.TagFieldElementArray TagField
        {
            get
            {
                return _TagField;
            }
            set
            {
                _TagField = value;
                Value1TextBox.Text = value.GetStringData()[0];
                Value2TextBox.Text = value.GetStringData()[1];
                Value3TextBox.Text = value.GetStringData()[2];
                Value4TextBox.Text = value.GetStringData()[3];
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

        private void Value1TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!IsLoaded) return;
            if (double.TryParse(Value1TextBox.Text, out _))
                _TagField.SetStringData(new string[] { Value1TextBox.Text, Value2TextBox.Text, Value3TextBox.Text, Value4TextBox.Text });
        }

        private void Value2TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!IsLoaded) return;
            if (double.TryParse(Value2TextBox.Text, out _))
                _TagField.SetStringData(new string[] { Value1TextBox.Text, Value2TextBox.Text, Value3TextBox.Text, Value4TextBox.Text });
        }

        private void Value3TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!IsLoaded) return;
            if (double.TryParse(Value3TextBox.Text, out _))
                _TagField.SetStringData(new string[] { Value1TextBox.Text, Value2TextBox.Text, Value3TextBox.Text, Value4TextBox.Text });
        }

        private void Value4TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!IsLoaded) return;
            if (double.TryParse(Value4TextBox.Text, out _))
                _TagField.SetStringData(new string[] { Value1TextBox.Text, Value2TextBox.Text, Value3TextBox.Text, Value4TextBox.Text });
        }
    }
}
