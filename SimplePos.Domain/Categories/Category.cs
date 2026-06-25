using SimplePos.Domain.Common;
using SimplePos.Domain.Companies;

namespace SimplePos.Domain.Category
{
    public class Category
    {
        public Guid CategoryId;
        public Guid CompanyId;
        public string Name;
        public bool IsActive;
        public bool SoftDeleted;

        private Category() { }

        private Category(Guid companyId, string name)
        {
            CategoryId = Guid.CreateVersion7();
            CompanyId = companyId;
            Name = name;
            IsActive = true;
            SoftDeleted = false;
        }

        public static Category CreateCategory(Guid companyId, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new DomainException("Category name cannot be empty.");
            }

            if (companyId == Guid.Empty)
            {
                throw new DomainException("companyId cannot be empty.");
            }

            return new Category(companyId, name);
        }
    }
}