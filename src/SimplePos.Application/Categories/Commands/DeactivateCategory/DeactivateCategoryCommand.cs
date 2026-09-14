using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Domain.Common.ResultPattern;
using System;

namespace SimplePos.Application.Categories.Commands.DeactivateCategory;

public record DeactivateCategoryCommand(Guid CategoryId, Guid CompanyId) : ICommand<Result>;
