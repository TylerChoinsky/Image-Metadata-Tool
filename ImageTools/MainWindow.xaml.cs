using Microsoft.Win32;
using System;
using System.Drawing;
using System.IO;
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
using System.Windows.Navigation;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using Shell32;

namespace ImageTools
{
    /// Interaction logic for MainWindow.xaml
    public partial class MainWindow : Window
    {
        string task = "NULL";
        string imagePath = "NULL";

        // Creates the main window.
        public MainWindow()
        {
            InitializeComponent();
        }

        // Opens, loads, and displays an image file into the window.
        // The window automatically resizes to the image's size.
        // The image's file path is recorded in imagePath
        private void buttonOpenImageFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Select an image";
            openFileDialog.Filter = "JPEG |*.jpg;*.jpeg|" +
                                    "Portable Network Graphic |*.png";
            if (openFileDialog.ShowDialog() == true)
            {
                Uri fileUri = new Uri(openFileDialog.FileName);
                openedImage.Source = new BitmapImage(fileUri);
                theMainWindow.SizeToContent = SizeToContent.WidthAndHeight;
                imagePath = fileUri.LocalPath;
            }
        }

        // Runs the currently selected task.
        // Exports the result if the export checkbox is checked
        private void buttonRunTask(object sender, RoutedEventArgs e)
        {
            string metadataToString = null;

            if (task == "View Metadata")
            {
                List<string> metadata = getImageMetadata(imagePath);
                metadataToString = String.Join(",\n", metadata);
                MessageBox.Show(metadataToString);
            }

            if (exportCheckbox.IsChecked == true)
            {
                // export results to the same directory as the selected image.
                string imageDirectoryPath = Path.GetDirectoryName(imagePath) + "/";
                string imageFileName = Path.GetFileName(imagePath);
                using (StreamWriter outputFile = new StreamWriter(imageDirectoryPath + imageFileName + "_METADATA.txt"))
                {
                    outputFile.WriteLine(metadataToString);
                }
            }
        }

        // Sets the window's current task.
        private void cmbBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedItem = ((ComboBoxItem)cmbBox.SelectedItem).Content.ToString();
            task = selectedItem;
        }

        // Uses Shell32 to parse metadata directly from a windows folder namespace (i think that is how it works).
        private List<string> getImageMetadata(string imagePath)
        {
            List<string> arrValues = new List<string>();

            Shell32.Shell shell = new Shell32.Shell();
            Folder rFolder = shell.NameSpace(Path.GetDirectoryName(imagePath));
            FolderItem rFiles = rFolder.ParseName(Path.GetFileName(imagePath));

            for (int i = 0; i < short.MaxValue; i++)
            {
                string header = rFolder.GetDetailsOf(null, i).Trim();
                string value = rFolder.GetDetailsOf(rFiles, i).Trim();
                if (!string.IsNullOrEmpty(header) && !string.IsNullOrEmpty(value))
                    arrValues.Add(header + ": " + value);
            }

            return arrValues;
        }
    }
}
