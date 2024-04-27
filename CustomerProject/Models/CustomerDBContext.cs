
using Microsoft.EntityFrameworkCore;

namespace CustomerProject.Models
{
	public class CustomerDBContext : DbContext
	{
		public CustomerDBContext(DbContextOptions<CustomerDBContext> options) : base(options)
		{
		}
		
		public DbSet<Customer> Customers { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{

			base.OnModelCreating(modelBuilder);


		}
	}
}




