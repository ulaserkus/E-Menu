using E_Menu.Internal.Commands.SaveImagesInLocal;
using MediatR;
using Quartz;

namespace E_Menu.JobScheduler.Jobs;

public class ImageSyncJob(IMediator mediator) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        await mediator.Send(new SaveImagesInLocalCommand());
    }
}
