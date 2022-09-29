using Common.Models.Queries;
using MediatR;

namespace Application.Features.Queries.GetEntries.GetUserDetail;

public class GetUserDetailQuery : IRequest<UserDetailViewModel>
{
    public Guid UserId { get; set; }

    public string UserName { get; set; }

    public GetUserDetailQuery(Guid userId, string userName = null)
    {
        UserId = userId;
        UserName = userName;
    }
}
