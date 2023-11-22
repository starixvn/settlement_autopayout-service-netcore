using AutoMapper;
using AutoPayout.Result.Models;
using AutoPayout.Sportsbook.Criteria;
using AutoPayout.Sportsbook.Models;
using Fanex.Settlement.ScaleOut.Request;

namespace AutoPayout.Sportsbook;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<MatchChecking, Goal>();
        CreateMap<MatchSettleCriteria, UpdateMatchInfoRequest>();
        CreateMap<XmlResource, ResourceItem>();
    }
}