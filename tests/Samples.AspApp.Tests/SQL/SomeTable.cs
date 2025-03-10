namespace Samples.AspApp.Tests.SQL
{
  public class SomeTable
  {
    public int Id { get; set; }

    public required int SomeInt { get; set; }

    public string? SomeString { get; set; }

    public string? SomeNullableString { get; set; }

    public bool? SomeBool { get; set; }

    public double? SomeDouble { get; set; }

    public decimal? SomeDecimal { get; set; }
  }
}
