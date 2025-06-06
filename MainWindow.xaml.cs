using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq.Expressions;
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
        private bool _firstLaunch = true;
        public string _filePath = Path.Combine("data", DateTime.Today.ToString("dd-MM-yyyy") + ".json");


        public ObservableCollection<NumberRow> Rows { get; set; }
        public ObservableCollection<NumberRowSell> RowsSell { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            Rows = new ObservableCollection<NumberRow> { };
            RowsSell = new ObservableCollection<NumberRowSell> { };

            DataContext = this;
            Calendar.SelectedDate = DateTime.Today;
            Calendar.DisplayDate = DateTime.Today;

        }
        private void DataGrid_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (e.Text == ".")
            {
                e.Handled = true;
                var textBox = Keyboard.FocusedElement as TextBox;
                if (textBox != null)
                {
                    int caret = textBox.CaretIndex;
                    textBox.Text = textBox.Text.Insert(caret, ",");
                    textBox.CaretIndex = caret + 1;
                }

            }
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
                    var data = JsonConvert.DeserializeObject<RootData>(json);

                    Rows.Clear();

                    foreach (var row in data.NumberRows)
                    {
                        Rows.Add(row);
                    }


                    foreach (var row in data.NumberRowsSells)
                    {
                        RowsSell.Add(row);
                    }
                }
                else
                {
                    Rows.Clear();
                    RowsSell.Clear();
                }
            }
            SumChange();
            Mouse.Capture(null);
            _firstLaunch = false;
        }

        private void SumChange()
        {
            Number2Sum.Text = Rows.Sum(r => r.Number2).ToString();
            ResultSum.Text = (Rows.Sum(r => r.Result) - RowsSell.Sum(r => r.Result)).ToString();
            Profit.Text = (RowsSell.Sum(r => r.Number2) - Rows.Sum(r => r.Number2)).ToString();
        }

        private void dataGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (sender is not DataGrid grid) { return; }

                var click = VisualTreeHelper.HitTest(grid, e.GetPosition(grid));
                if (click == null || !IsChildOfDataGrid(click.VisualHit, grid))
                {
                    dataGrid.CommitEdit(DataGridEditingUnit.Cell, true);
                    dataGrid.CommitEdit(DataGridEditingUnit.Row, true);
                    dataGridSell.CommitEdit(DataGridEditingUnit.Cell, true);
                    dataGridSell.CommitEdit(DataGridEditingUnit.Row, true);
                    dataGrid.SelectedCells.Clear();
                    dataGrid.SelectedItem = null;
                    dataGridSell.SelectedCells.Clear();
                    dataGridSell.SelectedItem = null;
                }
                Keyboard.ClearFocus();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"error dataGrid_PreviewMouseDown metod {ex.Message}{ex.StackTrace}");
            }

        }
        private bool IsChildOfDataGrid(DependencyObject element, DataGrid grid)
        {
            while (element != null && element != grid)
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
            var data = new RootData
            {
                NumberRows = Rows,
                NumberRowsSells = RowsSell
            };
            if (Rows.Count != 0 && RowsSell.Count != 0)
            {
                var json = JsonConvert.SerializeObject(data, Formatting.Indented);
                File.WriteAllText(_filePath, json);
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is TextBox textbox && double.TryParse(textbox.Text, out double newValue))
                {
                    var data = textbox.DataContext;
                    switch (data)
                    {
                        case NumberRow buyRow:
                            buyRow.Percent = newValue;
                            break;

                        case NumberRowSell sellRow:
                            sellRow.Percent = newValue;
                            break;
                    }

                }
                SumChange();
            }
            catch (Exception ex)
            { MessageBox.Show($"error TextBox_LostFocus\n\n{ex.Message}\n\n{ex.StackTrace}"); }

        }
    }

}