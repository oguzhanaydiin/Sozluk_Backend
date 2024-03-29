﻿using Common.Models.Page;
using Common.Models.Queries;
using MediatR;

namespace Application.Features.Queries.GetEntries.GetMyEntries;

public class GetUserEntriesQuery : BasePagedQuery, IRequest<PagedViewModel<GetUserEntriesDetailViewModel>>
{
    public Guid? UserId { get; set; }

    public string UserName { get; set; }

    public GetUserEntriesQuery(Guid? userId, string userName = null, int page = 1, int pageSize = 10) : base(page, pageSize)
    {
        UserId = userId;
        UserName = userName;
    }
}
