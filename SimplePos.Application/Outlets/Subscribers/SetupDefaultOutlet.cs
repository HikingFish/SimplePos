using System.Diagnostics;
using SimplePos.Domain.Common.DomainEvent;
using SimplePos.Domain.Common.ResultPattern;
using SimplePos.Domain.Companies.Events;

namespace SimplePos.Domain.Outlets.Subscribers;

public class SetupDefaultOutlet : IDomainEventHandler<CompanyCreatedDomainEvent>
{
    public Task Handle(CompanyCreatedDomainEvent @event, CancellationToken cancellationToken = default)
    {
        Result<Outlet> resultOutlet = Outlet.Create(@event.CompanyId, @event.Name, @event.Address, @event.PhoneNumber);
        Debug.WriteLine($"Default outlet created for company with ID: {@event.CompanyId}");
        return Task.CompletedTask;
    }
}