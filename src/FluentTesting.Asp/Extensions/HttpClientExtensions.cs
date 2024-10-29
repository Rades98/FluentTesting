using System.Net.Http.Json;
using System.Security.Claims;
using Testing.Asp.Helpers;

namespace Testing.Asp.Extensions
{
	/// <summary>
	/// Http client extensions
	/// Used to send requests as user with fever effort
	/// </summary>
	public static class HttpClientExtensions
	{
		/// <summary>
		/// Get as user async
		/// </summary>
		/// <param name="client">http client</param>
		/// <param name="requestUri">request uri</param>
		/// <param name="userId">user id which is used in JWT token with role User</param>
		public static Task<HttpResponseMessage> GetAsUserAsync(this HttpClient client, string requestUri, int userId)
			=> client.AddBearerAuthHeader($"{userId}").GetAsync(requestUri);

		/// <summary>
		/// Get as user async
		/// </summary>
		/// <param name="client">http client</param>
		/// <param name="requestUri">request uri</param>
		/// <param name="userId">user id which is used in JWT token with role User</param>
		public static Task<HttpResponseMessage> GetAsUserAsync(this HttpClient client, string requestUri, string userId)
			=> client.AddBearerAuthHeader(userId).GetAsync(requestUri);

		/// <summary>
		/// Put as user async
		/// </summary>
		/// <param name="client">http client</param>
		/// <param name="requestUri">request uri</param>
		/// <param name="value">object to send</param>
		/// <param name="userId">user id which is used in JWT token with role User</param>
		public static Task<HttpResponseMessage> PutAsUserAsync<TValue>(this HttpClient client, string requestUri, TValue value, int userId)
			=> client.AddBearerAuthHeader($"{userId}").PutAsJsonAsync(requestUri, value);

		/// <summary>
		/// Put as user async
		/// </summary>
		/// <param name="client">http client</param>
		/// <param name="requestUri">request uri</param>
		/// <param name="value">object to send</param>
		/// <param name="userId">user id which is used in JWT token with role User</param>
		public static Task<HttpResponseMessage> PutAsUserAsync<TValue>(this HttpClient client, string requestUri, TValue value, string userId)
			=> client.AddBearerAuthHeader(userId).PutAsJsonAsync(requestUri, value);

		/// <summary>
		/// Post as user async
		/// </summary>
		/// <param name="client">http client</param>
		/// <param name="requestUri">request uri</param>
		/// <param name="value">object to send</param>
		/// <param name="userId">user id which is used in JWT token with role User</param>
		public static Task<HttpResponseMessage> PostAsUserAsync<TValue>(this HttpClient client, string requestUri, TValue value, int userId)
			=> client.AddBearerAuthHeader($"{userId}").PostAsJsonAsync(requestUri, value);

		/// <summary>
		/// Post as user async
		/// </summary>
		/// <param name="client">http client</param>
		/// <param name="requestUri">request uri</param>
		/// <param name="value">object to send</param>
		/// <param name="userId">user id which is used in JWT token with role User</param>
		public static Task<HttpResponseMessage> PostAsUserAsync<TValue>(this HttpClient client, string requestUri, TValue value, string userId)
			=> client.AddBearerAuthHeader(userId).PostAsJsonAsync(requestUri, value);

		/// <summary>
		/// Delete as user async
		/// </summary>
		/// <param name="client">http client</param>
		/// <param name="requestUri">request uri</param>
		/// <param name="userId">user id which is used in JWT token with role User</param>
		public static Task<HttpResponseMessage> DeleteAsUserAsync(this HttpClient client, string requestUri, int userId)
			=> client.AddBearerAuthHeader($"{userId}").DeleteAsync(requestUri);

		/// <summary>
		/// Delete as user async
		/// </summary>
		/// <param name="client">http client</param>
		/// <param name="requestUri">request uri</param>
		/// <param name="userId">user id which is used in JWT token with role User</param>
		public static Task<HttpResponseMessage> DeleteAsUserAsync(this HttpClient client, string requestUri, string userId)
			=> client.AddBearerAuthHeader(userId).DeleteAsync(requestUri);

		/// <summary>
		/// AddBearerAuthHeader
		/// </summary>
		/// <param name="client">http client</param>
		/// <param name="userId">userID</param>
		/// <param name="claims">claims</param>
		/// <param name="roles">roles</param>
		/// <param name="userSe">userSe</param>
		/// <returns></returns>
		public static HttpClient AddBearerAuthHeader(this HttpClient client, string userId, List<Claim>? claims = null, List<string>? roles = null)
		{
			client.DefaultRequestHeaders.Authorization = null;

			client.DefaultRequestHeaders.Add("Authorization", "Bearer " + JwtHelper.GetJwt(userId, claims, roles));

			return client;
		}
	}
}
