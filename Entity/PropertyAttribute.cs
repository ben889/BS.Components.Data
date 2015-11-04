namespace BS.Components.Data.Entity
{
    using System;

    public class PropertyAttribute : Attribute
    {
        private ColumnTypes _columnType;
        private string _tableName;

        public PropertyAttribute(string tableName)
        {
            this._tableName = tableName;
        }

        public PropertyAttribute(ColumnTypes columnType)
        {
            this._columnType = columnType;
        }

        public ColumnTypes ColumnType
        {
            get
            {
                return this._columnType;
            }
            set
            {
                this._columnType = value;
            }
        }

        public string TableName
        {
            get
            {
                return this._tableName;
            }
            set
            {
                this._tableName = value;
            }
        }
    }
}

