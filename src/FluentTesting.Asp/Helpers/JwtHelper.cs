using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace Testing.Asp.Helpers
{
	/// <summary>
	/// JWT Helper
	/// </summary>
	public static class JwtHelper
	{
		/// <summary>
		/// Get jwt
		/// </summary>
		/// <param name="userId">user id</param>
		/// <param name="claims">list of claims</param>
		/// <param name="roles">list of roles</param>
		/// <param name="userSe">user se - if not filled it is computed form id like "SE + id"</param>
		/// <returns></returns>
		public static string GetJwt(string userId, List<Claim>? claims = null, List<string>? roles = null)
		{
			var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("w5pl9VHH4ZTjY1560OwGP7NwEOHtNeYWoyS35y30/bZMA8Zym8ppwp7qJMENf1QB"));
			var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

			roles ??= ["User"];

			var allClaims = new List<Claim>()
			{
				new("id", $"{userId}"),
				new (ClaimTypes.Role, JsonSerializer.Serialize(roles), JsonClaimValueTypes.JsonArray)
			};

			if (claims is not null)
			{
				allClaims.AddRange(claims);
			}

			var securityToken = new JwtSecurityToken("",
			  "",
			  allClaims,
			  expires: DateTime.Now.AddDays(1),
			  signingCredentials: credentials);

			return new JwtSecurityTokenHandler().WriteToken(securityToken);
		}
	}
}
