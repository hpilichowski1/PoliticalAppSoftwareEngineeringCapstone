using PoliticalApp.ViewModels;

namespace PoliticalApp.Views
{
    /// <summary>
    /// Page for displaying and searching representatives.
    /// </summary>
    public partial class RepresentativesPage : ContentPage
    {
        public RepresentativesPage(RepresentativesViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        private void OnRepresentativeSelected(object sender, SelectionChangedEventArgs e)
        {
            if (sender is CollectionView collectionView)
            {
                collectionView.SelectedItem = null;
            }
        }
    }
}
