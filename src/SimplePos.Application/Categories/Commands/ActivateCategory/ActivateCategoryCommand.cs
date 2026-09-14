using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Domain.Common.ResultPattern;
using System;

namespace SimplePos.Application.Categories.Commands.ActivateCategory;

public record ActivateCategoryCommand(Guid CategoryId, Guid CompanyId) : ICommand<Result>;
