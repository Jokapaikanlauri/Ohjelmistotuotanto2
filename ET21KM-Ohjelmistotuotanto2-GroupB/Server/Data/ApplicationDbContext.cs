using Duende.IdentityServer.EntityFramework.Options;
using ET21KM_Ohjelmistotuotanto2_GroupB.Server.Models;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ET21KM_Ohjelmistotuotanto2_GroupB.Server.Data
{
	public class ApplicationDbContext : ApiAuthorizationDbContext<Traveller>
	{
		public ApplicationDbContext(
			DbContextOptions options,
			IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
		{
		}
	}
}