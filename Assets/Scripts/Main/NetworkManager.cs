using Network.NetRequests;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public static class NetworkManager
{
    private static Socket socket;
    public static byte[] buffer = new byte[1024];

    /// <summary>
    /// 连接服务器，绑定至按钮调用
    /// </summary>
    public static void Connect()
    {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        socket.BeginConnect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8000), ConnectCallBack, socket);
    }

    private static void ConnectCallBack(IAsyncResult ar)
    {
        Socket socket = (Socket)ar.AsyncState;
        socket.EndAccept(ar);
        //开始异步接收数据
        socket.BeginReceive(buffer, 0, 1024, 0, ReceiveCallBack, socket);
    }

    /// <summary>
    /// 向服务器发送数据，绑定至按钮调用
    /// </summary>
    [Obsolete("旧的发送方式")]
    public static void Send(string message, RequestType requestType, string url)
    {
        string send_msg = $"{NetRequests.RequestTypeKVPairs[requestType]} {url}\r\n{message}";
        Debug.Log($"发送数据：{send_msg}");
        byte[] bytes = Encoding.UTF8.GetBytes(send_msg);
        socket.BeginSend(bytes, 0, bytes.Length, 0, SendCallBack, url);
    }

    public static void Send(ProtoBase proto)
    {
        var bytes = Encode(proto);
        socket.Send(bytes, bytes.Length, SocketFlags.None);
    }

    private static void SendCallBack(IAsyncResult ar)
    {
        string url = (string)ar.AsyncState;
        int length = socket.EndSend(ar);
    }

    /// <summary>
    /// 接收数据的回调方法
    /// </summary>
    private static void ReceiveCallBack(IAsyncResult result)
    {
        Socket socket = (Socket)result.AsyncState;
        int length = socket.EndReceive(result);
        string request_back = Encoding.UTF8.GetString(buffer, 0, length);
        Debug.Log($"接收数据：{request_back}");

        var proto = Decode(request_back);
        GameManager.instance.HandleProto(proto);

        socket.BeginReceive(buffer, 0, 1024, 0, ReceiveCallBack, socket);
    }

    /// <summary>
    /// 关闭连接，在程序退出时调用
    /// </summary>
    public static void Close()
    {
        socket?.Close();
    }

    public static byte[] Encode(ProtoBase proto)
    {
        return Encoding.UTF8.GetBytes($"{proto.name}\r\n{JsonUtility.ToJson(proto)}");
    }

    public static ProtoBase Decode(string request_back)
    {
        string[] args = request_back.Split("\r\n", 2);
        if(args.Length < 2)
        {
            Debug.LogWarning($"接收到了未知的请求{args[0]}"); 
            return null;
        }
        return args[0] switch
        {
            "message" => JsonUtility.FromJson<MessageProto>(args[1]),
            "color" => JsonUtility.FromJson<ColorProto>(args[1]),
            "ready" => JsonUtility.FromJson<ReadyProto>(args[1]),
            "play" => JsonUtility.FromJson<PlayProto>(args[1]),
            "end" => JsonUtility.FromJson<EndProto>(args[1]),
            _ => null,
        };
    }
}
