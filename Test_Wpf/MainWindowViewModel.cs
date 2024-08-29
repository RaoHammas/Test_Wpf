using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Test_Wpf;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty] private int _dataValue;

    public MainWindowViewModel()
    {
    }


    [RelayCommand]
    private void OpenNewWindow()
    {
        Thread thread = new Thread(() =>
        {
            var newWind = new ConsumerWindow();
            newWind.DataContext = new ConsumerWindowViewModel();
            newWind.Show();

            newWind.Closed += (sender2, e2) =>
                newWind.Dispatcher.InvokeShutdown();

            System.Windows.Threading.Dispatcher.Run();
        });

        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
    }
}