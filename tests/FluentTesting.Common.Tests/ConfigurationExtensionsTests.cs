using FluentAssertions;

using FluentTesting.Common.Extensions;

using Microsoft.Extensions.Configuration;

using Xunit;

namespace FluentTesting.Common.Tests;

public class ConfigurationExtensionsTests
{
	/// <summary>
	/// Binds class that contains Timespan property, timespan should be correct in resulting object. 
	/// </summary>
	[Fact]
	public void AddObjectWithTimespan_ShouldHaveCorrectData()
	{
		var cBuilder = new ConfigurationBuilder();
		cBuilder.AddObject("MySection", new ClassWithTimespan { Expiration = TimeSpan.FromDays(5) });
		var configurationRoot = cBuilder.Build();
		
		var myClass = configurationRoot.GetSection("MySection").Get<ClassWithTimespan>();
		
		myClass!.Expiration.Should().Be(TimeSpan.FromDays(5));
	}

	class ClassWithTimespan
	{
		public TimeSpan Expiration { get; set; }
	}
}