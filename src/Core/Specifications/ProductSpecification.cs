using Core.Entities;

namespace Core.Specifications;

public class ProductSpecification : BaseSpecification<Product>
{
 public ProductSpecification(ProductSpecPramas specPramas) : base(x =>
    (string.IsNullOrEmpty(specPramas.Search) || x.Name.ToLower().Contains(specPramas.Search)) &&
    (specPramas.Brands.Count == 0 || specPramas.Brands.Contains(x.Brand)) &&
    (specPramas.Types.Count == 0 ||  specPramas.Types.Contains(x.Type)))
 {
    ApplyPaging(specPramas.PageSize * (specPramas.PageIndex - 1), specPramas.PageSize);

    switch (specPramas.Sort)
    {
        case "priceAsc":
            AddOrderBy(p => p.Price);
            break;
        case "priceDesc":
            AddOrderByDescending(p => p.Price);
            break;
        default:
            AddOrderBy(p => p.Name);
            break;
    }
    
 }

    
    
}