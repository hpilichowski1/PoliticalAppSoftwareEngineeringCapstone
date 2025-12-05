using PoliticalApp.Models;
using PoliticalApp.ViewModels;

namespace PoliticalApp.Views
{
    public partial class BillsPage : ContentPage
    {
        private readonly BillsViewModel _vm;

        public BillsPage(BillsViewModel vm)
        {
            InitializeComponent();
            BindingContext = _vm = vm;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (_vm.Bills.Count == 0 && _vm.LoadCommand.CanExecute(null))
            {
                _vm.LoadCommand.Execute(null);
            }
        }
    }
}
