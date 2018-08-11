using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DataBaseHelper
{
    public static class DALHelper
    {
        #region 【更新数据库条目】

        /// <summary>
        /// 修改数据库条目
        /// </summary>
        /// <param name="tableName">要更新的表名称</param>
        /// <param name="model">要更新的Model，可以为匿名对象</param>
        /// <param name="condition">推荐使用参数式写法</param>
        /// <returns></returns>
        public static int Modify(string connectionString, string tableName, object model, string condition = "")
        {
            Dictionary<string, object> paramsDic = GetModelDic(model);
            string paramsSql = string.Join(", ", paramsDic.Select(m => $"{m.Key}=@{m.Key}"));
            string sql = $"UPDATE {tableName} SET {paramsSql} {(string.IsNullOrWhiteSpace(condition) ? "" : " WHERE ")} {condition}";
            SqlParameter[] sqlParameters = GetSqlParameters(paramsDic);
            return new DBHelper(connectionString).ExecuteCommand(sql, sqlParameters);
        }

        #endregion


        #region 【对传入的Model对象进行处理】

        /// <summary>
        /// 获取model对象的属性值
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private static Dictionary<string, object> GetModelDic(object model)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            foreach (PropertyInfo p in model.GetType().GetProperties())
            {
                dic[p.Name] = p.GetValue(model);
            }
            return dic;
        }

        private static SqlParameter[] GetSqlParameters(Dictionary<string, object> modelDic)
        {
            if (modelDic == null)
            {
                return new SqlParameter[0];
            }
            SqlParameter[] sqlParameters = new SqlParameter[modelDic.Count];
            int index = 0;
            foreach (var item in modelDic)
            {
                sqlParameters[index++] = new SqlParameter($"@{item.Key}", item.Value);
            }
            return sqlParameters;
        }

        #endregion
    }
}
