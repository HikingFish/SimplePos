using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Domain.Common;
using SimplePos.Domain.Common.ResultPattern;
using SimplePos.Domain.Companies;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SimplePos.Application.Companies.Commands.CreateCompany;

public class CreateCompanyCommandHandler : ICommandHandler<CreateCompanyCommand, Result>
{
    private readonly IDomainEventDispatcher _domainEventDispatcher;

    public CreateCompanyCommandHandler(IDomainEventDispatcher domainEventDispatcher)
    {
        _domainEventDispatcher = domainEventDispatcher;
    }

    public async Task<Result> HandleAsync(CreateCompanyCommand command, CancellationToken cancellationToken)
    {
        Result<Address> resultAddress = Address.Create(command.Street, command.City, command.State, command.PostalCode, command.Country);

        if (resultAddress.IsFailure)
            return resultAddress;

        if (resultAddress.Data is null)
            return Result.Failure(AddressError.AddressNotFound);

        Result<EmailAddress> resultEmail = EmailAddress.Create(command.Email);

        if (resultEmail.IsFailure)
            return resultEmail;

        if (resultEmail.Data is null)
            return Result.Failure(EmailAddressError.EmailAddressNotFound);

        Result<Company> companyResult = Company.Create(command.Name, resultAddress.Data, command.PhoneNumber, resultEmail.Data);

        var DomainEvents = companyResult.Data.DomainEvents.ToList();
        companyResult.Data.ClearDomainEvents();

        foreach (var domainEvent in DomainEvents)
        {
            await _domainEventDispatcher.PublishAsync<domainEvent>(domainEvent, cancellationToken);
        }

        Debug.WriteLine($"Company created with ID: {companyResult.Data.CompanyId}");

        return Result.Success();
    }
}
