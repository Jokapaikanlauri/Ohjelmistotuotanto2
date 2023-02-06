using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace backend.Data
{
    public class InitDb
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new MatkaContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<MatkaContext>>()))
            {
                if (context == null)
                {
                    throw new ArgumentNullException("Null MatkaContext");
                }

                context.Database.EnsureCreated();


                if (context.Matkaajat.Any())
                {
                    return;   // DB has been seeded 
                              // To re-seed the db: delete the existing *.db file and let the app create a new one
                }

                context.Matkaajat.AddRange(
                    new Matkaaja
                    {
                        MatkaajaId = 1,
                        Nimimerkki = "seppo",
                        Email = "seppo@gmale.couk",
                        Password = "salakala123",
                        Etunimi = "seppo",
                        Sukunimi = "sepponen"
                    }
                );
                context.SaveChanges();
            }
        }
    }
}
