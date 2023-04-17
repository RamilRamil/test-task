
// There is a method SendMessageAsync in class BusMessageWriter 
// for sending messages to Message Bus with bufferization, 
// it sends messages when the buffer size reaches a threshold.

// The code is not thread safe. Modify the code to make it thread safe. 
// How to modify the code to make it better in a clean code way? You can change the
// code as you want, the main thing is that there must be the method SendMessageAsync, 
// and messages must be sent to Message Broker with bufferization.

// The code wasn't thread safe because multiple threads have access to the _buffer 
// We can use a locker to fix this

public class BusMessageWriter
{

 private readonly IBusConnection _connection;
 private readonly MemoryStream _buffer = new();
 private readonly object locker = new();

    public async Task SendMessageAsync(byte[] nextMessage) 
        {
            lock(locker) {
            _buffer.Write(nextMessage, 0, nextMessage.Length);
                {
                    if (_buffer.Length > 1000)
                    {
                        await _connection.PublishAsync(_buffer.ToArray());
                        _buffer.SetLength(0);
                    }
                }
            }
        }
}


// We can also use SemaphoreSlim class to make the code thread safe

public class BusMessageWriter
{

 private readonly IBusConnection _connection;
 private readonly MemoryStream _buffer = new();
 private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1)

    public async Task SendMessageAsync(byte[] nextMessage) 
        {
            await _semaphore.WaitAsync();
            try {
            _buffer.Write(nextMessage, 0, nextMessage.Length)
                {
                    if (_buffer.Length > 1000)
                    {
                        await _connection.PublishAsync(_buffer.ToArray());
                        _buffer.SetLength(0);
                    }
                }
            } catch (Exception ex) {
                logger.LogError(exception, "Error while processing SendMessageAsync");
            } finally {
                _semaphore.Release();
            }
        }
}