using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using PresentationODST.ManagedBlam;

namespace PresentationODST
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            PresentationODST.Properties.Settings.Default.OpenTags = new System.Collections.Specialized.StringCollection();
            if (!Bungie.ManagedBlamSystem.IsInitialized) return;
            if (Tags.OpenTags == null) return;
            foreach (Bungie.Tags.TagFile tagFile in Tags.OpenTags)
            {
                if (tagFile == null) continue;
                if (tagFile.Path == null) continue;
                PresentationODST.Properties.Settings.Default.OpenTags.Add(tagFile.Path.RelativePathWithExtension);
            }
            PresentationODST.Properties.Settings.Default.Save();
        }
    }
}
