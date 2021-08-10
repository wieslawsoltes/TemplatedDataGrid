using ReactiveUI;

namespace Demo.ViewModels
{
    public class ItemViewModel : ViewModelBase
    {
        private string _column1;
        private string _column2;
        private string _column3;

        public string Column1
        {
            get => _column1;
            set => this.RaiseAndSetIfChanged(ref _column1, value);
        }

        public string Column2
        {
            get => _column2;
            set => this.RaiseAndSetIfChanged(ref _column2, value);
        }

        public string Column3
        {
            get => _column3;
            set => this.RaiseAndSetIfChanged(ref _column3, value);
        }
    }
}