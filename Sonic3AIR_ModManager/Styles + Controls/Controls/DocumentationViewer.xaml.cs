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
using System.Windows.Navigation;
using System.Windows.Shapes;
using MoonPdfLib.MuPdf;
using System.IO;

namespace Sonic3AIR_ModManager
{
    /// <summary>
    /// Interaction logic for DocumentationViewer.xaml
    /// </summary>
    public partial class DocumentationViewer : Window
    {
        private string FileName;
        public DocumentationViewer()
        {
            InitializeComponent();
            var Instance = this;
            UserLanguage.ApplyLanguage(ref Instance);
        }

        public void ShowDialog(string _fileName)
        {
            this.Show();
            FileName = _fileName;
            LoadFile(FileName);
        }

        private void LoadFile(string filename)
        {
            try
            {
                pdfViewer.OpenFile(filename);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "");
            }
        }

        bool PDFisLoaded = false;

        private void pdfViewer_PdfLoaded(object sender, EventArgs e)
        {
            PDFisLoaded = true;
        }

        private void ExternalOpenButton_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(FileName)) System.Diagnostics.Process.Start(FileName);
        }

        private void pdfViewer_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            LoadFile(FileName);
        }
    }
}
