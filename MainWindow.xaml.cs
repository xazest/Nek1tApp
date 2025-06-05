using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;


namespace WpfApp2_Nikita
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _firstLaunch=true;
        private string _filePath = Path.Combine("data", DateTime.Today.ToString("dd-MM-yyyy") + ".json");


        public ObservableCollection<NumberRow> Rows { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            Rows = new ObservableCollection<NumberRow> { };
            DataContext = this;
            Calendar.SelectedDate = DateTime.Today;
            Calendar.DisplayDate = DateTime.Today;

        }
        private void DataGrid_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (e.Text == ",")
            {
                e.Handled = true;
                var textBox = Keyboard.FocusedElement as TextBox;
                if (textBox != null)
                {
                    int caret = textBox.CaretIndex;
                    textBox.Text = textBox.Text.Insert(caret, ".");
                    textBox.CaretIndex = caret + 1;
                }

            }
        }

        private void DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.Column.DisplayIndex == 1 && e.EditAction == DataGridEditAction.Commit)
            {
                var row = (NumberRow)e.Row.Item;
                var textBox = e.EditingElement as TextBox;
                if (textBox != null && double.TryParse(textBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double newValue))
                {
                    if (newValue != row.Number2)
                    {
                        double number1 = row.Number1;
                        double number2 = newValue;
                        double newNumber2 = number1 * (number2 / 100);
                        row.Number2 = Math.Round(newNumber2);
                    }
                }
            }
            SumChange();

        }

        private void SaveButton(object sender, RoutedEventArgs e)
        {
            SaveData();
        }


        private void Calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_firstLaunch) 
            { 
                SaveData(); 
            }
            DateTime? selectedDate = Calendar.SelectedDate;
            if (selectedDate.HasValue)
            {
                _filePath = Path.Combine("data", selectedDate.Value.ToString("dd-MM-yyyy") + ".json");

                if (File.Exists(_filePath))
                {
                    var json = File.ReadAllText(_filePath);
                    var loadedRows = JsonConvert.DeserializeObject<ObservableCollection<NumberRow>>(json);

                    Rows.Clear();
                    foreach (var row in loadedRows)
                    {
                        Rows.Add(row);
                    }
                }
                else
                {
                    Rows.Clear();
                }
            }
            SumChange();
            Mouse.Capture(null);
            _firstLaunch = false;
        }


        private void SumChange()
        {
            Number2Sum.Text = Rows.Sum(r => r.Number2).ToString();
            ResultSum.Text = Rows.Sum(r => r.Result).ToString();
        }

        private void dataGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var click = VisualTreeHelper.HitTest(dataGrid, e.GetPosition(dataGrid));
            if (click == null || !IsChildOfDataGrid(click.VisualHit))
            {
                dataGrid.CommitEdit(DataGridEditingUnit.Cell, true);
                dataGrid.CommitEdit(DataGridEditingUnit.Row, true);
                Keyboard.ClearFocus();
            }


        }
        private bool IsChildOfDataGrid(DependencyObject element)
        {
            while (element != null && element != dataGrid)
            {
                if (element is System.Windows.Controls.DataGridCell) { return true; }
                element = VisualTreeHelper.GetParent(element);
            }
            return false;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveData();
        }
        private void SaveData()
        {
            Directory.CreateDirectory("data");
            if (Rows.Count != 0)
            {
                var json = JsonConvert.SerializeObject(Rows, Formatting.Indented);
                File.WriteAllText(_filePath, json);
            }
        }
    }

}