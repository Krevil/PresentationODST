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
using PresentationODST.Utilities;

namespace PresentationODST.Controls.ShaderControls
{
    /// <summary>
    /// Interaction logic for ShaderComboBox.xaml
    /// </summary>
    public partial class ShaderComboBox : UserControl
    {
        public ShaderComboBox()
        {
            InitializeComponent();
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
                // Get whatever is currently selected and make the ComboBox show that.
                int.TryParse(value.GetStringData(), out int MethodOptionValueIndex);
                ValueComboBox.SelectedIndex = MethodOptionValueIndex;
                _TagField = value;
            }
        }

        private void ValueComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded) return;
            TagField.SetStringData(ValueComboBox.SelectedIndex.ToString());
            // also needs to set the shader template in postprocess I think
        }
    }
}
