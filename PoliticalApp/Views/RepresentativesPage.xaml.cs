using System.Linq;
using Microsoft.Maui.Controls;
using PoliticalApp.Models;
using PoliticalApp.ViewModels;

namespace PoliticalApp.Views
{
    public partial class RepresentativesPage : ContentPage
    {
        private RepresentativesViewModel ViewModel => (RepresentativesViewModel)BindingContext;

        public RepresentativesPage(RepresentativesViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // TODO: replace "FL" with the user's state (or pass in via navigation params)
            if (!ViewModel.HasLoaded)
            {
                await ViewModel.LoadRepresentativesAsync("FL");
            }
        }

        private async void OnRepresentativeSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection?.FirstOrDefault() is Representative rep)
            {
                // For now just show a simple detail popup.
                await DisplayAlert(rep.Name, rep.Bio, "OK");

                if (sender is CollectionView cv)
                {
                    cv.SelectedItem = null;
                }
            }
        }
    }
}
