using System;
using System.Collections.Generic;
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

namespace HERO_GUI
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();


            //Starting Threads
            Thread tcpReadThread = new Thread(() => tcpReadContin("192.168.4.1", 12345));
        }

        void MeineMethode(object param)
        {

        }

        private void camLaunch_btn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void camStop_btn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void camRec_btn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void camStopRec_btn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
