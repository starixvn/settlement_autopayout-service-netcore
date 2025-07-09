using AutoPayout.Core.Models;
using DirectPayout.Early.Criteria;
using DirectPayout.Early.Models;
using DirectPayout.Early.Services;
using Fanex.Data.Repository;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace DirectPayout.Early.Tests.Services;

public class ResultDetailServiceTests
{
    private readonly IDynamicRepository _repository;
    private readonly IOptions<AppSettingsKey> _appSettings;
    private readonly ResultDetailServiceImp _service;

    public ResultDetailServiceTests()
    {
        _repository = Substitute.For<IDynamicRepository>();
        _appSettings = Options.Create(new AppSettingsKey
        {
            RetryProcessScore = 1000 // 1 second in ms
        });
        _service = new ResultDetailServiceImp(_repository, _appSettings);
    }

    [Fact]
    public async Task GetResultDetailsAsync_ShouldSetExpiredFalse_WhenWithinRetryProcessScore()
    {
        // Arrange  
        var now = DateTimeOffset.Now;
        var details = new List<ResultDetail>
        {
            new() { LastModifyDate = now.AddMilliseconds(-500).DateTime }
        };
        _repository.FetchAsync<ResultDetail>(Arg.Any<ScheduleSelCriteria>())
            .Returns(details);

        // Act  
        var result = await _service.GetResultDetailsAsync();

        // Assert  
        Assert.False(((List<ResultDetail>)result)[0].Expired);
    }

    [Fact]
    public async Task GetResultDetailsAsync_ShouldSetExpiredTrue_WhenExceedsRetryProcessScore()
    {
        // Arrange  
        var now = DateTimeOffset.Now;
        var details = new List<ResultDetail>
        {
            new ResultDetail { LastModifyDate = now.AddMilliseconds(-2000).DateTime }
        };
        _repository.FetchAsync<ResultDetail>(Arg.Any<ScheduleSelCriteria>())
            .Returns(details);

        // Act  
        var result = await _service.GetResultDetailsAsync();

        // Assert  
        Assert.True(((List<ResultDetail>)result)[0].Expired);
    }

    [Fact]
    public async Task GetResultDetailsAsync_ShouldHandleMultipleResults()
    {
        // Arrange  
        var now = DateTimeOffset.Now;
        var details = new List<ResultDetail>
        {
            new ResultDetail { LastModifyDate = now.AddMilliseconds(-500).DateTime },   // Not expired  
            new ResultDetail { LastModifyDate = now.AddMilliseconds(-2000).DateTime }   // Expired  
        };
        _repository.FetchAsync<ResultDetail>(Arg.Any<ScheduleSelCriteria>())
            .Returns(details);

        // Act  
        var result = await _service.GetResultDetailsAsync();
        var list = new List<ResultDetail>(result);

        // Assert  
        Assert.False(list[0].Expired);
        Assert.True(list[1].Expired);
    }
}
