using Microsoft.Data.SqlClient;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace QuestionExample.Function.IFunctions
{
	public interface ISqlExcute
	{
		DataTable OracleExecuteQuery(string query, OracleParameter[]? parameters = null);
		DataTable SqlExecuteQuery(string query, SqlParameter[]? parameters = null);
		int OracleExecuteNonQuery(string query, OracleParameter[]? parameters = null);
		int SqlExecuteNonQuery(string query, SqlParameter[]? parameters = null);
		dynamic OracleExecuteScalar(string query, OracleParameter[]? parameters = null);
		dynamic SqlExecuteScalar(string query, SqlParameter[]? parameters = null);
	}
}
