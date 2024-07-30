using System;
using System.IO;
using System.Net.WebSockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class PLGWebSocket
{
    private readonly ClientWebSocket WebSocket = new();
    private readonly Guid ServerID = Guid.NewGuid();
    private readonly string Key = "";
    private readonly string URL = "ws://127.0.0.1:10050/api/Gateway";
    private readonly string ExternalIP = "127.0.0.1";
    private readonly int ServerPort = 10501;

    public async Task ConnectAsync()
    {
        WebSocket.Options.SetRequestHeader("server_id", ServerID.ToString());
        await WebSocket.ConnectAsync(new Uri(URL), CancellationToken.None);
        await Send($"{{ \"Type\": \"SERVER_INFO\", \"From\": \"{ServerID}\", \"To\": \"API\", \"Content\": \"{{ \\\"Type\\\": \\\"DIRECTORY\\\", \\\"Address\\\": \\\"{ExternalIP}\\\", \\\"Port\\\": \\\"{ServerPort}\\\", \\\"ServerPrivateKey\\\": \\\"MIGrAgEAAiEAq0cOe8L1tOpnc7e+ouVD\\\" }}\"}}");
    }

    private async Task Send(string message)
    {
        byte[] bytes;

        if (!string.IsNullOrEmpty(Key))
            bytes = Encoding.UTF8.GetBytes(Encrypt(message));
        else
            bytes = Encoding.UTF8.GetBytes(message);

        if (WebSocket.State == WebSocketState.Open)
            await WebSocket.SendAsync(bytes, WebSocketMessageType.Text, true, CancellationToken.None);
    }

    private string Encrypt(string message)
    {
        var aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(Key);
        aes.Mode = CipherMode.ECB;
        aes.Padding = PaddingMode.None;

        var stream = new MemoryStream();
        var cryptoTransform = aes.CreateEncryptor(aes.Key, null);
        var cryptoStream = new CryptoStream(stream, cryptoTransform, CryptoStreamMode.Write);

        cryptoStream.Write(Encoding.UTF8.GetBytes(message));

        return Convert.ToBase64String(stream.ToArray());
    }
}
