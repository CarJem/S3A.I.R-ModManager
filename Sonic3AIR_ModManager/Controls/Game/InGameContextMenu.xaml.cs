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

namespace Sonic3AIR_ModManager.Controls
{
    /// <summary>
    /// Interaction logic for InGameContextMenu.xaml
    /// </summary>
    public partial class InGameContextMenu : ContextMenu
    {
        private InGameContextMenu Instance;
        public InGameContextMenu()
        {
            InitializeComponent();
            Instance = this;
            Management.UserLanguage.ApplyLanguage(ref Instance);
        }

        public void Reload()
        {
            InvalidateVisual();
            Management.UserLanguage.ApplyLanguage(ref Instance);
        }

        private void OpenDownloadsFolderToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Management.ProcessLauncher.OpenMMDownloadsFolder();
        }

        private void OpenVersionsFolderToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Management.ProcessLauncher.OpenMMVersionsFolder();
        }

        private void OpenConfigFileToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Management.ProcessLauncher.OpenConfigFile();
        }

        private void OpenModsFolder_Click(object sender, RoutedEventArgs e)
        {
            Management.ProcessLauncher.OpenModsFolder();
        }

        private void OpenEXEFolderButton_Click(object sender, RoutedEventArgs e)
        {
            Management.ProcessLauncher.OpenEXEFolder();
        }

        private void OpenAppDataFolderButton_Click(object sender, RoutedEventArgs e)
        {
            Management.ProcessLauncher.OpenAppDataFolder();
        }

        private void OpenConfigFile_Click(object sender, RoutedEventArgs e)
        {
            Management.ProcessLauncher.OpenSettingsFile();
        }

        private void OpenSampleModsFolderButton_Click(object sender, RoutedEventArgs e)
        {
            Management.ProcessLauncher.OpenSampleModsFolder();

        }

        private void OpenUserManualButton_Click(object sender, RoutedEventArgs e)
        {
            Management.ProcessLauncher.OpenUserManual();
        }

        private void OpenModDocumentationButton_Click(object sender, RoutedEventArgs e)
        {
            Management.ProcessLauncher.OpenModDocumentation();
        }

        private void ShowLogFileButton_Click(object sender, RoutedEventArgs e)
        {
            Management.ProcessLauncher.OpenLogFile();
        }

        private void OpenModdingTemplatesFolder_Click(object sender, RoutedEventArgs e)
        {
            Management.ProcessLauncher.OpenModdingTemplatesFolder();
        }

        private void CloseContextMenuButton_Click(object sender, RoutedEventArgs e)
        {
            this.IsOpen = false;
        }

        private void openMMlogsFolderToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Management.ProcessLauncher.OpenMMLogsFolder();
        }

        private void openMMSettingFileToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Management.ProcessLauncher.OpenMMSettingsFile();
        }

        private void openGameRecordingsFolderToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Management.ProcessLauncher.OpenGameRecordingsFolder();
        }

        private void openSettingsGlobalFileToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Management.ProcessLauncher.OpenGlobalSettingsFile();
        }

        private void openSettingsInputFileToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Management.ProcessLauncher.OpenInputSettingsFile();
        }

        private void ForceCloseMenuButton_Click(object sender, RoutedEventArgs e)
        {
            Management.GameHandler.ForceQuitSonic3AIR();
        }
    }
}
