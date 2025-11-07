namespace PoliticalAppAPI.Models
{
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
}