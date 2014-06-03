using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Ajax.DBUtility
{
    /// <summary>
    /// 列名字符串生成类
    /// </summary>
    public class ColumnGenerator
    {
        /// <summary>
        /// 生成列名字符串
        /// </summary>
        /// <typeparam name="T">泛型T</typeparam>
        /// <returns>列名字符串 eg:ID,Name,Age...</returns>
        public static string Columns<T>()
        {
            StringBuilder temp = new StringBuilder();

            foreach (PropertyInfo propertyInfo in typeof(T).GetProperties())
            {
                object[] propList = propertyInfo.GetCustomAttributes(typeof(ColumnAttribute), false);
                if (propList.Length > 0)
                {
                    ColumnAttribute column = (ColumnAttribute)propList[0];
                    temp.Append(column.Code).Append(",");
                }
            }
            if (temp.Length > 0)
            {
                temp.Remove(temp.Length - 1, 1);
            }
            return temp.ToString();
        }
    }
}
