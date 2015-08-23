using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbTextEditor
{
    /// <summary>
    /// Main class that encapsulates database-related operations
    /// </summary>
    class TextEditor
    {
        /// <summary>
        /// Save file to Db
        /// </summary>
        /// <param name="name">file name</param>
        /// <param name="text">actual text</param>
        /// <returns></returns>
        public static async Task SaveFile(string name, string text)
        {
            byte[] compressedFile = TextCompression.Zip(text);
            await FileRepository.SaveFileAsync(name, compressedFile);
        }

        /// <summary>
        /// Open file from Db by name
        /// </summary>
        /// <param name="name">file name</param>
        /// <returns>actual text from that file</returns>
        public static async Task<string> OpenFile(string name)
        {
            byte[] compressedFile = await FileRepository.LoadFileAsync(name);
            string text = TextCompression.Unzip(compressedFile);

            return text;
        }

        /// <summary>
        /// Get list of all files Db is currently holding
        /// </summary>
        /// <returns>list of file names</returns>
        public static async Task<List<string>> GetFileList()
        {
            return await FileRepository.GetAllFileNamessAsync();
        }

        /// <summary>
        /// Creates Database if it is not exist by default connection string
        /// </summary>
        public static void CreateDbIfNotExist()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            if (!FileRepository.DbExists(connectionString))
                FileRepository.CreateDatabase(connectionString);
        }

        public static void WPFMessageBoxException(Exception exception)
        {
            var ownerWindow = System.Windows.Application.Current.MainWindow as MainWindow;
            System.Windows.MessageBox.Show(ownerWindow, exception.Message, "Database Exception", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
        }
    }
}
