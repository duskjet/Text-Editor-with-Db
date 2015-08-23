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
using System.Windows.Shapes;

namespace DbTextEditor
{
    /// <summary>
    /// Interaction logic for OpenWindow.xaml
    /// </summary>
    public partial class OpenWindow : Window
    {
        public OpenWindow()
        {
            InitializeComponent();
        }

        private async void listBox_Initialized(object sender, EventArgs e)
        {
            // Populate window list with names from DB
            listBox.ItemsSource = await TextEditor.GetFileList();
        }

        private async void button_Open_Click(object sender, RoutedEventArgs e)
        {
            // Load selected file from DB
            string text = await TextEditor.OpenFile((string)listBox.SelectedItem);

            // Place loaded text in the editor window
            var mainWindow = Application.Current.MainWindow as MainWindow;
            mainWindow.textbox.Text = text;
            
            this.Close();
        }
    }
}
