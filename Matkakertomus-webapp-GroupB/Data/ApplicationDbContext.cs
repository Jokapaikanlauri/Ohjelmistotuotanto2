﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Matkakertomus_webapp_GroupB.Data
{
    public class ApplicationDbContext : IdentityDbContext<Matkaaja>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Matkaaja> Matkaajat { get; set; }
        public DbSet<Matka> Matkat { get; set; }
        public DbSet<Tarina> Tarinat { get; set; }
        public DbSet<Kuva> Kuvat { get; set; }
        public DbSet<Matkakohde> Matkakohteet { get; set; }


    }
}