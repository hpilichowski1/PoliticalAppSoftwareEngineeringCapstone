namespace PoliticalApp.Api.Data.DBContext
{
    public class CivicDb(DbContextOptions<CivicDb> options) : DbContext(options), DbContext
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<Representative> Representatives => Set<Representative>();
        public DbSet<Candidate> Candidates => Set<Candidate>();
        public DbSet<Legislation> Legislation => Set<Legislation>();
        public DbSet<VoteRecord> VoteRecords => Set<VoteRecord>();
        public DbSet<VoteSimulation> VoteSimulations => Set<VoteSimulation>();
        public DbSet<Quiz> Quizzes => Set<Quiz>();
        public DbSet<Question> Questions => Set<Question>();
        public DbSet<QuestionOption> QuestionOptions => Set<QuestionOption>();
        public DbSet<QuizQuestion> QuizQuestions => Set<QuizQuestion>();
        public DbSet<UserQuizAttempt> UserQuizAttempts => Set<UserQuizAttempt>();
        public DbSet<UserQuizAnswer> UserQuizAnswers => Set<UserQuizAnswer>();
        public DbSet<NewsItem> NewsItems => Set<NewsItem>();
        public DbSet<CivicHub> CivicHubs => Set<CivicHub>();
        public DbSet<CivicHubAlignedRep> CivicHubAlignedReps => Set<CivicHubAlignedRep>();
        public DbSet<CivicHubCuratedNews> CivicHubCuratedNews => Set<CivicHubCuratedNews>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            // keys
            b.Entity<Legislation>().HasKey(x => x.BillId);
            b.Entity<QuizQuestion>().HasKey(x => new { x.QuizId, x.QuestionId });
            b.Entity<UserQuizAnswer>().HasKey(x => new { x.AttemptId, x.QuestionId });
            b.Entity<CivicHub>().HasKey(x => x.UserId);
            b.Entity<CivicHubAlignedRep>().HasKey(x => new { x.UserId, x.RepId });
            b.Entity<CivicHubCuratedNews>().HasKey(x => new { x.UserId, x.NewsId });

            // relationships
            b.Entity<Representative>()
                .HasOne(r => r.User).WithOne().HasForeignKey<Representative>(r => r.UserId);
            b.Entity<Candidate>()
                .HasOne(c => c.User).WithOne().HasForeignKey<Candidate>(c => c.UserId);
            b.Entity<VoteRecord>()
                .HasOne(v => v.Rep).WithMany(r => r.VotingHistory).HasForeignKey(v => v.RepId);
            b.Entity<VoteRecord>()
                .HasOne(v => v.Bill).WithMany(l => l.VoteRecords).HasForeignKey(v => v.BillId);
            b.Entity<VoteSimulation>()
                .HasOne(vs => vs.User).WithMany(u => u.VoteSimulations).HasForeignKey(vs => vs.UserId);
            b.Entity<VoteSimulation>()
                .HasOne(vs => vs.Bill).WithMany(l => l.Simulations).HasForeignKey(vs => vs.BillId);
            b.Entity<VoteSimulation>()
                .HasOne(vs => vs.ComparedRep).WithMany().HasForeignKey(vs => vs.ComparedRepId);
            b.Entity<QuizQuestion>()
                .HasOne(qq => qq.Quiz).WithMany(q => q.QuizQuestions).HasForeignKey(qq => qq.QuizId);
            b.Entity<QuizQuestion>()
                .HasOne(qq => qq.Question).WithMany(q => q.QuizQuestions).HasForeignKey(qq => qq.QuestionId);
            b.Entity<QuestionOption>()
                .HasOne(o => o.Question).WithMany(q => q.Options).HasForeignKey(o => o.QuestionId);
            b.Entity<UserQuizAttempt>()
                .HasOne(a => a.User).WithMany(u => u.QuizAttempts).HasForeignKey(a => a.UserId);
            b.Entity<UserQuizAttempt>()
                .HasOne(a => a.Quiz).WithMany().HasForeignKey(a => a.QuizId);
            b.Entity<UserQuizAnswer>()
                .HasOne(a => a.Attempt).WithMany(p => p.Answers).HasForeignKey(a => a.AttemptId);
            b.Entity<UserQuizAnswer>()
                .HasOne(a => a.Question).WithMany().HasForeignKey(a => a.QuestionId);
            b.Entity<UserQuizAnswer>()
                .HasOne(a => a.Option).WithMany().HasForeignKey(a => a.OptionId);
            b.Entity<CivicHub>()
                .HasOne(ch => ch.User).WithOne(u => u.CivicHub).HasForeignKey<CivicHub>(ch => ch.UserId);
            b.Entity<CivicHubAlignedRep>()
                .HasOne(x => x.CivicHub).WithMany(ch => ch.AlignedReps).HasForeignKey(x => x.UserId);
            b.Entity<CivicHubAlignedRep>()
                .HasOne(x => x.Rep).WithMany().HasForeignKey(x => x.RepId);
            b.Entity<CivicHubCuratedNews>()
                .HasOne(x => x.CivicHub).WithMany(ch => ch.CuratedNews).HasForeignKey(x => x.UserId);
            b.Entity<CivicHubCuratedNews>()
                .HasOne(x => x.News).WithMany().HasForeignKey(x => x.NewsId);

            // column sizes/indexes (samples)
            b.Entity<User>().HasIndex(u => u.Email).IsUnique();
            b.Entity<NewsItem>().HasIndex(n => n.Url).IsUnique();
        }
    }
}