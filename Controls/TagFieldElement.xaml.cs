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
    /// Interaction logic for TagFieldElement.xaml
    /// </summary>
    public partial class TagFieldElement : UserControl
    {
        public TagFieldElement()
        {
            InitializeComponent();
            DataContext = this;
        }

        public string ElementName
        {
            get
            {
                return NameTextBox.Text;
            }
            set
            {
                NameTextBox.Text = value;
            }
        }

        public string ElementTooltip
        {
            get
            {
                ToolTip tt = (ToolTip)ToolTipService.GetToolTip(HintTextBox);
                return tt.Content.ToString();
            }
            set
            {
                ToolTip tt = (ToolTip)ToolTipService.GetToolTip(HintTextBox);
                tt.Content = value;
            }
        }

        public string ElementType
        {
            get
            {
                return TypeTextBox.Text;
            }
            set
            {
                TypeTextBox.Text = value;
            }
        }

        public string ElementValue
        {
            get
            {
                return ValueTextBox.Text;
            }
            set
            {
                ValueTextBox.Text = value;
            }
        }
    }
}
