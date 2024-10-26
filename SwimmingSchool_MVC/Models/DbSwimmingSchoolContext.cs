using Microsoft.EntityFrameworkCore;

namespace SwimmingSchool_MVC.Models;

public partial class DbSwimmingSchoolContext : DbContext
{
    public DbSet<Day> Days { get; set; }

    public DbSet<Event> Events { get; set; }

    public DbSet<Group> Groups { get; set; }

    public DbSet<GroupSchedule> GroupSchedules { get; set; }

    public DbSet<GroupType> GroupTypes { get; set; }

    public DbSet<Pupil> Pupils { get; set; }

    public DbSet<PupilsEvent> PupilsEvents { get; set; }

    public DbSet<Trainer> Trainers { get; set; }
    public DbSwimmingSchoolContext(DbContextOptions<DbSwimmingSchoolContext> options)
        : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Server=DESKTOP-JCD2U32\\SQLEXPRESS; Database=DbSwimmingSchool_MVC; Trusted_Connection=True; Trust Server Certificate=True; ");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Day>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.NameOfDay).HasMaxLength(30);
        });

        modelBuilder.Entity<GroupType>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasMaxLength(30);
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Date).HasColumnType("smalldatetime");
            entity.Property(e => e.MaxPupilsAmount).HasColumnType("smallint");
            entity.Property(e => e.Locations).HasMaxLength(100);
            entity.Property(e => e.Description).HasColumnType("ntext");
            entity.Property(e => e.Decree).HasColumnType("ntext");
            entity.Property(e => e.Logo).HasColumnType("varbinary(max)");
            entity.Property(e => e.IsHeld).HasColumnType("bit");
        });

        modelBuilder.Entity<Group>(entity =>
        {

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.GroupName).HasMaxLength(50);

            entity.HasOne(d => d.Trainer).WithMany(p => p.Groups)
                .HasForeignKey(d => d.TrainerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Groups_Trainers");
            entity.HasOne(d => d.GroupType).WithMany(p => p.Groups)
                .HasForeignKey(d => d.GroupTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Groups_GroupTypes");
        });

        modelBuilder.Entity<GroupSchedule>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.TimeOfTrainingStart).HasColumnType("time(3)");
            entity.Property(e => e.TrainingTime).HasColumnType("smallint");

            entity.HasOne(d => d.Day).WithMany(p => p.GroupSchedules)
                .HasForeignKey(d => d.DayId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GroupSchedules_Days");

            entity.HasOne(d => d.Group).WithMany(p => p.GroupSchedules)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GroupSchedules_Groups");
        });

        modelBuilder.Entity<Pupil>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.MiddleName).HasMaxLength(50);
            entity.Property(e => e.Birthday).HasColumnType("date");
            entity.Property(e => e.ParentsPhoneNumber).HasMaxLength(20);

            entity.HasOne(d => d.Group).WithMany(p => p.Pupils)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Pupils_Groups");
        });

        modelBuilder.Entity<PupilsEvent>(entity =>
        {
            entity.HasKey(e => new { e.PupilsId, e.EventId }).HasName("PK_PupilsEvent_1");

            entity.ToTable("PupilsEvent");

            entity.Property(e => e.Info).HasMaxLength(50);

            entity.Property(e => e.Result).HasColumnType("time(3)");

            entity.HasOne(d => d.Event).WithMany(p => p.PupilsEvents)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PupilsEvent_Events");

            entity.HasOne(d => d.Pupils).WithMany(p => p.PupilsEvents)
                .HasForeignKey(d => d.PupilsId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PupilsEvent_Pupils");
        });

        modelBuilder.Entity<Trainer>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.MiddleName).HasMaxLength(50);
            entity.Property(e => e.Birthday).HasColumnType("date");
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.Email).HasMaxLength(255);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
