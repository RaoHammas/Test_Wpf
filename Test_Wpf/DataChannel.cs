using System.Threading.Channels;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Test_Wpf;

public static class DataChannel
{
    private static readonly Dictionary<string, Subscription<int>> Subscriptions;

    static DataChannel()
    {
        Subscriptions = new Dictionary<string, Subscription<int>>();
    }

    public static ChannelReader<int> Subscribe(string key)
    {
        if (Subscriptions.TryGetValue(key, out var existingSubscription))
        {
            return existingSubscription.DataChannel.Reader;
        }

        var newSub = new Subscription<int>
        {
            Key = key,
            DataChannel = Channel.CreateUnbounded<int>()
        };

        Task.Run(async () =>
        {
            var i = 0;
            while (true)
            {
                newSub.DataChannel.Writer.TryWrite(++i);
                await Task.Delay(500);
            }
        });

        Subscriptions.Add(key, newSub);
        return Subscriptions[key].DataChannel.Reader;
    }
}

public partial class Subscription<T> : ObservableObject
{
    [ObservableProperty] private Channel<T> _dataChannel = null!;
    [ObservableProperty] private string _key = null!;
}