using SimplePos.Application.Abstractions;
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
    private readonly ICompanyRepository _companyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCompanyCommandHandler(IDomainEventDispatcher domainEventDispatcher, ICompanyRepository companyRepository, IUnitOfWork unitOfWork)
    {
        _domainEventDispatcher = domainEventDispatcher;
        _companyRepository = companyRepository;
        _unitOfWork = unitOfWork;
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

        if (companyResult.IsFailure)
            return companyResult;

        if (companyResult.Data is null)
            return Result.Failure(CompanyError.CompanyNotFound);

        Debug.WriteLine($"Company created with ID: {companyResult.Data.CompanyId}");

        await _companyRepository.AddCompanyAsync(companyResult.Data);

        var DomainEvents = companyResult.Data.DomainEvents.ToList();
        companyResult.Data.ClearDomainEvents();

        foreach (var domainEvent in DomainEvents)
        {
            await _domainEventDispatcher.PublishAsync(domainEvent, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync();

        return Result.Success();
    }
}
