using BloggingPlatformAPI.Data;
using BloggingPlatformAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BloggingPlatformAPI.Controllers
{
	[Route("api/posts")]
	[ApiController]
	public class BlogPostsController : Controller
	{
		private readonly BlogPostDbContext _context;

		public BlogPostsController(BlogPostDbContext context)
		{
			_context = context;
		}
		//GET : api/BlogPosts
		[HttpGet]
		public async Task<ActionResult<IEnumerable<BlogPostModel>>> GetBlogPosts()
		{
			return await _context.BlogPosts.ToListAsync();
		}

		//GET : api/posts/5
		[HttpGet("{id}")]
		public async Task<ActionResult<BlogPostModel>> GetBlogPost(int id)
		{
			var blogPost = await _context.BlogPosts.FindAsync(id);
			if (blogPost == null)
			{
				return NotFound();
			}
			return blogPost;
		}
		//POST : api/posts
		[HttpPost]
		public async Task<ActionResult<BlogPostModel>> PostBlogPost(BlogPostModel blogPost)
		{
			_context.BlogPosts.Add(blogPost);
			await _context.SaveChangesAsync();

			return CreatedAtAction(nameof(GetBlogPosts), new { id = blogPost.id }, blogPost);
		}
		//PUT : api/posts/id
		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateBlogPost(int id, BlogPostModel blogPost)
		{
			if (id != blogPost.id)
			{
				return BadRequest("Post ID does not exist");
			}
			var existingBlogPost = await _context.BlogPosts.FindAsync(id);

			if (existingBlogPost == null)
			{
				return NotFound();
			}
			existingBlogPost.Title = blogPost.Title;
			existingBlogPost.Content = blogPost.Content;
			existingBlogPost.Category = blogPost.Category;
			existingBlogPost.Tags = blogPost.Tags;
			existingBlogPost.UpdatedAt = DateTime.UtcNow;


			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!BlogPostExists(id))
				{
					return NotFound();
				}
				else
				{
					throw;
				}
			}
			return NoContent();
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteBlogPost(int id)
		{
			var blogPost = await _context.BlogPosts.FindAsync(id);
			if (blogPost == null)
			{
				return NotFound();
			}
			_context.BlogPosts.Remove(blogPost);
			await _context.SaveChangesAsync();

			return NoContent();
		}
		[HttpGet("search")]
		public async Task<ActionResult<IEnumerable<BlogPostModel>>> SearchBlogPosts([FromQuery] string query)
		{
			if (string.IsNullOrEmpty(query))
			{
				return BadRequest("Query is empty");
			}
			var blogPosts = await _context.BlogPosts
				.Where(post => post.Title.Contains(query) || post.Content.Contains(query) || post.Tags.Contains(query))
				.ToListAsync();
			if (blogPosts.Count == 0)
			{
				return NotFound("No posts found");
			}
			else
			{
				return Ok(blogPosts);
			}
		}

		private bool BlogPostExists(int id)
		{
			return _context.BlogPosts.Any(e => e.id == id);
		}
	}
}
