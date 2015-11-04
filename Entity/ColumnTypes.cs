namespace BS.Components.Data.Entity
{
    using System;

    [Serializable, Flags]
    public enum ColumnTypes
    {
        Extend = 4,
        /// <summary>
        /// 主键
        /// </summary>
        Identity = 2,
        /// <summary>
        /// 自增
        /// </summary>
        Increment = 1,
        Read = 8,
        ReadInsert = 32,//0x20,
        ReadUpdate = 16,//0x10
    }
}

