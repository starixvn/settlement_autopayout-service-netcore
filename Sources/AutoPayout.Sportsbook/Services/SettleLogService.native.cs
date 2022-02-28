using System.Threading.Tasks;
using Fanex.Data.Repository;
using AutoPayout.Sportsbook.Criteria;
using AutoPayout.Sportsbook.Models;
using AutoPayout.Sportsbook.Exceptions;

namespace AutoPayout.Sportsbook.Services
{
    public interface SettleLogService
    {
        Task Log(Match match, string errorMessage);

        Task Log(Match match, AutoPayoutException exception);
    }

    public class SettleLogServiceImp : SettleLogService
    {
        private readonly IDynamicRepository repository;

        public SettleLogServiceImp(IDynamicRepository repository)
        {
            this.repository = repository;
        }

        public async Task Log(Match match, string errorMessage)
        {
            var criteria = new SettleLogCriteria(match, errorMessage);
            await repository.ExecuteAsync(criteria);
        }

        public async Task Log(Match match, AutoPayoutException exception)
        {
            var criteria = new SettleLogCriteria(match, exception);
            await repository.ExecuteAsync(criteria);
        }
    }
}