using System;
using System.Collections.Generic;

namespace QuestionExample.Data;

/// <summary>
/// 品牌
/// </summary>
public partial class Brand
{
    /// <summary>
    /// 品牌編號
    /// </summary>
    public string BrandNo { get; set; } = null!;

    /// <summary>
    /// 品牌說明
    /// </summary>
    public string? BrandDesc { get; set; }

    /// <summary>
    /// 品牌負責人
    /// </summary>
    public string? TeamLeader { get; set; }

    public string? Division { get; set; }
}
