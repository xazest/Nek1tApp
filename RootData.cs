﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp2_Nikita
{
    internal class RootData
    {
        public ObservableCollection<NumberRow> NumberRows { get; set; } = new();
        public ObservableCollection<NumberRowSell> NumberRowsSells { get; set; } = new();

      
    }
}
