using System.Diagnostics;
using SimplePos.Domain.Common.DomainEvent;
using SimplePos.Domain.Common.ResultPattern;
using SimplePos.Domain.Companies.Events;
using SimplePos.Domain.Outlets;

namespace SimplePos.Application.Outlets.Subscribers;

public class SetupDefaultOutlet : IDomainEventHandler<CompanyCreatedDomainEvent>
{
    private readonly IOutletRepository _outletRepository;

    public SetupDefaultOutlet(IOutletRepository outletRepository)
    {
        _outletRepository = outletRepository;
    }

    public Task Handle(CompanyCreatedDomainEvent @event, CancellationToken cancellationToken = default)
    {
        Result<Outlet> resultOutlet = Outlet.Create(@event.CompanyId, @event.Name, @event.Address, @event.PhoneNumber);

        if (resultOutlet.IsFailure)
        {
            throw new InvalidOperationException($"Failed to create default outlet for company with ID: {@event.CompanyId}. Error: {resultOutlet.Error}");
        }

        if (resultOutlet.Data is null)
        {
            throw new InvalidOperationException($"Failed to create default outlet for company with ID: {@event.CompanyId}. Outlet data is null.");
        }

        _outletRepository.AddOutletAsync(resultOutlet.Data);
        Debug.WriteLine($"Default outlet created for company with ID: {@event.CompanyId}");
        return Task.CompletedTask;
    }
}