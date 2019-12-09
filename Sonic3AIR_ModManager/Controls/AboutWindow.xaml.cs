using System;
using System.Diagnostics;
using System.Windows;
using System.IO;
using System.Reflection;
using System.Text;


namespace Sonic3AIR_ModManager
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    partial class AboutWindow : Window
	{
		public AboutWindow()
		{

            InitializeComponent();
            var Instance = this;
            UserLanguage.ApplyLanguage(ref Instance);
			Title = String.Format("{1} {0}", Program.LanguageResource.GetString("ApplicationTitle"), Program.LanguageResource.GetString("AboutString"));
            labelProductName.Text = AssemblyProduct;
			labelVersion.Text = String.Format("{1}: {0}", Program.Version, Program.LanguageResource.GetString("Version"));
            buildDateLabel.Text = String.Format("{1}: {0}", GetBuildTime, Program.LanguageResource.GetString("BuildDateString"))
            + Environment.NewLine + String.Format("{1}: {0}", GetProgramType, Program.LanguageResource.GetString("ArchitectureString"));

            labelCopyright.Text = AssemblyCopyright;
            labelCopyright.Text = labelCopyright.Text.Replace("�", "©");

        }

		#region Assembly Attribute Accessors

		public string AssemblyTitle
		{
			get
			{
				object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
				if (attributes.Length > 0)
				{
					AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
					if (titleAttribute.Title != "")
					{
						return titleAttribute.Title;
					}
				}
				return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
			}
		}

		public string GetProgramType
		{
			get
			{
				if (Environment.Is64BitProcess)
				{
					return "x64";
				}
				else
				{
					return "x86";
				}
			}
		}

		public string AssemblyProduct
		{
			get
			{
				object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
				if (attributes.Length == 0)
				{
					return "";
				}
				return ((AssemblyProductAttribute)attributes[0]).Product;
			}
		}

		public string AssemblyCopyright
		{
			get
			{
				object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
				if (attributes.Length == 0)
				{
					return "";
				}
				return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
			}
		}

		private string GetBuildTime
		{
			get
			{
				DateTime buildDate = new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).LastWriteTime;
				String buildTimeString = buildDate.ToString();
				return buildTimeString;
			}

		}
		#endregion

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void Hyperlink_Click(object sender, RoutedEventArgs e)
		{

		}

        private void checkForModManagerUpdatesButton_Click(object sender, RoutedEventArgs e)
        {
            if (Program.MMUpdaterState == Program.UpdateState.NeverStarted || Program.MMUpdaterState == Program.UpdateState.Finished) new ModManagerUpdater(true);
        }

        private void CheckForUpdates_Click(object sender, RoutedEventArgs e)
        {
            if (Program.AIRUpdaterState == Program.UpdateState.NeverStarted || Program.AIRUpdaterState == Program.UpdateState.Finished) new Updater(true);
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
		{
			Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
			e.Handled = true;
		}
	}
}
