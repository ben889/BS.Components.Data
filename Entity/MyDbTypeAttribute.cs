using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace BS.Components.Data.Entity
{
    public class MyDbTypeAttribute : Attribute
    {
        /// <summary>
        /// SQL数据类型
        /// </summary>
        public SqlDbType dbType { set; get; }
    }
}
