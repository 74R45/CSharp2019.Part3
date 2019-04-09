using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using KMA.ProgrammingInCSharp2019.Kreshchenko.Part3.Tools;

namespace KMA.ProgrammingInCSharp2019.Kreshchenko.Part3.ViewModels
{
    internal class TaskDataViewModel : BaseNotifyProperty, ILoaderOwner
    {
        private readonly Dictionary<int, string> _processProperties = new Dictionary<int, string>();

        #region Fields
        private ObservableCollection<ProcessViewModel> _processes;
        private int _sortedColumnIndex = -1;
        private ListSortDirection _sortDirection;
        private Visibility _loaderVisibility = Visibility.Hidden;
        private bool _isControlEnabled = true;
        #endregion

        #region Properties
        public ObservableCollection<ProcessViewModel> Processes
        {
            get => _processes;
            private set
            {
                _processes = value;
                OnPropertyChanged();
            }
        }

        public Visibility LoaderVisibility
        {
            get => _loaderVisibility;
            set
            {
                _loaderVisibility = value;
                OnPropertyChanged();
            }
        }
        public bool IsControlEnabled
        {
            get => _isControlEnabled;
            set
            {
                _isControlEnabled = value;
                OnPropertyChanged();
            }
        }
        #endregion

        public TaskDataViewModel()
        {
            LoaderManager.Instance.Initialize(this);
            Processes = new ObservableCollection<ProcessViewModel>();
            Initialize();
        }

        private async void Initialize()
        {
            LoaderManager.Instance.ShowLoader();
            await Task.Run(() =>
            {
                foreach (Process process in Process.GetProcesses())
                {
                    Application.Current.Dispatcher.Invoke(delegate
                    {
                        Processes.Add(new ProcessViewModel(process)); 
                    });
                }
            });
            LoaderManager.Instance.HideLoader();
            Thread updateThread = new Thread(UpdateProcesses);
            _processProperties.Add(0, "Name");
            _processProperties.Add(1, "Id");
            _processProperties.Add(2, "Active");
            _processProperties.Add(3, "Cpu");
            _processProperties.Add(4, "MemoryPercentage");
            _processProperties.Add(5, "Memory");
            _processProperties.Add(6, "NumberOfThreads");
            _processProperties.Add(7, "Username");
            _processProperties.Add(8, "FilePath");
            _processProperties.Add(9, "StartTime");
            updateThread.Start();
        }

        private void UpdateProcesses()
        {
            try
            {
                while (true)
                {
                    // Updating metadata, removing processes that has ended.
                    for (int i = 0; i < 2; i++)
                    {
                        List<ProcessViewModel> toRemove = new List<ProcessViewModel>();
                        foreach (ProcessViewModel process in Processes)
                        {
                            if (!process.ProcessInfo.UpdateMetadata())
                            {
                                toRemove.Add(process);
                            }
                        }
                        SortProcesses();

                        foreach (ProcessViewModel processViewModel in toRemove)
                        {
                            Application.Current.Dispatcher.Invoke(delegate
                            {
                                Processes.Remove(processViewModel);
                            });
                        }
                        Thread.Sleep(1000);
                    }

                    // Updating process list.
                    foreach (Process process in Process.GetProcesses())
                    {
                        bool processPresent = false;
                        foreach (ProcessViewModel processViewModel in Processes)
                        {
                            if (process.Id == processViewModel.ProcessInfo.Id)
                            {
                                processPresent = true;
                                break;
                            }
                        }
                        if (!processPresent)
                        {
                            Application.Current.Dispatcher.Invoke(delegate
                            {
                                _processes.Add(new ProcessViewModel(process));
                                SortProcesses();
                            });
                        }
                    }
                }
            }
            catch (NullReferenceException)
            {
                Environment.Exit(0);
            }
        }
        
        public void OnSortUpdated(object sender, DataGridSortingEventArgs e)
        {
            _sortDirection = e.Column.SortDirection ?? ListSortDirection.Ascending;
            _sortedColumnIndex = e.Column.DisplayIndex;
            SortProcesses();
        }

        private void SortProcesses()
        {
            if (_sortedColumnIndex == -1)
            {
                return;
            }

            if (_sortDirection == ListSortDirection.Ascending)
            {
                Processes = new ObservableCollection<ProcessViewModel>(_processes.OrderBy(x =>
                    GetPropValue(x.ProcessInfo, _processProperties[_sortedColumnIndex])));
            }
            else
            {
                Processes = new ObservableCollection<ProcessViewModel>(_processes.OrderByDescending(x =>
                    GetPropValue(x.ProcessInfo, _processProperties[_sortedColumnIndex])));
            }
        }

        private static object GetPropValue(object src, string propName)
        {
            return src.GetType().GetProperty(propName)?.GetValue(src, null);
        }
    }
}