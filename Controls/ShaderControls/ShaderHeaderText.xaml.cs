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
    /// <summary>
    /// Interaction logic for ShaderHeaderText.xaml
    /// </summary>
    public partial class ShaderHeaderText : UserControl
    {
        public ShaderHeaderText()
        {
            InitializeComponent();
        }

        public string Text
        {
            get
            {
                return HeaderText.Text;
            }
            set
            {
                HeaderText.Text = value;
            }
        }
    }
}
