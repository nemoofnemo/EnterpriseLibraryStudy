using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Communication;
using DALSQLServer;
using IDAL;
using Models;

namespace BLL
{
    /// <summary>
    /// bussiness logic layer.
    /// funtions:
    /// 1.create IMessageService instancs from DALSQLServer.MessageServices.
    /// 2.create ServerModule instance.
    /// 3.control database.
    /// </summary>
    public class MessageManager
    {
        private const int BUF_SIZ = 8192;
        private ServerModule server;
        private IMessageService ims;

        /// <summary>
        /// constructor
        /// </summary>
        public MessageManager()
        {
            server = new ServerModule();
            ims = MessageService.GetInstance();
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
            while((count = arg.socket.Receive(buf, BUF_SIZ, System.Net.Sockets.SocketFlags.None)) != 0)
            {
                string str = ASCIIEncoding.ASCII.GetString(buf, 0, count);
                string[] arr = str.Split(new char[] { ';' });

                if(arr.Length == 1 || arr.Length == 2)
                {
                    switch (arr[0])
                    {
                        case "insert":
                            Insert(arr[1], arg);
                            break;
                        case "delete":
                            Delete(arr[1], arg);
                            break;
                        case "update":
                            Update(arr[1], arg);
                            break;
                        case "select":
                            SelectAll(arg);
                            break;
                        default:
                            Console.WriteLine("Invalid Managers Branch.");
                            break;
                    }
                }
            }
            
        }

        private bool Insert(string str, SvrCallbackArgs arg)
        {
            bool ret = false;
            try
            {
                string[] arr = str.Split(new char[] { ':' });
                if (arr.Length == 2)
                {
                    CustomMessage msg = new CustomMessage();
                    msg.ID = Int32.Parse(arr[0]);
                    msg.TimeStamp = arg.timeStamp;
                    msg.IP = arg.ip;
                    msg.Msg = arr[1] ;
                    ret = ims.Insert(msg);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                ret = false;
            }

            try
            {
                string response;
                if (ret == true)
                {
                    response = "Server To Client: Insert Success.";
                }
                else
                {
                    response = "Server To Client: Insert Failed.";
                }
                arg.socket.Send(Encoding.Default.GetBytes(response));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                ret = false;
            }

            return ret;
        }
        
        private bool Delete(string str, SvrCallbackArgs arg)
        {
            bool ret = false;
            try
            {
                ret = ims.Delete(Int32.Parse(str));
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }

            try
            {
                string response;
                if (ret == true)
                {
                    response = "Server To Client: Delete Success.";
                }
                else
                {
                    response = "Server To Client: Delete Failed.";
                }
                arg.socket.Send(Encoding.Default.GetBytes(response));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                ret = false;
            }

            return ret;
        }

        private bool Update(string str, SvrCallbackArgs arg)
        {
            bool ret = false;
            try
            {
                string[] arr = str.Split(new char[] { ':' });
                if (arr.Length == 2)
                {
                    CustomMessage msg = new CustomMessage();
                    msg.ID = Int32.Parse(arr[0]);
                    msg.TimeStamp = arg.timeStamp;
                    msg.IP = arg.ip;
                    msg.Msg = arr[1];
                    ret = ims.Update(msg);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            try
            {
                string response;
                if (ret == true)
                {
                    response = "Server To Client: Update Success.";
                }
                else
                {
                    response = "Server To Client: Update Failed.";
                }
                arg.socket.Send(Encoding.Default.GetBytes(response));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                ret = false;
            }

            return ret;
        }

        private bool SelectAll(SvrCallbackArgs arg)
        {
            bool ret = false;
            string response = "";
            try
            {
                IList<CustomMessage> list = ims.SelectAll();
                response += "ID\tTIMESATMP\tIP\tMESSAGE\n";

                foreach (var item in list)
                {
                    response += String.Format("{0}\t{1}\t{2}\t{3}\n", item.ID, item.TimeStamp, item.IP, item.Msg);
                }

                ret = true;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }

            try
            {
                if (ret == true)
                {
                    //....
                }
                else
                {
                    response = "Server To Client: Select Failed.";
                }
                arg.socket.Send(Encoding.Default.GetBytes(response));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                ret = false;
            }
            return ret;
        }
    }
}
