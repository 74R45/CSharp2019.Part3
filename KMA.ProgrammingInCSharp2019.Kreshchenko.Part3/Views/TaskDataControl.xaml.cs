using System.Windows.Controls;
using KMA.ProgrammingInCSharp2019.Kreshchenko.Part3.ViewModels;

namespace KMA.ProgrammingInCSharp2019.Kreshchenko.Part3.Views
{
    /// <summary>
    /// Interaction logic for TaskDataControl.xaml
    /// </summary>
    public partial class TaskDataControl : UserControl
    {
        private TaskDataViewModel _viewModel;

        public TaskDataControl()
        {
            InitializeComponent();
            _viewModel = new TaskDataViewModel();
            DataContext = _viewModel;
        }

        private void OnSortUpdated(object sender, DataGridSortingEventArgs e)
        {
            _viewModel.OnSortUpdated(sender, e);
        }
    }
}