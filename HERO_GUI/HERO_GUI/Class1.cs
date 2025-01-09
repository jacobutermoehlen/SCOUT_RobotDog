using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Net.Sockets;

namespace HERO_GUI
{
    public partial class MainWindow : Window
    {
        static async void tcpReadContin(string serverIp, int serverPort)
        {
            try
            {
                using TcpClient client = new TcpClient();

                //
                // update the log
                //

                await client.ConnectAsync(serverIp, serverPort);
                // update the log

            } catch { }
        }
    }
}
