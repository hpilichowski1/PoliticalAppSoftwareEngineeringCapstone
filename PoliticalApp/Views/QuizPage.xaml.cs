using PoliticalApp.ViewModels;
using PoliticalApp.Models;

namespace PoliticalApp.Views;

public partial class QuizPage : ContentPage
{
    public QuizPage(QuizViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    private void OnAnswerSelected(object sender, CheckedChangedEventArgs e)
    {
        if (!e.Value)
            return;

        var rb = (RadioButton)sender;

        // The question bound to this visual item
        var question = (QuizQuestion)rb.BindingContext;

        if (rb.Value is null)
            return;

        int value = int.Parse(rb.Value.ToString()!);

        var vm = (QuizViewModel)BindingContext;

        vm.SelectAnswerCommand.Execute(new AnswerSelection
        {
            Question = question,
            Value = value
        });
    }
}
