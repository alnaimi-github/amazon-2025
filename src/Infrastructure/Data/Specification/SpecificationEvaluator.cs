namespace Infrastructure.Data.Specifications;

public class SpecificationEvaluator<T> where T : BaseEntity
{
    public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecification<T> spec)
    {
        var query = inputQuery.Select(x => x);


     query = spec.Criteria is not null ? query.Where(spec.Criteria) : query;
     query = spec.OrderBy is not null ? query.OrderBy(spec.OrderBy) : query;
     query = spec.OrderByDescending is not null ? query.OrderByDescending(spec.OrderByDescending) : query;
     query = spec.IsDistinct is true ? query.Distinct() : query;

        return query;
    }

      public static IQueryable<TResult> GetQuery<TSpec,TResult>(IQueryable<T> inputQuery, 
      ISpecification<T,TResult> spec)
    {
        var query = inputQuery;


     query = spec.Criteria is not null ? query.Where(spec.Criteria) : query;
     query = spec.OrderBy is not null ? query.OrderBy(spec.OrderBy) : query;
     query = spec.OrderByDescending is not null ? query.OrderByDescending(spec.OrderByDescending) : query;

       var  selectQuery = query as IQueryable<TResult>;

         if (spec.Select is not null)
         {
             selectQuery = query.Select(spec.Select);
         }

         selectQuery = spec.IsDistinct is true ? selectQuery?.Distinct() : selectQuery;

        return selectQuery ?? query.Cast<TResult>();
    }
}