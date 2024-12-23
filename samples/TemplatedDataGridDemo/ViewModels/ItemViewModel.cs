using Avalonia;

namespace TemplatedDataGridDemo.ViewModels
{
    public partial class ItemViewModel : ViewModelBase
    {
        [Reactive]
        public partial string Column1 { get; set; }
        
        [Reactive]
        public partial string Column2 { get; set; }
        
        [Reactive]
        public partial string Column3 { get; set; }
        
        [Reactive]
        public partial bool Column4 { get; set; }
        
        [Reactive]
        public partial int Column5 { get; set; }
        
        [Reactive]
        public partial Thickness Margin { get; set; }

        public ItemViewModel(string column1, string column2, string column3, bool column4, int column5, Thickness margin)
        {
            _column1 = column1;
            _column2 = column2;
            _column3 = column3;
            _column4 = column4;
            _column5 = column5;
            _margin = margin;
        }

        public override string ToString()
        {
            return $"{_column5}";
        }
    }
}
