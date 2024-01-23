using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Moq;

namespace ExpenseTrackerApi.Tests;
public class JwtTokenGeneratorTests
{
	private readonly IConfiguration _configuration;

	public JwtTokenGeneratorTests()
	{
		_configuration = new ConfigurationBuilder()
			.AddInMemoryCollection(new Dictionary<string, string>
			{
					{"Jwt:Issuer", "https://joydipkanjilal.com/"},
					{"Jwt:Audience", "https://joydipkanjilal.com/"},
					{"Jwt:Key", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.	eyJkZGFzYWRoYXNiZCBhc2RhZHMgc2Rhc3AgZGFzIGRhc2RhcyBhc2RhcyBkYXNkIGFzZGFzZGFzZCBhcyBkYXNhZGFzIGFzIGRhcyBkYXNhZGFzIGFzIGRhcyBkYXNhZGFzZGFzZCBhcyBkYXNhIGRhcyBkYXNhIGRhcyBkYXNhIGRhcyBkYXNhIGRhcyBkYXNhIGRhcyBkYXNhIGRhcyBkYXNhIGFzIGRhcyBkYXNhIGRhcyBkYXNhZGFzIGRhcyBkYXNhZGphcyBkYXNhIGRhcyBkYXNhIGRhcyBkYXNhIGRhcyBkYXNhIGRhcyBkYXNhIGRhcyBkYXNhIGRhcyBkYXNhIGRhcyBkYXNhIGRhcyBkYXNhIGRhcyBkYXNhZGphcyIsImlhdCI6MTYzNDEwNTUyMn0.S7G4f8pW7sGJ7t9PIShNElA0RRve-HlPfZRvX8hnZ6c"}
			})
			.Build();
	}

	[Fact]
	public void GenerateJwtToken_ValidUser_ReturnsValidToken()
	{
		// Arrange
		var dbContextMock = new Mock<MainDatabaseContext>();

		// Create a mock user object
		var user = new User
		{
			Id = 1,
			Username = "mockUser",
			Email = "mock@example.com"
		};

		// Create an instance of the AuthService with the mock database context and configuration
		var jwtTokenGenerator = new AuthService(dbContextMock.Object, _configuration);

		// Act
		var token = jwtTokenGenerator.GenerateJwtToken(user);

		// Assert
		Assert.NotNull(token);
		Assert.NotEmpty(token);

		var tokenHandler = new JwtSecurityTokenHandler();
		var validationParameters = new TokenValidationParameters
		{
			ValidateIssuer = true,
			ValidateAudience = true,
			ValidateIssuerSigningKey = true,
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? "key")),
			ValidIssuer = _configuration["Jwt:Issuer"],
			ValidAudience = _configuration["Jwt:Audience"],
		};

		tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

		Assert.NotNull(validatedToken);
	}
}
