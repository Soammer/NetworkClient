using Network.NetRequests;
using System;
using UnityEngine;

[Serializable]
public class ProtoBase 
{
    public string name;
}

[Serializable]
public class MessageProto : ProtoBase
{
    public string content;

    public MessageProto(string content)
    {
        name = "message";
        this.content = content;
    }
}

[Serializable]
public class ReadyProto : ProtoBase
{
    public ReadyProto()
    {
        name = "ready";
    }
}

[Serializable]
public class ColorProto : ProtoBase
{
    public ChessType color;

    public ColorProto(ChessType color)
    {
        name = "color";
        this.color = color;
    }
}

[Serializable]
public class PlayProto : ProtoBase
{
    public int x, y;
    public ChessType color;

    public PlayProto(int x, int y, ChessType color)
    {
        name = "play";
        this.x = x;
        this.y = y;
        this.color = color;
    }
    public PlayProto(Vector3 v, ChessType color)
    {
        name = "play";
        x = (int)v.x;
        y = (int)v.y;
        this.color = color;
    }
}

[Serializable]
public class EndProto : ProtoBase
{
    public ChessType winner;

    public EndProto(ChessType winner)
    {
        name = "end";
        this.winner = winner;
    }
}

[Serializable]
public class LoginProto : ProtoBase
{
    public string username;
    public string password;

    public LoginProto(string name, string username, string password)
    {
        this.name = name;
        this.username = username;
        this.password = password;
    }
}