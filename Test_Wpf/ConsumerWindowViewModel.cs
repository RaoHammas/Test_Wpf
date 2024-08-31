using CommunityToolkit.Mvvm.ComponentModel;

namespace Test_Wpf_Channels;

public partial class ConsumerWindowViewModel : ObservableObject
{
    [ObservableProperty] private int _dataValue;

    public ConsumerWindowViewModel()
    {
        _ = SubscribeForData();
    }


    private Task SubscribeForData()
    {
        Task.Factory.StartNew(async () =>
        {
            var reader = DataChannel.Subscribe("IntData");
            await foreach (var data in reader.ReadAllAsync())
            {
                DataValue = data;
            }
        }, TaskCreationOptions.LongRunning);

        return Task.CompletedTask;
    }
}