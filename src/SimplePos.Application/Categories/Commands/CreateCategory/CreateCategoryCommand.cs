using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Domain.Common.ResultPattern;
using System;

namespace SimplePos.Application.Categories.Commands.CreateCategory;

public record CreateCategoryCommand(Guid CompanyId, string Name) : ICommand<Result<Guid>>;
