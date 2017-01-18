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
        private static MessageManager MsgManager = null;
        private const int BUF_SIZ = 8192;
        private IMessageDAL ims;

        /// <summary>
        /// constructor
        /// </summary>
        private MessageManager()
        {
            ims = MessageDAL.Instance;
        }

        static public MessageManager GetInstance()
        {
            if(MsgManager == null)
            {
                MsgManager = new MessageManager();
            }
            return MsgManager;
        }

        public bool Insert(string str, SvrCallbackArgs arg)
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

        public bool Delete(string str, SvrCallbackArgs arg)
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

        public bool Update(string str, SvrCallbackArgs arg)
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

        public bool SelectAll(SvrCallbackArgs arg)
        {
            bool ret = false;
            string response = "";
            try
            {
                IList<CustomMessage> list = ims.SelectAll();
                response += "ID\tTIMESATMP\t\tIP\tMESSAGE\n";

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
