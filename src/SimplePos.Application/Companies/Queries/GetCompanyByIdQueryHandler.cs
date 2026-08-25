using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Domain.Common.ResultPattern;
using SimplePos.Domain.Companies;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimplePos.Application.Companies.Queries
{
    public class GetCompanyByIdQueryHandler : IQueryHandler<GetCompanyByIdQuery, Result<CompanyResponse>>
    {
        private readonly ICompanyRepository _companyRepository;

        public GetCompanyByIdQueryHandler(ICompanyRepository companyRepository)
        {
            _companyRepository = companyRepository;
        }

        public async Task<Result<CompanyResponse>> HandleAsync(GetCompanyByIdQuery query, CancellationToken cancellationToken)
        {
            var companyResult = await _companyRepository.GetCompanyByIdAsync(query.companyId);

            if(companyResult == null)
            {
                return Result<CompanyResponse>.Failure(CompanyError.CompanyNotFound);
            }

            CompanyResponse companyResponse = new CompanyResponse(
                companyResult.CompanyId, 
                companyResult.Name, 
                companyResult.PhoneNumber, 
                companyResult.CompanyAddress.Street, 
                companyResult.CompanyAddress.City, 
                companyResult.CompanyAddress.State, 
                companyResult.CompanyAddress.PostalCode,
                companyResult.CompanyAddress.Country,
                companyResult.Email.Value);

            return Result<CompanyResponse>.Success(companyResponse);
        }
    }
}
