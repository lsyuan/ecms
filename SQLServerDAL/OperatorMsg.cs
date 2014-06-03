using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using Ajax.Model;
using Ajax.DBUtility;
using System.Collections.Generic;
namespace Ajax.DAL
{
    /// <summary>
    /// 数据访问类:OperatorMsg
    /// </summary>
    public partial class OperatorMsgDAL
    {
        public OperatorMsgDAL()
        { }
        #region  Method

        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(string ID)
        {
            using (DBHelper db = DBHelper.Create())
            {
                return db.Exist<OperatorMsg>(ID);
            }
        }


        /// <summary>
        /// 增加一条数据
        /// </summary>
        public void Add(OperatorMsg model)
        {
            using (DBHelper db = DBHelper.Create())
            {
                db.Insert<OperatorMsg>(model);
            }
        }
        /// <summary>
        /// 增加多条记录
        /// </summary>
        /// <param name="list"></param>
        public void AddMul(List<OperatorMsg> list)
        {
            using (DBHelper db = DBHelper.Create())
            {
                db.InsertBatch<OperatorMsg>(list.ToArray());
            }
        }
        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(OperatorMsg model)
        {
            using (DBHelper db = DBHelper.Create())
            {
                db.Update<OperatorMsg>(model);
                return true;
            }
        }

        /// <summary>
        /// 更改状态消息是否已读的状态
        /// </summary>
        /// <param name="operatorMsgID">已读消息的ID</param> 
        /// <returns></returns>
        public Message ReadMsg(string operatorMsgID, string operatorID)
        {
            using (DBHelper db = DBHelper.Create())
            {
                db.BeginTransaction();
                string sql = "update T_OperatorMsg set status = 1 where MsgID = @msgID and OperatorID = @OperatorID";
                Dictionary<string, object> param = new Dictionary<string, object>();
                param.Add("msgID", operatorMsgID);
                param.Add("OperatorID", operatorID);
                int updateResult = db.ExecuteNonQuery(sql, param);
                Message msg = db.GetById<Message>(operatorMsgID);
                db.Commit();
                return msg;
            }
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(string ID)
        {
            using (DBHelper db = DBHelper.Create())
            {
                return db.DeleteByID<OperatorMsg>(ID);
            }
        }
        /// <summary>
        /// 根据消息ID删除多条记录
        /// </summary>
        /// <param name="MsgIDList"></param>
        /// <returns></returns>
        public bool DeleteMul(List<string> MsgIDList)
        {
            StringBuilder strSql = new StringBuilder("delete from T_OperatorMsg where MsgID in(");
            for (int i = 0; i < MsgIDList.Count; i++)
            {
                strSql.AppendFormat("'{0}'", MsgIDList[i]);
                strSql.Append(i == MsgIDList.Count - 1 ? "" : ",");
            }
            strSql.Append(")");
            using (DBHelper db = DBHelper.Create())
            {
                return db.ExecuteNonQuery(strSql.ToString()) > 0;
            }
        }
        /// <summary>
        /// 批量删除数据
        /// </summary>
        public bool DeleteList(string IDlist)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from T_OperatorMsg ");
            strSql.Append(" where ID in (" + IDlist + ")  ");
            using (DBHelper db = DBHelper.Create())
            {
                return db.ExecuteNonQuery(strSql.ToString()) > 0;
            }
        }


        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public OperatorMsg GetModel(string ID)
        {
            using (DBHelper db = DBHelper.Create())
            {
                return db.GetById<OperatorMsg>(ID);
            }
        }

        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<OperatorMsg> GetList(string strWhere)
        {
            using (DBHelper db = DBHelper.Create())
            {
                return db.GetList<OperatorMsg>(strWhere);
            }
        } 
        #endregion  Method
    }
}

