using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
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
using CloneACar.J2534Consumer.VehicleCloning;
using CloneACar.J2534Consumer.VehicleInit;
using CloneACar.LoggingHelpers;
using CloneACar.LogicalHelpers;

// Mahapps Metro
using MahApps.Metro.Controls;

// Global using.
using static CloneACar.GlobalObjects;

namespace CloneACar
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow, INotifyPropertyChanged
    {
        private string _loadingFlyoutText;
        private bool _buttonsEnabled = true;


        public string WindowTitleString
        {
            get
            {
                string DisplayVersion = "Version " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
                string ReleaseEnv = AppConfigHelper.ReturnConfigItem("ReleaseType");

                if (ReleaseEnv.StartsWith("DEV")) { return $"CloneACar - Beta Build - {DisplayVersion}"; }
                return $"CloneACar - {DisplayVersion}";
            }
        }
        public string LoadingFlyoutText
        {
            get { return _loadingFlyoutText; }
            set
            {
                if (value == _loadingFlyoutText) { return; }

                _loadingFlyoutText = value;
                OnPropertyChanged("LoadingFlyoutText");
            }
        }
        public bool ButtonsEnabled
        {
            get { return _buttonsEnabled; }
            set
            {
                _buttonsEnabled = value;
                OnPropertyChanged("ButtonsEnabled");
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            // This setup right here is able to read all DLLs and devices on a machine.
            // It then auto picks the first one found on the device. Since most
            // people only have a single PT device hooked up this will usually be more than ok.
            SetupGlobalObjects();
        }

        // private void TitleFlyoutButton_Click(object sender, RoutedEventArgs e) { UpdateTaskFlyout(""); }
        /// <summary>
        /// Updates loading/progress flyout object on mainwindow.
        /// </summary>
        /// <param name="TextToShow">Text to show in the flyout window.</param>
        private void UpdateTaskFlyout(string TextToShow, bool TaskUpdate = true)
        {
            if (TextToShow != "") { LoadingFlyoutText = TextToShow; }
            LoadingStatusFlyout.IsOpen = true;

            if (!TaskUpdate) { return; }
            Task.Run(() =>
            {
                Thread.Sleep(3000);
                Dispatcher.Invoke(() => LoadingStatusFlyout.IsOpen = false);
            });
        }


        private void AutoClonerButton_Click(object sender, RoutedEventArgs e)
        {
            SetupGlobalsButton_Click(sender, e);
            RunAutoIDButton_Click(sender, e);
            GenerateCloneMessagesButton_Click(sender, e);
            RunClonerCommandsButton_Click(sender, e);
        }

        private async void SetupGlobalsButton_Click(object sender, RoutedEventArgs e)
        {
            // Log info to UI
            AppLogger.WriteLog("SETTING UP GLOBALS AND V0404 HARDWARE NOW");
            UpdateTaskFlyout("Setting up Hardware Now...", false);

            // Disable all buttons.
            ButtonsEnabled = false;

            // Update working button.
            Button SendBtn = (Button)sender;
            string OldContent = SendBtn.Content.ToString();
            SendBtn.Content = "Running...";

            // Invoke call to run global setup so UI does not hang.
            await Task.Run(SetupV0404DLLsAndDevices);

            // Set old content back, close flyout, enable buttons again.
            LoadingStatusFlyout.IsOpen = false;
            SendBtn.Content = OldContent;
            ButtonsEnabled = true;
        }
        private async void RunAutoIDButton_Click(object sender, RoutedEventArgs e)
        {
            // Log info to UI
            AppLogger.WriteLog("RUNNING AUTO ID METHODS");
            UpdateTaskFlyout("Pulling VIN Number Now...", false);

            // Disable all buttons.
            ButtonsEnabled = false;

            // Update working button.
            Button SendBtn = (Button)sender;
            SendBtn.Content = "Run Auto ID Process";

            string OldContent = SendBtn.Content.ToString();
            SendBtn.Content = "Pulling VIN Number...";

            // Invoke call to run global setup so UI does not hang.
            await Task.Run(AutoID.TryAutoID);
            if (AutoID.VIN != "UNKNOWN")
            {
                // show VIN on flyout.
                LoadingStatusFlyout.IsOpen = true;
                LoadingFlyoutText = $"Found a VIN Number: {VIN_NUMBER}";

                // Set old content back, close flyout, enable buttons again.
                SendBtn.Content = OldContent;
                ButtonsEnabled = true;
            }
            else
            {
                // Set old content back, show failed, enable buttons again.
                SendBtn.Content = "FAILED TO FIND VIN NUMBER";
                ButtonsEnabled = true;
            }
        }
        private async void GenerateCloneMessagesButton_Click(object sender, RoutedEventArgs e)
        {
            // Log info to UI
            AppLogger.WriteLog("GENERATING CLONER MESSAGES NOW. THIS MAY BE A BIT.");
            UpdateTaskFlyout("Setting Up Cloner. This may be a while...");

            // Disable all buttons.
            ButtonsEnabled = false;

            // Update working button.
            Button SendBtn = (Button)sender;
            SendBtn.Content = "Generate Clone Info";

            string OldContent = SendBtn.Content.ToString();
            SendBtn.Content = "Running...";

            // Invoke call to run global setup so UI does not hang.
            await Task.Run(() =>
            {
                VehicleCloner = new VehicleCloningController(AutoID);
                VehicleCloner.GenerateMessages();
            });

            // Set old content back, close flyout, enable buttons again.
            LoadingStatusFlyout.IsOpen = false;
            ButtonsEnabled = true; 
            SendBtn.Content = OldContent;

            // Inform for long task.
            UpdateTaskFlyout("Generated Clone Messages OK! Ready for module copying");
        }
        private async void RunClonerCommandsButton_Click(object sender, RoutedEventArgs e)
        {
            // Log info to UI
            AppLogger.WriteLog("GENERATING CLONER MESSAGES NOW. THIS MAY BE A BIT.");
            UpdateTaskFlyout("Setting Up Cloner. This may be a while...");

            // Disable all buttons.
            ButtonsEnabled = false;

            // Update working button.
            Button SendBtn = (Button)sender;
            SendBtn.Content = "Run Module Cloner";

            string OldContent = SendBtn.Content.ToString();
            SendBtn.Content = "Running...";

            // Invoke call to run global setup so UI does not hang.
            await Task.Run(() =>
            {
                VehicleCloner.RunCloner();
                if (VehicleCloner.ModuleCommsList != null) { FinalCloneResults = VehicleCloner.ModuleCommsList; }
            });

            // Set old content back, close flyout, enable buttons again.
            LoadingStatusFlyout.IsOpen = false;
            ButtonsEnabled = true;
            SendBtn.Content = OldContent;

            // Inform for long task.
            UpdateTaskFlyout("Module Cloner Is Complete! Check the debug logs for more info.");
        }
    }
}
