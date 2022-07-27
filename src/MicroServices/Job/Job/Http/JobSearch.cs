namespace Job.Http;

public record JobSearchModel
{
    public bool IsCompleted { get; set; } = false;

    /// <summary>
    /// 条件に一致する要素リストから先頭のデータを省く個数
    /// </summary>
    public int SkipCount { get; set; }

    /// <summary>
    /// 条件に一致する要素リストから取得する最大要素数
    /// </summary>
    public int TakeCount { get; set; }
}
