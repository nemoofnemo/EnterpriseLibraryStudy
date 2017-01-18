using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    
    [Model(TableName = "table_msg", Index = 0)]
    public class CustomMessage
    {
        [Model(DBDataType = System.Data.SqlDbType.Int, FieldName = "id", Index = 1)]
        public int ID
        {
            get;
            set;
        }


        [Model(DBDataType = System.Data.SqlDbType.NChar, FieldName = "timestamp", Index = 3)]
        public string TimeStamp
        {
            get;
            set;
        }

        [Model(DBDataType = System.Data.SqlDbType.NChar, FieldName = "message", Index = 2)]
        public string Msg
        {
            get;
            set;
        }

        [Model(DBDataType = System.Data.SqlDbType.NChar, FieldName = "ip", Index = 4)]
        public string IP
        {
            get;
            set;
        }
    }
}
