using System;
using System.Collections.Generic;

namespace QuestionExample.Data;

/// <summary>
/// 工廠/公司
/// </summary>
public partial class Factory
{
    /// <summary>
    /// 工廠/公司編號
    /// </summary>
    public string FactNo { get; set; } = null!;

    /// <summary>
    /// 工廠/公司名稱
    /// </summary>
    public string FactName { get; set; } = null!;

    /// <summary>
    /// 國別(TW、CN、VN)
    /// </summary>
    public string Country { get; set; } = null!;

    public DateTime UpdateDate { get; set; }
}
