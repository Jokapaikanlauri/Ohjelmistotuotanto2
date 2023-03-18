using Duende.IdentityServer.EntityFramework.Options;
using MatkakertomusGroupB.Shared.Models;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace MatkakertomusGroupB.Server.Data
{
	//Traveller now extends IdentityUser, replaced below
	public class ApplicationDbContext : ApiAuthorizationDbContext<Traveller>
	{
		public ApplicationDbContext(
			DbContextOptions options,
			IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
		{
		}
		//Define DB sets
		public DbSet<Destination> Destinations { get; set; }
		public DbSet<Picture> Pictures { get; set; }
		public DbSet<Story> Stories { get; set; }
		public DbSet<Traveller> Travellers { get; set; }
		public DbSet<Trip> Trip { get; set; }
	}
}