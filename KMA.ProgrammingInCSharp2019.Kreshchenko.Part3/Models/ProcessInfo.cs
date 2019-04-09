using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading;
using System.Windows;
using KMA.ProgrammingInCSharp2019.Kreshchenko.Part3.Tools;
using ThreadState = System.Diagnostics.ThreadState;

namespace KMA.ProgrammingInCSharp2019.Kreshchenko.Part3.Models
{
    internal class ProcessInfo : BaseNotifyProperty
    {
        #region Fields
        
        private string _name;
        private bool _active;
        private double _cpu;
        private double _memory;
        private double _memoryPercentage;
        private int _numberOfThreads;
        private readonly PerformanceCounter _cpuCounter;

        #endregion

        #region Properties

        public string Name
        {
            get => _name;
            private set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public bool Active
        {
            get => _active;
            private set
            {
                _active = value;
                OnPropertyChanged();
            }
        }

        public double Cpu
        {
            get => _cpu;
            private set
            {
                _cpu = value;
                OnPropertyChanged();
            }
        }

        public double Memory
        {
            get => _memory;
            private set
            {
                _memory = value;
                OnPropertyChanged();
            }
        }

        public double MemoryPercentage
        {
            get => _memoryPercentage;
            private set
            {
                _memoryPercentage = value;
                OnPropertyChanged();
            }
        }

        public int NumberOfThreads
        {
            get => _numberOfThreads;
            private set
            {
                _numberOfThreads = value;
                OnPropertyChanged();
            }
        }

        public int Id { get; }
        public string Username { get; }
        public string FilePath { get; }
        public DateTime? StartTime { get; }
        public Process CurrentProcess { get; }

        public List<Tuple<string, string>> Modules
        {
            get
            {
                List<Tuple<string, string>> modules = new List<Tuple<string, string>>();
                try
                {
                    foreach (ProcessModule processModule in CurrentProcess.Modules)
                    {
                        modules.Add(new Tuple<string, string>(processModule.ModuleName, processModule.FileName));
                    }
                }
                catch (Win32Exception) { }
                catch (InvalidOperationException) { }

                return modules;
            }
        }

        public List<Tuple<int, ThreadState, DateTime?>> Threads
        {
            get
            {
                List<Tuple<int, ThreadState, DateTime?>> threads = new List<Tuple<int, ThreadState, DateTime?>>();
                try
                {
                    foreach (ProcessThread processThread in CurrentProcess.Threads)
                    {
                        DateTime? startTime = null;
                        try
                        {
                            startTime = processThread.StartTime;
                        }
                        catch (Win32Exception)
                        {
                        }
                        threads.Add(new Tuple<int, ThreadState, DateTime?>(
                            processThread.Id, processThread.ThreadState, startTime));
                    }
                }
                catch (InvalidOperationException) { }

                return threads;
            }
        }

        public bool FullyAccessible { get; } = true;

        #endregion

        public ProcessInfo(Process process)
        {
            try
            {
                CurrentProcess = process;

                // Process name, ID, whether it's responding.
                Name = process.ProcessName;
                Id = process.Id;
                Active = process.Responding;

                // CPU usage.
                _cpuCounter = new PerformanceCounter("Process",
                    "% Processor Time", process.ProcessName);
                // Due to a bug in PerformanceCounter class
                // the first time NextValue() is called, it freezes for 8 seconds
                // but there doesn't seem to be an alternative.
                _cpuCounter.NextValue();
                Thread.Sleep(5);
                Cpu = Math.Round(_cpuCounter.NextValue() / Environment.ProcessorCount, 2);

                // Process Memory.
                Memory = Math.Round(Convert.ToDouble(process.WorkingSet64) / 1024, 2);
                MemoryPercentage = Math.Round(Convert.ToDouble(process.WorkingSet64) * 100 /
                                              Convert.ToDouble(MemoryCounterHelper.GetTotalPhysicalMemory()), 2);

                // Number of threads, username, path to the file, start time.
                NumberOfThreads = process.Threads.Count;
                Username = GetProcessUser(process);
                try
                {
                    FilePath = process.MainModule.FileName;
                }
                catch (Win32Exception)
                {
                    FullyAccessible = false;
                }

                try
                {
                    StartTime = process.StartTime;
                }
                catch (Win32Exception)
                {
                    FullyAccessible = false;
                }
            }
            catch (InvalidOperationException)
            {
                // Process exited before the constructor reached the end.
            }
        }

        #region ProcessUsernameCalculation
        private static string GetProcessUser(Process process)
        {
            IntPtr processHandle = IntPtr.Zero;
            try
            {
                OpenProcessToken(process.Handle, 8, out processHandle);
                WindowsIdentity wi = new WindowsIdentity(processHandle);
                string user = wi.Name;
                return user.Contains(@"\") ? user.Substring(user.IndexOf(@"\", StringComparison.Ordinal) + 1) : user;
            }
            catch
            {
                return null;
            }
            finally
            {
                if (processHandle != IntPtr.Zero)
                {
                    CloseHandle(processHandle);
                }
            }
        }

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool OpenProcessToken(IntPtr processHandle, uint desiredAccess, out IntPtr tokenHandle);
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle(IntPtr hObject);
        #endregion

        public bool UpdateMetadata()
        {
            try
            {
                CurrentProcess.Refresh();

                Name = CurrentProcess.ProcessName;
                Active = CurrentProcess.Responding;
                Cpu = Math.Round(_cpuCounter.NextValue() / Environment.ProcessorCount, 2);
                Memory = Math.Round(Convert.ToDouble(CurrentProcess.WorkingSet64) / 1024, 2);
                MemoryPercentage = Math.Round(Convert.ToDouble(CurrentProcess.WorkingSet64) * 100 /
                                              Convert.ToDouble(MemoryCounterHelper.GetTotalPhysicalMemory()), 2);
                NumberOfThreads = CurrentProcess.Threads.Count;
            }
            catch (InvalidOperationException)
            {
                return false;
            }

            return true;
        }

        public void KillProcess()
        {
            try
            {
                CurrentProcess.Kill();
            }
            catch (Win32Exception)
            {
                MessageBox.Show("Access is denied.");
            }
        }
    }
}