namespace PoliticalAppAPI.Models
{
    public class Quiz
    {
        public string QuizId { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; } = "";
        public ICollection<QuizQuestion> QuizQuestions { get; set; } = [];
    }

    public class QuizQuestion
    {
        public string QuizId { get; set; } = "";
        public Quiz Quiz { get; set; } = null!;
        public string QuestionId { get; set; } = "";
        public Question Question { get; set; } = null!;
        public int Position { get; set; }
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
}