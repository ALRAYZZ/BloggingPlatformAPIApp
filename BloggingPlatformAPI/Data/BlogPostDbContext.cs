using BloggingPlatformAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BloggingPlatformAPI.Data
{
	public class BlogPostDbContext : DbContext
	{
		public BlogPostDbContext(DbContextOptions<BlogPostDbContext> options) : base(options)
		{

		}
		public DbSet<BlogPostModel> BlogPosts { get; set; }
	}
}
