using Microsoft.Data.SqlClient;
using Oracle.ManagedDataAccess.Client;
using QuestionExample.Data;
using QuestionExample.Function.IFunctions;
using System.Data;

namespace QuestionExample.Function
{
	public class SqlExcute : ISqlExcute
	{
		private OracleConnection? _oracleConnection;
		private SqlConnection? _sqlConnection;
		private string? _oracleConnectionString;
		private string? _sqlConnectionString;

		public SqlExcute()
		{
			var builder = new ConfigurationBuilder()
				  .SetBasePath(Directory.GetCurrentDirectory())
				  .AddJsonFile("appsettings.json");

			var config = builder.Build();

			_sqlConnectionString = config.GetConnectionString("MsSqlConnection");
			_oracleConnectionString = config.GetConnectionString("OracleSqlConnection");
		}

		private OracleConnection GetOracleConnection()
		{
			if (_oracleConnection == null)
			{
				_oracleConnection = new OracleConnection(_oracleConnectionString);

				if (_oracleConnection.State != ConnectionState.Open)
				{
					_oracleConnection.Open();
				}
			}
			else if (_oracleConnection.State != ConnectionState.Open)
			{
				_oracleConnection.Open();
			}

			return _oracleConnection;
		}

		private SqlConnection GetSqlConnection()
		{
			if (_sqlConnection == null)
			{
				_sqlConnection = new SqlConnection(_sqlConnectionString);

				if (_sqlConnection.State != ConnectionState.Open)
				{
					_sqlConnection.Open();
				}
			}
			else if (_sqlConnection.State != ConnectionState.Open)
			{
				_sqlConnection.Open();
			}

			return _sqlConnection;
		}

		public DataTable OracleExecuteQuery(string query, OracleParameter[]? parameters = null)
		{
			DataTable dataTable = new DataTable();

			using (OracleCommand command = new OracleCommand(query, GetOracleConnection()))
			{
				if (parameters != null)
				{
					command.Parameters.AddRange(parameters);
				}

				using (OracleDataAdapter adapter = new OracleDataAdapter(command))
				{
					adapter.Fill(dataTable);
				}
			}

			return dataTable;
		}

		public DataTable SqlExecuteQuery(string query, SqlParameter[]? parameters = null)
		{
			DataTable dataTable = new DataTable();

			using (SqlCommand command = new SqlCommand(query, GetSqlConnection()))
			{
				if (parameters != null)
				{
					command.Parameters.AddRange(parameters);
				}

				using (SqlDataAdapter adapter = new SqlDataAdapter(command))
				{
					adapter.Fill(dataTable);
				}
			}

			return dataTable;
		}

		public int OracleExecuteNonQuery(string query, OracleParameter[]? parameters = null)
		{
			using (OracleCommand command = new OracleCommand(query, GetOracleConnection()))
			{
				if (parameters != null)
				{
					command.Parameters.AddRange(parameters);
				}

				return command.ExecuteNonQuery();
			}
		}

		public int SqlExecuteNonQuery(string query, SqlParameter[]? parameters = null)
		{
			using (SqlCommand command = new SqlCommand(query, GetSqlConnection()))
			{
				if (parameters != null)
				{
					command.Parameters.AddRange(parameters);
				}

				return command.ExecuteNonQuery();
			}
		}

		public dynamic OracleExecuteScalar(string query, OracleParameter[]? parameters = null)
		{
			using (OracleCommand command = new OracleCommand(query, GetOracleConnection()))
			{
				if (parameters != null)
				{
					command.Parameters.AddRange(parameters);
				}

				return command.ExecuteScalar();
			}
		}

		public dynamic SqlExecuteScalar(string query, SqlParameter[]? parameters = null)
		{
			using (SqlCommand command = new SqlCommand(query, GetSqlConnection()))
			{
				if (parameters != null)
				{
					command.Parameters.AddRange(parameters);
				}

				return command.ExecuteScalar();
			}
		}
	}
}
