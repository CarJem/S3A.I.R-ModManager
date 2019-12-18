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
using System.Drawing;
using System.Runtime.InteropServices;

namespace Sonic3AIR_ModManager
{
    /// <summary>
    /// Interaction logic for DocumentationViewer.xaml
    /// </summary>
    public partial class DocumentationViewer : Window
    {
        private string FileName;
        private PdfViewer View { get; set; }
        private PdfDocument Doc { get; set; }

        public DocumentationViewer(bool useWebRendering = true)
        {
            InitializeComponent();
            var Instance = this;
            UserLanguage.ApplyLanguage(ref Instance);
            SetupViewer();
        }

        public void SetupViewer()
        {
            View = new PdfViewer();
            FormsViewer.Child = View;
        }

        public void ShowDialog(string _fileName)
        {
            FileName = _fileName;
            LoadFile(FileName);
            base.ShowDialog();
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
                (myControl as System.Windows.Forms.TreeView).ShowPlusMinus = true;
            }

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
                Doc = PdfDocument.Load(filename);;
                View.Document = Doc;
                View.ShowToolbar = false;
                View.ZoomMode = PdfViewerZoomMode.FitWidth;
                View.MouseMove += AltViewer_MouseMove;
                UpdateColorControls(View);

            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "");
            }
        }

        private void AltViewer_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            View.Update();
            View.Invalidate();
        }

        private void ExternalOpenButton_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(FileName)) System.Diagnostics.Process.Start(FileName);

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Closing -= Window_Closing;
            Doc.Dispose();
            Doc = null;
            View.MouseMove -= AltViewer_MouseMove;
            View.Dispose();
            View = null;
            FormsViewer.Child = null;
            FormsViewer.Dispose();
            FormsViewer = null;

        }
    }
}
