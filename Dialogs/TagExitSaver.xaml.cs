using Microsoft.Win32;
using PresentationODST.Controls.LayoutDocuments;
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

namespace PresentationODST.Dialogs
{
    public partial class TagExitSaver : Window
    {
        public List<TagToSave> TagsToSave { get; set; }

        public TagExitSaver(List<TagToSave> tagsToSave)
        {
            TagsToSave = tagsToSave;
            DataContext = this;
            InitializeComponent();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (TagToSave tts in TagsToSave)
            {
                if (!tts.ShouldSave) continue;

                if (tts.TagView.TagExists())
                {
                    tts.TagView.Save();
                }
                else
                {
                    CustomMessageBox cmb = new CustomMessageBox("Save the tag " + tts.TagName + " as a new tag?", "Save Tag", CustomMessageBox.ButtonType.YesNo);
                    if (cmb.ShowDialog() == true)
                    {
                        SaveFileDialog sfd = new SaveFileDialog
                        {
                            InitialDirectory = Utilities.Path.ODSTEKTagsPath
                        };
                        if (sfd.ShowDialog() == true)
                        {
                            string[] SavePath = Utilities.Path.GetTagsRelativePath(sfd.FileName).Split('.');
                            tts.TagView.SaveAs(Bungie.Tags.TagPath.FromPathAndExtension(SavePath[0], SavePath[1]));
                        }
                    }
                        
                }
                
            }
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }

    public class TagToSave
    {
        public string TagName { get; set; }
        public TagView TagView { get; set; }
        public bool ShouldSave { get; set; }
    }
}
