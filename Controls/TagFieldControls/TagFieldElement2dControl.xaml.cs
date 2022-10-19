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
    public partial class TagFieldElement2dControl : UserControl
    {
        public TagFieldElement2dControl()
        {
            InitializeComponent();
            DataContext = this;

        }

        private Bungie.Tags.TagFieldElementArraySingle _TagField;
        public Bungie.Tags.TagFieldElementArraySingle TagField
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
                switch (value.FieldType)
                {
                    case Bungie.Tags.TagFieldType.RealEulerAngles2d:
                        Value1TypeTextBlock.Text = "y";
                        Value2TypeTextBlock.Text = "p";
                        break;
                    case Bungie.Tags.TagFieldType.Point2d:
                    case Bungie.Tags.TagFieldType.RealPlane2d:
                    case Bungie.Tags.TagFieldType.RealPoint2d:
                    case Bungie.Tags.TagFieldType.RealVector2d:
                    case Bungie.Tags.TagFieldType.Rectangle2d:
                        Value1TypeTextBlock.Text = "x";
                        Value2TypeTextBlock.Text = "y";
                        break;
                    case Bungie.Tags.TagFieldType.RealFractionBounds:
                    case Bungie.Tags.TagFieldType.RealBounds:
                    case Bungie.Tags.TagFieldType.AngleBounds:
                        Value1TypeTextBlock.Text = "";
                        Value2TypeTextBlock.Text = "to";
                        break;
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
                _TagField.SetStringData(new string[] { Value1TextBox.Text, Value2TextBox.Text });
        }

        private void Value2TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!IsLoaded) return;
            if (double.TryParse(Value2TextBox.Text, out _))
                _TagField.SetStringData(new string[] { Value1TextBox.Text, Value2TextBox.Text });
        }

    }
}
