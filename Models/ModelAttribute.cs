using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Models
{
    public class ModelAttribute : Attribute
    {
        public SqlDbType DBDataType { get; set; }
        public string TableName { get; set; }
        public string FieldName { get; set; }
        public int Length { get; set; }
        public int Index { get; set; }

        public ModelAttribute()
        {
            TableName = "";
            FieldName = "";
            Length = 0;
            Index = 0;
        }
    }
}
