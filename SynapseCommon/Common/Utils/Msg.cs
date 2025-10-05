using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

public class Msg
{
    /// <summary>
    /// name of the rpc method
    /// </summary>
    public string methodName { get; private set; }

    /// <summary>
    /// id of the instance of the method
    /// </summary>
    public string instanceId { get; private set; }

    /// <summary>
    /// method args (in serializable Node)
    /// </summary>
    public ListNode arg { get; private set; }
    public Msg(string methodName_, string instanceId_, params Node[] args)
    {
        methodName = methodName_;
        instanceId = instanceId_;
        arg = new ListNode();
        foreach (Node a in args)
        {
            arg.Add(a);
        }
    }

    public void Serialize(Proxy proxy, BinaryWriter writer)
    {
        writer.Write(methodName);
        writer.Write(instanceId);
        NodeStreamer.Serialize(arg, writer);
    }
}

public static class MsgStreamer
{
    public static byte[] Serialize(Proxy proxy, Msg msg)
    {
        using var stream = new MemoryStream();
        using var writer = new BinaryWriter(stream);
        msg.Serialize(proxy, writer);
        return stream.ToArray();
    }

    public static Msg? Deserialize(byte[] data)
    {
        using var stream = new MemoryStream(data);
        using var reader = new BinaryReader(stream);
        
        try
        {
            string methodName = reader.ReadString();
            string instanceId = reader.ReadString();
            Node arg = NodeStreamer.Deserialize(reader);
            if (arg != null && arg is ListNode listNode)
            {
                return new Msg(methodName, instanceId, listNode.ToArray());
            }
            else
            {
                return null;
            }
        }
        catch
        {
            return null;
        }

    }

    public static (bool succ, Msg? msg) ReadMsgFromStream(NetworkStream? stream)
    {
        if (stream == null)
        {
            Log.Error("Stream is null");
            return (false, null);
        }

        byte[] lengthBuffer = new byte[4];
        int lengthBytesRead = 0;
        while (lengthBytesRead < 4)
        {
            int read = stream.Read(lengthBuffer, lengthBytesRead, 4 - lengthBytesRead);
            if (read == 0) break;
            lengthBytesRead += read;
        }
        if (lengthBytesRead < 4)
        {
            return (false, null);
        }

        int messageLength = BitConverter.ToInt32(lengthBuffer, 0);
        if (messageLength <= 0)
        {
            return (false, null);
        }
        byte[] messageBuffer = new byte[messageLength];
        int totalBytesRead = 0;
        while (totalBytesRead < messageLength)
        {
            int read = stream.Read(messageBuffer, totalBytesRead, messageLength - totalBytesRead);
            if (read == 0) break;
            totalBytesRead += read;
        }
        if (totalBytesRead < messageLength)
        {
            return (false, null);
        }
        Msg? msg = Deserialize(messageBuffer);
        if (msg == null)
        {
            return (false, null);
        }
        return (true, msg);
    }

    public static async Task<(bool succ, Msg? msg)> ReadMsgFromStreamAsync(NetworkStream? stream, CancellationToken cancellationToken = default)
    {
        if (stream == null)
        {
            Log.Error("Stream is null");
            return (false, null);
        }
        try
        {
            byte[] lengthBuffer = new byte[4];
            int lengthBytesRead = 0;
            while (lengthBytesRead < 4)
            {
                try
                {
                    int read = await stream.ReadAsync(
                        lengthBuffer,
                        lengthBytesRead,
                        4 - lengthBytesRead,
                        cancellationToken
                    ).ConfigureAwait(false);
                    if (read == 0) break;
                    lengthBytesRead += read;
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    Log.Info("Read operation was canceled as requested");
                    return (false, null);
                }
                catch (Exception ex)
                {
                    Log.Error($"Read exception happens: {ex}");
                    return (false, null);
                }                
            }
            if (lengthBytesRead < 4)
            {
                Log.Error("Head read failed");
                return (false, null);
            }

            int messageLength = BitConverter.ToInt32(lengthBuffer, 0);
            if (messageLength <= 0)
            {
                Log.Error("Head length abnormal");
                return (false, null);
            }
            byte[] messageBuffer = new byte[messageLength];
            int totalBytesRead = 0;
            while (totalBytesRead < messageLength)
            {
                int read = await stream.ReadAsync(
                    messageBuffer,
                    totalBytesRead,
                    messageLength - totalBytesRead,
                    cancellationToken
                ).ConfigureAwait(false);

                if (read == 0) break;
                totalBytesRead += read;
            }
            if (totalBytesRead < messageLength)
            {
                Log.Error("Read data incomplete");
                return (false, null);
            }
            Msg? msg = Deserialize(messageBuffer);
            if (msg == null)
            {
                Log.Error("Read msg failed");
                return (false, null);
            }
            return (true, msg);
        }
        catch (Exception ex)
        {
            Log.Error($"Read exception happens: {ex}");
            return (false, null);
        }
    }

    public static bool WriteMsgToStream(Proxy proxy, Msg msg)
    {
        NetworkStream? stream = proxy.stream;
        if (stream == null) return false;

        byte[] buffer = Serialize(proxy, msg);
        if (buffer.Length <= 0) return false;

        byte[] lengthPrefix = BitConverter.GetBytes(buffer.Length);
        stream.Write(lengthPrefix, 0, 4);
        stream.Write(buffer, 0, buffer.Length);
        stream.Flush();
        return true;
    }

    public static async Task<bool> WriteMsgToStreamAsync(Proxy proxy, Msg msg, CancellationToken cancellationToken = default)
    {
        NetworkStream? stream = proxy.stream;
        if (stream == null) return false;

        try
        {
            byte[] buffer = Serialize(proxy, msg);
            if (buffer.Length <= 0) return false;

            byte[] lengthPrefix = BitConverter.GetBytes(buffer.Length);
            await stream.WriteAsync(lengthPrefix, 0, 4, cancellationToken).ConfigureAwait(false);
            await stream.WriteAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false);
            await stream.FlushAsync(cancellationToken).ConfigureAwait(false);
            return true;
        }
        catch (Exception ex) when (ex is OperationCanceledException || ex is IOException)
        {
            return false;
        }
    }
}
