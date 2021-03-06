﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Music;
namespace ServerTest
{
    class Program
    {
        static int clientCount = 0;
        static MusicReader reader = new MusicReader();
        static void Main(string[] args)
        {
            Console.WriteLine(LocalIPAddress());
            //create the object
            AsynchronousSocketListener.StartListening();
            Console.WriteLine("Test");
        }

        public class AsynchronousSocketListener
        {
            // Thread signal.
            public static ManualResetEvent allDone = new ManualResetEvent(false);

            public AsynchronousSocketListener()
            {
            }

            public static void StartListening()
            {
                // Data buffer for incoming data.
                byte[] bytes = new Byte[4096];

                // Establish the local endpoint for the socket.
                IPAddress ipAddress = LocalIPAddress();
                IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 8000);

                // Create a TCP/IP socket.
                Socket listener = new Socket(AddressFamily.InterNetwork,
                    SocketType.Stream, ProtocolType.Tcp);

                // Bind the socket to the local endpoint and listen for incoming connections.
                try
                {
                    listener.Bind(localEndPoint);
                    listener.Listen(100);

                    while (true)
                    {
                        // Set the event to nonsignaled state.
                        allDone.Reset();

                        // Start an asynchronous socket to listen for connections.
                        Console.WriteLine("Waiting for a connection...");
                        listener.BeginAccept(
                            new AsyncCallback(AcceptCallback),
                            listener);

                        // Wait until a connection is made before continuing.
                        allDone.WaitOne();
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }

                Console.WriteLine("\nPress ENTER to continue...");
                Console.Read();

            }

            public static void AcceptCallback(IAsyncResult ar)
            {
                // Signal the main thread to continue.
                allDone.Set();

                // Get the socket that handles the client request.
                Socket listener = (Socket)ar.AsyncState;
                Socket handler = listener.EndAccept(ar);

                // Create the state object.
                StateObject state = new StateObject();
                state.workSocket = handler;
                handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
                clientCount++;
            }

            public static void ReadCallback(IAsyncResult ar)
            {


                // Retrieve the state object and the handler socket
                // from the asynchronous state object.
                StateObject state = (StateObject)ar.AsyncState;
                Socket handler = state.workSocket;

                // Read data from the client socket. 
                try
                { 

                    int bytesRead = handler.EndReceive(ar);

                    if (bytesRead > 0)
                    {
                        // There  might be more data, so store the data received so far.
                        state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));
                        Boolean isValidSong = false;
                        do
                        {
                            try
                            {
                                Console.WriteLine("What song do you want to send?");
                                int song;
                                Int32.TryParse(Console.ReadLine(), out song);
                                //Int32 song = Int32.Parse(Console.ReadLine().Trim());
                                var songToSend = reader.songList[song];

                                var serialize = new BinaryFormatter();
                                //serialize the conents to send to client
                                using (var ms = new MemoryStream())
                                {
                                    serialize.Serialize(ms, songToSend);
                                    var bytes = ms.ToArray();
                                    var test = Encoding.ASCII.GetString(bytes);
                                    //Console.WriteLine("Message:{0}", test);
                                    //Console.WriteLine("as ints {0}", string.Join(" ", bytes));
                                    // Echo the data back to the client.
                                    Send(handler, bytes);
                                }
                                isValidSong = true;
                            }

                            catch (Exception e)
                            {
                                Console.WriteLine("Try Again");
                            }
                        } while (!isValidSong);

                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Someone disconneted");
                }
            }

            private static void Send(Socket handler, byte[] byteData)
            {
                // Convert the string data to byte data using ASCII encoding.
                //byte[] byteData = Encoding.ASCII.GetBytes(data);

                // Begin sending the data to the remote device.
                handler.BeginSend(byteData, 0, byteData.Length, 0,
                    new AsyncCallback(SendCallback), handler);
            }

            private static void SendCallback(IAsyncResult ar)
            {
                try
                {
                    // Retrieve the socket from the state object.
                    Socket handler = (Socket)ar.AsyncState;

                    // Complete sending the data to the remote device.
                    int bytesSent = handler.EndSend(ar);
                    //Console.WriteLine("Sent {0} bytes to client.", bytesSent);

                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }

            //method used to determin the IP the server is runnning on
            
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
        // State object for reading client data asynchronously
        public class StateObject
        {
            // Client  socket.
            public Socket workSocket = null;
            // Size of receive buffer.
            public const int BufferSize = 1024;
            // Receive buffer.
            public byte[] buffer = new byte[BufferSize];
            // Received data string.
            public StringBuilder sb = new StringBuilder();
        }
    
}
