using Application.Interfaces.Repositories;
using AutoMapper;
using Common.Models.RequestModels;
using MediatR;

namespace Application.Features.Commands.Entry.Create;
public class CreateEntryCommandHandler : IRequestHandler<CreateEntryCommand, Guid>
{
    private readonly IEntryRepository entryRepository;
    private readonly IMapper mapper;

    public CreateEntryCommandHandler(IEntryRepository entryRepository, IMapper mapper)
    {
        this.entryRepository = entryRepository;
        this.mapper = mapper;
    }

    public async Task<Guid> Handle(CreateEntryCommand request, CancellationToken cancellationToken)
    {
        var dbEntry = mapper.Map<Domain.Models.Entry>(request);

        await entryRepository.AddAsync(dbEntry);

        return dbEntry.Id;
    }
}
