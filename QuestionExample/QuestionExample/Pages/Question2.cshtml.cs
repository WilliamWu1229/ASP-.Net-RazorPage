using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using QuestionExample.Data;
using QuestionExample.Function.IFunctions;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.DirectoryServices.ActiveDirectory;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace QuestionExample.Pages
{
    public class Question2Model : PageModel
    {
		private readonly SalesRepContext _salesRepDb;
		private readonly ISqlExcute _sqlExcute;
		public SelectList? BrandList { get; set; }

		[BindProperty(SupportsGet = true)]
		public QueryItem? Query { get; set; }
		public class QueryItem
		{
			[Display(Name = "資料年度：")]
			[RegularExpression("^(20[2-9][3-9]|[3-9][0-9]{3})$", ErrorMessage = "統計年度必須輸入2023~9999之間的數字。")]
			public int Year { get; set; }

			[Display(Name = "品牌：")]
			public string Brand { get; set; }
		}

		[BindProperty]
		public List<OrderSum>? OrdSumList { get; set; } = new List<OrderSum>();
		public class OrderSum
		{
			public string OrdDate { get; set; } = null!;
			public string EngMonth { get; set; } = null!;
			public string Brand { get; set; } = null!;
			public decimal? SumAmount { get; set; }
			public decimal? ForecastPrice { get; set; }
			public bool IsEdit { get; set; } = false;
		}

		public Question2Model(SalesRepContext salesRepDb, ISqlExcute sqlExcute)
		{
			_salesRepDb = salesRepDb;
			_sqlExcute = sqlExcute;
		}

		public void OnGet()
        {
			InsertBrandList();
		}

		public void OnGetQuery()
		{
			string[] engMonAry = new string[] { "Jan", "Feb", "Mar", "Apr", "May", "June", "July", "Aug", "Sep", "Oct", "Nov", "Dec" };

			for (int idx = 0; idx < 12; idx++)
			{
				OrderSum OrdSum = new OrderSum();

				OrdSum.OrdDate = Query.Year.ToString() + (idx + 1).ToString().PadLeft(2, '0');
				OrdSum.EngMonth = engMonAry[idx];
				OrdSum.Brand = Query.Brand;
				OrdSum.SumAmount = null;
				OrdSum.ForecastPrice = null;

				OrdSumList.Add(OrdSum);
			}

			string orderSql = "select SubString(ORDER_M.ORDER_DATE,1,6) as ODR_DATE," +
									"Round(SUM(ORDER_D.QTY * ORDER_D.PRICE * (CASE WHEN ORDER_M.DOR_NO in ('USD','NTD') THEN 1 ELSE rate_usd.RATE END) / (CASE ORDER_M.DOR_NO WHEN 'USD' THEN 1 ELSE rate_usd.RATE END)),2) as SUM_AMOUNT" +
							  " from ORDER_M" +
						 " left join EXCHANGE_RATE rate_usd on rate_usd.DOR_NO = 'USD' and ORDER_M.ORDER_DATE between rate_usd.CHG_DATES and rate_usd.CHG_DATEE" +
						 " left join EXCHANGE_RATE rate_ord on ORDER_M.DOR_NO = rate_ord.DOR_NO and ORDER_M.ORDER_DATE between rate_ord.CHG_DATES and rate_ord.CHG_DATEE," +
									"ORDER_D" +
							 " where ORDER_M.FACT_NO = ORDER_D.FACT_NO" +
							   " and ORDER_M.ORDER_NO = ORDER_D.ORDER_NO" +
							   " and ORDER_M.CANCEL_MK <> '1'" +
							   " and ORDER_D.CANCEL_MK <> '1'" +
							   " and ORDER_D.PAYMENT <> '1'" +
							   " and ORDER_D.UNIT = 'YD'" +
							   " and SubString(ORDER_M.ORDER_DATE,1,4) = @year" +
							   " and ORDER_D.BRAND_NO = @brand" +
						  " group by SubString(ORDER_M.ORDER_DATE,1,6)" +
						  " order by SubString(ORDER_M.ORDER_DATE,1,6)";

			SqlParameter[] parameters = new SqlParameter[]
			{
				new SqlParameter("@year", Query.Year.ToString()),
				new SqlParameter("@brand", Query.Brand)
			};

			DataTable orderData = _sqlExcute.SqlExecuteQuery(orderSql, parameters);

			int orderColumnsCount = orderData.Columns.Count;
			int orderRowsCount = orderData.Rows.Count;

			if (orderData == null || orderColumnsCount == 0 || orderRowsCount == 0)
			{

			}

			foreach (DataRow row in orderData.Rows)
			{
				OrderSum? findList = OrdSumList.Find(e => e.OrdDate == Convert.ToString(row["ODR_DATE"]));

				if (findList == null)
				{

				}
				else
				{
					findList.SumAmount = Math.Round(Convert.ToDecimal(row["SUM_AMOUNT"]),2);
				}
			}

			IEnumerable<ForecastOrder> forecastData = from forecast in _salesRepDb.ForecastOrders
													  where forecast.Ym.Substring(0, 4) == Query.Year.ToString()
													  orderby forecast.Ym
													  select forecast;

			foreach (ForecastOrder forecastOrder in forecastData)
			{
				OrderSum? findList = OrdSumList.Find(e => e.OrdDate == forecastOrder.Ym);

				if (findList == null)
				{

				}
				else
				{
					findList.SumAmount = forecastOrder.Price;
				}
			}

			InsertBrandList();
		}

		public void OnPost()
		{
			if (ModelState.IsValid)
			{
				foreach (var data in OrdSumList)
				{
					
				}
			}

			InsertBrandList();
		}

		public void InsertBrandList()
		{
			DataTable dataTable = _sqlExcute.SqlExecuteQuery("select brand_no from brand");

			List<SelectListItem> selectListItems = new List<SelectListItem>();

			foreach (DataRow row in dataTable.Rows)
			{
				selectListItems.Add(new SelectListItem
				{
					Value = Convert.ToString(row["BRAND_NO"]),
					Text = Convert.ToString(row["BRAND_NO"])
				});
			}

			BrandList = new SelectList(selectListItems, "Value", "Text", "NIKE");
		}
	}
}
