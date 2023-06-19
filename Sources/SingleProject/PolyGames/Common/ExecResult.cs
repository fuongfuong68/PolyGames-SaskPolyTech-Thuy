
namespace PolyGames.Common
{
    public class ExecResult
    {
        public bool Status { get; set; }
        public string Message { get; set; }
    }

    public class ExecResult<T> : ExecResult
    {
        public T Data { get; set; }
    }
}