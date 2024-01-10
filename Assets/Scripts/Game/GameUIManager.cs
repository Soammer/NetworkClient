using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    public Button connectBtn;
    public TMP_Text ContentText;

    public static GameUIManager instance;

    public void Awake()
    {
        instance = this;
    }

    public void Start()
    {
        connectBtn.onClick.AddListener(() =>
        {
            GameManager.instance.GameInit();
            NetworkManager.Connect();
            NetworkManager.Send(new ReadyProto());
            connectBtn.gameObject.SetActive(false);
        });
    }
}
