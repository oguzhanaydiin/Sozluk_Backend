﻿using Common.Models.Queries;
using MediatR;

namespace Application.Features.Queries.GetEntries;
public class GetEntriesQuery: IRequest<List<GetEntriesViewModel>>
{
    public bool TodaysEntries { get; set; }

    public int Count { get; set; } = 100;
}