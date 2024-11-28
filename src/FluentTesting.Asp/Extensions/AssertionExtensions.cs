using FluentAssertions;
using FluentAssertions.Json;
using FluentTesting.Asp;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Runtime.CompilerServices;

namespace FluentTesting.Asp.Extensions
{
    /// <summary>
    /// Assertation extensions
    /// </summary>
    public static class AssertionExtensions
    {
        /// <summary>
        /// Assert json response by file name. Json file must lay in same location as calling test
        /// </summary>
        /// <param name="factory">Application factory</param>
        /// <param name="message">http response message</param>
        /// <param name="jsonFileName">name of json file within same location as test class</param>
        /// <param name="path">automagically filled path by attribute</param>
        public static async Task AssertJsonResponseAsync(this AspApplicationFactory factory, HttpResponseMessage message, string jsonFileName, [CallerFilePath] string? path = null)
        {
            if (factory.AssertionRegex is null)
            {
                throw new InvalidOperationException("Missing AssertationRegex! Register assertion regex in factory via .SetAssertionRegex(<your regex>)");
            }

            var data = JToken.Parse(await message.Content.ReadAsStringAsync().ConfigureAwait(false));

            var folderPath = factory.AssertionRegex.Match(path!).Groups[1].Value;
            var jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), folderPath, jsonFileName);

            var json = await File.ReadAllTextAsync(jsonFilePath).ConfigureAwait(false);

            data.Should().BeEquivalentTo(json);
        }

        /// <summary>
        /// Assert json object response. Assert Http response message agains string
        /// </summary>
        /// <param name="message">http response message</param>
        /// <param name="json">json as a string</param>
        /// <returns></returns>
        public static async Task AssertJsonObjectResponseAsync(this HttpResponseMessage message, string json)
        {
            var data = JToken.Parse(await message.Content.ReadAsStringAsync().ConfigureAwait(false));

            data.Should().BeEquivalentTo(json);
        }

        /// <summary>
        /// Assert http response message status code
        /// </summary>
        /// <param name="message">http response message</param>
        /// <param name="statusCode">status code</param>
        public static void AssertStatusCode(this HttpResponseMessage message, HttpStatusCode statusCode)
        {
            message.StatusCode.Should().Be(statusCode);
        }
    }
}
