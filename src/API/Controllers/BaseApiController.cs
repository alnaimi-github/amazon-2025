namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BaseApiController : ControllerBase
{
    protected async Task<IActionResult> CreatePagedResponseAsync<T>(IGenericRepository<T> repo, ISpecification<T> spec, 
    int pageIndex,int pageSize, CancellationToken cancellationToken) where T : BaseEntity
    {
        var items = await repo.ListSpecificationAsync(spec, cancellationToken);
        var count = await repo.CountAsync(spec, cancellationToken);
        var pagination = new Pagination<T>(
            pageIndex,
            pageSize,
            count,
            items);

        return Ok(pagination);
    }
}