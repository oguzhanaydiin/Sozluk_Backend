﻿using Application.Interfaces.Repositories;
using Domain.Models;
using Infrastructure.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories;

public class EntryRepository : GenericRepository<Entry>, IEntryRepository
{
    public EntryRepository(BlazorSozlukContext dbContext) : base(dbContext)
    {
    }
}
