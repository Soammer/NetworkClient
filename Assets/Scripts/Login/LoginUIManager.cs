using Network.NetRequests;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginUIManager : MonoBehaviour
{
    public Button Loginbtn;
    public Button RegistBtn;
    public TMP_InputField UserInputField;
    public TMP_InputField PasswordInputField;
    public TMP_Text ContentText;

    public static LoginUIManager instance;

    public void Awake()
    {
        instance = this;
    }

    public void Start()
    {
        NetworkManager.Connect();
        Loginbtn.onClick.AddListener(() =>
        {
            //对服务器进行数据发送
            NetworkManager.Send(new LoginProto("login", UserInputField.text, PasswordInputField.text));
        });
        RegistBtn.onClick.AddListener(() =>
        {
            //对服务器进行连接
            NetworkManager.Send(new LoginProto("register", UserInputField.text, PasswordInputField.text));
        });
    }
}
