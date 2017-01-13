using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Communication
{
    public class SvrCallbackArgs
    {
        public Socket socket;
        public string timeStamp;
        public string ip;
    }

    public delegate void SvrCallback(SvrCallbackArgs arg);

    public interface ISvrCallback{
        void Run(SvrCallback callback);
    }

    public class ServerModule : ISvrCallback
    {
        private const int BUFFER_SIZE = 8192;
        private IPEndPoint ipEndPoint;
        private Socket serverSocket;
        private SvrCallback svrCallback;

        /// <summary>
        /// constructor
        /// </summary>
        public ServerModule()
        {
            ipEndPoint = new IPEndPoint(IPAddress.Any, 6001);
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(ipEndPoint);
            serverSocket.Listen(100);
            svrCallback = null;
        }

        /// <summary>
        /// start server.
        /// </summary>
        /// <param name="callback"></param>
        public void Run(SvrCallback callback)
        {
            svrCallback = callback;
            Console.WriteLine("Server start.");

            while (true)
            {
                SvrCallbackArgs args = new SvrCallbackArgs();
                Console.WriteLine("Waiting connection.");
                args.socket = serverSocket.Accept();
                Console.WriteLine("Accept new connection.");
                args.timeStamp = Util.DateTimeString.Get();
                args.ip = args.socket.RemoteEndPoint.ToString();
                ThreadPool.QueueUserWorkItem(new WaitCallback(MessageHandler), args);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        private void MessageHandler(object obj)
        {
            SvrCallbackArgs args = (SvrCallbackArgs)obj;
            try
            {
                svrCallback(args);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                args.socket.Close();
                Console.WriteLine("Connection Closed.");
            }
        }
    }
}
