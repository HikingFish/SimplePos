using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Domain.Common.ResultPattern;
using System;

namespace SimplePos.Application.Categories.Commands.DeleteCategory;

public record DeleteCategoryCommand(Guid CategoryId, Guid CompanyId) : ICommand<Result>;
