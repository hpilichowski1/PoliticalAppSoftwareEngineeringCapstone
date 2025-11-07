using Microsoft.EntityFrameworkCore;

namespace PoliticalAppAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Representative> Representatives => Set<Representative>();
        public DbSet<Candidate> Candidates => Set<Candidate>();
        public DbSet<Legislation> Legislations => Set<Legislation>();
        public DbSet<VoteRecord> VoteRecords => Set<VoteRecord>();
        public DbSet<VoteSimulation> VoteSimulations => Set<VoteSimulation>();
        public DbSet<Quiz> Quizzes => Set<Quiz>();
        public DbSet<Question> Questions => Set<Question>();
        public DbSet<QuizQuestion> QuizQuestions => Set<QuizQuestion>();
        public DbSet<QuestionOption> QuestionOptions => Set<QuestionOption>();
        public DbSet<UserQuizAttempt> UserQuizAttempts => Set<UserQuizAttempt>();
        public DbSet<UserQuizAnswer> UserQuizAnswers => Set<UserQuizAnswer>();
        public DbSet<NewsItem> NewsItems => Set<NewsItem>();
        public DbSet<CivicHub> CivicHubs => Set<CivicHub>();
        public DbSet<CivicHubAlignedRep> CivicHubAlignedReps => Set<CivicHubAlignedRep>();
        public DbSet<CivicHubCuratedNews> CivicHubCuratedNews => Set<CivicHubCuratedNews>();

        protected override void OnModelCreating(ModelBuilder mb)
        {
            base.OnModelCreating(mb);

            // ---- Users
            mb.Entity<User>(e =>
            {
                e.HasKey(x => x.UserId);
                e.Property(x => x.UserId).HasColumnType("char(36)");
                e.Property(x => x.Name).IsRequired().HasMaxLength(200);
                e.Property(x => x.Email).IsRequired().HasMaxLength(320);
                e.HasIndex(x => x.Email).IsUnique();

                // Role stored as MySQL ENUM (matches your comment)
                e.Property(x => x.Role)
                 .IsRequired()
                 .HasColumnType("ENUM('citizen','representative','candidate','admin')")
                 .HasDefaultValue("citizen");

                e.Property(x => x.PasswordHash).HasMaxLength(255);

                // 1:1 User ↔ CivicHub (CivicHub PK/FK = UserId)
                e.HasOne(x => x.CivicHub)
                 .WithOne(h => h.User)
                 .HasForeignKey<CivicHub>(h => h.UserId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasMany(x => x.VoteSimulations)
                 .WithOne(v => v.User)
                 .HasForeignKey(v => v.UserId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasMany(x => x.QuizAttempts)
                 .WithOne(a => a.User)
                 .HasForeignKey(a => a.UserId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // ---- Representative
            mb.Entity<Representative>(e =>
            {
                e.HasKey(x => x.RepId);
                e.Property(x => x.RepId).HasColumnType("char(36)");
                e.Property(x => x.Name).IsRequired().HasMaxLength(200);
                e.Property(x => x.District).IsRequired().HasMaxLength(100);

                // optional link to a User (no back-collection on User)
                e.Property(x => x.UserId).HasColumnType("char(36)");
                e.HasOne(x => x.User)
                 .WithMany()
                 .HasForeignKey(x => x.UserId)
                 .OnDelete(DeleteBehavior.SetNull);
            });

            // ---- Candidate
            mb.Entity<Candidate>(e =>
            {
                e.HasKey(x => x.CandidateId);
                e.Property(x => x.CandidateId).HasColumnType("char(36)");
                e.Property(x => x.Name).IsRequired().HasMaxLength(200);
                e.Property(x => x.UserId).HasColumnType("char(36)");
                e.HasOne(x => x.User)
                 .WithMany()
                 .HasForeignKey(x => x.UserId)
                 .OnDelete(DeleteBehavior.SetNull);

                // JSON as string (use LONGTEXT)
                e.Property(x => x.CampaignInfoJson).HasColumnType("longtext");
            });

            // ---- Legislation
            mb.Entity<Legislation>(e =>
            {
                e.HasKey(x => x.BillId);
                e.Property(x => x.BillId).HasMaxLength(64); // external ids can be shorter
                e.Property(x => x.Title).IsRequired().HasMaxLength(500);
                e.Property(x => x.Summary).HasColumnType("longtext");
                e.Property(x => x.Status)
                 .IsRequired()
                 .HasColumnType("ENUM('introduced','committee','floor','passed','failed')")
                 .HasDefaultValue("introduced");

                e.HasMany(x => x.VoteRecords)
                 .WithOne(v => v.Bill)
                 .HasForeignKey(v => v.BillId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasMany(x => x.Simulations)
                 .WithOne(s => s.Bill)
                 .HasForeignKey(s => s.BillId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // ---- VoteRecord
            mb.Entity<VoteRecord>(e =>
            {
                e.HasKey(x => x.VoteId);
                e.Property(x => x.RepId).HasColumnType("char(36)").IsRequired();
                e.HasOne(x => x.Rep)
                 .WithMany(r => r.VotingHistory)
                 .HasForeignKey(x => x.RepId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.Property(x => x.BillId).IsRequired();
                e.HasIndex(x => new { x.RepId, x.BillId }).IsUnique(false);

                e.Property(x => x.Vote)
                 .IsRequired()
                 .HasColumnType("ENUM('YEA','NAY','PRESENT','NOT_VOTING')")
                 .HasDefaultValue("YEA");

                e.Property(x => x.VotedAt).HasColumnType("datetime(6)");
            });

            // ---- VoteSimulation
            mb.Entity<VoteSimulation>(e =>
            {
                e.HasKey(x => x.SimulationId);
                e.Property(x => x.SimulationId).HasColumnType("char(36)");
                e.Property(x => x.UserId).HasColumnType("char(36)").IsRequired();
                e.Property(x => x.BillId).IsRequired();

                e.HasOne(x => x.User)
                 .WithMany(u => u.VoteSimulations)
                 .HasForeignKey(x => x.UserId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(x => x.Bill)
                 .WithMany(b => b.Simulations)
                 .HasForeignKey(x => x.BillId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.Property(x => x.SelectedVote)
                 .IsRequired()
                 .HasColumnType("ENUM('YEA','NAY','PRESENT')")
                 .HasDefaultValue("YEA");

                e.Property(x => x.ComparedRepId).HasColumnType("char(36)");
                e.HasOne(x => x.ComparedRep)
                 .WithMany()
                 .HasForeignKey(x => x.ComparedRepId)
                 .OnDelete(DeleteBehavior.SetNull);

                e.Property(x => x.CompareResult)
                 .HasColumnType("ENUM('MATCH','DIFFER','N/A')");
            });

            // ---- Quiz
            mb.Entity<Quiz>(e =>
            {
                e.HasKey(x => x.QuizId);
                e.Property(x => x.QuizId).HasColumnType("char(36)");
                e.Property(x => x.Title).IsRequired().HasMaxLength(300);
            });

            // ---- Question
            mb.Entity<Question>(e =>
            {
                e.HasKey(x => x.QuestionId);
                e.Property(x => x.QuestionId).HasColumnType("char(36)");
                e.Property(x => x.Text).IsRequired().HasColumnType("longtext");
                e.Property(x => x.Topic).HasMaxLength(100);
            });

            // ---- QuizQuestion (join with payload: Position)
            mb.Entity<QuizQuestion>(e =>
            {
                e.HasKey(x => new { x.QuizId, x.QuestionId });
                e.Property(x => x.QuizId).HasColumnType("char(36)");
                e.Property(x => x.QuestionId).HasColumnType("char(36)");
                e.Property(x => x.Position).IsRequired();

                e.HasOne(x => x.Quiz)
                 .WithMany(q => q.QuizQuestions)
                 .HasForeignKey(x => x.QuizId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(x => x.Question)
                 .WithMany(q => q.QuizQuestions)
                 .HasForeignKey(x => x.QuestionId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // ---- QuestionOption
            mb.Entity<QuestionOption>(e =>
            {
                e.HasKey(x => x.OptionId);
                e.Property(x => x.OptionId).HasColumnType("char(36)");
                e.Property(x => x.QuestionId).HasColumnType("char(36)").IsRequired();

                e.HasOne(x => x.Question)
                 .WithMany(q => q.Options)
                 .HasForeignKey(x => x.QuestionId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.Property(x => x.Label).IsRequired().HasMaxLength(300);
                e.Property(x => x.ValueInt);
                e.Property(x => x.ValueJson).HasColumnType("longtext");
            });

            // ---- UserQuizAttempt
            mb.Entity<UserQuizAttempt>(e =>
            {
                e.HasKey(x => x.AttemptId);
                e.Property(x => x.AttemptId).HasColumnType("char(36)");
                e.Property(x => x.UserId).HasColumnType("char(36)").IsRequired();
                e.Property(x => x.QuizId).HasColumnType("char(36)").IsRequired();

                e.HasOne(x => x.User)
                 .WithMany(u => u.QuizAttempts)
                 .HasForeignKey(x => x.UserId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(x => x.Quiz)
                 .WithMany()
                 .HasForeignKey(x => x.QuizId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.Property(x => x.CreatedAt).HasColumnType("datetime(6)");
                e.Property(x => x.AlignmentJson).HasColumnType("longtext");
            });

            // ---- UserQuizAnswer (join Attempt↔Question with optional Option)
            mb.Entity<UserQuizAnswer>(e =>
            {
                e.HasKey(x => new { x.AttemptId, x.QuestionId });
                e.Property(x => x.AttemptId).HasColumnType("char(36)");
                e.Property(x => x.QuestionId).HasColumnType("char(36)");
                e.Property(x => x.OptionId).HasColumnType("char(36)");

                e.HasOne(x => x.Attempt)
                 .WithMany(a => a.Answers)
                 .HasForeignKey(x => x.AttemptId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(x => x.Question)
                 .WithMany()
                 .HasForeignKey(x => x.QuestionId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(x => x.Option)
                 .WithMany()
                 .HasForeignKey(x => x.OptionId)
                 .OnDelete(DeleteBehavior.SetNull);

                e.Property(x => x.FreeText).HasMaxLength(4000);
                e.Property(x => x.ValueInt);
            });

            // ---- NewsItem
            mb.Entity<NewsItem>(e =>
            {
                e.HasKey(x => x.NewsId);
                e.Property(x => x.NewsId).HasColumnType("char(36)");
                e.Property(x => x.Source).IsRequired().HasMaxLength(200);
                e.Property(x => x.Url).IsRequired().HasMaxLength(1000);
                e.Property(x => x.Headline).IsRequired().HasMaxLength(500);
                e.Property(x => x.PublishedAt).HasColumnType("datetime(6)");
                e.Property(x => x.RawJson).HasColumnType("longtext");
                e.HasIndex(x => x.Url).IsUnique();
            });

            // ---- CivicHub (1:1 with User, PK=FK=UserId)
            mb.Entity<CivicHub>(e =>
            {
                e.HasKey(x => x.UserId);
                e.Property(x => x.UserId).HasColumnType("char(36)");
                e.Property(x => x.UpdatedAt).HasColumnType("datetime(6)");
            });

            // ---- CivicHubAlignedRep (join w/ payload)
            mb.Entity<CivicHubAlignedRep>(e =>
            {
                e.HasKey(x => new { x.UserId, x.RepId });
                e.Property(x => x.UserId).HasColumnType("char(36)");
                e.Property(x => x.RepId).HasColumnType("char(36)");

                e.HasOne(x => x.CivicHub)
                 .WithMany(h => h.AlignedReps)
                 .HasForeignKey(x => x.UserId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(x => x.Rep)
                 .WithMany()
                 .HasForeignKey(x => x.RepId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.Property(x => x.Score).HasPrecision(10, 4);
            });

            // ---- CivicHubCuratedNews (join w/ payload)
            mb.Entity<CivicHubCuratedNews>(e =>
            {
                e.HasKey(x => new { x.UserId, x.NewsId });
                e.Property(x => x.UserId).HasColumnType("char(36)");
                e.Property(x => x.NewsId).HasColumnType("char(36)");

                e.HasOne(x => x.CivicHub)
                 .WithMany(h => h.CuratedNews)
                 .HasForeignKey(x => x.UserId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(x => x.News)
                 .WithMany()
                 .HasForeignKey(x => x.NewsId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.Property(x => x.Score).HasPrecision(10, 4);
            });
        }
    }
}
