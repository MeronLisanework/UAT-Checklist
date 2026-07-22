using Microsoft.EntityFrameworkCore;
using ECIMS.Web.Models.Entities;

namespace ECIMS.Web.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Role> Roles { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Customer> Customers { get; set; } = null!;
        public DbSet<CustomerBranch> CustomerBranches { get; set; } = null!;
        public DbSet<Project> Projects { get; set; } = null!;
        public DbSet<UatSection> UatSections { get; set; } = null!;
        public DbSet<UatMasterItem> UatMasterItems { get; set; } = null!;
        public DbSet<ProjectUatAttempt> ProjectUatAttempts { get; set; } = null!;
        public DbSet<ProjectUatResult> ProjectUatResults { get; set; } = null!;
        public DbSet<ProjectUatResultHistory> ProjectUatResultHistories { get; set; } = null!;
        public DbSet<DigitalSignature> DigitalSignatures { get; set; } = null!;
        public DbSet<AcceptanceCert> AcceptanceCerts { get; set; } = null!;
        public DbSet<CompanyStampAsset> CompanyStampAssets { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CustomerBranch>().HasKey(b => b.BranchId);
modelBuilder.Entity<UatSection>().HasKey(s => s.SectionId);
modelBuilder.Entity<UatMasterItem>().HasKey(m => m.MasterItemId);
modelBuilder.Entity<ProjectUatAttempt>().HasKey(a => a.AttemptId);
modelBuilder.Entity<ProjectUatResult>().HasKey(r => r.ResultId);
modelBuilder.Entity<ProjectUatResultHistory>().HasKey(h => h.HistoryId);
modelBuilder.Entity<DigitalSignature>().HasKey(s => s.SignatureId);
modelBuilder.Entity<AcceptanceCert>().HasKey(c => c.CertificateId);
modelBuilder.Entity<CompanyStampAsset>().HasKey(a => a.AssetId);


            modelBuilder.Entity<Customer>()
                .HasIndex(c => c.CustomerName)
                .IsUnique();

            modelBuilder.Entity<CustomerBranch>()
                .HasIndex(b => new { b.CustomerId, b.BranchName })
                .IsUnique();

            modelBuilder.Entity<ProjectUatAttempt>()
                .HasIndex(a => new { a.ProjectId, a.AttemptNumber })
                .IsUnique();
                modelBuilder.Entity<ProjectUatAttempt>()
    .HasOne(a => a.DecidedBy)
    .WithMany()
    .HasForeignKey(a => a.DecidedById)
    .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProjectUatResult>()
                .HasIndex(r => new { r.AttemptId, r.MasterItemId })
                .IsUnique();

            modelBuilder.Entity<DigitalSignature>()
                .HasIndex(s => new { s.AttemptId, s.SignatoryRole })
                .IsUnique();

            modelBuilder.Entity<AcceptanceCert>()
                .HasIndex(c => c.AttemptId)
                .IsUnique();


            modelBuilder.Entity<Project>()
                .Property(p => p.Status)
                .HasColumnType("tinyint");

            modelBuilder.Entity<ProjectUatAttempt>()
                .Property(a => a.OverallStatus)
                .HasColumnType("tinyint");

            modelBuilder.Entity<ProjectUatResult>()
                .Property(r => r.PassStatus)
                .HasColumnType("tinyint");

            modelBuilder.Entity<ProjectUatResult>()
                .Property(r => r.LastModifiedByRole)
                .HasColumnType("tinyint");

            modelBuilder.Entity<ProjectUatResultHistory>()
                .Property(h => h.PreEditPassStatus)
                .HasColumnType("tinyint");

            modelBuilder.Entity<ProjectUatResultHistory>()
                .Property(h => h.PostEditPassStatus)
                .HasColumnType("tinyint");

            modelBuilder.Entity<DigitalSignature>()
                .Property(s => s.SignatoryRole)
                .HasColumnType("tinyint");

            modelBuilder.Entity<Role>()
                .HasMany(r => r.Users)
                .WithOne(u => u.Role)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Customer>()
                .HasOne(c => c.CreatedBy)
                .WithMany()
                .HasForeignKey(c => c.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Customer>()
                .HasMany(c => c.Branches)
                .WithOne(b => b.Customer)
                .HasForeignKey(b => b.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CustomerBranch>()
                .HasOne(b => b.CreatedBy)
                .WithMany()
                .HasForeignKey(b => b.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CustomerBranch>()
                .HasMany(b => b.Projects)
                .WithOne(p => p.Branch)
                .HasForeignKey(p => p.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Project>()
                .HasOne(p => p.ProjectManager)
                .WithMany()
                .HasForeignKey(p => p.ProjectManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Project>()
                .HasOne(p => p.Consultant)
                .WithMany()
                .HasForeignKey(p => p.ConsultantId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Project>()
                .HasOne(p => p.CreatedBy)
                .WithMany()
                .HasForeignKey(p => p.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Project>()
                .HasMany(p => p.Attempts)
                .WithOne(a => a.Project)
                .HasForeignKey(a => a.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UatSection>()
                .HasMany(s => s.MasterItems)
                .WithOne(m => m.Section)
                .HasForeignKey(m => m.SectionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProjectUatAttempt>()
                .HasOne(a => a.InitiatedBy)
                .WithMany()
                .HasForeignKey(a => a.InitiatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProjectUatAttempt>()
                .HasMany(a => a.Results)
                .WithOne(r => r.Attempt)
                .HasForeignKey(r => r.AttemptId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProjectUatAttempt>()
                .HasMany(a => a.Signatures)
                .WithOne(s => s.Attempt)
                .HasForeignKey(s => s.AttemptId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProjectUatAttempt>()
                .HasOne(a => a.Certificate)
                .WithOne(c => c.Attempt)
                .HasForeignKey<AcceptanceCert>(c => c.AttemptId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UatMasterItem>()
                .HasMany<ProjectUatResult>()
                .WithOne(r => r.MasterItem)
                .HasForeignKey(r => r.MasterItemId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProjectUatResult>()
                .HasOne(r => r.ExecutedBy)
                .WithMany()
                .HasForeignKey(r => r.ExecutedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProjectUatResult>()
                .HasOne(r => r.LastModifiedBy)
                .WithMany()
                .HasForeignKey(r => r.LastModifiedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProjectUatResult>()
                .HasMany(r => r.History)
                .WithOne(h => h.Result)
                .HasForeignKey(h => h.ResultId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProjectUatResultHistory>()
                .HasOne(h => h.EditedBy)
                .WithMany()
                .HasForeignKey(h => h.EditedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DigitalSignature>()
                .HasOne(s => s.SignedBy)
                .WithMany()
                .HasForeignKey(s => s.SignedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CompanyStampAsset>()
                .HasOne(a => a.UploadedBy)
                .WithMany()
                .HasForeignKey(a => a.UploadedById)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}