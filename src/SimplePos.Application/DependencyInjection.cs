using Microsoft.Extensions.DependencyInjection;
using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Domain.Common.DomainEvent;
using SimplePos.Domain.Companies.Events;
using SimplePos.Application.Outlets.Subscribers;
using SimplePos.Application.Companies.Commands.CreateCompany;
using SimplePos.Domain.Common.ResultPattern;
using SimplePos.Application.Abstractions;
using SimplePos.Application.Registrations.Commands.RegisterBusiness;
using SimplePos.Application.Auths.Commands.Login;
using SimplePos.Application.Auths.Queries;

using SimplePos.Application.Users.Queries.GetOutletAccess;
using SimplePos.Application.Users.Commands.GrantOutletAccess;
using SimplePos.Application.Users.Commands.SetOutletAccess;
using SimplePos.Application.Users.Commands.RevokeOutletAccess;
using SimplePos.Application.Sales.Commands.CreateSale;
using SimplePos.Application.Sales.Commands.VoidSale;
using SimplePos.Application.Sales.Commands.UnvoidSale;
using SimplePos.Application.Sales.Commands.CloseSale;
using SimplePos.Application.Sales.Commands.DeleteSale;
using SimplePos.Application.Sales.Commands.AddSaleItem;
using SimplePos.Application.Sales.Commands.UpdateSaleItem;
using SimplePos.Application.Sales.Commands.UpdateSaleItemQuantity;
using SimplePos.Application.Sales.Commands.VoidSaleItem;
using SimplePos.Application.Sales.Commands.UnvoidSaleItem;
using SimplePos.Application.Sales.Commands.RemoveSaleItem;
using SimplePos.Application.Sales.Commands.AddSalePayment;
using SimplePos.Application.Sales.Commands.RemoveSalePayment;
using SimplePos.Application.Sales.Queries.GetSaleById;
using SimplePos.Application.Sales.Queries.GetSales;
using SimplePos.Application.Sales.Queries.GetSalePayments;
using SimplePos.Application.Sales.Common;
using SimplePos.Application.Companies.Queries.GetCompanyById;
using SimplePos.Application.Products.Queries.ProductListByCompanyId;
using SimplePos.Application.Products.Queries.ProductByProductId;
using SimplePos.Application.Common;
using SimplePos.Application.Products.Commands.CreateProduct;
using SimplePos.Application.Products.Commands.DeleteProduct;
using SimplePos.Application.Categories.Commands.CreateCategory;
using SimplePos.Application.Categories.Commands.UpdateCategory;
using SimplePos.Application.Categories.Commands.DeleteCategory;
using SimplePos.Application.Categories.Commands.ActivateCategory;
using SimplePos.Application.Categories.Commands.DeactivateCategory;
using SimplePos.Application.Categories.Queries.GetCategoriesByCompany;
using SimplePos.Application.Categories.Queries.GetCategoryById;
using SimplePos.Application.Categories.Common;

namespace SimplePos.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICqrsDispatcher, CqrsDispatcher>();
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
        services.AddScoped<IDomainEventHandler<CompanyCreatedDomainEvent>, SetupDefaultOutlet>();

        //Company Commands and Queries
        services.AddScoped<ICommandHandler<CreateCompanyCommand, Result>, CreateCompanyCommandHandler>();

        services.AddScoped<IQueryHandler<GetCompanyByIdQuery, Result<CompanyResponse>>, GetCompanyByIdQueryHandler>();

        services.AddScoped<ICommandHandler<RegisterBusinessCommand, Result>, RegisterBusinessCommandHandler>();

        services.AddScoped<ICommandHandler<LoginCommand, Result<string>>, LoginCommandHandler>();
        services.AddScoped<IQueryHandler<GetCurrentUserQuery, Result<CurrentUserResponse>>, GetCurrentUserQueryHandler>();

        //Sale Commands and Queries
        services.AddScoped<ICommandHandler<CreateSaleCommand, Result<Guid>>, CreateCommandSaleCommandHandler>();
        services.AddScoped<ICommandHandler<VoidSaleCommand, Result>, VoidSaleCommandHandler>();
        services.AddScoped<ICommandHandler<UnvoidSaleCommand, Result>, UnvoidSaleCommandHandler>();
        services.AddScoped<ICommandHandler<CloseSaleCommand, Result>, CloseSaleCommandHandler>();
        services.AddScoped<ICommandHandler<DeleteSaleCommand, Result>, DeleteSaleCommandHandler>();
        services.AddScoped<ICommandHandler<AddSaleItemCommand, Result<Guid>>, AddSaleItemCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateSaleItemCommand, Result>, UpdateSaleItemCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateSaleItemQuantityCommand, Result>, UpdateSaleItemQuantityCommandHandler>();
        services.AddScoped<ICommandHandler<VoidSaleItemCommand, Result>, VoidSaleItemCommandHandler>();
        services.AddScoped<ICommandHandler<UnvoidSaleItemCommand, Result>, UnvoidSaleItemCommandHandler>();
        services.AddScoped<ICommandHandler<RemoveSaleItemCommand, Result>, RemoveSaleItemCommandHandler>();
        services.AddScoped<ICommandHandler<AddSalePaymentCommand, Result<Guid>>, AddSalePaymentCommandHandler>();
        services.AddScoped<ICommandHandler<RemoveSalePaymentCommand, Result>, RemoveSalePaymentCommandHandler>();
        services.AddScoped<IQueryHandler<GetSaleByIdQuery, Result<SaleResponse>>, GetSaleByIdQueryHandler>();
        services.AddScoped<IQueryHandler<GetSalesQuery, Result<List<SaleResponse>>>, GetSalesQueryHandler>();
        services.AddScoped<IQueryHandler<GetSalePaymentsQuery, Result<List<SalePaymentResponse>>>, GetSalePaymentsQueryHandler>();

        // User Outlet Access Commands and Queries
        services.AddScoped<IQueryHandler<GetUserOutletAccessesQuery, Result<List<UserOutletAccessResponse>>>, GetUserOutletAccessesQueryHandler>();
        services.AddScoped<ICommandHandler<GrantUserOutletAccessCommand, Result>, GrantUserOutletAccessCommandHandler>();
        services.AddScoped<ICommandHandler<SetUserOutletAccessesCommand, Result>, SetUserOutletAccessesCommandHandler>();
        services.AddScoped<ICommandHandler<RevokeUserOutletAccessCommand, Result>, RevokeUserOutletAccessCommandHandler>();

        //Product Commands and Queries
        services.AddScoped<IQueryHandler<GetProductByCompanyIdQuery, Result<PagedList<ProductListItemResponse>>>, GetProductByCompanyIdQueryHandler>();
        services.AddScoped<IQueryHandler<GetProductByProductIdQuery, Result<ProductResponse>>, GetProductByProductIdQueryHandler>();
        services.AddScoped<ICommandHandler<CreateProductCommand, Result<Guid>>, CreateProductCommandHandler>();
        services.AddScoped<ICommandHandler<DeleteProductCommand, Result>, DeleteProductCommandHandler>();

        //Category Commands and Queries
        services.AddScoped<ICommandHandler<CreateCategoryCommand, Result<Guid>>, CreateCategoryCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateCategoryCommand, Result>, UpdateCategoryCommandHandler>();
        services.AddScoped<ICommandHandler<DeleteCategoryCommand, Result>, DeleteCategoryCommandHandler>();
        services.AddScoped<ICommandHandler<ActivateCategoryCommand, Result>, ActivateCategoryCommandHandler>();
        services.AddScoped<ICommandHandler<DeactivateCategoryCommand, Result>, DeactivateCategoryCommandHandler>();
        services.AddScoped<IQueryHandler<GetCategoriesByCompanyQuery, Result<List<CategoryResponse>>>, GetCategoriesByCompanyQueryHandler>();
        services.AddScoped<IQueryHandler<GetCategoryByIdQuery, Result<CategoryResponse>>, GetCategoryByIdQueryHandler>();

        return services;
    }
}