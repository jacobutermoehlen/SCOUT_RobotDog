using System;
using System.Collections.Generic;
using System.Globalization;
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
using System.Windows.Media.Animation;
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
        private Thread jetsonTcpThread;
        private TcpClient tcpClient;
        private NetworkStream networkStream;
        private bool jetsonTcpIsRunning = true;
        private ManualResetEvent messageReceivedEvent = new ManualResetEvent(false);
        private string receivedMessage = string.Empty;

        private bool heightThreadRunning = false;

        //variables
        double velocity = 50;
        int rideHeigth = 200;
        int angle = 0;
        int curveAngle = 55;

        private bool isHoldingW = false;
        private bool isHoldingA = false;
        private bool isHoldingD = false;

        private bool keyboardControl = false;

        public MainWindow()
        {
            InitializeComponent();
            double percent = .1;
            //Drawing SCOUT's distance circles
            DrawPizzaSlice(348, 380, 100, 50, 80, Brushes.Green);
            DrawPizzaSlice(348, 380, 60, 50, 80, Brushes.Yellow);
            DrawPizzaSlice(348, 380, 30, 50, 80, Brushes.Red);
            DrawRingSlice(348, 380, Convert.ToInt16(percent*100), Convert.ToInt16(percent * 100 + 5), 45, 90, Brushes.LightGray);

            DrawPizzaSlice(348, 180, 100, 230, 80, Brushes.Green);
            DrawPizzaSlice(348, 180, 60, 230, 80, Brushes.Yellow);
            DrawPizzaSlice(348, 180, 30, 230, 80, Brushes.Red);

            DrawPizzaSlice(298, 280, 100, 120, 120, Brushes.Green);
            DrawPizzaSlice(298, 280, 60, 120, 120, Brushes.Yellow);
            DrawPizzaSlice(298, 280, 30, 120, 120, Brushes.Red);

            DrawPizzaSlice(398, 280, 100, 300, 120, Brushes.Green);
            DrawPizzaSlice(398, 280, 60, 300, 120, Brushes.Yellow);
            DrawPizzaSlice(398, 280, 30, 300, 120, Brushes.Red);

            //Starting Threads
            //Thread tcpSensor_readThread = new Thread(() => tcpSensor_ReadContin(sensorTCPIp, sensorTCPPort));
            //Start the Jetson TCP Control Thread
            jetsonTcpThread = new Thread(InitializeTcpConnection);
            jetsonTcpThread.IsBackground = true;
            jetsonTcpThread.Start();

            double batteryPercent = 0.5;

            //Canvas.SetLeft(batLevel_rec, (int)490*batteryPercent);

            // Create the animation
            DoubleAnimation animation = new DoubleAnimation
            {
                From = 0,                  // Start position
                To = 490,                 // End position
                Duration = TimeSpan.FromSeconds(2), // Duration of animation
                EasingFunction = new QuadraticEase() // Optional: Add easing for smoothness
            };

            // Apply the animation to the Canvas.Left property
            batLevel_rec.BeginAnimation(Canvas.LeftProperty, animation);

            this.PreviewKeyDown += moveConstantlyForwardStart;
            this.PreviewKeyUp += moveConstantlyForwardStop;
        }



        // move constantly
        private void moveConstantlyForwardStart(Object sender, KeyEventArgs e)
        {
            if (keyboardControl)
            {
                if (e.Key == Key.W && !isHoldingW)
                {
                    isHoldingW = true;
                    if (angle == 0)
                    {
                        new Thread(() => SendMessage(EncodeBinaryCommand("moveCForward1"))).Start();
                    }
                    else
                    {
                        new Thread(() => SendAndWaitForResponse(
                            EncodeBinaryCommand("changeAngle0"),
                            EncodeBinaryCommand("changedAngle"),
                            EncodeBinaryCommand("moveCForward1"))).Start();
                    }
                }
                else if (e.Key == Key.A && !isHoldingA)
                {
                    isHoldingA = true;

                    if (angle == 0)
                    {
                        angle = -curveAngle;
                        scout_rec.Fill = Brushes.Purple;
                        SendMessage(EncodeBinaryCommand($"changeAngle-15"));
                    }
                }
                else if (e.Key == Key.D && !isHoldingD)
                {
                    isHoldingD = true;

                    if (angle == 0)
                    {
                        angle = curveAngle;
                        scout_rec.Fill = Brushes.Yellow;
                        SendMessage(EncodeBinaryCommand($"changeAngle15"));
                    }
                }
            }
        }

        private void moveConstantlyForwardStop(Object sender, KeyEventArgs e)
        {
            if (keyboardControl)
            {
                if (e.Key == Key.W)
                {
                    isHoldingW = false;
                    if (angle == 0)
                    {
                        new Thread(() => SendMessage(EncodeBinaryCommand("moveCForward0"))).Start();
                        grid_main.Background = Brushes.White;
                    }
                    else
                    {
                        new Thread(() => SendAndWaitForResponse(
                            EncodeBinaryCommand("changeAngle0"),
                            EncodeBinaryCommand("changedAngle"),
                            EncodeBinaryCommand("moveCForward0"))).Start();
                    }
                }
                else if (e.Key == Key.A)
                {
                    isHoldingA = false;

                    if (angle == -curveAngle)
                    {

                        angle = 0;

                        new Thread(() => SendMessage(EncodeBinaryCommand($"changeAngle{angle}"))).Start();
                    }
                }
                else if (e.Key == Key.D)
                {
                    isHoldingD = false;

                    if (angle == curveAngle)
                    {

                        angle = 0;

                        new Thread(() => SendMessage(EncodeBinaryCommand($"changeAngle{angle}"))).Start();
                    }
                }
            }
        }



        private void move_forward_btn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            new Thread(() => SendAndWaitForResponse(
                            EncodeBinaryCommand("changeAngle0"),
                            EncodeBinaryCommand("changedAngle"),
                            EncodeBinaryCommand("moveCForward1"))).Start();
        }

        private void move_forward_btn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            new Thread(() => SendAndWaitForResponse(
                            EncodeBinaryCommand("changeAngle0"),
                            EncodeBinaryCommand("changedAngle"),
                            EncodeBinaryCommand("moveCForward0"))).Start();
        }



        // move with buttons
        private void move_stop_btn_Click(object sender, RoutedEventArgs e)
        {
            new Thread(() => SendMessage(EncodeBinaryCommand("stopMovement"))).Start();
        }

        private void move_left_btn_Click(object sender, RoutedEventArgs e)
        {
            new Thread(() => SendAndWaitForResponse(
                            EncodeBinaryCommand("changeAngle-30"),
                            EncodeBinaryCommand("changedAngle"),
                            EncodeBinaryCommand("moveCForward5"))).Start();
        }

        private void move_right_btn_Click(object sender, RoutedEventArgs e)
        {
            new Thread(() => SendAndWaitForResponse(
                            EncodeBinaryCommand("changeAngle30"),
                            EncodeBinaryCommand("changedAngle"),
                            EncodeBinaryCommand("moveCForward5"))).Start();
        }

        private void move_backward_btn_Click(object sender, RoutedEventArgs e)
        {
            DoubleAnimation animation = new DoubleAnimation
            {
                From = 0,                  // Start position
                To = 490,                 // End position
                Duration = TimeSpan.FromSeconds(2), // Duration of animation
                EasingFunction = new QuadraticEase() // Optional: Add easing for smoothness
            };

            // Apply the animation to the Canvas.Left property
            batLevel_rec.BeginAnimation(Canvas.LeftProperty, animation);
        }

        private void move_forward_btn_Click(object sender, RoutedEventArgs e)
        {
            //int veloc = Convert.ToInt16(velocity_sld.Value);
            new Thread(() => SendAndWaitForResponse(
                            EncodeBinaryCommand("changeAngle0"),
                            EncodeBinaryCommand("changedAngle"),
                            EncodeBinaryCommand("moveCForward5"))).Start();
        }

        //
        // Move Funktion Ende 
        //


        private byte[] EncodeBinaryCommand(string command)
        {
            switch (command)
            {
                case "moveCForward1": return new byte[] { 0x01 };
                case "changeAngle0": return new byte[] { 0x02 };
                case "changedAngle": return new byte[] { 0x03 };
                case "changeAngle-15": return new byte[] { 0x04 };
                case "changeAngle15": return new byte[] { 0x05 };
                case "moveForward5": return new byte[] { 0x06 };
                case "cahngeAngle30": return new byte[] { 0x07 };
                case "changeAngle-30": return new byte[] { 0x08 };
                case "stopMovement": return new byte[] { 0x09 };
                case "changeInter": return new byte[] { 0x10  };
                default: return new byte[] { 0xFF }; // Unknown Command 
            }
        }


        private void InitializeTcpConnection()
        {
            try
            {
                tcpClient = new TcpClient(jetsonTCPIp, jetsonTCPPort);
                networkStream = tcpClient.GetStream();

                while (jetsonTcpIsRunning)
                {
                    if (networkStream.DataAvailable)
                    {
                        byte[] buffer = new byte[1024];
                        int bytesRead = networkStream.Read(buffer, 0, buffer.Length);
                        receivedMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();

                        // Check if we have a waiting thread
                        if (!string.IsNullOrEmpty(receivedMessage))
                        {
                            messageReceivedEvent.Set(); // Unblock waiting thread
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }


        private void SendMessage(byte[] data)
        {
            if (networkStream != null && networkStream.CanWrite)
            {
                networkStream.Write(data, 0, data.Length);
            }
        }


        private void SendAndWaitForResponse(byte[] command, byte[] expectedResponse, byte[] followUpCommand)
        {
            new Thread(() =>
            {
                SendMessage(command);
                messageReceivedEvent.Reset(); // Bereit zum Warten

                byte[] receivedBuffer = new byte[1024];
                bool received = false;

                try
                {
                    if (networkStream != null && networkStream.CanRead)
                    {
                        int bytesRead = networkStream.Read(receivedBuffer, 0, receivedBuffer.Length);
                        if (bytesRead > 0)
                        {
                            byte[] actualResponse = receivedBuffer.Take(bytesRead).ToArray();
                            if (actualResponse.SequenceEqual(expectedResponse))
                            {
                                received = true;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    
                }
            }).Start();
        }







        //movement control methods
        private void velocity_sld_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            velocity = velocity_sld.Value;

            byte[] velocityCommand = EncodeDynamicBinaryCommand("changeInter", velocity);
            byte[] expectedResponse = EncodeBinaryCommand("changedInter");
            byte[] emptyCommand = new byte[] { };

            SendAndWaitForResponse(velocityCommand, expectedResponse, emptyCommand);
        }

        private void height_sld_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            rideHeigth = (int)height_sld.Value * -1;

            if (!heightThreadRunning)
            {
                heightThreadRunning = true;
                int heightCopy = rideHeigth;

                new Thread(() =>
                {
                    byte[] heightCommand = EncodeDynamicBinaryCommand("changeRideHeight", heightCopy);
                    SendMessage(heightCommand);
                    Thread.Sleep(100);
                    heightThreadRunning = false;
                })
                { IsBackground = true }.Start();
            }
        }

        private byte[] EncodeDynamicBinaryCommand(string command, double value)
        {
            List<byte> byteList = new List<byte>();

            switch (command)
            {
                case "changeInter":
                    byteList.Add(0x11);
                    break;
                case "changeRideHeight":
                    byteList.Add(0x12);
                    break;
                default: return new byte[] { 0xFF }; // Unknown Command 
            }

            // Konvertiere die Zahl in ein 4-Byte-Float (IEEE 754 Standard)
            byte[] valueBytes = BitConverter.GetBytes((float)value);
            byteList.AddRange(valueBytes);

            return byteList.ToArray();
        }





        //keyboardToggle

        private void keyboardToggle_ckb_Checked(object sender, RoutedEventArgs e)
        {
            keyboardControl = true;

            move_forward_btn.IsEnabled = false;
            move_left_btn.IsEnabled = false;
            move_right_btn.IsEnabled = false;
            move_backward_btn.IsEnabled = false;
        }

        private void keyboardToggle_ckb_Unchecked(object sender, RoutedEventArgs e)
        {
            keyboardControl = false;

            move_forward_btn.IsEnabled = true;
            move_left_btn.IsEnabled = true;
            move_right_btn.IsEnabled = true;
            move_backward_btn.IsEnabled = true;
        }
    }
}
