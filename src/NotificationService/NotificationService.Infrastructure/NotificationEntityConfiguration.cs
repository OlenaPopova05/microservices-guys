using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NotificationService.Infrastructure;

public class NotificationEntityConfiguration : IEntityTypeConfiguration<NotificationEntity>
{
    public void Configure(EntityTypeBuilder<NotificationEntity> builder)
    {
        builder.ToTable("notifications");
        
        builder.HasKey(x => x.EventId);

        builder.Property(x => x.EventId)
            .HasColumnName("event_id");

        builder.Property(x => x.CorrelationId)
            .HasColumnName("correlation_id")
            .IsRequired();

        builder.Property(x => x.HabitId)
            .HasColumnName("habit_id");

        builder.Property(x => x.OwnerUserId)
            .HasColumnName("owner_user_id");

        builder.Property(x => x.Summary)
            .HasColumnName("summary")
            .IsRequired();

        builder.Property(x => x.Payload)
            .HasColumnName("payload")
            .IsRequired();

        builder.Property(x => x.OccurredAt)
            .HasColumnName("occurred_at");

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at");

        builder.HasIndex(x => x.EventId)
            .IsUnique();
    }
}