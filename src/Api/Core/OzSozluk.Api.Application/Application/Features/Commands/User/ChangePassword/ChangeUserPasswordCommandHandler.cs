using Application.Interfaces.Repositories;
using Common.Events.User;
using Common.Infrastructure;
using Common.Infrastructure.Exceptions;
using MediatR;

namespace Application.Features.Commands.User.ChangePassword;
public class ChangeUserPasswordCommandHandler : IRequestHandler<ChangeUserPasswordCommand, bool>
{
    private readonly IUserRepository userRepository;

    public ChangeUserPasswordCommandHandler(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    public async Task<bool> Handle(ChangeUserPasswordCommand request, CancellationToken cancellationToken)
    {
        if (!request.UserId.HasValue)
            throw new ArgumentNullException(nameof(request.UserId));

        var dbUser = await userRepository.GetByIdAsync(request.UserId.Value);

        if (dbUser is null)
            throw new DatabaseValidationException("User not found!");

        var encPass = PasswordEncryptor.Encrpt(request.OldPassword);
        if(dbUser.Password != encPass)
            throw new DatabaseValidationException("Old password wrong!");

        dbUser.Password = PasswordEncryptor.Encrpt(request.NewPassword);

        await userRepository.UpdateAsync(dbUser);

        return true;
    }
}
