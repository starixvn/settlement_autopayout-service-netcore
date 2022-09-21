using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Fanex.Data.Repository;
using Fanex.Settlement.ScaleOut.Criteria;
using Fanex.Settlement.ScaleOut.Models;
using Fanex.Settlement.ScaleOut.Request;
using Microsoft.Extensions.Configuration;

namespace Fanex.Settlement.ScaleOut.Services
{
    public interface ValidationMatchService
    {
        Task<ResultMessage> ValidateEventAndTicket(EventTicketRequest request);
    }

    public class ValidationMatchServiceImp : ValidationMatchService
    {
        private readonly IDynamicRepository repository;

        private readonly IConfiguration configuration;

        private readonly IMapper mapper;

        public ValidationMatchServiceImp(
            IDynamicRepository repository,
            IConfiguration configuration,
            IMapper mapper)
        {
            this.repository = repository;
            this.configuration = configuration;
            this.mapper = mapper;
        }

        public async Task<ResultMessage> ValidateEventAndTicket(EventTicketRequest request)
        {
            var eventCriteria = mapper.Map<EventTicketRequest, ValidateEventCriteria>(request);
            var validateEvent = await ValidateEvent(eventCriteria);

            if (validateEvent.ErrorCode > 0)
            {
                return validateEvent;
            }

            var ticketCriteria = mapper.Map<EventTicketRequest, ValidateTicketCriteria>(request);
            return await ValidateTickets(ticketCriteria);
        }

        private async Task<ResultMessage> ValidateTickets(ValidateTicketCriteria criteria)
        {
            var servers = configuration.GetSection("SiteDbServer").Get<List<string>>();
            foreach (var server in servers)
            {
                criteria.DbServer = server;
                await repository.ExecuteAsync(criteria);
                if (criteria.ErrCode > 0)
                {
                    return new ResultMessage
                    {
                        ErrorCode = criteria.ErrCode,
                        ErrorMessage = criteria.ErrMsg
                    };
                }
            }

            return new ResultMessage();
        }

        private async Task<ResultMessage> ValidateEvent(ValidateEventCriteria criteria)
        {
            await repository.ExecuteAsync(criteria);

            return new ResultMessage
            {
                ErrorCode = criteria.ErrCode,
                ErrorMessage = criteria.ErrMsg
            };
        }
    }
}
