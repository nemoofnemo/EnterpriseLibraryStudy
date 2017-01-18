using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.Data;
using System.Data.Common;
using System.Reflection;
using Models;

namespace DALSQLServer
{
    /// <summary>
    /// 任何派生自该基类的类，具有T类型对象在sqlserver上的增删改查功能。
    /// 作为arg的实体类对象需要按照modelattribute配置。
    /// 该类使用反射获取属性，特性等信息。
    /// 实体类的属性应按照数据库内字段进行排序
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DALTemplate<T> where T: class, new()
    {
        private static string typeName = "";
        private string tableName = "";
        private SqlDatabase sqlServerDB;

        protected DALTemplate()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            sqlServerDB = factory.CreateDefault() as SqlDatabase;


            //get table name.
            Type type = typeof(T);
            typeName = type.Name;
            object[] arr = type.GetCustomAttributes(typeof(ModelAttribute), true);

            //warning: arr.Length >= 1 .
            if(arr.Length >= 1)
            {
                ModelAttribute classAttr = arr[0] as ModelAttribute;
                if(classAttr != null)
                {
                    tableName = classAttr.TableName;
                }
                else
                {
                    //todo:error
                    Console.WriteLine("model class attribute error: tablename is not set");
                }
            }
        }

        /// <summary>
        /// insert an object
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public virtual bool Insert(T arg)
        {
            //1.get field name(in database table)
            //2.get field name(in class)
            //3.get value
            Type type = arg.GetType();
            PropertyInfo[] infoArr = type.GetProperties();
            List<ModelAttribute> cusAttrList = new List<ModelAttribute>();
            List<string> nameList = new List<string>();
            List<object> valueList = new List<object>();

            if(infoArr.Length == 0)
            {
                return false;
            }

            foreach(var item in infoArr)
            {
                //get custom attributes
                object[] objAttrs = item.GetCustomAttributes(typeof(ModelAttribute), true);
                if (objAttrs.Length > 0)
                {
                    ModelAttribute attr = objAttrs[0] as ModelAttribute;
                    if (attr != null)
                    {
                        cusAttrList.Add(attr);
                    }
                    else
                    {
                        Console.WriteLine("in DALTemplate insert: property attribute error.");
                        return false;
                    }
                }

                //get name and value
                string _temp = item.Name;
                if (_temp.Length > 0)
                {
                    nameList.Add(item.Name);
                    valueList.Add(item.GetValue(arg));
                }
                else
                {
                    //todo:error
                    Console.WriteLine("in DALTemplate insert: property attribute error.");
                    return false;
                }

            }

            //create sql statement and execute query
            string sql = "INSERT INTO " + this.tableName + " VALUES(";
            int count = cusAttrList.Count;

            for(int i = 0; i < count; ++i)
            {
                sql += "@";
                sql += nameList[i];
                if(i != count - 1)
                {
                    sql += ",";
                }
                else
                {
                    sql += ")";
                }
            }

            bool ret = false;
            using(DbCommand cmd = sqlServerDB.GetSqlStringCommand(sql))
            {
                for(int i = 0; i < count; ++i)
                {
                    sqlServerDB.AddInParameter(cmd, nameList[i], cusAttrList[i].DBDataType, valueList[i]);
                }

                ret = sqlServerDB.ExecuteNonQuery(cmd) == 1;
            }

            return ret;
        }

        /// <summary>
        /// delete
        /// identified by the first elements of arg class.
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public virtual bool Delete(T arg) {
            //1.get field name(in database table)
            //2.get field name(in class)
            //3.get value
            Type type = arg.GetType();
            PropertyInfo[] infoArr = type.GetProperties();
            List<ModelAttribute> cusAttrList = new List<ModelAttribute>();
            List<string> nameList = new List<string>();
            List<object> valueList = new List<object>();

            if(infoArr.Length == 0)
            {
                return false;
            }

            foreach (var item in infoArr)
            {
                //get custom attributes
                object[] objAttrs = item.GetCustomAttributes(typeof(ModelAttribute), true);
                if (objAttrs.Length > 0)
                {
                    ModelAttribute attr = objAttrs[0] as ModelAttribute;
                    if (attr != null)
                    {
                        cusAttrList.Add(attr);
                    }
                    else
                    {
                        Console.WriteLine("in DALTemplate insert: property attribute error.");
                        return false;
                    }
                }

                //get name and value
                string _temp = item.Name;
                if (_temp.Length > 0)
                {
                    nameList.Add(item.Name);
                    valueList.Add(item.GetValue(arg));
                }
                else
                {
                    //todo:error
                    Console.WriteLine("in DALTemplate insert: property attribute error.");
                    return false;
                }

            }

            //create sql statement and execute query
            ////DELETE FROM table_msg WHERE id = @ID
            string sql = "DELETE FROM " + this.tableName + " WHERE " + cusAttrList[0].FieldName +"=@" + nameList[0];

            bool ret = false;
            using (DbCommand cmd = sqlServerDB.GetSqlStringCommand(sql))
            {
                sqlServerDB.AddInParameter(cmd, nameList[0], cusAttrList[0].DBDataType, valueList[0]);
                ret = sqlServerDB.ExecuteNonQuery(cmd) == 1;
            }

            return ret;
        }

        /// <summary>
        /// ...
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public virtual bool Update(T arg)
        {
            //1.get field name(in database table)
            //2.get field name(in class)
            //3.get value
            Type type = arg.GetType();
            PropertyInfo[] infoArr = type.GetProperties();
            List<ModelAttribute> cusAttrList = new List<ModelAttribute>();
            List<string> nameList = new List<string>();
            List<object> valueList = new List<object>();

            if (infoArr.Length < 2)
            {
                return false;
            }

            foreach (var item in infoArr)
            {
                //get custom attributes
                object[] objAttrs = item.GetCustomAttributes(typeof(ModelAttribute), true);
                if (objAttrs.Length > 0)
                {
                    ModelAttribute attr = objAttrs[0] as ModelAttribute;
                    if (attr != null)
                    {
                        cusAttrList.Add(attr);
                    }
                    else
                    {
                        Console.WriteLine("in DALTemplate insert: property attribute error.");
                        return false;
                    }
                }

                //get name and value
                string _temp = item.Name;
                if (_temp.Length > 0)
                {
                    nameList.Add(item.Name);
                    valueList.Add(item.GetValue(arg));
                }
                else
                {
                    //todo:error
                    Console.WriteLine("in DALTemplate insert: property attribute error.");
                    return false;
                }

            }

            //create sql statement and execute query
            //UPDATE table_msg SET timestamp=@TimeStamp, message=@Msg, ip=@IP WHERE id=@ID
            string sql = "UPDATE " + this.tableName + " SET ";
            int count = cusAttrList.Count;
            for (int i = 1; i < count; ++i)
            {
                sql += cusAttrList[i].FieldName;
                sql += "=@";
                sql += nameList[i];
                if (i != count - 1)
                {
                    sql += ",";
                }
                else
                {
                    sql += " ";
                }
            }
            sql += "WHERE " + cusAttrList[0].FieldName + "=@" + nameList[0];

            bool ret = false;
            using (DbCommand cmd = sqlServerDB.GetSqlStringCommand(sql))
            {
                for (int i = 1; i < count; ++i)
                {
                    sqlServerDB.AddInParameter(cmd, nameList[i], cusAttrList[i].DBDataType, valueList[i]);
                }
                sqlServerDB.AddInParameter(cmd, nameList[0], cusAttrList[0].DBDataType, valueList[0]);
                ret = sqlServerDB.ExecuteNonQuery(cmd) == 1;
            }

            return ret;
        }

        /// <summary>
        /// return a list of result.
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public virtual IDataReader SelectAll(T arg)
        {
            //1.get field name(in database table)
            //2.get field name(in class)
            //3.get value
            Type type = arg.GetType();
            PropertyInfo[] infoArr = type.GetProperties();
            List<ModelAttribute> cusAttrList = new List<ModelAttribute>();
            List<string> nameList = new List<string>();
            List<object> valueList = new List<object>();

            if (infoArr.Length == 0)
            {
                return null;
            }

            foreach (var item in infoArr)
            {
                //get custom attributes
                object[] objAttrs = item.GetCustomAttributes(typeof(ModelAttribute), true);
                if (objAttrs.Length > 0)
                {
                    ModelAttribute attr = objAttrs[0] as ModelAttribute;
                    if (attr != null)
                    {
                        cusAttrList.Add(attr);
                    }
                    else
                    {
                        Console.WriteLine("in DALTemplate SelectAll: property attribute error.");
                        return null;
                    }
                }

                //get name and value
                string _temp = item.Name;
                if (_temp.Length > 0)
                {
                    nameList.Add(item.Name);
                    valueList.Add(item.GetValue(arg));
                }
                else
                {
                    //todo:error
                    Console.WriteLine("in DALTemplate SelectAll: property attribute error.");
                    return null;
                }

            }

            //create sql statement and execute query
            string sql = "SELECT * FROM " + this.tableName;
            IDataReader reader;
            using (DbCommand cmd = sqlServerDB.GetSqlStringCommand(sql))
            {
                reader = sqlServerDB.ExecuteReader(cmd);
            }

            return reader;
        }
    }
}

/*
 * 1.获取属性上的Attribute
 * CustomMessage msg = new CustomMessage();
    msg.ID = 1;
    msg.IP = "aaa";
    msg.Msg = "sss";
    msg.TimeStamp = "aaaaa";

    Type type = msg.GetType();
    foreach(var item in type.GetProperties())
    {
        object[] objAttrs = item.GetCustomAttributes(typeof(ModelAttribute), true);
        if (objAttrs.Length > 0)
        {
            ModelAttribute attr = objAttrs[0] as ModelAttribute;
            if (attr != null)
            {
                Console.WriteLine(attr.FieldName);
            }
        }
    }
 *
 * 2.获取属性名
 *  Type type = msg.GetType();
    foreach (var item in type.GetProperties())
    {
        Console.WriteLine(item.Name);
    }
 * 
 * 3.item.GetValue
 * 
 * 
 */
