using System;
using System.Collections.Generic;
using System.Text;

namespace SimplePos.Application.Abstractions;
public interface IUnitOfWork
{
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
