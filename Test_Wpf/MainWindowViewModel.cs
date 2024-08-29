using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Test_Wpf;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty] private int _dataValue;

    public MainWindowViewModel()
    {
        _ = SubscribeForData();
    }

    private Task SubscribeForData()
    {
        _ = Task.Run(async () =>
        {
            while (await DataChannel.Subscribe("IntData").WaitToReadAsync())
            {
                DataValue = await DataChannel.Subscribe("IntData").ReadAsync();
            }
        });

        return Task.CompletedTask;
        /*await foreach (var data in DataChannel.Subscribe("IntData").ReadAllAsync())
        {
            DataValue = data;
        }*/
    }

    [RelayCommand]
    private void OpenNewWindow()
    {
        var newWind = new MainWindow();
        newWind.Show();
    }
}