namespace DirectPayout.Early.Services;

public class LogSettleService
{
    private readonly IDynamicRepository repository;

    public LogSettleService(IDynamicRepository repository)
    {
        this.repository = repository;
    }

    public async Task WriteLog()
    {
    }

    public async Task WriteLogResettle()
    {

    }

    public async Task WriteLogErrorSettle()
    {

    }
}
