namespace BlazorApp.WASM.Shared.Contracts
{
  public interface IExcelReader<T> where T : class
  {
    Task<IList<T>> Read(Stream stream);
  }
}
