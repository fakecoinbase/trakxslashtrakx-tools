namespace Trakx.Coinbase.Custody.Client.Models
{
    public class PaginationOptions
    {
        public static readonly PaginationOptions Default = new PaginationOptions();

        /// <summary>
        /// Optional custom pagination options. Allows to start searching from a custom point
        /// in time, or to change default number of results returned per page.
        /// </summary>
        /// <param name="before">Request page before (newer than) this pagination id.</param>
        /// <param name="after">Request page after (older than) this pagination id.</param>
        /// <param name="pageSize">Number of results per request. Maximum 100. Default 25.</param>
        public PaginationOptions(string? before = default, string? after = default, int? pageSize = default)
        {
            Before = before;
            After = after;
            PageSize = pageSize ?? 25;
        }

        public string? Before { get; }
        public string? After { get; }
        public int PageSize { get; }
    }
}