
namespace PoliticalApp.Api.Models.EntityModels
{
    public record Uuid(string Value)
    {
        public static implicit operator string(Uuid id) => id.Value;
        public static implicit operator Uuid(string v) => new(v);
    }

    // --- Users
    public class User
    {
        public string UserId { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string Role { get; set; } = "citizen"; // enum in DB
        public string? PasswordHash { get; set; }

        public CivicHub? CivicHub { get; set; }
        public ICollection<VoteSimulation> VoteSimulations { get; set; } = new List<VoteSimulation>();
        public ICollection<UserQuizAttempt> QuizAttempts { get; set; } = new List<UserQuizAttempt>();
    }

    public class Representative
    {
        public string RepId { get; set; } = Guid.NewGuid().ToString();
        public string? UserId { get; set; }
        public User? User { get; set; }

        public string Name { get; set; } = "";
        public string District { get; set; } = "";
        public ICollection<VoteRecord> VotingHistory { get; set; } = [];
    }

    public class Candidate
    {
        public string CandidateId { get; set; } = Guid.NewGuid().ToString();
        public string? UserId { get; set; }
        public User? User { get; set; }

        public string Name { get; set; } = "";
        public string? CampaignInfoJson { get; set; }
    }

    public class Legislation
    {
        public string BillId { get; set; } = "";
        public string Title { get; set; } = "";
        public string? Summary { get; set; }
        public string Status { get; set; } = "introduced";

        public ICollection<VoteRecord> VoteRecords { get; set; } = [];
        public ICollection<VoteSimulation> Simulations { get; set; } = [];
    }

    public class VoteRecord
    {
        public long VoteId { get; set; }
        public string RepId { get; set; } = "";
        public Representative Rep { get; set; } = null!;
        public string BillId { get; set; } = "";
        public Legislation Bill { get; set; } = null!;
        public string Vote { get; set; } = "YEA"; // enum in DB
        public DateTime VotedAt { get; set; }
    }

    public class VoteSimulation
    {
        public string SimulationId { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; } = "";
        public User User { get; set; } = null!;
        public string BillId { get; set; } = "";
        public Legislation Bill { get; set; } = null!;
        public string SelectedVote { get; set; } = "YEA";
        public string? ComparedRepId { get; set; }
        public Representative? ComparedRep { get; set; }
        public string? CompareResult { get; set; } // MATCH/DIFFER/N/A
    }

    public class Quiz
    {
        public string QuizId { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; } = "";
        public ICollection<QuizQuestion> QuizQuestions { get; set; } = [];
    }

    public class Question
    {
        public string QuestionId { get; set; } = Guid.NewGuid().ToString();
        public string Text { get; set; } = "";
        public string? Topic { get; set; }
        public ICollection<QuestionOption> Options { get; set; } = [];
        public ICollection<QuizQuestion> QuizQuestions { get; set; } = [];
    }

    public class QuestionOption
    {
        public string OptionId { get; set; } = Guid.NewGuid().ToString();
        public string QuestionId { get; set; } = "";
        public Question Question { get; set; } = null!;
        public string Label { get; set; } = "";
        public int? ValueInt { get; set; }
        public string? ValueJson { get; set; } // store JSON as string
    }

    public class UserQuizAttempt
    {
        public string AttemptId { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; } = "";
        public User User { get; set; } = null!;
        public string QuizId { get; set; } = "";
        public Quiz Quiz { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? AlignmentJson { get; set; }
        public ICollection<UserQuizAnswer> Answers { get; set; } = [];
    }

    public class UserQuizAnswer
    {
        public string AttemptId { get; set; } = "";
        public UserQuizAttempt Attempt { get; set; } = null!;
        public string QuestionId { get; set; } = "";
        public Question Question { get; set; } = null!;
        public string? OptionId { get; set; }
        public QuestionOption? Option { get; set; }
        public string? FreeText { get; set; }
        public int? ValueInt { get; set; }
    }

    public class NewsItem
    {
        public string NewsId { get; set; } = Guid.NewGuid().ToString();
        public string Source { get; set; } = "";
        public string Url { get; set; } = "";
        public string Headline { get; set; } = "";
        public DateTime? PublishedAt { get; set; }
        public string? RawJson { get; set; }
    }

    public class CivicHub
    {
        public string UserId { get; set; } = "";
        public User User { get; set; } = null!;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<CivicHubAlignedRep> AlignedReps { get; set; } = [];
        public ICollection<CivicHubCuratedNews> CuratedNews { get; set; } = [];
    }

    public class CivicHubAlignedRep
    {
        public string UserId { get; set; } = "";
        public CivicHub CivicHub { get; set; } = null!;
        public string RepId { get; set; } = "";
        public Representative Rep { get; set; } = null!;
        public int? Rank { get; set; }
        public decimal? Score { get; set; }
    }

    public class CivicHubCuratedNews
    {
        public string UserId { get; set; } = "";
        public CivicHub CivicHub { get; set; } = null!;
        public string NewsId { get; set; } = "";
        public NewsItem News { get; set; } = null!;
        public decimal? Score { get; set; }
    }
}
