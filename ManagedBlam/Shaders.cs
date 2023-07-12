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
using PresentationODST.Controls.ShaderControls;
using System.Windows.Media.Media3D;
using System.Diagnostics;

namespace PresentationODST.ManagedBlam
{
    public static class Shaders
    {
        public static readonly Bungie.Tags.TagGroupType[] TagGroups = Bungie.Tags.TagGroupType.GetTagGroups();
        public static List<Bungie.Tags.TagFile> OpenTags = new List<Bungie.Tags.TagFile>();

        public static void OpenShader(string filename)
        {
            string[] OpenPath = Path.GetTagsRelativePath(filename).Split('.');
            if (Bungie.Tags.TagGroupType.GetGroupTypeFromExtension(OpenPath[1]) == null)
            {
                MessageBox.Show("Unsupported file type", "Tag Load Error");
                return;
            }
            Bungie.Tags.TagPath OpenTagPath = Bungie.Tags.TagPath.FromPathAndExtension(OpenPath[0], OpenPath[1]);
            if (!System.IO.File.Exists(Path.ODSTEKTagsPath + OpenTagPath.RelativePathWithExtension))
            {
                MessageBox.Show("Couldn't find tag file", "Tag Load Error");
                return;
            }
            // Maybe stop users from opening the same tag more than once? Won't break anything by doing so, other than the user's fragile sanity
            LayoutDocumentPane ldp = MainWindow.Main_Window.TagDocuments;
            Bungie.Tags.TagFile NewTag = new Bungie.Tags.TagFile(OpenTagPath);

            LayoutDocument ShaderTab = new LayoutDocument
            {
                Title = NewTag.Path.ShortNameWithExtension,
                Content = new ShaderView()
            };
            ShaderView NewShaderView = (ShaderView)ShaderTab.Content;
            NewShaderView.TagFile = NewTag;
            ShaderTab.ToolTip = new ToolTip { Content = OpenTagPath.RelativePathWithExtension };           
            AddShaderFields(NewShaderView, NewTag);         
            ldp.Children.Add(ShaderTab);
            ldp.SelectedContentIndex = ldp.Children.IndexOf(ShaderTab);
            OpenTags.Add(NewTag);
        }

        public static void NewShader()
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
                    Content = new ShaderView()
                };
                ShaderView NewShaderView = (ShaderView)TagTab.Content;
                NewShaderView.TagFile = NewTag;
                if (!AddShaderFields(NewShaderView, NewTag)) return;
                ldp.Children.Add(TagTab);
                ldp.SelectedContentIndex = ldp.IndexOfChild(TagTab);
                MainWindow.NewTagCount++;
                OpenTags.Add(NewTag);
            }
        }

        // Values in the options block are the selected indexes for the render_method_definition categories. This is the only block of the render_method_definition we should have
        // to care about. These values also determine the shader template used, so when these change the shader template in postprocess must match it, even if the template doesn't
        // exist.

        // For each option in a RMD's categories, it will have an option name that should display in as the name for the ShaderComboBox and a reference to a render_method_option,
        // this tag will determine the parameters - for us the Controls - that display under the header for the category.
        // If the Category is "off", the header should be collapsed. If there is no render_method_option referenced, then the category has no further parameters. Note that none is
        // different to off and may still display parameters.
        // The other important part of render_method_options is that they will determine both the parameter type - for us the type of the Control, whether it should be a ComboBox,
        // TextBox or other, and also the default values for the parameter which also depends on the parameter type. More on default types later - since parameter types are 
        // hardcoded, it's up to us (me, I don't know why I'm bothering to talk in third person - I'm the only one that works on this) to figure out which fields to read for each
        // parameter type.
        // Another element to consider which interestingly is NOT displayed in Guerilla is that some parameters have Help Text. We should ideally display this help text as a ToolTip
        // when mousing over the Control, or alternatively we could take the Foundation route and have it as a button with a question mark to the side. I like my idea more though
        // as current XAML designs I've made are styled after Guerilla's shader view instead of Foundation's.

        // The parameters blocks in the shader tag are only created when the shader has a "modifier" applied to one of the options in the categories and this "modifier" is then
        // applies to the Animated Parameters block. What types of modifiers can a parameter have? For bitmaps, the modifiers are the bitmap used and the Animated Parameters, the
        // types of which are hardcoded, so we need to write some code for that.

        // Besides the shader template, the rest of the postprocess block appears to only get filled in at runtime. Which is a real relief because this is already going to be a job.

        // There are three unique categories in a shader that are NOT defined by the render_method_definition and are instead hardcoded parts of every shader tag. These are the 
        // Atmosphere Properties, Sorting Properties and the Material(s) and their fields are at the bottom of the shader tag. The Atmosphere and Sorting Properties should be
        // added last, while the Material should be the first thing added to the UI despite it being at the bottom. Thankfully, it's the simplest part of the shader, though we do
        // need to keep in mind that these categories differ for shader_terrain which can have up to 4 materials and shader_water and shader_decal which don't have any material.

        public static bool AddShaderFields(ShaderView shaderView, Bungie.Tags.TagFile tagFile)
        {
            // The first field in a shader is one of those *so lovely* invisible structs that contains everything except the material name. Thanks, Bungie. Not.
            if (!(((Bungie.Tags.TagFieldStruct)tagFile.Fields[0]).Elements[0].Fields[0] is Bungie.Tags.TagFieldReference))
            {
                WPF.Log("Shader View: First field was not a reference - are you trying to open a non-shader tag?");
                return false;
            }
            if (!((Bungie.Tags.TagFieldReference)((Bungie.Tags.TagFieldStruct)tagFile.Fields[0]).Elements[0].Fields[0]).Reference.ReferencePointsSomewhere())
            {
                WPF.Log("Shader View: Shader tag is missing a reference to a render method. This is very bad.");
                return false;
            }

            // These fields should be at the bottom but Guerilla and Foundation put it at the top so I'm not going to rock the boat and change it
            int RowIndex;
            switch (tagFile.Path.Extension)
            {
                case "shader":
                case "shader_cortana":
                case "shader_custom":
                case "shader_foliage":
                case "shader_halogram":
                case "shader_skin":
                    // Ugly code, will have to be changed if shader tags ever get modified to have different field names
                    // What I *SHOULD* do is just have it automatically add all the fields that aren't in the stupid struct but then I need to filter out all the other garbo
                    RowIndex = shaderView.ShaderGrid.Children.Add(new ShaderHeaderText { Text = "MATERIAL" });
                    WPF.AddNewRow(shaderView.ShaderGrid, RowIndex);
                    RowIndex = shaderView.ShaderGrid.Children.Add(new ShaderTextBox { TagField = (Bungie.Tags.TagFieldElementStringID)tagFile.Fields.FirstOrDefault(x => x.FieldName == "material name" ) });
                    WPF.AddNewRow(shaderView.ShaderGrid, RowIndex);
                    break;
                case "shader_terrain":
                    RowIndex = shaderView.ShaderGrid.Children.Add(new ShaderHeaderText { Text = "MATERIALS" });
                    WPF.AddNewRow(shaderView.ShaderGrid, RowIndex);
                    RowIndex = shaderView.ShaderGrid.Children.Add(new ShaderTextBox { TagField = (Bungie.Tags.TagFieldElementStringID)tagFile.Fields.FirstOrDefault(x => x.FieldName == "material name 0") });
                    WPF.AddNewRow(shaderView.ShaderGrid, RowIndex);
                    RowIndex = shaderView.ShaderGrid.Children.Add(new ShaderTextBox { TagField = (Bungie.Tags.TagFieldElementStringID)tagFile.Fields.FirstOrDefault(x => x.FieldName == "material name 1") });
                    WPF.AddNewRow(shaderView.ShaderGrid, RowIndex);
                    RowIndex = shaderView.ShaderGrid.Children.Add(new ShaderTextBox { TagField = (Bungie.Tags.TagFieldElementStringID)tagFile.Fields.FirstOrDefault(x => x.FieldName == "material name 2") });
                    WPF.AddNewRow(shaderView.ShaderGrid, RowIndex);
                    RowIndex = shaderView.ShaderGrid.Children.Add(new ShaderTextBox { TagField = (Bungie.Tags.TagFieldElementStringID)tagFile.Fields.FirstOrDefault(x => x.FieldName == "material name 3") });
                    WPF.AddNewRow(shaderView.ShaderGrid, RowIndex);
                    break;
                case "shader_decal":
                case "shader_water":
                    break;
                default:
                    WPF.Log("Shader View: Unrecognised shader type, cannot add tag fields");
                    throw new Exception("Unrecognised shader type, cannot add tag fields");
            }

            // Now to handle the category options. This has a lot of casting so I will try to comment it for my own later sanity.
            RowIndex = shaderView.ShaderGrid.Children.Add(new ShaderHeaderText { Text = "CATEGORIES" });
            WPF.AddNewRow(shaderView.ShaderGrid, RowIndex);

            // Yeah, we start out with this ugliness. It really doesn't get much better. This grabs the render method definition from the top of the shader tag and opens it up since we'll need it for this part.
            Bungie.Tags.TagFile RenderMethodDefinition = new Bungie.Tags.TagFile(((Bungie.Tags.TagFieldReference)((Bungie.Tags.TagFieldStruct)tagFile.Fields[0]).Elements[0].Fields[0]).Reference.Path);
            // For every element in the categories block, do the following:
            for (int i = 0; i < ((Bungie.Tags.TagFieldBlock)RenderMethodDefinition.Fields[1]).Elements.Count; i++)
            {
                // The current category element
                Bungie.Tags.TagElement Category = ((Bungie.Tags.TagFieldBlock)RenderMethodDefinition.Fields[1]).Elements[i];
                // The value the shader tag has for the category element
                Bungie.Tags.TagFieldElement MethodOptionValue = (Bungie.Tags.TagFieldElement)((Bungie.Tags.TagFieldBlock)((Bungie.Tags.TagFieldStruct)tagFile.Fields[0]).Elements[0].Fields[1]).Elements[i].Fields[0];
                // The combo box we're going to display everthing in. We pass in the TagField so it can be modified by the control.
                ShaderComboBox scb = new ShaderComboBox { TagField = MethodOptionValue };
                // We grab the name of the category from the render method definition
                scb.FieldNameTextBlock.Text = ((Bungie.Tags.TagFieldElementStringID)Category.Fields[0]).GetStringData();
                // The default value for a category doesn't need to be shown, so just set it to an empty string
                scb.DefaultValueTextBlock.Text = "";
                // Now we need to populate the combo box of the ShaderComboBox with the items you can choose from.
                foreach (Bungie.Tags.TagElement te in (Bungie.Tags.TagFieldBlock)Category.Fields[1])
                {
                    scb.ValueComboBox.Items.Add(new ComboBoxItem { Content = new TextBlock { Text = ((Bungie.Tags.TagFieldElementStringID)te.Fields[0]).GetStringData() } });
                    // Open the option?
                    //if (((Bungie.Tags.TagFieldReference)te.Fields[1]).Reference.ReferencePointsSomewhere())
                    //{

                    //}
                }
                
                RowIndex = shaderView.ShaderGrid.Children.Add(scb);
                WPF.AddNewRow(shaderView.ShaderGrid, RowIndex);
            }

            // Next up, a category header for each category, unless it's set to Off
            // This should be handled by the ShaderComboBox so it may be time to start porting code over to it
            // This is where we need to open up the render_method_option tags, so may be worth doing it in the prior foreach loop and hiding the elements?

            // for each category in the render_method_defintion
            for (int i = 0; i < ((Bungie.Tags.TagFieldBlock)RenderMethodDefinition.Fields[1]).Elements.Count; i++)
            {
                // A copy from earlier but we need it again. Notice how the i here doesn't correspond to the shader tag itself? It shouldn't matter because they both should always have the same number of blocks
                // and if this ever changes something has gone really wrong.
                // We should then use this value to determine which Options to add/display in the ShaderView Grid
                Bungie.Tags.TagFieldElement MethodOptionValue = (Bungie.Tags.TagFieldElement)((Bungie.Tags.TagFieldBlock)((Bungie.Tags.TagFieldStruct)tagFile.Fields[0]).Elements[0].Fields[1]).Elements[i].Fields[0];
                // the actual integer value of the shader's option field
                int.TryParse(MethodOptionValue.GetStringData(), out int MethodOptionValueIndex);
                // the current category tag block element
                Bungie.Tags.TagElement MethodOption = ((Bungie.Tags.TagFieldBlock)RenderMethodDefinition.Fields[1]).Elements[i];
                

                // the selected option block, from the shader's value which we can set using the previously created comboboxes
                Bungie.Tags.TagElement te = ((Bungie.Tags.TagFieldBlock)MethodOption.Fields[1]).Elements[MethodOptionValueIndex];
                // do stuff if there is a render method option tag, otherwise we don't need to do anything because the option is off. 
                if (((Bungie.Tags.TagFieldReference)te.Fields[1]).Reference.ReferencePointsSomewhere())
                {
                    // the render method option tag
                    Bungie.Tags.TagFile RenderMethodOption = new Bungie.Tags.TagFile(((Bungie.Tags.TagFieldReference)te.Fields[1]).Reference.Path);

                    // add a new header with the category name - we do this here so that if there is no render_method_option referenced we don't add an empty category header. i think this works.
                    // we use toupper because guerilla has it in caps
                    // i am writing in tolower because i'm tired and today sucked


                        RowIndex = shaderView.ShaderGrid.Children.Add(new ShaderHeaderText { Text = ((Bungie.Tags.TagFieldElementStringID)MethodOption.Fields[0]).GetStringData().ToUpper() });
                        WPF.AddNewRow(shaderView.ShaderGrid, RowIndex);


                    // add all the options to the shaderView.ShaderGrid - each option can have parameters so that needs to be a function of each parameter
                    foreach (Bungie.Tags.TagElement subte in (Bungie.Tags.TagFieldBlock)RenderMethodOption.Fields[0])
                    {
                        UIElement elementToAdd;
                        switch (((Bungie.Tags.TagFieldEnum)subte.Fields[1]).Value)
                        {
                            // the tagfield here is the parameters block in the shader so we can add to it when needed
                            case 0: //bitmap
                                elementToAdd = new ShaderParameterBitmap 
                                { 
                                    FieldName = ((Bungie.Tags.TagFieldElementStringID)subte.Fields[0]).GetStringData(),
                                    TagField = (Bungie.Tags.TagFieldBlock)((Bungie.Tags.TagFieldStruct)tagFile.Fields[0]).Elements[0].Fields[1],
                                    DefaultValue = ((Bungie.Tags.TagFieldReference)subte.Fields[3]).Reference.Path.Filename,
                                    //DefaultFilterMode = (Bungie.Tags.TagFieldEnum)subte.Fields[7], // trilinear
                                    //DefaultAddressMode = (Bungie.Tags.TagFieldEnum)subte.Fields[9], // wrap
                                    HelpText = ((Bungie.Tags.TagFieldData)subte.Fields[13]).DataAsText
                                };
                                ((ShaderParameterBitmap)elementToAdd).FindParameterValues();
                                RowIndex = shaderView.ShaderGrid.Children.Add(elementToAdd);
                                WPF.AddNewRow(shaderView.ShaderGrid, RowIndex);
                                // We have to add the parameters to the Grid here, even if there's no intention to use them.
                                break;
                            /*
                            case 1: //color
                                elementToAdd = new ShaderParameterColor
                                {
                                    TemplateTagField = subte,
                                    TagField = (Bungie.Tags.TagFieldBlock)((Bungie.Tags.TagFieldStruct)tagFile.Fields[0]).Elements[0].Fields[1],
                                    DefaultColor = (Bungie.Tags.TagFieldElementArray)subte.Fields[11],
                                    HelpText = (Bungie.Tags.TagFieldData)subte.Fields[13]
                                };
                                break;
                            case 2: //real
                                elementToAdd = new ShaderParameterReal
                                {
                                    TemplateTagField = subte,
                                    TagField = (Bungie.Tags.TagFieldBlock)((Bungie.Tags.TagFieldStruct)tagFile.Fields[0]).Elements[0].Fields[1],
                                    DefaultReal = (Bungie.Tags.TagFieldElementArray)subte.Fields[4],
                                    HelpText = (Bungie.Tags.TagFieldData)subte.Fields[13]
                                };
                                break;
                            case 3: //int
                                elementToAdd = new ShaderParameterInt
                                {
                                    TemplateTagField = subte,
                                    TagField = (Bungie.Tags.TagFieldBlock)((Bungie.Tags.TagFieldStruct)tagFile.Fields[0]).Elements[0].Fields[1],
                                    DefaultInt = (Bungie.Tags.TagFieldElementArray)subte.Fields[5],
                                    HelpText = (Bungie.Tags.TagFieldData)subte.Fields[13]
                                };
                                break;
                            case 4: //bool - which is represented by a tickbox so i don't forget
                                elementToAdd = new ShaderParameterBool
                                {
                                    TemplateTagField = subte,
                                    TagField = (Bungie.Tags.TagFieldBlock)((Bungie.Tags.TagFieldStruct)tagFile.Fields[0]).Elements[0].Fields[1],
                                    DefaultBool = (Bungie.Tags.TagFieldElementArray)subte.Fields[5],
                                    HelpText = (Bungie.Tags.TagFieldData)subte.Fields[13]
                                };
                                break;
                            case 5: //argb color
                                elementToAdd = new ShaderParameterArgbColor
                                {
                                    TemplateTagField = subte,
                                    TagField = (Bungie.Tags.TagFieldBlock)((Bungie.Tags.TagFieldStruct)tagFile.Fields[0]).Elements[0].Fields[1],
                                    DefaultColor = (Bungie.Tags.TagFieldElementArray)subte.Fields[11],
                                    HelpText = (Bungie.Tags.TagFieldData)subte.Fields[13]
                                };
                                break;
                            */
                            default:
                                WPF.Log("Shader method has an unrecognised value, cannot create view");
                                return false;
                        }
                    }
                }
            }

            return true;
        }
    }
}
