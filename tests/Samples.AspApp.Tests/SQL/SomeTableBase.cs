namespace Samples.AspApp.Tests.SQL
{
   public class SomeTableBase
   {
      public required int SomeBaseInt { get; set; }

      public float SomeBaseFloat { get; set; }

      public required Guid SomeGuid { get; set; }

      public required DateOnly SomeDateOnly { get; set; }

      public required TimeOnly SomeTimeOnly { get; set; }
   }
}
