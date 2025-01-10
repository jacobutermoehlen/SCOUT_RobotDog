using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Net.Sockets;
using System.Windows.Threading;
using System.Net.NetworkInformation;

namespace HERO_GUI
{
    public partial class MainWindow : Window
    {
        public async void tcpSensor_ReadContin(string serverIp, int serverPort)
        {
            try
            {
                using TcpClient client = new TcpClient();

                //
                // update the log
                //

                await client.ConnectAsync(serverIp, serverPort);
                // update the log

                using NetworkStream stream = client.GetStream();

                while (true)
                {
                    //reading data from the server
                    byte[] buffer = new byte[1024];
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    if (message.Contains("SENSOUT"))
                    {
                        Task updateSensorDataTask = new Task(() => updateSensorData("message"));
                    }
                }

            } catch { }
        }

        public void updateSensorData(string dataString)
        {
            //split the dataString by ',' into string array
            string[] sensorData = dataString.Split(',');

            main_sensor_grid.Dispatcher.BeginInvoke(() =>
            {
                //tof front

                //tof back

                //hc left
                
                //hc right

                //hc front left

                //hc fron right

                //hc front down

                //icm pitch
                orientationX_blk.Text = sensorData[8];

                //icm roll
                orientationY_blk.Text = sensorData[9];

                //mag data for orientation
                double magX = Convert.ToDouble(sensorData[10]);
                double magY = Convert.ToDouble(sensorData[11]);
                double magZ = Convert.ToDouble(sensorData[12]);

                //icm temp
                temp_blk.Text = sensorData[13];

                //ina bus voltage
                batVoltage_blk.Text = sensorData[14];
                //handle voltage percent meter
            });
        }
    }
}
