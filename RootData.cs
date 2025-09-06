using System.Collections.ObjectModel;

namespace WpfApp2_Nikita
{
    internal class RootData
    {
        public ObservableCollection<NumberRow> NumberRows { get; set; } = new();
        public ObservableCollection<NumberRowSell> NumberRowsSells { get; set; } = new();
    }
}
