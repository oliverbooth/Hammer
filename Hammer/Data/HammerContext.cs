using System.Diagnostics.CodeAnalysis;
using Hammer.Data.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace Hammer.Data;

/// <summary>
///     Represents a session with the <c>hammer.db</c> database.
/// </summary>
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Instantiated by DI")]
[SuppressMessage("ReSharper", "InheritdocConsiderUsage", Justification = "Unnecessary")]
internal sealed class HammerContext : DbContext
{
    private const string DataSource = "hammer.db";

    /// <summary>
    ///     Gets or sets the set of infractions.
    /// </summary>
    /// <value>The set of infractions.</value>
    public DbSet<Infraction> Infractions { get; set; } = null!; // assigned when context is created

    /// <summary>
    ///     Gets or sets the set of tracked messages.
    /// </summary>
    /// <value>The set of tracked messages.</value>
    public DbSet<TrackedMessage> TrackedMessages { get; set; } = null!; // assigned when context is created

    /// <summary>
    ///     Gets or sets the set of rules.
    /// </summary>
    /// <value>The set of rules.</value>
    public DbSet<Rule> Rules { get; set; } = null!; // assigned when context is created

    /// <summary>
    ///     Gets or sets the set of staff messages.
    /// </summary>
    /// <value>The set of staff messages.</value>
    public DbSet<StaffMessage> StaffMessages { get; set; } = null!; // assigned when context is created

    /// <inheritdoc />
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseSqlite($"Data Source={DataSource}");
    }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new InfractionConfiguration());
        modelBuilder.ApplyConfiguration(new TrackedMessageConfiguration());
        modelBuilder.ApplyConfiguration(new RuleConfiguration());
    }
}
