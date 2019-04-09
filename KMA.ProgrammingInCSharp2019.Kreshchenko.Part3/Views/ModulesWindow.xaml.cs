using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace KMA.ProgrammingInCSharp2019.Kreshchenko.Part3.Views
{
    /// <summary>
    /// Interaction logic for ModulesWindow.xaml
    /// </summary>
    public partial class ModulesWindow : Window
    {
        public ObservableCollection<Tuple<string, string>> Modules { get; }

        public ModulesWindow(List<Tuple<string, string>> modules)
        {
            Modules = new ObservableCollection<Tuple<string, string>>(modules);
            InitializeComponent();
            DataContext = this;
        }
    }
}
