namespace TorshiaApp.Data.Configs
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using TorshiaApp.Models;
    internal class SectorCFG : IEntityTypeConfiguration<Sector>
    {
        public void Configure(EntityTypeBuilder<Sector> builder)
        {
            builder.HasData(
                new Sector { Id=1,Name= "Customers" },
                new Sector { Id=2,Name= "Marketing" },
                new Sector { Id=3,Name= "Finances" },
                new Sector { Id=4,Name= "Internal" },
                new Sector { Id=5,Name= "Management" });
        }
    }
}