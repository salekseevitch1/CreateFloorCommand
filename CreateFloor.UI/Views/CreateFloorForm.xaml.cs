using CreateFloor.UI.ViewModels;
using System.Text.RegularExpressions;
using System.Windows;

namespace CreateFloor.UI.Views
{
    public partial class CreateFloorWindow : Window
    {
        private CreateFloorViewModel _createFloorViewModel;

        public CreateFloorWindow(CreateFloorViewModel createFloorViewModel)
        {
            _createFloorViewModel = createFloorViewModel;
            this.InitializeComponent();
            this.DataContext = _createFloorViewModel;
        }


        private void IsTextAllowed(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.-]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
