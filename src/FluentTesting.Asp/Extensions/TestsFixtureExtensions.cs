using FluentAssertions.Json;
using Newtonsoft.Json.Linq;
using System.Runtime.CompilerServices;
using Testing.Common.Interfaces;

namespace Testing.Asp.Extensions
{
	public static class TestsFixtureExtensions
	{
		/// <summary>
		/// Assert json response by file name. Json file must lay in same location as calling test
		/// </summary>
		/// <param name="fixture">Tests fixture</param>
		/// <param name="message">http response message</param>
		/// <param name="jsonFileName">name of json file within same location as test class</param>
		/// <param name="path">automagically filled path by attribute</param>
		public static async Task AssertJsonResponseAsync(this ITestFixture fixture, HttpResponseMessage message, string jsonFileName, [CallerFilePath] string? path = null)
		{
			var data = JToken.Parse(await message.Content.ReadAsStringAsync().ConfigureAwait(false));

			if (fixture.ApplicationFactory.AsAspFactory().AssertionRegex is null)
			{
				throw new InvalidOperationException("Missing AssertationRegex! Fill AssertionRegex via SetAssertionRegex(string regex)");
			}

			var folderPath = fixture.ApplicationFactory.AsAspFactory().AssertionRegex!.Match(path!).Groups[1].Value;
			var jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), folderPath, jsonFileName);

			var json = await File.ReadAllTextAsync(jsonFilePath).ConfigureAwait(false);

			data.Should().BeEquivalentTo(json);
		}
	}
}
