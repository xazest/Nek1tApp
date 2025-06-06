using System.ComponentModel;


namespace WpfApp2_Nikita
{
    public class NumberRowSell : INotifyPropertyChanged
    {
        private double _number1;
        private double _percent;
        private double _number2;

        public double Number1
        {
            get => _number1;
            set
            {
                if (_number1 != value)
                {
                    _number1 = value;
                    UpdateNumber2();
                    OnPropertyChanged(nameof(Number1));
                    OnPropertyChanged(nameof(Result));
                }
            }
        }

        public double Percent
        {
            get => _percent;
            set
            {
                if (_percent != value)
                {
                    _percent = value;
                    UpdateNumber2();
                    OnPropertyChanged(nameof(Percent));
                    OnPropertyChanged(nameof(Result));
                }
            }
        }

        public double Number2
        {
            get => _number2;
            set
            {
                if (_number2 != value)
                {
                    _number2 = value;
                    OnPropertyChanged(nameof(Number2));
                    OnPropertyChanged(nameof(Result));

                }
            }
        }

        private void UpdateNumber2()
        {
            Number2 = Math.Round(Number1 * Percent / 100);
            OnPropertyChanged(nameof(Result));
        }
        public int ConvertPercent()
        {
            int x = Convert.ToInt32(_number1 - _number2);
            return x;
        }

        public int Result => ConvertPercent();



        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
