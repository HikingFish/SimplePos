using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Domain.Common;
using SimplePos.Domain.Common.ResultPattern;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimplePos.Application.Companies.Commands.CreateCompany;

public class CreateCompanyCommandHandler : ICommandHandler<CreateCompanyCommand, Result>
{
    public async Task<Result> HandleAsync(CreateCompanyCommand command, CancellationToken cancellationToken)
    {
        Result resultAddress = Address.Create(command.Street, command.City, command.State, command.PostalCode, command.Country);

        if (resultAddress.IsFailure)
        {
            return resultAddress;
        }

        // Continue with company creation logic here
        // ...

        return Result.Success();
    }
}
