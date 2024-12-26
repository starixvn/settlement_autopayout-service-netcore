using AutoPayout.Core.Models;
using DirectPayout.Early.Criteria;
using DirectPayout.Early.Models;
using Microsoft.Extensions.Options;

namespace DirectPayout.Early.Services;

public interface ResultDetailService
{
    Task<IEnumerable<ResultDetail>> GetResultDetailsAsync();
}

public class ResultDetailServiceImp(IDynamicRepository repository, IOptions<AppSettingsKey> appSettings)
    : ResultDetailService
{
    public async Task<IEnumerable<ResultDetail>> GetResultDetailsAsync()
    {
        var resultDetails = await repository.FetchAsync<ResultDetail>(new ScheduleSelCriteria());

        SetExpiredResultDetails(resultDetails);

        return resultDetails;
    }

    private void SetExpiredResultDetails(IEnumerable<ResultDetail> resultDetails)
    {
        foreach (var resultDetail in resultDetails)
        {
            var processTime = DateTimeOffset.Now - resultDetail.LastModifyDate;
            resultDetail.Expired = processTime.TotalMilliseconds > appSettings.Value.RetryProcessScore;
        }
    }
}