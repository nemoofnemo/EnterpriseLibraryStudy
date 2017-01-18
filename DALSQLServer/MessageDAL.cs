using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.Data;
using System.Data.Common;
using Models;
using IDAL;
using System.Threading;

namespace DALSQLServer
{
    /// <summary>
    /// 通过MessageServic.Instance获取实例。
    /// </summary>
    public class MessageDAL : DALTemplate<CustomMessage>, IMessageDAL
    {
        /// <summary>
        /// 获取单例
        /// </summary>
        static private readonly MessageDAL instance = new MessageDAL();

        static public MessageDAL Instance
        {
            get { return instance; }
        }

        private MessageDAL()
        {
            
        }

        public bool Delete(int id)
        {
            CustomMessage msg = new CustomMessage();
            msg.ID = id;
            msg.IP = "";
            msg.Msg = "";
            msg.TimeStamp = "";
            return base.Delete(msg);
        }

        override public bool Insert(CustomMessage msg)
        {
            return base.Insert(msg);
        }

        public IList<CustomMessage> SelectAll()
        {
            CustomMessage msg = new CustomMessage();
            msg.ID = 0;
            msg.IP = "";
            msg.Msg = "";
            msg.TimeStamp = "";
            IDataReader reader = base.SelectAll(msg);

            List<CustomMessage> list = new List<CustomMessage>();
            while (reader.Read())
            {
                list.Add(new CustomMessage
                {
                    ID = reader[0] is System.DBNull ? -1 : (int)reader[0],
                    TimeStamp = reader[1] is System.DBNull ? "null" : (string)reader[1],
                    Msg = reader[2] is System.DBNull ? "null" : (string)reader[2],
                    IP = reader[3] is System.DBNull ? "null" : (string)reader[3]
                });
            }
            return list;
        }

        public CustomMessage SelectByID(int id)
        {
            throw new NotImplementedException();
        }

        override public bool Update(CustomMessage msg)
        {
            return base.Update(msg);
        }
    }


}
