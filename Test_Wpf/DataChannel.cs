using System.Collections.Concurrent;
using System.Threading.Channels;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Test_Wpf_Channels;

public static class DataChannel
{
    private static readonly ConcurrentDictionary<string, Subscription<int>> Subscriptions = new();

    public static ChannelReader<int> Subscribe(string key)
    {
        if (Subscriptions.TryGetValue(key, out var existingSubscription))
        {
            return existingSubscription.DataChannel.Reader;
        }

        var newSub = new Subscription<int>
        {
            Key = key,
            DataChannel = Channel.CreateUnbounded<int>(new UnboundedChannelOptions
            {
                SingleReader = false,
                SingleWriter = true
            })
        };

        newSub.SubTask = StartWriterTask(newSub.DataChannel.Writer);
        Subscriptions.TryAdd(key, newSub);
        return newSub.DataChannel.Reader;
    }

    private static async Task StartWriterTask(ChannelWriter<int> writer)
    {
        var i = 0;
        while (true)
        {
            await writer.WriteAsync(++i);
            await Task.Delay(500);
        }
    }
}

public partial class Subscription<T> : ObservableObject
{
    [ObservableProperty] private Channel<T> _dataChannel = null!;
    [ObservableProperty] private string _key = null!;
    [ObservableProperty] private Task _subTask = null!;
}