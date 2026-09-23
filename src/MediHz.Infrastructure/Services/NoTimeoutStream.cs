using System.IO;

namespace MediHz.Infrastructure.Services;

/// <summary>
/// A stream wrapper that prevents timeout properties from being set.
/// This solves the "timeouts are not supported on this stream" error
/// when using FileStream with ASP.NET Core response handling.
/// </summary>
public class NoTimeoutStream : Stream
{
    private readonly Stream _innerStream;

    public NoTimeoutStream(Stream innerStream)
    {
        _innerStream = innerStream ?? throw new ArgumentNullException(nameof(innerStream));
    }

    public override bool CanRead => _innerStream.CanRead;
    public override bool CanSeek => _innerStream.CanSeek;
    public override bool CanWrite => _innerStream.CanWrite;
    public override long Length => _innerStream.Length;

    public override long Position
    {
        get => _innerStream.Position;
        set => _innerStream.Position = value;
    }

    // Hide timeout properties to prevent "timeouts are not supported on this stream" error
    public override int ReadTimeout
    {
        get => -1; // Infinite timeout
        set { } // Ignore attempts to set timeout
    }

    public override int WriteTimeout
    {
        get => -1; // Infinite timeout
        set { } // Ignore attempts to set timeout
    }

    public override void Flush() => _innerStream.Flush();

    public override int Read(byte[] buffer, int offset, int count)
        => _innerStream.Read(buffer, offset, count);

    public override long Seek(long offset, SeekOrigin origin)
        => _innerStream.Seek(offset, origin);

    public override void SetLength(long value)
        => _innerStream.SetLength(value);

    public override void Write(byte[] buffer, int offset, int count)
        => _innerStream.Write(buffer, offset, count);

    public override async Task FlushAsync(CancellationToken cancellationToken)
        => await _innerStream.FlushAsync(cancellationToken);

    public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        => await _innerStream.ReadAsync(buffer, offset, count, cancellationToken);

    public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        => await _innerStream.ReadAsync(buffer, cancellationToken);

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _innerStream.Dispose();
        }

        base.Dispose(disposing);
    }
}
