using Common.Models.Queries;
using MediatR;

namespace Application.Features.Queries.GetEntries.GetEntryDetail;
public class GetEntryDetailQuery : IRequest<GetEntryDetailViewModel>
{
    public Guid EntryId { get; set; }

    public Guid? UserId { get; set; }

    public GetEntryDetailQuery(Guid entryId, Guid? userId)
    {
        EntryId = entryId;
        UserId = userId;
    }
}
