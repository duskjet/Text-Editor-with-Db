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
    /// Interaction logic for SaveWindow.xaml
    /// </summary>
    public partial class SaveWindow : Window
    {
        private string textToSave;

        public SaveWindow(string text)
        {
            InitializeComponent();

            textToSave = text;

            TextBox_FileName.Focus();
        }

        private async void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            // Future file name
            string name = TextBox_FileName.Text;

            // Save file to Db
            await TextEditor.SaveFile(name, textToSave);

            this.Close();
        }
    }
}
