using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace DataBaseHelper
{
    public class DALHelper : DBHelper
    {
        public DALHelper(string connectionString):base(connectionString)
        {
        }

        #region 【添加数据库条目】

        /// <summary>
        /// 添加单条数据库条目
        /// </summary>
        /// <param name="tableName">要添加的表名称</param>
        /// <param name="model">要添加的Model，可以为匿名对象</param>
        /// <param name="condition">推荐使用参数式写法</param>
        /// <returns></returns>
        public int Add(string tableName, object model)
        {
            Dictionary<string, object> paramsDic = GetModelDic(model);

            string cols = $"({string.Join(", ", paramsDic.Select(m => m.Key))})";
            string value = string.Join(", ", paramsDic.Select(m => $"@{m.Key}"));
            string sql = $"INSERT INTO {tableName} {cols} VALUE ({value}) ";
            SqlParameter[] sqlParameters = GetSqlParameters(paramsDic);
            return ExecuteCommand(sql, sqlParameters);
        }

        /// <summary>
        /// 添加多条数据库条目
        /// </summary>
        /// <param name="tableName">要添加的表名称</param>
        /// <param name="models">要添加的Model，可以为匿名对象</param>
        /// <param name="condition">推荐使用参数式写法</param>
        /// <returns></returns>
        public int Add(string tableName, List<object> models)
        {
            if (!IsModelsCanUsed(models))
            {
                return -1;
            }
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            List<string> values = new List<string>();
            for (int i = 0; i < models.Count; i++)
            {
                Dictionary<string, object> paramsDic = GetModelDic(models[i]);
                string value = string.Join(", ", paramsDic.Select(m => $"@{m.Key}{i}"));
                values.Add($"({value})");
                sqlParameters.AddRange(GetSqlParameters(paramsDic, i.ToString()));
            }
            string cols = $"({string.Join(", ", GetModelDic(models[0]).Select(m => m.Key))})";
            string sql = $"INSERT INTO {tableName} {cols} VALUE {string.Join(", ", values)} ";

            return ExecuteCommand(sql, sqlParameters.ToArray());
        }

        #endregion

        #region 【更新数据库条目】

        /// <summary>
        /// 修改单条数据库条目
        /// </summary>
        /// <param name="tableName">要更新的表名称</param>
        /// <param name="model">要更新的Model，可以为匿名对象</param>
        /// <param name="condition">推荐使用参数式写法</param>
        /// <returns></returns>
        public int Modify(string tableName, object model, string condition = "")
        {
            Dictionary<string, object> paramsDic = GetModelDic(model);
            string paramsSql = string.Join(", ", paramsDic.Select(m => $"{m.Key}=@{m.Key}"));
            string sql = $"UPDATE {tableName} SET {paramsSql} {(string.IsNullOrWhiteSpace(condition) ? "" : " WHERE ")} {condition}";
            SqlParameter[] sqlParameters = GetSqlParameters(paramsDic);
            return ExecuteCommand(sql, sqlParameters);
        }

        /// <summary>
        /// 修改多条数据库条目
        /// </summary>
        /// <param name="tableName">要更新的表名称</param>
        /// <param name="model">要更新的Model，可以为匿名对象</param>
        /// <param name="condition">推荐使用参数式写法</param>
        /// <returns></returns>
        public int Modify(string tableName, List<object> models, string condition = "")
        {
            if (!IsModelsCanUsed(models))
            {
                return -1;
            }
            string sql = "";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            for (int i = 0; i < models.Count; i++)
            {
                Dictionary<string, object> paramsDic = GetModelDic(models[i]);
                string paramsSql = string.Join(", ", paramsDic.Select(m => $"{m.Key}=@{m.Key}{i}"));
                sql += $"UPDATE {tableName} SET {paramsSql} {(string.IsNullOrWhiteSpace(condition) ? "" : " WHERE ")} {condition} {Environment.NewLine}";
                sqlParameters.AddRange(GetSqlParameters(paramsDic, i.ToString()));
            }
            return ExecuteCommand(sql, sqlParameters.ToArray());
        }

        #endregion

        #region 【删除数据库条目】

        /// <summary>
        /// 清空表或者删除符合条件的所有条目
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public int Delete(string tableName, string condition = "")
        {
            string sql = "";
            if (string.IsNullOrWhiteSpace(condition))
            {
                sql = $"TRUNCATE TABLE {tableName}";
            }
            else
            {
                sql = $"DELETE FROM {tableName} WHERE {condition}";
            }
            return ExecuteCommand(sql);
        }

        #endregion

        #region 【对传入的Model对象进行处理】

        /// <summary>
        /// 获取model对象的属性值
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private Dictionary<string, object> GetModelDic(object model)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            foreach (PropertyInfo p in model.GetType().GetProperties())
            {
                dic[p.Name] = p.GetValue(model);
            }
            return dic;
        }

        /// <summary>
        /// 获取由model对象传入的参数
        /// </summary>
        /// <param name="modelDic"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        private SqlParameter[] GetSqlParameters(Dictionary<string, object> modelDic, string i = "")
        {
            if (modelDic == null)
            {
                return new SqlParameter[0];
            }
            SqlParameter[] sqlParameters = new SqlParameter[modelDic.Count];
            int index = 0;
            foreach (var item in modelDic)
            {
                sqlParameters[index++] = new SqlParameter($"@{item.Key}{(string.IsNullOrWhiteSpace(i) ? "" : i)}", item.Value);
            }
            return sqlParameters;
        }

        private bool IsModelsCanUsed(List<object> models)
        {
            if (models.Count <= 0)
            {
                return false;
            }

            Type type = models[0].GetType();
            for (int i = 1; i < models.Count; i++)
            {
                if (type != models[i].GetType())
                {
                    return false;
                }
                else
                {
                    type = models[i].GetType();
                }
            }
            return true;
        }

        #endregion
    }
}
