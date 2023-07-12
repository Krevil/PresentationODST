using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public partial class TagFieldElementRgbColorControl : UserControl, ITagFieldControlBase
    {
        public TagFieldElementRgbColorControl()
        {
            InitializeComponent();
            DataContext = this;

        }

        public Bungie.Tags.TagField GetTagField()
        {
            return _TagField;
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
                Color fieldColor = new Color();
                switch (value.FieldType)
                {
                    case Bungie.Tags.TagFieldType.RgbPixel32:
                        // In case the parse fails at any point
                        double R, G, B;
                        R = G = B = 0;
                        // This is really weird but I'm pretty sure it's neccessary because Halo doesn't like unsigned values
                        // Also, this is one of the few things that this application does better than Foundation - Foundation will give you the wrong values here
                        double.TryParse(value.GetStringData()[0], out R);
                        double.TryParse(value.GetStringData()[1], out G);
                        double.TryParse(value.GetStringData()[2], out B);
                        Value1TextBox.Text = ((byte)R).ToString();
                        Value2TextBox.Text = ((byte)G).ToString();
                        Value3TextBox.Text = ((byte)B).ToString();
                        fieldColor.A = 255;
                        fieldColor.R = (byte)R;
                        fieldColor.G = (byte)G;
                        fieldColor.B = (byte)B;
                        break;
                    case Bungie.Tags.TagFieldType.RealRgbColor:
                        Value1TextBox.Text = value.GetStringData()[0];
                        Value2TextBox.Text = value.GetStringData()[1];
                        Value3TextBox.Text = value.GetStringData()[2];
                        fieldColor.A = 255;
                        fieldColor.R = (byte)(double.Parse(Value1TextBox.Text) * 255d);
                        fieldColor.G = (byte)(double.Parse(Value2TextBox.Text) * 255d);
                        fieldColor.B = (byte)(double.Parse(Value3TextBox.Text) * 255d);
                        break;
                }
                ValueColorPicker.SelectedColor = fieldColor;
                NameTextBlock.Text = value.FieldName;
                // Pretty sure this isn't needed for colours?
                /*
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
                    case Bungie.Tags.TagFieldType.RealEulerAngles3d:
                    case Bungie.Tags.TagFieldType.RealVector3d:
                        Value1TypeTextBlock.Text = "i";
                        Value2TypeTextBlock.Text = "j";
                        Value3TypeTextBlock.Text = "k";
                        break;
                }
                */
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
                _TagField.SetStringData(new string[] { Value1TextBox.Text, Value2TextBox.Text, Value3TextBox.Text });
        }

        private void Value2TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!IsLoaded) return;
            if (double.TryParse(Value2TextBox.Text, out _))
                _TagField.SetStringData(new string[] { Value1TextBox.Text, Value2TextBox.Text, Value3TextBox.Text });
        }

        private void Value3TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!IsLoaded) return;
            if (double.TryParse(Value3TextBox.Text, out _))
                _TagField.SetStringData(new string[] { Value1TextBox.Text, Value2TextBox.Text, Value3TextBox.Text });
        }

        private void ValueColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (!IsLoaded) return;
            switch (_TagField.FieldType)
            {
                case Bungie.Tags.TagFieldType.RgbPixel32:
                    Value1TextBox.Text = e.NewValue.Value.R.ToString();
                    Value2TextBox.Text = e.NewValue.Value.G.ToString();
                    Value3TextBox.Text = e.NewValue.Value.B.ToString();
                    break;
                case Bungie.Tags.TagFieldType.RealRgbColor:
                    Value1TextBox.Text = (e.NewValue.Value.R / 255d).ToString();
                    Value2TextBox.Text = (e.NewValue.Value.G / 255d).ToString();
                    Value3TextBox.Text = (e.NewValue.Value.B / 255d).ToString();
                    break;

            }
        }
    }
}
