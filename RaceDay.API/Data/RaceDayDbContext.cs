using Microsoft.EntityFrameworkCore;
using RaceDay.API.Models;

namespace RaceDay.API.Data;

public class RaceDayDbContext(DbContextOptions<RaceDayDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Venue> Venues => Set<Venue>();
    public DbSet<Event> Events => Set<Event>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Enrolment> Enrolments => Set<Enrolment>();
    public DbSet<Result> Results => Set<Result>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<User>().HasIndex(x => x.Email).IsUnique();
        modelBuilder.Entity<User>().ToTable(t => t.HasCheckConstraint("CK_Users_Role", "[Role] IN ('Organiser','Participant')"));
        modelBuilder.Entity<Event>().ToTable(t => t.HasCheckConstraint("CK_Events_EventType", "[EventType] IN ('Running','Walking','Cycling')"));
        modelBuilder.Entity<Category>().HasOne(x => x.Event).WithMany(x => x.Categories).HasForeignKey(x => x.EventID).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Event>().HasOne(x => x.Organiser).WithMany(x => x.OrganisedEvents).HasForeignKey(x => x.OrganiserID).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Event>().HasOne(x => x.Venue).WithMany(x => x.Events).HasForeignKey(x => x.VenueID).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Enrolment>().HasIndex(x => new { x.ParticipantID, x.CategoryID }).IsUnique();
        modelBuilder.Entity<Enrolment>().Property(x => x.Status).HasDefaultValue("Confirmed");
        modelBuilder.Entity<Enrolment>().ToTable(t => t.HasCheckConstraint("CK_Enrolments_Status", "[Status] IN ('Confirmed','Cancelled')"));
        modelBuilder.Entity<Result>().HasIndex(x => x.EnrolmentID).IsUnique();
        modelBuilder.Entity<Result>().Property(x => x.Status).HasDefaultValue("Finished");
        modelBuilder.Entity<Result>().ToTable(t => t.HasCheckConstraint("CK_Results_Status", "[Status] IN ('Finished','DNF','DNS')"));
        modelBuilder.Entity<Result>().HasOne(x => x.Enrolment).WithOne(x => x.Result).HasForeignKey<Result>(x => x.EnrolmentID).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<User>().Property(x => x.CreatedAt).HasDefaultValueSql("GETDATE()");
        modelBuilder.Entity<Event>().Property(x => x.CreatedAt).HasDefaultValueSql("GETDATE()");
        modelBuilder.Entity<Enrolment>().Property(x => x.EnrolmentDate).HasDefaultValueSql("GETDATE()");
        modelBuilder.Entity<Category>().Property(x => x.EntryFee).HasDefaultValue(0m);
        modelBuilder.Entity<Category>().Property(x => x.MaxParticipants).HasDefaultValue(100);
        modelBuilder.Entity<Venue>().HasData(new Venue { VenueID = 1, Name = "RaceDay Demo Venue", AddressLine = "Main Road", City = "Johannesburg", Province = "Gauteng", Latitude = null, Longitude = null });
    }
}
