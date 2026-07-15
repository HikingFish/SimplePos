using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Domain.Common;
using SimplePos.Domain.Common.ResultPattern;
using SimplePos.Domain.Companies;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimplePos.Application.Companies.Commands.CreateCompany;

public class CreateCompanyCommandHandler : ICommandHandler<CreateCompanyCommand, Result>
{
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

        

        return Result.Success();
    }
}
