using BlazorApp.WASM.Shared.Entities;
using System.Linq.Expressions;

namespace BlazorApp.WASM.Shared.Contracts
{
  public interface IPersistenceService
  {
    Task<IEnumerable<T>> GetAllAsync<T>() where T : BaseItem;
    Task<IEnumerable<T>> GetAsync<T>(Expression<Func<T, bool>> whereExpression) where T : BaseItem;
    Task<int> InsertAsync<T>(T entity) where T : BaseItem;
    Task UpdateAsync<T>(T entity) where T : BaseItem;
    Task DeleteAsync<T>(T entity) where T : BaseItem;
    Task InitAsync();
  }
}
