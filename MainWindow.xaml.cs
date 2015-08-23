using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.IO.Compression;
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

namespace DbTextEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            TextEditor.CreateDbIfNotExist();
        }

        private void ToolBar_Loaded(object sender, RoutedEventArgs e)
        {
            #region UI-specific code
            ToolBar toolBar = sender as ToolBar;
            var overflowGrid = toolBar.Template.FindName("OverflowGrid", toolBar) as FrameworkElement;
            if (overflowGrid != null)
            {
                overflowGrid.Visibility = Visibility.Collapsed;
            }
            var mainPanelBorder = toolBar.Template.FindName("MainPanelBorder", toolBar) as FrameworkElement;
            if (mainPanelBorder != null)
            {
                mainPanelBorder.Margin = new Thickness();
            }
            #endregion
        }

        private void Button_Click_New(object sender, RoutedEventArgs e)
        {
            textbox.Clear();
        }

        private void Button_Click_Open(object sender, RoutedEventArgs e)
        {
            OpenWindow window = new OpenWindow();

            // Centerize new window
            window.Left = this.Left + this.Width / 2 - window.Width / 2;
            window.Top = this.Top + this.Height / 2 - window.Height / 2;

            window.ShowDialog();
        }

        private void Button_Click_Save(object sender, RoutedEventArgs e)
        {
            SaveWindow window = new SaveWindow(textbox.Text);

            // Centerize new window
            window.Left = this.Left + this.Width / 2 - window.Width / 2;
            window.Top = this.Top + this.Height / 2 - window.Height / 2;

            window.ShowDialog();
        }
    }
}
