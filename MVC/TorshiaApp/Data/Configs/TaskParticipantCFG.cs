namespace TorshiaApp.Data.Configs
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using TorshiaApp.Models;
    internal class TaskParticipantCFG : IEntityTypeConfiguration<TaskParticipant>
    {
        public void Configure(EntityTypeBuilder<TaskParticipant> builder)
        {
            builder.HasKey(x => new { x.TaskId, x.ParticipantId });
            builder.HasOne(tp => tp.Participant).WithMany(p => p.ParticipantTasks).HasForeignKey(tp => tp.ParticipantId);
            builder.HasOne(tp => tp.Task).WithMany(t => t.TaskParticipants).HasForeignKey(tp => tp.TaskId);
        }
    }
}