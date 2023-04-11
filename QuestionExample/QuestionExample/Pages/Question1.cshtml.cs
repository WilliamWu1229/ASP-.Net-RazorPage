using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using QuestionExample.Data;
using QuestionExample.Function.IFunctions;
using System.Data;

namespace QuestionExample.Pages
{
    public class Question1Model : PageModel
    {
		private readonly SalesRepContext _salesRepDb;
		private readonly ISqlExcute _sqlExcute;
		//Oracle DB command
		private readonly string factoryCmd = "select comp_no,comp_name,'TW' as country,sysdate as update_date from COMPANY" +
											" where trunc(build_date) = trunc(sysdate) or trunc(modify_date) = trunc(sysdate)";
		private readonly string factoryCount = "select count(1) from FACTORY where FACT_NO = @fact_no";

		public Question1Model(SalesRepContext salesRepDb, ISqlExcute sqlExcute)
		{
			_salesRepDb = salesRepDb;
			_sqlExcute = sqlExcute;
		}

		//將dataTabel資料寫入_salesRepDb.Factories並存檔
		//在Onget任意地方下中斷點並看_salesRepDb.Factories內，有存在資料庫原有的資料
		/*Question：因為以前的開發經驗，通常推薦使用程式內部的工具去做存檔，所以想問我如果要透過EntityFramework
					儲存資料，有辦法不查詢出資料庫原有的資料嗎(效能問題)，還是有其他方法
					我目前的處理方式：在SalesRepContext檔案，加入-->entity.HasQueryFilter(e => e.UpdateDate.Date == 當天日期);*/
		public void OnGet()
        {
			DataTable dataTable = _sqlExcute.OracleExecuteQuery(factoryCmd);

			int columnsCount = dataTable.Columns.Count;
			int rowsCount = dataTable.Rows.Count;

			if (dataTable == null || columnsCount == 0)
			{
				//PASS
			}
			else if (rowsCount == 0)
			{
				//PASS
			}
			else
			{
				try
				{
					foreach (DataRow row in dataTable.Rows)
					{
						Factory factory = new Factory();

						string factNo = Convert.ToString(row["comp_no"]);
						string factName = Convert.ToString(row["comp_name"]);
						string country = Convert.ToString(row["country"]);
						DateTime updateDate = Convert.ToDateTime(row["update_date"]);

						SqlParameter[] parameters = new SqlParameter[]
						{
							new SqlParameter("@fact_no", factNo),
						};

						int count = (int)_sqlExcute.SqlExecuteScalar(factoryCount, parameters);

						if (count > 0)
						{
							factory.FactNo = factNo;

							_salesRepDb.Factories.Attach(factory);

							factory.FactName = factName;
							factory.Country = country;
							factory.UpdateDate = updateDate;
						}
						else
						{
							factory.FactNo = factNo;
							factory.FactName = factName;
							factory.Country = country;
							factory.UpdateDate = updateDate;

							_salesRepDb.Factories.Add(factory);
						}
					}

					_salesRepDb.SaveChanges();
				}
				catch (Exception ex)
				{
					//PASS
				}
			}

		}
    }
}
