using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using PresentationODST.Controls;
using PresentationODST.Controls.TagFieldControls;
using PresentationODST.Controls.LayoutDocuments;
using PresentationODST.Utilities;
using Xceed.Wpf.AvalonDock.Layout;
using PresentationODST.Dialogs;

namespace PresentationODST.ManagedBlam
{
    public static class Tags
    {
        public static readonly Bungie.Tags.TagGroupType[] TagGroups = Bungie.Tags.TagGroupType.GetTagGroups();
        public static List<Bungie.Tags.TagFile> OpenTags = new List<Bungie.Tags.TagFile>();

        public static void OpenTag(string filename)
        {
            string[] OpenPath = Path.GetTagsRelativePath(filename).Split('.');
            if (Bungie.Tags.TagGroupType.GetGroupTypeFromExtension(OpenPath[1]) == null)
            {
                CustomMessageBox.Show("Unsupported file type", "Tag Load Error");
                return;
            }
            Bungie.Tags.TagPath OpenTagPath = Bungie.Tags.TagPath.FromPathAndExtension(OpenPath[0], OpenPath[1]);
            if (!System.IO.File.Exists(Path.ODSTEKTagsPath + OpenTagPath.RelativePathWithExtension))
            {
                CustomMessageBox.Show("Couldn't find tag file", "Tag Load Error");
                return;
            }
            // Maybe stop users from opening the same tag more than once? Won't break anything by doing so, other than the user's fragile sanity
            LayoutDocumentPane ldp = MainWindow.Main_Window.TagDocuments;
            Bungie.Tags.TagFile NewTag = new Bungie.Tags.TagFile(OpenTagPath);
            
            LayoutDocument TagTab = new LayoutDocument
            {
                Title = NewTag.Path.ShortNameWithExtension,
                Content = new TagView()
            };
            TagView NewTagView = (TagView)TagTab.Content;
            NewTagView.TagFile = NewTag;
            NewTagView.TagRelativePath.Text = NewTag.Path.RelativePathWithExtension;
            TagTab.ToolTip = new ToolTip { Content = OpenTagPath.RelativePathWithExtension };
            foreach (Bungie.Tags.TagField field in NewTag.Fields)
            {
                AddFieldValues(NewTagView.TagGrid, field);
            }
            ldp.Children.Add(TagTab);
            ldp.SelectedContentIndex = ldp.Children.IndexOf(TagTab);
            OpenTags.Add(NewTag);
        }

        public static void NewTag()
        {
            MainWindow.GroupSelector = new Dialogs.TagGroupSelector();
            if (MainWindow.GroupSelector.ShowDialog() == true)
            {
                LayoutDocumentPane ldp = MainWindow.Main_Window.TagDocuments;
                Bungie.Tags.TagFile NewTag = new Bungie.Tags.TagFile();
                Bungie.Tags.TagGroupType SelectedItem = (Bungie.Tags.TagGroupType)MainWindow.GroupSelector.TagListBox.SelectedItem;
                Bungie.Tags.TagPath NewPath = Bungie.Tags.TagPath.FromPathAndExtension("tag" + MainWindow.NewTagCount, SelectedItem.Extension);
                NewTag.New(NewPath);
                LayoutDocument TagTab = new LayoutDocument
                {
                    Title = "tag" + MainWindow.NewTagCount + "." + SelectedItem.Extension,
                    Content = new TagView()
                };
                TagView NewTagView = (TagView)TagTab.Content;
                NewTagView.TagFile = NewTag;
                NewTagView.TagRelativePath.Text = NewTag.Path.RelativePathWithExtension;
                foreach (Bungie.Tags.TagField field in NewTag.Fields)
                {
                    AddFieldValues(NewTagView.TagGrid, field);
                }
                ldp.Children.Add(TagTab);
                ldp.SelectedContentIndex = ldp.IndexOfChild(TagTab);
                MainWindow.NewTagCount++;
                OpenTags.Add(NewTag);
            }
        }

        // Migrate each case's operations to new methods
        public static void AddFieldValues(Grid grid, Bungie.Tags.TagField field)
        {
            int RowIndex;
            bool FieldVisible = WPF.ExpertModeVisibility(field);
            switch (field.FieldType)
            {
                case Bungie.Tags.TagFieldType.RealFraction:
                case Bungie.Tags.TagFieldType.Angle:
                case Bungie.Tags.TagFieldType.CharInteger:
                case Bungie.Tags.TagFieldType.ShortInteger:
                case Bungie.Tags.TagFieldType.LongInteger:
                case Bungie.Tags.TagFieldType.Int64Integer:
                case Bungie.Tags.TagFieldType.Real:
                    RowIndex = grid.Children.Add(new TagFieldElementControl { TagField = (Bungie.Tags.TagFieldElement)field, Visibility = WPF.ExpertModeVisibility(field) ? Visibility.Visible : Visibility.Collapsed });
                    if (FieldVisible) 
                        WPF.AddNewRow(grid, RowIndex, 31);
                    break;
                case Bungie.Tags.TagFieldType.CharEnum:
                case Bungie.Tags.TagFieldType.ShortEnum:
                case Bungie.Tags.TagFieldType.LongEnum:
                    RowIndex = grid.Children.Add(new TagFieldEnumControl { TagField = (Bungie.Tags.TagFieldEnum)field, Visibility = WPF.ExpertModeVisibility(field) ? Visibility.Visible : Visibility.Collapsed });
                    if (FieldVisible)
                        WPF.AddNewRow(grid, RowIndex, 31);
                    break;
                case Bungie.Tags.TagFieldType.Reference:
                    RowIndex = grid.Children.Add(new TagFieldReferenceControl { TagField = (Bungie.Tags.TagFieldReference)field, Visibility = WPF.ExpertModeVisibility(field) ? Visibility.Visible : Visibility.Collapsed });
                    if (FieldVisible)
                        WPF.AddNewRow(grid, RowIndex, 31);
                    break;
                case Bungie.Tags.TagFieldType.Block:
                    RowIndex = grid.Children.Add(new TagFieldBlockControl { TagField = (Bungie.Tags.TagFieldBlock)field, Visibility = WPF.ExpertModeVisibility(field) ? Visibility.Visible : Visibility.Collapsed });
                    if (FieldVisible)
                        WPF.AddNewRow(grid, RowIndex);
                    break;
                case Bungie.Tags.TagFieldType.Struct:
                    foreach (Bungie.Tags.TagElement StructElement in ((Bungie.Tags.TagFieldStruct)field).Elements)
                    {
                        foreach (Bungie.Tags.TagField subfield in StructElement.Fields)
                        {
                            AddFieldValues(grid, subfield); // Probably don't need a new control for struct fields?
                        }
                    }
                    break;
                case Bungie.Tags.TagFieldType.StringId:
                    RowIndex = grid.Children.Add(new TagFieldStringIDControl { TagField = (Bungie.Tags.TagFieldElementStringID)field, Visibility = WPF.ExpertModeVisibility(field) ? Visibility.Visible : Visibility.Collapsed });
                    if (FieldVisible)
                        WPF.AddNewRow(grid, RowIndex, 31);
                    break;
                case Bungie.Tags.TagFieldType.OldStringId: // Why does this exist?
                    RowIndex = grid.Children.Add(new TagFieldOldStringIDControl { TagField = (Bungie.Tags.TagFieldElementOldStringID)field, Visibility = WPF.ExpertModeVisibility(field) ? Visibility.Visible : Visibility.Collapsed });
                    if (FieldVisible)
                        WPF.AddNewRow(grid, RowIndex, 31);
                    break;
                case Bungie.Tags.TagFieldType.LongString:
                case Bungie.Tags.TagFieldType.String:
                    RowIndex = grid.Children.Add(new TagFieldStringControl { TagField = (Bungie.Tags.TagFieldElementString)field, Visibility = WPF.ExpertModeVisibility(field) ? Visibility.Visible : Visibility.Collapsed });
                    if (FieldVisible)
                        WPF.AddNewRow(grid, RowIndex, 31);
                    break;
                case Bungie.Tags.TagFieldType.ByteFlags:
                case Bungie.Tags.TagFieldType.WordFlags:
                case Bungie.Tags.TagFieldType.Flags:
                    RowIndex = grid.Children.Add(new TagFieldFlagsControl { TagField = (Bungie.Tags.TagFieldFlags)field, Visibility = WPF.ExpertModeVisibility(field) ? Visibility.Visible : Visibility.Collapsed });
                    if (FieldVisible)
                        WPF.AddNewRow(grid, RowIndex, 29 + ((((Bungie.Tags.TagFieldFlags)field).Items.Length - 1) * 19)); //flag fields size varies
                    break;
                case Bungie.Tags.TagFieldType.BlockFlags:
                case Bungie.Tags.TagFieldType.ByteBlockFlags:
                case Bungie.Tags.TagFieldType.WordBlockFlags:
                    RowIndex = grid.Children.Add(new TagFieldBlockFlagsControl { TagField = (Bungie.Tags.TagFieldBlockFlags)field, Visibility = WPF.ExpertModeVisibility(field) ? Visibility.Visible : Visibility.Collapsed });
                    if (FieldVisible)
                        WPF.AddNewRow(grid, RowIndex, 29 + ((((Bungie.Tags.TagFieldBlockFlags)field).Items.Length - 1) * 19)); //flag fields size varies
                    break;
                case Bungie.Tags.TagFieldType.ShortBlockIndexCustomSearch:
                case Bungie.Tags.TagFieldType.CharBlockIndexCustomSearch:
                case Bungie.Tags.TagFieldType.LongBlockIndexCustomSearch:
                case Bungie.Tags.TagFieldType.CharBlockIndex:
                case Bungie.Tags.TagFieldType.ShortBlockIndex:
                case Bungie.Tags.TagFieldType.LongBlockIndex:
                    RowIndex = grid.Children.Add(new TagFieldBlockIndexControl { TagField = (Bungie.Tags.TagFieldBlockIndex)field, Visibility = WPF.ExpertModeVisibility(field) ? Visibility.Visible : Visibility.Collapsed });
                    if (FieldVisible)
                        WPF.AddNewRow(grid, RowIndex, 31);
                    break;
                case Bungie.Tags.TagFieldType.RealQuaternion:
                    RowIndex = grid.Children.Add(new TagFieldElement4dControl { TagField = (Bungie.Tags.TagFieldElementArray)field, Visibility = WPF.ExpertModeVisibility(field) ? Visibility.Visible : Visibility.Collapsed });
                    if (FieldVisible)
                        WPF.AddNewRow(grid, RowIndex, 31);
                    break;
                case Bungie.Tags.TagFieldType.RealVector3d:
                case Bungie.Tags.TagFieldType.RealEulerAngles3d:
                case Bungie.Tags.TagFieldType.RealPlane3d:
                case Bungie.Tags.TagFieldType.RealPoint3d:
                    RowIndex = grid.Children.Add(new TagFieldElement3dControl { TagField = (Bungie.Tags.TagFieldElementArray)field, Visibility = WPF.ExpertModeVisibility(field) ? Visibility.Visible : Visibility.Collapsed });
                    if (FieldVisible)
                        WPF.AddNewRow(grid, RowIndex, 31);
                    break;
                case Bungie.Tags.TagFieldType.RealEulerAngles2d:
                case Bungie.Tags.TagFieldType.AngleBounds:
                case Bungie.Tags.TagFieldType.Point2d:
                case Bungie.Tags.TagFieldType.RealPlane2d:
                case Bungie.Tags.TagFieldType.RealPoint2d:
                case Bungie.Tags.TagFieldType.RealVector2d:
                case Bungie.Tags.TagFieldType.Rectangle2d:
                case Bungie.Tags.TagFieldType.RealFractionBounds:
                case Bungie.Tags.TagFieldType.RealBounds:
                case Bungie.Tags.TagFieldType.ShortIntegerBounds:
                    RowIndex = grid.Children.Add(new TagFieldElement2dControl { TagField = (Bungie.Tags.TagFieldElementArray)field, Visibility = WPF.ExpertModeVisibility(field) ? Visibility.Visible : Visibility.Collapsed });
                    if (FieldVisible)
                        WPF.AddNewRow(grid, RowIndex, 31);
                    break;
                case Bungie.Tags.TagFieldType.RealRgbColor:
                case Bungie.Tags.TagFieldType.RgbPixel32:
                    RowIndex = grid.Children.Add(new TagFieldElementRgbColorControl { TagField = (Bungie.Tags.TagFieldElementArray)field, Visibility = WPF.ExpertModeVisibility(field) ? Visibility.Visible : Visibility.Collapsed });
                    if (FieldVisible)
                        WPF.AddNewRow(grid, RowIndex, 31);
                    break;
                case Bungie.Tags.TagFieldType.ArgbPixel32:
                case Bungie.Tags.TagFieldType.RealArgbColor:
                    RowIndex = grid.Children.Add(new TagFieldElementArgbColorControl { TagField = (Bungie.Tags.TagFieldElementArray)field, Visibility = WPF.ExpertModeVisibility(field) ? Visibility.Visible : Visibility.Collapsed });
                    if (FieldVisible)
                        WPF.AddNewRow(grid, RowIndex, 31);
                    break;
                case Bungie.Tags.TagFieldType.Data:
                    RowIndex = grid.Children.Add(new TagFieldDataControl { TagField = (Bungie.Tags.TagFieldData)field, Visibility = WPF.ExpertModeVisibility(field) ? Visibility.Visible : Visibility.Collapsed });
                    if (FieldVisible)
                        WPF.AddNewRow(grid, RowIndex, 106);
                    break;
                case Bungie.Tags.TagFieldType.Explanation:
                    RowIndex = grid.Children.Add(new TagFieldExplanationControl { TagField = (Bungie.Tags.TagFieldExplanation)field, Visibility = WPF.ExpertModeVisibility(field) ? Visibility.Visible : Visibility.Collapsed });
                    if (FieldVisible)
                        WPF.AddNewRow(grid, RowIndex);
                    break;
                case Bungie.Tags.TagFieldType.Custom:
                    // I don't think these are needed, at least not right now, as they appear to be used for special buttons within Guerilla - look into these when/if doing bitmap and/or model support
                    //WPF.Log("Tag Loader: " + field.FieldType + " with the path " + field.FieldPath + " could not be added as they are not yet supported");
                    break;
                default:
                    WPF.Log("Tag Loader: " + field.FieldType + " with the path " + field.FieldPath + " could not be added as no case for the type exists");
                    break;
            }
        }
    }
}
