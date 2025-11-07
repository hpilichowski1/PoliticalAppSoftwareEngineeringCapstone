using PoliticalApp.ViewModels;

namespace PoliticalApp.Views
{
    /// <summary>
    /// Page for displaying policies and casting simulated votes.
    /// </summary>
    public partial class PoliciesPage : ContentPage
    {
        public PoliciesPage(PoliciesViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
