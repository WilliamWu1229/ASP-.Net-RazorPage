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

		//�NdataTabel��Ƽg�J_salesRepDb.Factories�æs��
		//�bOnget���N�a��U���_�I�ì�_salesRepDb.Factories���A���s�b��Ʈw�즳�����
		/*Question�G�]���H�e���}�o�g��A�q�`���˨ϥε{���������u��h���s�ɡA�ҥH�Q�ݧڦp�G�n�z�LEntityFramework
					�x�s��ơA����k���d�ߥX��Ʈw�즳����ƶ�(�į���D)�A�٬O����L��k
					�ڥثe���B�z�覡�G�bSalesRepContext�ɮסA�[�J-->entity.HasQueryFilter(e => e.UpdateDate.Date == ��Ѥ��);*/
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
