namespace AutoPayout.Core.Models;

public class Result
{
    public int ErrCode { get; set; }

    public string ErrMsg { get; set; }

    public Result()
    {

    }

    public Result(int errCode, string errMsg)
    {
        ErrCode = errCode;
        ErrMsg = errMsg;
    }
}