using System;
using System.Collections.Generic;

namespace QuestionExample.Data;

/// <summary>
/// 預告訂單
/// </summary>
public partial class ForecastOrder
{
    /// <summary>
    /// 訂單年月
    /// </summary>
    public string Ym { get; set; } = null!;

    /// <summary>
    /// 品牌編號
    /// </summary>
    public string BrandNo { get; set; } = null!;

    /// <summary>
    /// 金額(USD)
    /// </summary>
    public decimal Price { get; set; }
}
