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

namespace PresentationODST.Controls.LayoutDocuments
{
    /// <summary>
    /// Interaction logic for TagView.xaml
    /// </summary>
    public partial class TagView : UserControl
    {
        public Bungie.Tags.TagFile TagFile;

        public TagView()
        {
            InitializeComponent();
            DataContext = this;
            if (Properties.Settings.Default.EasterEggs)
            {
                Random rand = new Random();
                TagGrid.Background = new SolidColorBrush(Color.FromArgb(255, (byte)rand.Next(0, 255), (byte)rand.Next(0, 255), (byte)rand.Next(0, 255)));
            }
        }

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
