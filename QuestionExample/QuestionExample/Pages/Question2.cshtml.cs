using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using QuestionExample.Data;
using QuestionExample.Function.IFunctions;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.DirectoryServices.ActiveDirectory;

namespace QuestionExample.Pages
{
    public class Question2Model : PageModel
    {
		private readonly SalesRepContext _salesRepDb;
		private readonly ISqlExcute _sqlExcute;
		public SelectList? BrandList { get; set; }
		[BindProperty]
		public QueryItem? Query { get; set; }
		[BindProperty]
		public List<OrderSum>? OrdSumList { get; set; }

		public class QueryItem
		{
			[Display(Name = "資料年度：")]
			[RegularExpression("^(20[2-9][3-9]|[3-9][0-9]{3})$", ErrorMessage = "統計年度必須輸入2023~9999之間的數字。")]
			public int Year { get; set; }

			[Display(Name = "品牌：")]
			public string Brand { get; set; }
		}

		public class OrderSum
		{
			[Display(Name = "訂單年月：")]
			public string OrdDate { get; set; } = null!;

			[Display(Name = "英文月份：")]
			public string EngMonth { get; set; } = null!;

			[Display(Name = "品牌：")]
			public string Brand { get; set; } = null!;

			[Display(Name = "加總金額(USD)：")]
			public decimal? SumAmount { get; set; }

			[Display(Name = "預告訂單金額(USD)：")]
			public decimal? Forecase { get; set; }
		}

		public Question2Model(SalesRepContext salesRepDb, ISqlExcute sqlExcute)
		{
			_salesRepDb = salesRepDb;
			_sqlExcute = sqlExcute;
		}

		public void OnGet()
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

		public IActionResult OnPostQuery([FromForm] QueryItem query)
		{
			string[] engMonAry = new string[] { "Jan", "Feb", "Mar", "Apr", "May", "June", "July", "Aug", "Sep", "Oct", "Nov", "Dec" };

			for (int idx = 0; idx < 12; idx++)
			{
				OrderSum OrdSum = new OrderSum();

				OrdSum.OrdDate = query.Year.ToString() + (idx + 1).ToString().PadLeft(2, '0');
				OrdSum.EngMonth = engMonAry[idx];
				OrdSum.Brand = query.Brand;
				OrdSum.SumAmount = 0;
				OrdSum.Forecase = 0;

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
				new SqlParameter("@year", query.Year.ToString()),
				new SqlParameter("@brand", query.Brand)
			};

			DataTable orderData = _sqlExcute.SqlExecuteQuery(orderSql, parameters);

			int orderColumnsCount = orderData.Columns.Count;
			int orderRowsCount = orderData.Rows.Count;

			if (orderData == null || orderColumnsCount == 0 || orderRowsCount == 0)
			{
				return Content("Error!!沒有訂單合計資料");
			}

			foreach (DataRow row in orderData.Rows)
			{
				OrderSum? findList = OrdSumList.Find(e => e.OrdDate == Convert.ToString(row["ODR_DATE"]));

				if (findList == null)
				{
					return Content("Error!!沒有找到[訂單合計：" + Convert.ToString(row["ODR_DATE"]) + "]的對應資料列，異常");
				}
				else
				{
					findList.SumAmount = Convert.ToDecimal(row["SUM_AMOUNT"]);
				}
			}

			IEnumerable<ForecastOrder> forecastData = from forecast in _salesRepDb.ForecastOrders
													  where forecast.Ym.Substring(0, 4) == query.Year.ToString()
													  orderby forecast.Ym
													  select forecast;

			foreach (ForecastOrder forecastOrder in forecastData)
			{
				OrderSum? findList = OrdSumList.Find(e => e.OrdDate == forecastOrder.Ym);

				if (findList == null)
				{
					return Content("Error!!沒有找到[預告訂單：" + forecastOrder.Ym + "]的對應資料列，異常");
				}
				else
				{
					findList.SumAmount = forecastOrder.Price;
				}
			}

			return Content("OK!!");
		}
	}
}
