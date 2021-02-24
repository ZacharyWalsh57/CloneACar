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
using CloneACar.J2534Consumer.VehicleInit;
using CloneACar.LoggingHelpers;

// Global using.
using static CloneACar.GlobalObjects;

namespace CloneACar
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // This setup right here is able to read all DLLs and devices on a machine.
            // It then auto picks the first one found on the device. Since most
            // people only have a single PT device hooked up this will usually be more than ok.
            var Globals = new GlobalObjects();

            // If we can AutoID Ok, then we need to clone.
            // This is gonna be changed based on the vehicle protocol once all cloning classes are done.
            if (AutoID.TryAutoID()) { VehicleCloner.Clone15765_11Bit(); }
        }
    }
}
