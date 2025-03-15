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
        int sensorTCPPort = 12346;
        string jetsonTCPIp = "192.168.4.1";
        int jetsonTCPPort = 12345;

        //jetson control tcp 
        private Thread jetsonTcpThread;
        private TcpClient jetsonTcpClient;
        private NetworkStream jetsonNetworkStream;
        private bool jetsonTcpIsRunning = true;
        private ManualResetEvent jetsonMessageReceivedEvent = new ManualResetEvent(false);
        private string jetsonReceivedMessage = string.Empty;

        //sensor tcp
        private Thread sensorTcpThread;
        private TcpClient sensorTcpClient;
        private NetworkStream sensorNetworkStream;
        private bool sensorTcpIsRunning = true;
        private ManualResetEvent sensorMessageReceivedEvent = new ManualResetEvent(false);
        private string sensorReceivedMessage = string.Empty;

        private bool heightThreadRunning = false;

        //variables
        double velocity = 50;
        int rideHeigth = 200;
        int angle = 0;
        int curveAngle = 55;
        double bat_Voltage = 0;

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
            jetsonTcpThread = new Thread(InitializeJetsonTcpConnection);
            jetsonTcpThread.IsBackground = true;
            jetsonTcpThread.Start();

            //Start the sensor TCP thread
            sensorTcpThread = new Thread(InitializeSensorTcpConnection);
            sensorTcpThread.IsBackground = true;
            sensorTcpThread.Start();

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

        private void moveConstantlyForwardStart(Object sender, KeyEventArgs e)
        {
            if(keyboardControl)
            {
                if (e.Key == Key.W && !isHoldingW)
                {
                    isHoldingW = true;
                    if (angle == 0)
                    {
                        new Thread(() => SendMessage("moveCForward1")).Start();
                    }
                    else
                    {
                        new Thread(() => SendAndWaitForResponse("changeAngle0", "changedAngle", "moveCForward1")).Start();
                    }
                }
                else if (e.Key == Key.A && !isHoldingA)
                {
                    isHoldingA = true;

                    if (angle == 0)
                    {
                        angle = -curveAngle;
                        scout_rec.Fill = Brushes.Purple;
                        //new Thread(() => SendAndWaitForResponse("changeAngle15", "changedAngle", "test123456789"));
                        SendMessage($"changeAngle{angle}");
                    }
                }
                else if (e.Key == Key.D && !isHoldingD)
                {
                    isHoldingD = true;

                    if (angle == 0)
                    {
                        angle = curveAngle;
                        scout_rec.Fill = Brushes.Yellow;
                        //new Thread(() => SendAndWaitForResponse("changeAngle15", "changedAngle", "test123456789"));
                        SendMessage($"changeAngle{angle}");
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
                        new Thread(() => SendMessage("moveCForward0")).Start();
                        grid_main.Background = Brushes.White;
                    }
                    else
                    {
                        new Thread(() => SendAndWaitForResponse("changeAngle0", "changedAngle", "moveCForward0")).Start();
                    }
                }
                else if (e.Key == Key.A)
                {
                    isHoldingA = false;

                    if (angle == -curveAngle)
                    {

                        angle = 0;

                        SendMessage($"changeAngle{angle}");
                    }
                }
                else if (e.Key == Key.D)
                {
                    isHoldingD = false;

                    if (angle == curveAngle)
                    {

                        angle = 0;

                        SendMessage($"changeAngle{angle}");
                    }
                }
            }
        }

        private void InitializeSensorTcpConnection()
        {
            try
            {
                sensorTcpClient = new TcpClient(sensorTCPIp, sensorTCPPort);
                sensorNetworkStream = sensorTcpClient.GetStream();

                while (sensorTcpIsRunning)
                {
                    if (sensorNetworkStream.DataAvailable)
                    {
                        byte[] buffer = new byte[1024];
                        int bytesRead = sensorNetworkStream.Read(buffer, 0, buffer.Length);
                        sensorReceivedMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();

                        string[] sensorData = sensorReceivedMessage.Split(',');

                        List<string> sensorDataList = new List<string>();
                        for(int i = 0; i < sensorData.Length; i++)
                        {
                            if(i > 0 && sensorData[i] == "SENSOUT")
                            {
                                break;
                            }
                            else
                            {
                                sensorDataList.Add(sensorData[i]);
                            }
                        }

                        if (sensorData[0] == "SENSOUT")
                        {
                            main_sensor_grid.Dispatcher.BeginInvoke(() =>
                            {
                                //Write to sensor console
                                foreach(string sensorValue in sensorDataList)
                                {
                                    sensorString_TextBlock.Text += sensorValue + ",";
                                }
                                for(int j  = 0; j < sensorDataList.Count; j++)
                                {
                                    sensorData[j] = sensorDataList[j].ToString();
                                }
                                //sensorString_TextBlock.Text += "\n";
                                //sensorString_TextBlock.Text += sensorReceivedMessage + "\n";
                                sensorString_TextBlock.Text += "\n";
                                if (ConsoleScrollViewer.VerticalOffset >= ConsoleScrollViewer.ScrollableHeight)
                                {
                                    ConsoleScrollViewer.ScrollToEnd();
                                }

                                //tof front

                                //tof back

                                //hc left

                                //hc right

                                //hc front left

                                //hc fron right

                                //hc front down
                                try
                                {
                                    //icm pitch
                                    orientationX_blk.Text = sensorData[8];

                                    //icm roll
                                    orientationY_blk.Text = sensorData[9];
                                }
                                catch (Exception e)
                                {

                                }

                                //mag data for orientation
                                //double magX = Convert.ToDouble(sensorData[10]);
                                //double magY = Convert.ToDouble(sensorData[11]);
                                //double magZ = Convert.ToDouble(sensorData[12]);

                                //icm temp
                                temp_blk.Text = sensorData[13];

                                //ina bus voltage
                                //batVoltage_blk.Text = sensorData[14];
                                StringBuilder batterySb = new StringBuilder();
                                foreach (char b in sensorData[14])
                                {
                                    if (b != 'S')
                                    {
                                        batterySb.Append(b);
                                    }
                                }
                                batVoltage_blk.Text = batterySb.ToString();
                                try
                                {
                                    bat_Voltage = Convert.ToDouble(batterySb.ToString()) / 100.00;
                                }
                                catch (Exception e)
                                {
                                }
                                double bat_Percent = GetBatteryPercentage(bat_Voltage);
                                DoubleAnimation animation = new DoubleAnimation
                                {
                                    From = 490 * (bat_Percent / 100),                  // Start position
                                    To = 490 * (bat_Percent / 100),                 // End position
                                    Duration = TimeSpan.FromSeconds(0), // Duration of animation
                                    //EasingFunction = new QuadraticEase() // Optional: Add easing for smoothness
                                };

                                batLevel_rec.BeginAnimation(Canvas.LeftProperty, animation);
                            });
                        }
                       

                        // Check if we have a waiting thread
                        if (!string.IsNullOrEmpty(sensorReceivedMessage))
                        {
                            sensorMessageReceivedEvent.Set(); // Unblock waiting thread
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        private static readonly SortedDictionary<double, int> VoltagePercentageMap = new SortedDictionary<double, int>()
    {
        { 25.2, 100 }, { 24.9, 95 }, { 24.67, 90 }, { 24.49, 85 }, { 24.14, 80 },
        { 23.9, 75 }, { 23.72, 70 }, { 23.48, 65 }, { 23.25, 60 }, { 23.13, 55 },
        { 23.01, 50 }, { 22.89, 45 }, { 22.77, 40 }, { 22.72, 35 }, { 22.6, 30 },
        { 22.48, 25 }, { 22.36, 20 }, { 22.24, 15 }, { 22.12, 10 }, { 21.65, 5 }, { 19.64, 0 }
    };

        public static double GetBatteryPercentage(double voltage)
        {
            if (voltage >= 25.2)
            {
                return 100;
            }

            if (voltage <= 19.64)
            {
                return 0;
            }

            double lowerVoltage = 0;
            double upperVoltage = 0;
            int lowerPercentage = 0;
            int upperPercentage = 0;

            foreach (var kvp in VoltagePercentageMap)
            {
                if (kvp.Key <= voltage)
                {
                    lowerVoltage = kvp.Key;
                    lowerPercentage = kvp.Value;
                }
                else
                {
                    upperVoltage = kvp.Key;
                    upperPercentage = kvp.Value;
                    break;
                }
            }

            // Apply a non-linear interpolation (e.g., exponential)
            double ratio = (voltage - lowerVoltage) / (upperVoltage - lowerVoltage);
            double interpolatedPercentage = lowerPercentage + (upperPercentage - lowerPercentage) * Math.Pow(ratio, 0.8); // Adjust exponent for curve

            return interpolatedPercentage;
        }

        private void InitializeJetsonTcpConnection()
        {
            try
            {
                jetsonTcpClient = new TcpClient(jetsonTCPIp, jetsonTCPPort);
                jetsonNetworkStream = jetsonTcpClient.GetStream();

                while (jetsonTcpIsRunning)
                {
                    if (jetsonNetworkStream.DataAvailable)
                    {
                        byte[] buffer = new byte[1024];
                        int bytesRead = jetsonNetworkStream.Read(buffer, 0, buffer.Length);
                        jetsonReceivedMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();

                        // Check if we have a waiting thread
                        if (!string.IsNullOrEmpty(jetsonReceivedMessage))
                        {
                            jetsonMessageReceivedEvent.Set(); // Unblock waiting thread
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void SendMessage(string message)
        {
            if(jetsonNetworkStream != null && jetsonNetworkStream.CanWrite)
            {
                byte[] data = Encoding.UTF8.GetBytes(message);
                jetsonNetworkStream.Write(data, 0, data.Length);
            }
       }

        private void SendAndWaitForResponse(string messageToSend, string expectedResponse, string answer)
        {
            new Thread(() =>
            {
                SendMessage(messageToSend);
                jetsonMessageReceivedEvent.Reset(); // Prepare to wait

                // Wait until the expected response is received
                bool received = jetsonMessageReceivedEvent.WaitOne(TimeSpan.FromSeconds(10)); // Wait up to 10 seconds
                if (received && jetsonReceivedMessage == expectedResponse)
                {
                    SendMessage(answer);
                }
                else
                {

                }
            }).Start();
        }

        //movement control methods
        private void move_forward_btn_Click(object sender, RoutedEventArgs e)
        {
            //int veloc = Convert.ToInt16(velocity_sld.Value);
            new Thread(() => SendAndWaitForResponse("changeAngle0", "changedAngle","moveForward5")).Start();
        }
        private void velocity_sld_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            velocity = velocity_sld.Value;
            SendAndWaitForResponse($"changeInter{velocity.ToString(CultureInfo.InvariantCulture)}", "changedInter", "");
        }

        private void height_sld_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            rideHeigth = (int)height_sld.Value * -1;
            //SendMessage($"moveN{rideHeigth}");
            //new Thread(() => SendMessage($"moveN{rideHeigth}")).Start();

            if (!heightThreadRunning)
            {
                heightThreadRunning = true;
                int heightCopy = rideHeigth;
                //new Thread(() => { SendMessage($"moveN{heightCopy}"); Thread.Sleep(100); heightThreadRunning = false; }) { IsBackground = true }.Start();
                new Thread(() => { SendMessage($"changeRideHeight{heightCopy}"); Thread.Sleep(100); heightThreadRunning = false; }) { IsBackground = true }.Start();
            }
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

        private void move_stop_btn_Click(object sender, RoutedEventArgs e)
        {
            new Thread(() => SendMessage("stopMovement")).Start();
        }

        private void move_left_btn_Click(object sender, RoutedEventArgs e)
        {
            new Thread(() => SendAndWaitForResponse("changeAngle-30", "changedAngle", "moveForward5")).Start();
        }

        private void move_right_btn_Click(object sender, RoutedEventArgs e)
        {
            new Thread(() => SendAndWaitForResponse("changeAngle30", "changedAngle", "moveForward5")).Start();
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

        private void move_forward_btn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            new Thread(() => SendAndWaitForResponse("changeAngle0", "changedAngle", "moveCForward1")).Start();
        }

        private void move_forward_btn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            new Thread(() => SendAndWaitForResponse("changeAngle0", "changedAngle", "moveCForward0")).Start();
        }

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
