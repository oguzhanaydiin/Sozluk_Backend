using AutoMapper;
using Common.Models.Queries;
using Common.Models.RequestModels;
using Domain.Models;

namespace Application.Mapping;
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, LoginUserViewModel>()
            .ReverseMap();

        CreateMap<CreateUserCommand, User>();

        CreateMap<UpdateUserCommand, User>();

        CreateMap<UserDetailViewModel, User>()
            .ReverseMap();

        CreateMap<CreateEntryCommand, Entry>()
            .ReverseMap();

        CreateMap<Entry, GetEntriesViewModel>()
            .ForMember(x => x.CommentCount, y => y.MapFrom(z => z.EntryComments.Count));


        CreateMap<CreateEntryCommentCommand, EntryComment>()
            .ReverseMap();

        
    }
}
