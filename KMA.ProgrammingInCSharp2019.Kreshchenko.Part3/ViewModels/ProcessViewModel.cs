using System;
using System.Diagnostics;
using System.Windows.Input;
using KMA.ProgrammingInCSharp2019.Kreshchenko.Part3.Models;
using KMA.ProgrammingInCSharp2019.Kreshchenko.Part3.Tools;
using KMA.ProgrammingInCSharp2019.Kreshchenko.Part3.Views;

namespace KMA.ProgrammingInCSharp2019.Kreshchenko.Part3.ViewModels
{
    internal class ProcessViewModel : BaseNotifyProperty
    {
        public ProcessInfo ProcessInfo { get; }

        #region Commands
        private ICommand _showModulesCommand;
        private ICommand _showThreadsCommand;
        private ICommand _openFileLocationCommand;
        private ICommand _killCommand;

        public ICommand ShowModulesCommand => 
            _showModulesCommand ?? (_showModulesCommand = new RelayCommand<object>(ShowModulesImplementation, CanSeeModules));

        public ICommand ShowThreadsCommand => 
            _showThreadsCommand ?? (_showThreadsCommand = new RelayCommand<object>(ShowThreadsImplementation));

        public ICommand OpenFileLocationCommand => 
            _openFileLocationCommand ?? (_openFileLocationCommand = 
                new RelayCommand<object>(OpenFileLocationImplementation, CanOpenFilePath));

        public ICommand KillCommand => 
            _killCommand ?? (_killCommand = new RelayCommand<object>(KillImplementation, CanKillProcess));
        #endregion

        private bool CanSeeModules(object obj)
        {
            try
            {
                ProcessModule test = ProcessInfo.CurrentProcess.Modules[0];
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool CanOpenFilePath(object obj)
        {
            return !String.IsNullOrEmpty(ProcessInfo.FilePath);
        }

        private bool CanKillProcess(object obj)
        {
            return ProcessInfo.FullyAccessible;
        }

        public ProcessViewModel(Process process)
        {
            ProcessInfo = new ProcessInfo(process);
        }

        #region Implementations
        private void ShowModulesImplementation(object obj)
        {
            ModulesWindow modulesWindow = new ModulesWindow(ProcessInfo.Modules);
            modulesWindow.Show();
        }

        private void ShowThreadsImplementation(object obj)
        {
            ThreadsWindow threadsWindow = new ThreadsWindow(ProcessInfo.Threads);
            threadsWindow.Show();
        }

        private void OpenFileLocationImplementation(object obj)
        {
            Process.Start(ProcessInfo.FilePath.Substring(0, 
                ProcessInfo.FilePath.LastIndexOf('\\')));
        }

        private void KillImplementation(object obj)
        {
            ProcessInfo.KillProcess();
        }
        #endregion
    }
}
