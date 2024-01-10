using Network.NetRequests;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public static Queue<Action> actions = new();

    private GameObject CrossPrefab;
    private GameObject ChessPrefab;
    private Transform CrossParent;

    public static ChessType selfColor = ChessType.Unknown;
    public static GameStatus globalStatus = GameStatus.Waiting;
    public static ChessType turnColor = ChessType.Unknown;

    //当前场景的名字，用于判断在哪个场景
    public static string SceneID => SceneManager.GetActiveScene().name;

    private const int HEIGHT = 10, WIDTH = 10;

    private void Awake()
    {
        instance = this;
    }

    public void GameInit()
    {
        if (!CrossPrefab) CrossPrefab = Resources.Load<GameObject>("Prefabs/Cross");
        if (!CrossParent) CrossParent = GameObject.Find("CrossParent").transform;
        if (!ChessPrefab) ChessPrefab = Resources.Load<GameObject>("Prefabs/Chess");
        Camera.main.orthographicSize = 10;
        Camera.main.transform.position = new((WIDTH - 1f) / 2, 1, -10);
        for (int i = 0; i < HEIGHT; ++i)
        {
            for (int j = 0; j < WIDTH; ++j)
            {
                Instantiate(CrossPrefab, CrossParent).transform.localPosition = new Vector3(i, j, 0);
            }
        }
    }

    /// <summary>
    /// 点击棋盘的格子执行的方法
    /// </summary>
    public void CrossClicked(Cross cross)
    {
        if (globalStatus != GameStatus.Playing)
        {
            Debug.Log("游戏未开始");
            return;
        }
        if (turnColor != selfColor)
        {
            Debug.Log("还没轮到你");
            return;
        }
        NetworkManager.Send(new PlayProto(cross.transform.localPosition, selfColor));
    }

    private void Update()
    {
        while (actions.Count > 0)
        {
            actions.Dequeue().Invoke();
        }
    }

    public void HandleProto(ProtoBase proto)
    {
        Debug.Log($"处理数据{JsonUtility.ToJson(proto)}");
        if (proto is MessageProto)
        {
            actions.Enqueue(() => {
                ShowMessage(proto as MessageProto);
            });
        }
        else if (proto is ColorProto)
        {
            ColorProto colorProto = proto as ColorProto;
            selfColor = colorProto.color;
        }
        else if (proto is ReadyProto)
        {
            //游戏开始，黑棋先行
            globalStatus = GameStatus.Playing;
            turnColor = ChessType.Black;
            Debug.Log("游戏开始");
        }
        else if (proto is PlayProto)
        {
            PlayProto playProto = proto as PlayProto;
            actions.Enqueue(() =>
            {
                GameObject newChess = Instantiate(ChessPrefab, CrossParent);
                newChess.transform.localPosition = new Vector3(playProto.x, playProto.y, 0);
                if (playProto.color == ChessType.Black)
                {
                    //本身就是白棋子，黑色则染色
                    newChess.GetComponent<SpriteRenderer>().color = Color.black;
                    turnColor = ChessType.White;
                }
                else
                {
                    turnColor = ChessType.Black;
                }
            });
        }
        else if (proto is EndProto)
        {
            EndProto endProto = proto as EndProto;
            globalStatus = GameStatus.End;
            actions.Enqueue(() =>
            {
                GameUIManager.instance.ContentText.text += $"\n{endProto.winner}胜利";
            });
        }
        else
        {
            Debug.LogWarning($"接收到了未知的请求{proto.name}");
        }
    }

    //每个场景的消息显示方式不同
    private static void ShowMessage(MessageProto proto)
    {
        switch (SceneID)
        {
            case "Login":
                LoginUIManager.instance.ContentText.text += $"\n{proto.content}";
                break;

            case "Chat":
                UIManager.instance.ContentText.text += $"\n{proto.content}";
                break;

            case "Game":
                GameUIManager.instance.ContentText.text += $"\n系统: {proto.content}";
                break;
            default:
                break;
        }
    }

    public void OnApplicationQuit()
    {
        NetworkManager.Close();
    }
}