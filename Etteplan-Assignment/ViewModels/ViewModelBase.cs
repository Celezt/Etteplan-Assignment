using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace Etteplan_Assignment.ViewModels;

public partial class ViewModelBase : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<string>? _errorMessages;

    protected ViewModelBase()
    {
        ErrorMessages = new ObservableCollection<string>();
    }
}
