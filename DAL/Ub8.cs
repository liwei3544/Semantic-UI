using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text; 

namespace Lottery.Gather.DAL
{
   public  class Ub8
    {
        /// <summary>
        /// 添加一个号码
        /// </summary>
        public static bool Add(string round,string number)
        {
            var builder = new StringBuilder();
            builder.Append("IF NOT EXISTS (SELECT * FROM tblNumberUB8 WHERE Round = @Round)  ");
            builder.Append("INSERT INTO tblNumberUB8(");
            builder.Append("Round,Date,Number)");
            builder.Append("VALUES (");
            builder.Append("@Round,@Date,@Number)");

            var lstParameters = new List<SqlParameter>
			{
					 new SqlParameter("@Round",SqlDbType.VarChar,32) {Value =  round},
					 new SqlParameter("@Date",SqlDbType.Int,4) {Value = DateTime.Now.ToDateInt()},
					 new SqlParameter("@Number",SqlDbType.VarChar,16) {Value =  number}, 
			};

            return SqlHelper.ExecuteSql(SqlHelper.dbConnString, CommandType.Text, builder.ToString(), lstParameters.ToArray()) > 0;
        }
    }
}
