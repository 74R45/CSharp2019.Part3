using System.ComponentModel;
using System.Windows;

namespace KMA.ProgrammingInCSharp2019.Kreshchenko.Part3.Tools
{
    internal interface ILoaderOwner : INotifyPropertyChanged
    {
        Visibility LoaderVisibility { get; set; }
        bool IsControlEnabled { get; set; }
    }
}
