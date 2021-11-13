namespace BlazorApp.Services.Services
{
  public class PrimeService
  {
    public List<int> FindPrimeNumbersUpTo(int end)
    {
      var start = 0;

      return
        Enumerable.Range(start, end - start)
            .Where(IsPrime)
            .Select(number => number)
            .ToList();
    }
    private bool IsPrime(int number)
    {
      bool CalculatePrime(int value)
      {
        var possibleFactors = Math.Sqrt(number);
        
        for (var factor = 2; factor <= possibleFactors; factor++)
        {
          if (value % factor == 0)
          {
            return false;
          }
        }

        return true;
      }

      return number > 1 && CalculatePrime(number);
    }
  }
}
