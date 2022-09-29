using Application.Interfaces.Repositories;
using Domain.Models;
using Infrastructure.Persistence.Context;

namespace Infrastructure.Persistence.Repositories;

public class EntryCommentRepository : GenericRepository<EntryComment>, IEntryCommentRepository
{
    public EntryCommentRepository(BlazorSozlukContext dbContext) : base(dbContext)
    {
    }
}
