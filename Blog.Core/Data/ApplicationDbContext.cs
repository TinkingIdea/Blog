using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Blog.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Blog.Core.Data
{
    public class ApplicationDbContext: DbContext
    {
        public readonly ILoggerFactory MyLoggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        public ApplicationDbContext(DbContextOptions options): base(options)
        {
            
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseLoggerFactory(MyLoggerFactory);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            var entityType = typeof(Entity);
            entityType
                .Assembly
                .GetExportedTypes()
                .Where(type => !type.IsAbstract && !type.IsInterface && entityType.IsAssignableFrom(type))
                .ToList()
                .ForEach(eType => modelBuilder.Entity(eType));
        }
    }
}
