using Application.Interfaces.Repositories;
using Common.Infrastructure.Extensions;
using Common.Models.Page;
using Common.Models.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Queries.GetEntries.GetMyEntries;

public class GetUserEntriesQueryHandler : IRequestHandler<GetUserEntriesQuery, PagedViewModel<GetUserEntriesDetailViewModel>>
{
    private readonly IEntryRepository entryRepository;

    public GetUserEntriesQueryHandler(IEntryRepository entryRepository)
    {
        this.entryRepository = entryRepository;
    }

    public async Task<PagedViewModel<GetUserEntriesDetailViewModel>> Handle(GetUserEntriesQuery request, CancellationToken cancellationToken)
    {
        var query = entryRepository.AsQueryable();

        if (request.UserId != null && request.UserId.HasValue && request.UserId != Guid.Empty)
        {
            query = query.Where(i => i.CreatedById == request.UserId);
        }
        else if (!string.IsNullOrEmpty(request.UserName))
        {
            query = query.Where(i => i.CreatedBy.UserName == request.UserName);
        }
        else 
            return null;

        query = query.Include(i => i.EntryFavorites)
                     .Include(i => i.CreatedBy);

        var list = query.Select(i => new GetUserEntriesDetailViewModel()
        {
            Id = i.Id,
            Subject = i.Subject,
            Content = i.Content,
            IsFavorited = false,
            FavoritedCount = i.EntryFavorites.Count,
            CreatedDate = i.CreateDate,
            CreatedByUserName = i.CreatedBy.UserName
        });

        var entries = await list.GetPaged(request.Page, request.PageSize);

        return entries;
    }
}
