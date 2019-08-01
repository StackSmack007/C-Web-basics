using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using TorshiaApp.Models;

namespace TorshiaApp.Data.Configs
{
    internal class TaskSectorCFG : IEntityTypeConfiguration<TaskSector>
    {
        public void Configure(EntityTypeBuilder<TaskSector> builder)
        {
            builder.HasKey(x => new { x.TaskId, x.SectorId });
            builder.HasOne(ts => ts.Sector).WithMany(s => s.SectorTasks).HasForeignKey(ts => ts.SectorId);
            builder.HasOne(ts => ts.Task).WithMany(t => t.AffectedSectors).HasForeignKey(ts => ts.TaskId);
        }
    }
}