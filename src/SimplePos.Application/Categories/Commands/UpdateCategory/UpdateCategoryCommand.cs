using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Domain.Common.ResultPattern;
using System;

namespace SimplePos.Application.Categories.Commands.UpdateCategory;

public record UpdateCategoryCommand(Guid CategoryId, Guid CompanyId, string Name) : ICommand<Result>;
