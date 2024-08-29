using System.Collections.Concurrent;
using System.Threading.Channels;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Test_Wpf;

public static class DataChannel
{
    private static readonly ConcurrentDictionary<string, Subscription<int>> Subscriptions = new();
    private static readonly ConcurrentDictionary<string, Task> Writers = new();

    public static ChannelReader<int> Subscribe(string key)
    {
        var subscription = Subscriptions.GetOrAdd(key, k => new Subscription<int>
        {
            Key = k,
            DataChannel = Channel.CreateUnbounded<int>(new UnboundedChannelOptions
            {
                SingleReader = false,
                SingleWriter = true
            })
        });

        Writers.GetOrAdd(key, _ => StartWriterTask(subscription.DataChannel.Writer));

        return subscription.DataChannel.Reader;
    }

    private static Task StartWriterTask(ChannelWriter<int> writer)
    {
        return Task.Factory.StartNew(async () =>
        {
            var i = 0;
            while (true)
            {
                await writer.WriteAsync(++i);
                await Task.Delay(500);
            }
        }, TaskCreationOptions.LongRunning);
    }
}

public partial class Subscription<T> : ObservableObject
{
    [ObservableProperty] private Channel<T> _dataChannel = null!;
    [ObservableProperty] private string _key = null!;
}