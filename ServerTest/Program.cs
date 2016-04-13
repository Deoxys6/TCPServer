using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerTest
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                
                try
                {
                    //IP address variable, this is just a local value; will need to change it when I actually implement it
                    IPAddress ipaddress = LocalIPAddress();
                    //8000 is the port
                    TcpListener myList = new TcpListener(ipaddress, 8000);
                    myList.Start();
                    Console.WriteLine("Server running - Port 8000\nIP - " + ipaddress);
                    Console.WriteLine("local end point " + myList.LocalEndpoint);
                    Console.WriteLine("waiting for connections...");

                    Socket s = myList.AcceptSocket();
                    Console.WriteLine("Connection accepted from " + s.RemoteEndPoint);
                    //create the byte array  
                   
                    ASCIIEncoding asen = new ASCIIEncoding();
                    s.Send(asen.GetBytes("Automatic message:" + "String received by server!"));
                    
                    byte[] byteArray = new byte[100];
                    int k = s.Receive(byteArray);
                    Console.WriteLine("Received");
                    for (int x = 0; x < k; x++)
                    {
                        Console.WriteLine(Convert.ToChar(byteArray[x]));
                    }


                    Console.WriteLine("\n Automatic message sent!");
                    //closes all the stuff
                    s.Close();
                    myList.Stop();

                }
                //catch any errors and write them to console
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                }
            }
        }

        private static IPAddress LocalIPAddress()
        {
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                return null;
            }

            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

            return host
                .AddressList
                .FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
        }
        
    }
}
