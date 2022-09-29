using Common.Models.Page;
using Common.Models.Queries;
using MediatR;

namespace Application.Features.Queries.GetEntries.GetMainPageEntries;
public class GetMainPageEntriesQuery : BasePagedQuery, IRequest<PagedViewModel<GetEntryDetailViewModel>>
{
    public GetMainPageEntriesQuery(Guid? userId, int page, int pageSize) : base(page, pageSize)
    {
        UserId = userId;
    }

    public Guid? UserId { get; set; }
}
