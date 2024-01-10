using Network.NetRequests;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    //用的是TMP组件，如果是Legacy组件就用对应的类名绑定
    public TMP_Text ContentText;
    public Button connectButton;
    public Button sendButton;
    public TMP_InputField messageInput;
    //静态单例
    public static UIManager instance;
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        connectButton.onClick.AddListener(() =>
        {
            //对服务器进行连接
            NetworkManager.Connect();
        });
        sendButton.onClick.AddListener(() =>
        {
            //发送消息并清空发送文本框
            NetworkManager.Send(new MessageProto(messageInput.text));
            messageInput.text = string.Empty;
        });
    }
}
