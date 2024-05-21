using Avalonia.Controls;
using System.Collections.ObjectModel;

namespace Dashboard.ViewModels;

public partial class MainViewModel : ViewModelBase
{

    public ObservableCollection<Controls> ControlsList { get; } = new();
    //Avalonia.Controls.Button
    public MainViewModel()
    {
    }
}
