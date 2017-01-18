using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL;
using Communication;

namespace Application
{
    public class SystemCreator
    {
        private MessageManager msgManager;
        private const int BUF_SIZ = 8192;
        private ServerModule server;

        public SystemCreator()
        {
            server = new ServerModule();
            msgManager = MessageManager.GetInstance();
        }

        /// <summary>
        /// start Manager
        /// </summary>
        public void StartServer()
        {
            server.Run(new SvrCallback(Callback));
        }

        private void Callback(SvrCallbackArgs arg)
        {
            byte[] buf = new byte[BUF_SIZ];
            int count;
            while ((count = arg.socket.Receive(buf, BUF_SIZ, System.Net.Sockets.SocketFlags.None)) != 0)
            {
                string str = ASCIIEncoding.ASCII.GetString(buf, 0, count);
                string[] arr = str.Split(new char[] { ';' });

                if (arr.Length == 1 || arr.Length == 2)
                {
                    switch (arr[0])
                    {
                        case "insert":
                            msgManager.Insert(arr[1], arg);
                            break;
                        case "delete":
                            msgManager.Delete(arr[1], arg);
                            break;
                        case "update":
                            msgManager.Update(arr[1], arg);
                            break;
                        case "select":
                            msgManager.SelectAll(arg);
                            break;
                        default:
                            Console.WriteLine("Invalid Managers Branch.");
                            break;
                    }
                }
            }

        }
    }
}
