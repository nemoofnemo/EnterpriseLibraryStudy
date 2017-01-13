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
    /// 对CustomMessage进行数据库操作(线程安全)。
    /// 单例模式。使用GetInstance方法获取实例。
    /// </summary>
    public class MessageService : IMessageService
    {
        private static MessageService _Instance;
        private _MessageService _MsgService;

        private MessageService()
        {
            _MsgService = new _MessageService();
        }

        /// <summary>
        /// 获取实例
        /// </summary>
        /// <returns></returns>
        public static MessageService GetInstance()
        {
            if (_Instance == null)
            {
                _Instance = new MessageService();
            }
            return _Instance;
        }

        public bool Insert(CustomMessage msg)
        {
            return _Instance._MsgService.Insert(msg);
        }

        public bool Delete(int id)
        {
            return _Instance._MsgService.Delete(id);
        }

        public bool Update(Models.CustomMessage msg)
        {
            return _Instance._MsgService.Update(msg);
        }

        public IList<Models.CustomMessage> SelectAll()
        {
            return _Instance._MsgService.SelectAll();
        }

        public Models.CustomMessage SelectByID(int id)
        {
            return _Instance._MsgService.SelectByID(id);
        }
    }

    /// <summary>
    /// 通过MessageServic.GetInstance()获取。
    /// </summary>
    class _MessageService : IMessageService
    {
        private SqlDatabase sqlServerDB;
        private DbCommand insertCmd;
        private DbCommand updateCmd;
        private DbCommand deleteCmd;
        private DbCommand selectAllCmd;
        private DbCommand selectSingleCmd;

        /// <summary>
        /// 初始化数据库连接及sql语句
        /// </summary>
        public _MessageService()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            sqlServerDB = factory.CreateDefault() as SqlDatabase;

            insertCmd = sqlServerDB.GetSqlStringCommand("INSERT INTO table_msg VALUES(@ID, @TimeStamp, @Msg, @IP)");
            updateCmd = sqlServerDB.GetSqlStringCommand("UPDATE table_msg SET timestamp=@TimeStamp, message=@Msg, ip=@IP WHERE id=@ID"); 
            deleteCmd = sqlServerDB.GetSqlStringCommand("DELETE FROM table_msg WHERE id=@ID"); 
            selectAllCmd = sqlServerDB.GetSqlStringCommand("SELECT * FROM table_msg");
            selectSingleCmd = sqlServerDB.GetSqlStringCommand("SELECT * FROM table_msg WHERE id=@ID");

            //insert parameters
            sqlServerDB.AddInParameter(insertCmd, "ID", SqlDbType.Int, 0);
            sqlServerDB.AddInParameter(insertCmd, "TimeStamp", SqlDbType.NChar, "null");
            sqlServerDB.AddInParameter(insertCmd, "Msg", SqlDbType.NChar, "null");
            sqlServerDB.AddInParameter(insertCmd, "IP", SqlDbType.NChar, "null");

            //select single
            sqlServerDB.AddInParameter(selectSingleCmd, "ID", SqlDbType.Int, 0);

            //update parameters
            sqlServerDB.AddInParameter(updateCmd, "TimeStamp", SqlDbType.NChar, "null");
            sqlServerDB.AddInParameter(updateCmd, "Msg", SqlDbType.NChar, "null");
            sqlServerDB.AddInParameter(updateCmd, "IP", SqlDbType.NChar, "null");
            sqlServerDB.AddInParameter(updateCmd, "ID", SqlDbType.Int, 0);

            //delete parameter
            sqlServerDB.AddInParameter(deleteCmd, "ID", SqlDbType.Int, 0);
        }

        public bool Delete(int id)
        {
            bool ret;
            lock (sqlServerDB)
            {
                sqlServerDB.SetParameterValue(deleteCmd, "@ID", id);
                ret = (sqlServerDB.ExecuteNonQuery(deleteCmd) == 1);
            }
            return ret;
        }

        public bool Insert(CustomMessage msg)
        {
            bool ret;
            lock (sqlServerDB)
            {
                sqlServerDB.SetParameterValue(insertCmd, "ID", msg.ID);
                sqlServerDB.SetParameterValue(insertCmd, "TimeStamp", msg.TimeStamp);
                sqlServerDB.SetParameterValue(insertCmd, "Msg", msg.Msg);
                sqlServerDB.SetParameterValue(insertCmd, "IP", msg.IP);

                ret = (sqlServerDB.ExecuteNonQuery(insertCmd) == 1);
            }
            return ret;
        }

        public CustomMessage SelectByID(int id)
        {
            CustomMessage ret = null;

            lock (sqlServerDB)
            {
                sqlServerDB.SetParameterValue(selectSingleCmd, "ID", id);
                using(IDataReader reader = sqlServerDB.ExecuteReader(selectSingleCmd))
                {
                    if (reader.Read())
                    {
                        ret = new CustomMessage() {
                            ID = id,
                            TimeStamp = reader[1] is System.DBNull ? "null" : (string)reader[1],
                            Msg = reader[2] is System.DBNull ? "null" : (string)reader[2],
                            IP = reader[3] is System.DBNull ? "null" : (string)reader[3]
                        };
                    }
                }
            }

            return ret;
        }

        public IList<CustomMessage> SelectAll()
        {
            List<CustomMessage> list = new List<CustomMessage>();

            lock (sqlServerDB)
            {
                using (IDataReader reader = sqlServerDB.ExecuteReader(selectAllCmd))
                {
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
                }
            }

            return list;
        }

        public bool Update(CustomMessage msg)
        {
            bool ret;
            lock (sqlServerDB)
            {
                sqlServerDB.SetParameterValue(updateCmd, "TimeStamp", msg.TimeStamp);
                sqlServerDB.SetParameterValue(updateCmd, "Msg", msg.Msg);
                sqlServerDB.SetParameterValue(updateCmd, "IP", msg.IP);
                sqlServerDB.SetParameterValue(updateCmd, "ID", msg.ID);

                ret = (sqlServerDB.ExecuteNonQuery(updateCmd) == 1);
            }
            return ret;
        }
    }

   
}
