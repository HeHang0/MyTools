using System;
using System.Collections.Generic;
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
        public static int Modify(string tableName, object model, string condition = "")
        {
            Dictionary<string, string> paramsDic = GetModelDic(model);
            string paramsSql = string.Join(", ", paramsDic.Select(m => $"{m.Key}=@{m.Key}"));
            string sql = $"UPDATE {tableName} SET {paramsSql} {(string.IsNullOrWhiteSpace(condition) ? "" : " WHERE ")} {condition}";
        }

        #endregion


        #region 【获取model对象的属性值】

        public static Dictionary<string, string> GetModelDic(object model)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            foreach (PropertyInfo p in model.GetType().GetProperties())
            {
                dic[p.Name] = p.GetValue(model).ToString();
            }
            return dic;
        }

        #endregion
    }
}
