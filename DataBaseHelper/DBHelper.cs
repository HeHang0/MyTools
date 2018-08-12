using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBaseHelper
{
    public class DBHelper
    {
        protected string connectionString;
        public DBHelper(string connectionString) => this.connectionString = connectionString;

        #region 【执行sql语句】

        /// <summary>
        /// 执行sql语句，不带参数
        /// </summary>
        /// <param name="sqlCommand"></param>
        /// <returns></returns>
        public int ExecuteCommand(string sqlCommand)
        {
            return ExecuteCommand(sqlCommand, null);
        }

        /// <summary>
        /// 执行sql语句，带参数
        /// </summary>
        /// <param name="sqlCommand"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public int ExecuteCommand(string sqlCommand, params SqlParameter[] parameters)
        {
            int count = 0;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(sqlCommand, conn);
                PrepareCommand(command, conn, null, CommandType.Text, sqlCommand, parameters);
                count = command.ExecuteNonQuery();
            }
            return count;
        }

        #endregion

        #region 【执行sql语句，返回首行首列】

        /// <summary>
        /// 执行SQL语句 返回执行结果的首行首列, 不带参数
        /// </summary>
        /// <param name="sqlCommand"></param>
        /// <returns></returns>
        public object ExecuteScalar(string sqlCommand)
        {
            return ExecuteScalar(sqlCommand, null);
        }

        /// <summary>
        /// 执行SQL语句 返回执行结果的首行首列, 带参数
        /// </summary>
        /// <param name="sqlCommand"></param>
        /// <returns></returns>
        public object ExecuteScalar(string sqlCommand, params SqlParameter[] parameters)
        {
            object result = null;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(sqlCommand, conn);
                PrepareCommand(command, conn, null, CommandType.Text, sqlCommand, parameters);
                result = command.ExecuteScalar();
            }
            return result;
        }

        #endregion

        #region 【执行sql语句，返回DataReader】

        /// <summary>
        /// 执行一条SQL语句 返回DataReader对象(带参数)
        /// </summary>
        /// <param name="sqlCommand"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public SqlDataReader ExecuteReader(string sqlCommand)
        {
            return ExecuteReader(sqlCommand, null);
        }

        /// <summary>
        /// 执行一条SQL语句 返回DataReader对象(带参数)
        /// </summary>
        /// <param name="sqlCommand"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public SqlDataReader ExecuteReader(string sqlCommand, params SqlParameter[] parameters)
        {
            SqlDataReader dataReader = null;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(sqlCommand, conn);
                PrepareCommand(command, conn, null, CommandType.Text, sqlCommand, parameters);
                dataReader = command.ExecuteReader(CommandBehavior.CloseConnection);
            }
            return dataReader;
        }

        #endregion

        #region 【执行sql语句，返回DataSet】

        /// <summary>
        /// 执行sql语句，返回数据集DataSet， 不带参数
        /// </summary>
        /// <param name="sqlCommand"></param>
        /// <returns></returns>
        public DataSet ExecuteDataSet(string sqlCommand)
        {
            return ExecuteDataSet(sqlCommand, null);
        }

        /// <summary>
        /// 执行sql语句，返回数据集DataSet, 带参数
        /// </summary>
        /// <param name="sqlCommand"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public DataSet ExecuteDataSet(string sqlCommand, params SqlParameter[] parameters)
        {
            DataSet dataSet = new DataSet();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(sqlCommand, conn);
                PrepareCommand(command, conn, null, CommandType.Text, sqlCommand, parameters);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                adapter.Fill(dataSet);
            }
            return dataSet;
        }

        #endregion

        #region 【执行sql语句，返回DataTable】

        /// <summary>
        /// 执行sql语句，返回DataTable，不带参数
        /// </summary>
        /// <param name="sqlCommand"></param>
        /// <returns></returns>
        public DataTable ExecuteDataTable(string sqlCommand)
        {
            return ExecuteDataTable(sqlCommand, null);
        }

        /// <summary>
        /// 执行sql语句，返回DataTable，带参数
        /// </summary>
        /// <param name="sqlCommand"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public DataTable ExecuteDataTable(string sqlCommand, params SqlParameter[] parameters)
        {
            DataTable dataTable = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(sqlCommand, conn);
                PrepareCommand(command, conn, null, CommandType.Text, sqlCommand, parameters);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                adapter.Fill(dataTable);
            }
            return dataTable;
        }

        #endregion

        #region 【执行存储过程】

        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="procedureName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public DataTable ExecuteProcedureCommand(string procedureName, params SqlParameter[] parameters)
        {
            DataTable dataTable = new DataTable();
            
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand();
                PrepareCommand(command, conn, null, CommandType.Text, procedureName, parameters);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                adapter.Fill(dataTable);
            }
            return dataTable;
        }

        #endregion

        #region 【执行多条SQL语句，实现数据库事务】

        /// <summary>
        /// 执行多条SQL语句，实现数据库事务
        /// </summary>
        /// <param name="sqlCommandList"></param>
        public void ExecuteSqlTran(List<string> sqlCommandList)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand
                {
                    Connection = conn
                };
                SqlTransaction transaction = conn.BeginTransaction();
                command.Transaction = transaction;

                try
                {
                    foreach (string str in sqlCommandList)
                    {
                        if (str.Trim() != "")
                        {
                            command.CommandText = str;
                            command.ExecuteNonQuery();
                        }
                    }
                    transaction.Commit();
                }
                catch(Exception ex)
                {
                    transaction.Rollback();
                    conn.Close();
                    throw ex;
                }
            }
        }
        #endregion      

        #region 【执行sql语句之前准备参数】
        /// <summary>
        /// 为SqlCommand之类对象准备SqlParameter参数
        /// </summary>
        /// <param name="cmd">SqlCommand object</param>
        /// <param name="conn">SqlConnection object</param>
        /// <param name="trans">SqlTransaction object</param>
        /// <param name="cmdType">Cmd type e.g. stored procedure or text</param>
        /// <param name="cmdText">Command text, e.g. Select * from Products</param>
        /// <param name="cmdParms">SqlParameters to use in the command</param>
        private void PrepareCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, CommandType cmdType, string cmdText, SqlParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();

            cmd.Connection = conn;
            cmd.CommandText = cmdText;

            if (trans != null)
                cmd.Transaction = trans;

            cmd.CommandType = cmdType;

            if (cmdParms != null)
            {
                foreach (SqlParameter parm in cmdParms)
                    cmd.Parameters.Add(parm);
            }
        }
        #endregion
    }
}
