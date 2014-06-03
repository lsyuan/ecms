using System.Collections;
using System.Collections.Generic;
using System.Data.Common;

namespace Ajax.DBUtility
{
    public class SQLDataCache
    {

        /// <summary>
        /// 模型对应的 CRUD SQL缓存
        /// </summary>
        public static Hashtable GENERATED_SQL_CACHE = Hashtable.Synchronized(new Hashtable());
        /// <summary>
        /// 模型对应的默认参数缓存
        /// </summary>
        public static Hashtable MODEL_PARAMETER_CACHE = Hashtable.Synchronized(new Hashtable());

        #region SQL缓存存取操作
        //Note:将模型对应的增删改查几种操作生成的SQL语句放入缓存中,第二次访问时直接读取缓存数据 

        /// <summary>
        /// 将数据放入缓存中
        /// </summary>
        /// <param name="key">键值</param>
        /// <param name="value"></param>
        public static void ModelCURDHandleSQLPush(object model, CRUDEnum type, object value)
        {
            GENERATED_SQL_CACHE.Add(KeyStringForCRUDByModel(type, model), value);
        }

        /// <summary>
        /// 获取模型CRUD操作缓存语句
        /// </summary>
        /// <param name="model">模型</param>
        /// <param name="type">操作类型</param>
        /// <returns></returns>
        public static string GetHashTableData(object model, CRUDEnum type)
        {
            string KEY = KeyStringForCRUDByModel(type, model);
            if (GENERATED_SQL_CACHE.ContainsKey(KEY))
            {
                return GENERATED_SQL_CACHE[KEY].ToString();
            }
            else
            {
                return string.Empty;
            }
        }
        /// <summary>
        /// 生成模型CRUD操作的键值
        /// </summary>
        /// <param name="type">操作类型</param>
        /// <param name="model">模型</param>
        /// <returns></returns>
        private static string KeyStringForCRUDByModel(CRUDEnum type, object model)
        {
            string typeName = model.GetType().Name;
            switch (type)
            {
                case CRUDEnum.INSERT:
                    return "INSERT_" + typeName;
                case CRUDEnum.UDPATE:
                    return "UDPATE_" + typeName;
                case CRUDEnum.DELETE:
                    return "DELETE_" + typeName;
                case CRUDEnum.SELECT:
                    return "SELECT_" + typeName;
                case CRUDEnum.GETBYID:
                    return "GETBYID_" + typeName;
                case CRUDEnum.DELETEBYID:
                    return "DELETEBYID_" + typeName;
                default: return "";
            }
        }
        #endregion

        #region 参数缓存存取操作

        /// <summary>
        /// 将数据放入缓存中
        /// </summary>
        /// <param name="model">模型</param>
        /// <param name="value"></param>
        public static void ModelParameterPush<P>(object model, object value)
        {
            string KEY = KeyStringForParameterByModel(model);
            if (!MODEL_PARAMETER_CACHE.Contains(KEY))
            {
                MODEL_PARAMETER_CACHE.Add(KEY, value);
            }
        }

        /// <summary>
        /// 获取模型参数缓存
        /// </summary>
        /// <param name="model">模型</param> 
        /// <returns></returns>
        public static List<P> GetParameterCacheData<P>(object model)
        {
            string KEY = KeyStringForParameterByModel(model);
            if (MODEL_PARAMETER_CACHE.ContainsKey(KEY))
            {
                List<P> list = MODEL_PARAMETER_CACHE[KEY] as List<P>;
                List<P> newList = new List<P>();
                P[] newArray = new P[list.Count];
                newArray = list.ToArray();
                newList.AddRange(newArray);
                return newList;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 生成模型参数的键值
        /// </summary> 
        /// <param name="model">模型</param>
        /// <returns>Parameter_Agreements</returns>
        private static string KeyStringForParameterByModel(object model)
        {
            string typeName = model.GetType().Name;
            return "Parameter_" + typeName;
        }
        #endregion
    }
}
