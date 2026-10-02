using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Domain.Common.ResultPattern;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimplePos.Application.Outlets.Commands.CreateOutlet
{
    public class CreateOutletCommandHandler : ICommandHandler<CreateOutletCommand, Result<Guid>>
    {
        public Task<Result<Guid>> HandleAsync(CreateOutletCommand command, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
