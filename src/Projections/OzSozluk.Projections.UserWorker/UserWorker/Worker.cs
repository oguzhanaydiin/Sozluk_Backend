using Common;
using Common.Events.User;
using Common.Infrastructure;
using Projections.UserWorker.Services;

namespace Projections.UserWorker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly UserService userService;
    private readonly EmailService emailService;

    public Worker(ILogger<Worker> logger, UserService userService, EmailService emailService)
    {
        _logger = logger;
        this.userService = userService;
        this.emailService = emailService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        QueueFactory.CreateBasicConsumer()
            .EnsureExchange(SozlukConstants.UserExchangeName)
            .EnsureQueue(SozlukConstants.UserEmailChangedQueueName, SozlukConstants.UserExchangeName)
            .Receive<UserEmailChangedEvent>(user =>
            {
                // DB Insert 

                var confirmationId = userService.CreateEmailConfirmation(user).GetAwaiter().GetResult();

                // Generate Link

                var link = emailService.GenerateConfirmationLink(confirmationId);

                // Send Email

                emailService.SendEmail(user.NewEmailAddress, link).GetAwaiter().GetResult();
            })
            .StartConsuming(SozlukConstants.UserEmailChangedQueueName);
    }
}
