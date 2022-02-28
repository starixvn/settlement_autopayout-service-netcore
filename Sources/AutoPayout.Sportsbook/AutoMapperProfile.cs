using AutoMapper;
using AutoPayout.Result.Models;
using AutoPayout.Sportsbook.Models;

namespace AutoPayout.Sportsbook
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<MatchChecking, Goal>();
        }
    }
}
