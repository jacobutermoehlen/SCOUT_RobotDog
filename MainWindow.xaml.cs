using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
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
        //constants
        string sensorTCPIp = "192.168.4.1";
        int sensorTCPPort = 67890;
        string jetsonTCPIp = "192.168.4.1";
        int jetsonTCPPort = 12345;

        //variables
        int velocity = 50;
        int rideHeigth = 200;

        public MainWindow()
        {
            InitializeComponent();


            //Starting Threads
            Thread tcpSensor_readThread = new Thread(() => tcpSensor_ReadContin(sensorTCPIp, sensorTCPPort));

        }

        private void SendMessageToServer(string jetsonTCPIp, int jetsonTCPPort, string message)
        {
            try
            {
                using (TcpClient client = new TcpClient(jetsonTCPIp, jetsonTCPPort))
                using (NetworkStream stream = client.GetStream())
                {
                    byte[] data = Encoding.UTF8.GetBytes(message);
                    stream.Write(data, 0, data.Length);
                    Console.WriteLine($"Sent message to {jetsonTCPIp}:{jetsonTCPPort}: {message}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error sending message: {ex.Message}");
            }
        }

        //movement control methods
        private void move_forward_btn_Click(object sender, RoutedEventArgs e)
        {
            SendMessageToServer(jetsonTCPIp, jetsonTCPPort, "move Forward\n");
        }

        private void move_backword_btn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void move_stop_btn_Click(object sender, RoutedEventArgs e )
        {

        }

        private void move_left_btn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void move_right_btn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void velocity_sld_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            velocity = (int)velocity_sld.Value;
        }

        private void height_sld_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            rideHeigth = (int)height_sld.Value;
        }





        //camera control methods
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
