using Common;
using Common.Events.Entry;
using Common.Infrastructure;
using Common.Models.RequestModels;
using MediatR;

namespace Application.Features.Commands.Entry.CreateVote;
public class CreateEntryVoteCommandHandler : IRequestHandler<CreateEntryVoteCommand, bool>
{
    public async Task<bool> Handle(CreateEntryVoteCommand request, CancellationToken cancellationToken)
    {
        QueueFactory.SendMessageToExchange(exchangeName: SozlukConstants.VoteExchangeName,
            exchangeType: SozlukConstants.DefaultExchangeType,
            queueName: SozlukConstants.CreateEntryVoteQueueName,
            obj: new CreateEntryVoteEvent()
            {
                EntryId = request.EntryId,
                CreatedBy = request.CreatedBy,
                VoteType = request.VoteType
            });

        return await Task.FromResult(true);
    }
}
