using BlazorApp.WASM.Shared.Contracts;
using BlazorApp.WASM.Shared.Entities;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System.Linq.Expressions;

namespace BlazorApp.GeneralUI
{
  public class IndexedDB : IPersistenceService
  {
    private IJSRuntime _jsRuntime;
    private JsonSerializerSettings _settings;

    public IndexedDB(IJSRuntime jsRuntime)
    {
      _jsRuntime = jsRuntime;
      _settings = new JsonSerializerSettings();
      _settings.ContractResolver = new SimplePropertyContractResolver();
    }

    public async Task DeleteAsync<T>(T entity) where T : BaseItem
    {
      var tableName = typeof(T).Name;
      await _jsRuntime.InvokeVoidAsync("organizeIndexedDB.deleteAsync", tableName, entity.Id);
    }

    public async Task<IEnumerable<T>> GetAllAsync<T>() where T : BaseItem
    {
      var tableName = typeof(T).Name;
      var result = await _jsRuntime.InvokeAsync<T[]>("organizeIndexedDB.getAllAsync", tableName).ConfigureAwait(false);

      return result;
    }

    public async Task<IEnumerable<T>> GetAsync<T>(Expression<Func<T, bool>> whereExpression) where T : BaseItem
    {
      var tableName = typeof(T).Name;
      var entities = await _jsRuntime.InvokeAsync<T[]>("organizeIndexedDB.getAllAsync", tableName);
      return entities.Where(whereExpression.Compile());
    }

    public async Task InitAsync()
    {
      await _jsRuntime.InvokeVoidAsync("organizeIndexedDB.initAsync");
    }

    public async Task<int> InsertAsync<T>(T entity) where T : BaseItem
    {
      var tableName = typeof(T).Name;
      var serializedEntity = SerializeAndRemoveArraysAndNavigationProperties(entity);

      var id = await _jsRuntime.InvokeAsync<int>("organizeIndexedDB.addAsync", tableName, serializedEntity);
      return id;
    }

    public async Task UpdateAsync<T>(T entity) where T : BaseItem
    {
      var tableName = typeof(T).Name;
      var serializedEntity = SerializeAndRemoveArraysAndNavigationProperties(entity);
      await _jsRuntime.InvokeVoidAsync("organizeIndexedDB.putAsync", tableName, serializedEntity, entity.Id);
    }

    private string SerializeAndRemoveArraysAndNavigationProperties<T>(T entity) where T : BaseItem
    {
      var result = JsonConvert.SerializeObject(entity, _settings);
      return result;
    }
  }
}
