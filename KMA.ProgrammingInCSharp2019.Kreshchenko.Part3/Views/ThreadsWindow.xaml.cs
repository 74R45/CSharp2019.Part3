using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;

namespace KMA.ProgrammingInCSharp2019.Kreshchenko.Part3.Views
{
    /// <summary>
    /// Interaction logic for ThreadsWindow.xaml
    /// </summary>
    public partial class ThreadsWindow : Window
    {
        public ObservableCollection<Tuple<int, ThreadState, DateTime?>> Threads { get; }

        public ThreadsWindow(List<Tuple<int, ThreadState, DateTime?>> threads)
        {
            Threads = new ObservableCollection<Tuple<int, ThreadState, DateTime?>>(threads);
            InitializeComponent();
            DataContext = this;
        }
    }
}
