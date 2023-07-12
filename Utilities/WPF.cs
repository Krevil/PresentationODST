using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace PresentationODST.Utilities
{
    public static class WPF
    {
        public static readonly SolidColorBrush RedBrush = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
        public static readonly SolidColorBrush BlueBrush = new SolidColorBrush(Color.FromArgb(255, 0, 255, 0));
        public static readonly SolidColorBrush GreenBrush = new SolidColorBrush(Color.FromArgb(255, 0, 0, 255));
        public static readonly SolidColorBrush BlackBrush = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
        public static readonly SolidColorBrush WhiteBrush = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));

        public static void AddNewRow(Grid grid, int rowindex, Int32 length)
        {
            RowDefinition NewRow = new RowDefinition { Height = new GridLength(length) };
            grid.RowDefinitions.Add(NewRow);
            Grid.SetRow(grid.Children[rowindex], grid.RowDefinitions.IndexOf(NewRow));
        }

        public static void AddNewRow(Grid grid, int rowindex)
        {
            RowDefinition NewRow = new RowDefinition { Height  = new GridLength(1, GridUnitType.Auto) };
            grid.RowDefinitions.Add(NewRow);
            Grid.SetRow(grid.Children[rowindex], grid.RowDefinitions.IndexOf(NewRow));
        }

        public static bool ExpertModeVisibility(Bungie.Tags.TagField field)
        {
            return Properties.Settings.Default.ExpertMode || field.Visible;
        }

        public static void Log(string message)
        {
            MainWindow.Main_Window.OutputWindowTextBlock.Text += "\n" + message;
            MainWindow.Main_Window.OutputWindowTextBlock.ScrollToEnd();
            MainWindow.Main_Window.OutputWindowTextBlock.UpdateLayout();
        }

        public static void Log(string message, params object[] args)
        {
            MainWindow.Main_Window.OutputWindowTextBlock.Text += "\n" + string.Format(CultureInfo.InvariantCulture, message, args);
            MainWindow.Main_Window.OutputWindowTextBlock.ScrollToEnd();
            MainWindow.Main_Window.OutputWindowTextBlock.UpdateLayout();
        }
    }
}
