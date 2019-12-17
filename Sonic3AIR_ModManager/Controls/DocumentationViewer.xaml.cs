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
using System.IO;
using PdfiumViewer;
using System.Windows.Forms;

namespace Sonic3AIR_ModManager
{
    /// <summary>
    /// Interaction logic for DocumentationViewer.xaml
    /// </summary>
    public partial class DocumentationViewer : Window
    {
        private string FileName;
        private bool UsePDFViewer { get; set; } = false;

        private PDFViewer AltViewer { get; set; }

        public DocumentationViewer(bool useWebRendering = true)
        {
            InitializeComponent();
            var Instance = this;
            UserLanguage.ApplyLanguage(ref Instance);
            SetupAltViewer();
        }

        public void SetupAltViewer()
        {
            AltViewer = new PDFViewer();
            FormsViewer.Child = AltViewer;
        }

        public void ShowDialog(string _fileName)
        {
            this.Show();
            FileName = _fileName;
            LoadFile(FileName);
        }

        private void LoadFile(string filename)
        {
            LoadUsingWebViewer(filename);
        }

        public void UpdateColorControls(System.Windows.Forms.Control myControl)
        {
            if (myControl is System.Windows.Forms.TreeView)
            {
                myControl.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
                myControl.ForeColor = System.Drawing.Color.White;
                (myControl as System.Windows.Forms.TreeView).LineColor = System.Drawing.Color.White;
                (myControl as System.Windows.Forms.TreeView).ShowPlusMinus = true;
            }
            if (myControl is System.Windows.Forms.TreeNode)
            {
                myControl.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
                myControl.ForeColor = System.Drawing.Color.White;
            }


            // Any other non-standard controls should be implemented here aswell...

            foreach (System.Windows.Forms.Control subC in myControl.Controls)
            {
                UpdateColorControls(subC);
            }
        }

        private void MyControl_Paint(object sender, PaintEventArgs e)
        {
            if (sender is VScrollBar || sender is HScrollBar || sender is ScrollBar)
            {
                WinformsTheming.InvertGraphicsArea(e.Graphics, e.ClipRectangle);
            }
        }

        private void LoadUsingWebViewer(string filename)
        {
            try
            {
                AltViewer.View.Document = PdfDocument.Load(filename);
                AltViewer.View.ShowToolbar = false;
                AltViewer.View.ZoomMode = PdfViewerZoomMode.FitWidth;
                UpdateColorControls(AltViewer.View);

            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "");
            }
        }

        private void ExternalOpenButton_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(FileName)) System.Diagnostics.Process.Start(FileName);
        }
    }
}
