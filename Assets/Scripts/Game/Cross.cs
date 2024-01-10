using UnityEngine;

public class Cross : MonoBehaviour
{
    public void OnMouseUp()
    {
        GameManager.instance.CrossClicked(this);
    }
}
