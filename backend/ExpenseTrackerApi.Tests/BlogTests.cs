using FluentValidation;
using Moq;

namespace ExpenseTrackerApi.Tests;

public class BlogControllerTests
{
	private readonly Mock<IValidator<Blog>> validatorMock = new();

	[Fact]
	public async Task GetBlogs_ReturnsListOfBlogs()
	{
		// Arrange
		var options = new DbContextOptionsBuilder<MainDatabaseContext>()
			.UseInMemoryDatabase(databaseName: "TestDatabase")
			.Options;

		using var context = new MainDatabaseContext(options);
		var getAuthenticatedUserIdService = new GetAuthenticatedUserIdService();

		// Create an instance of the controller
		var controller = new BlogController(context, getAuthenticatedUserIdService, validatorMock.Object);

		// Add some test blogs to the database
		await context.Blogs.AddRangeAsync(
			new Blog { Description = "Blog 1", Author = "John Doe", Text = "Text 1", UserId = 1, CreatedAt = DateTime.Now },
			new Blog { Description = "Blog 2", Author = "Jane Doe", Text = "Text 2", UserId = 1, CreatedAt = DateTime.Now }
		);
		await context.SaveChangesAsync();

		// Act
		var result = await controller.GetBlogs();

		// Assert
		var blogs = Assert.IsType<List<Blog>>(result.Value);
		Assert.Equal(4, blogs.Count);
	}

	[Fact]
	public async Task GetBlog_ReturnsBlogById()
	{
		// Arrange
		var options = new DbContextOptionsBuilder<MainDatabaseContext>()
			.UseInMemoryDatabase(databaseName: "TestDatabase")
			.Options;

		using var context = new MainDatabaseContext(options);
		var getAuthenticatedUserIdService = new GetAuthenticatedUserIdService();

		// Create an instance of the controller
		var controller = new BlogController(context, getAuthenticatedUserIdService, validatorMock.Object);

		// Create a test blog and add it to the in-memory database
		var testBlog = new Blog { Description = "Test Blog", Author = "John Doe", Text = "Test Text", UserId = 1, CreatedAt = DateTime.Now };
		await context.Blogs.AddAsync(testBlog);
		await context.SaveChangesAsync();

		// Act
		var result = await controller.GetBlog(testBlog.Id);

		// Assert
		var blog = Assert.IsType<Blog>(result.Value);
		Assert.Equal(testBlog.Description, blog.Description);
		Assert.Equal(testBlog.Author, blog.Author);
		Assert.Equal(testBlog.Text, blog.Text);
		Assert.Equal(testBlog.UserId, blog.UserId);
		Assert.Equal(testBlog.CreatedAt, blog.CreatedAt);
	}

	[Fact]
	public async Task PutBlog_UpdatesBlog()
	{
		// Arrange
		var options = new DbContextOptionsBuilder<MainDatabaseContext>()
			.UseInMemoryDatabase(databaseName: "TestDatabase")
			.Options;

		using var context = new MainDatabaseContext(options);
		var getAuthenticatedUserIdService = new GetAuthenticatedUserIdService();

		// Create an instance of the controller 
		var controller = new BlogController(context, getAuthenticatedUserIdService, validatorMock.Object);

		// Create a test blog and add it to the in-memory database
		var testBlog = new Blog { Description = "Test Blog", Author = "John Doe", Text = "Test Text", UserId = 1, CreatedAt = DateTime.Now };
		await context.Blogs.AddAsync(testBlog);
		await context.SaveChangesAsync();

		// Modify the test blog with updated information
		testBlog.Description = "Updated Description";

		// Act
		var result = await controller.PutBlog(testBlog.Id, testBlog);

		// Assert
		Assert.IsType<NoContentResult>(result);

		// Check if the blog was actually updated in the database
		var updatedBlog = await context.Blogs.FindAsync(testBlog.Id);
		Assert.Equal(testBlog.Description, updatedBlog.Description);
	}

	[Fact]
	public async Task DeleteBlog_RemovesBlog()
	{
		// Arrange
		var options = new DbContextOptionsBuilder<MainDatabaseContext>()
			.UseInMemoryDatabase(databaseName: "TestDatabase")
			.Options;

		using var context = new MainDatabaseContext(options);
		var getAuthenticatedUserIdService = new GetAuthenticatedUserIdService();

		// Create an instance of the controller 
		var controller = new BlogController(context, getAuthenticatedUserIdService, validatorMock.Object);

		// Create a test blog and add it to the in-memory database
		var testBlog = new Blog { Description = "Test Blog", Author = "John Doe", Text = "Test Text", UserId = 1, CreatedAt = DateTime.Now };
		await context.Blogs.AddAsync(testBlog);
		await context.SaveChangesAsync();

		// Act
		var result = await controller.DeleteBlog(testBlog.Id);

		// Assert
		Assert.IsType<NoContentResult>(result);

		// Check if the blog was actually deleted from the database
		var deletedBlog = await context.Blogs.FindAsync(testBlog.Id);
		Assert.Null(deletedBlog);
	}
}

