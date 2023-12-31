﻿using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;
using ValesFise_api.Modelos;

namespace ValesFise_api.Contexto
{
    public class AppFileDbContext : DbContext
    {
        public AppFileDbContext(DbContextOptions<AppFileDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ME_Agentes>().HasKey(u => new
            {
                u.Empresa,
                u.Celular
            });

            modelBuilder.Entity<ME_Agentes>().ToTable(tb => tb.HasTrigger("upd_ME_Accesos"));
        }

        //Poner aqui los modelos 
        public DbSet<ME_Agentes> ME_Agentes { get; set; }
        public DbSet<ME_Ticket> ME_Ticket { get; set; }

    }
}
