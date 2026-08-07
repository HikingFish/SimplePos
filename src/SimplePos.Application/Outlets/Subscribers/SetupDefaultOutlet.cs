using SimplePos.Domain.Common;
using SimplePos.Domain.Common.DomainEvent;
using SimplePos.Domain.Common.ResultPattern;
using SimplePos.Domain.Companies.Events;
using SimplePos.Domain.Outlets;
using System.Diagnostics;

namespace SimplePos.Application.Outlets.Subscribers;

public class SetupDefaultOutlet : IDomainEventHandler<CompanyCreatedDomainEvent>
{
    private readonly IOutletRepository _outletRepository;

    public SetupDefaultOutlet(IOutletRepository outletRepository)
    {
        _outletRepository = outletRepository;
    }

    public async Task Handle(CompanyCreatedDomainEvent @event, CancellationToken cancellationToken = default)
    {
        var outletAddressResult = Address.Create(
            @event.Address.Street,
            @event.Address.City,
            @event.Address.State,
            @event.Address.PostalCode,
            @event.Address.Country);

        if (outletAddressResult.IsFailure || outletAddressResult.Data is null)
        {
            throw new InvalidOperationException($"Failed to create address for default outlet: {outletAddressResult.Error}");
        }

        Result<Outlet> resultOutlet = Outlet.Create(@event.CompanyId, @event.Name, outletAddressResult.Data, @event.PhoneNumber);

        if (resultOutlet.IsFailure)
        {
            throw new InvalidOperationException($"Failed to create default outlet for company with ID: {@event.CompanyId}. Error: {resultOutlet.Error}");
        }

        if (resultOutlet.Data is null)
        {
            throw new InvalidOperationException($"Failed to create default outlet for company with ID: {@event.CompanyId}. Outlet data is null.");
        }

        _outletRepository.AddOutlet(resultOutlet.Data);
        Debug.WriteLine($"Default outlet created for company with ID: {@event.CompanyId}");
    }
}