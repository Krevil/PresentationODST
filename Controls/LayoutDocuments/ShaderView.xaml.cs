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

namespace PresentationODST.Controls.LayoutDocuments
{
    /// <summary>
    /// Interaction logic for ShaderView.xaml
    /// </summary>
    public partial class ShaderView : UserControl
    {
        public ShaderView()
        {
            InitializeComponent();
        }

        public Bungie.Tags.TagFile TagFile;
        public Bungie.Tags.TagFile RenderMethodDefinitionTag;

        public void Save()
        {
            TagFile.Save();
        }

        public void SaveAs(Bungie.Tags.TagPath path)
        {
            TagFile.SaveAs(path);
        }
    }
}
