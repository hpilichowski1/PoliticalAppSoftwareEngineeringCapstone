using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace PoliticalApp.ViewModels
{
    public partial class QuizViewModel : ObservableObject
    {
        private readonly IAlignmentService _alignment;

        public QuizViewModel(IAlignmentService alignment)
        {
            _alignment = alignment;

            Questions = new ObservableCollection<QuizQuestion>
            {
                new("Government should invest heavily in renewable energy."),
                new("Taxes should be increased for top earners."),
                new("Healthcare should be publicly funded."),
                new("Immigration should be expanded."),
                new("Gun control laws should be stricter."),
                new("The U.S. should reduce military spending."),
                new("Climate change is a top priority."),
                new("Corporations require more regulation."),
                new("Education should be tuition-free."),
                new("Social programs should be expanded.")
            };
        }

        [ObservableProperty]
        public ObservableCollection<QuizQuestion> questions;

        [RelayCommand]
        void SelectAnswer(AnswerSelection selection)
        {
            if (selection?.Question == null)
                return;

            selection.Question.Answer = selection.Value;
        }

        [RelayCommand]
        public async Task SubmitQuiz()
        {
            int sum = Questions.Sum(q => q.Answer);
            int score = (int)(((double)sum / (Questions.Count * 5)) * 100);

            await _alignment.SubmitScoreAsync(score);

            await Shell.Current.DisplayAlert("Submitted", $"Your score: {score}%", "OK");

            await Shell.Current.GoToAsync("..");
        }
    }

    public class QuizQuestion : ObservableObject
    {
        public QuizQuestion(string text) { Text = text; }
        public string Text { get; }
        public int Answer { get; set; }
    }

    public class AnswerSelection {
        public required QuizQuestion Question { get; set; }
        public int Value { get; set; }
    }
}
