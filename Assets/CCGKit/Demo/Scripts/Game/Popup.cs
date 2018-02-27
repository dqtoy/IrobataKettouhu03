using UnityEngine;

public class Popup : MonoBehaviour
{
    [HideInInspector]
    public BaseScene parentScene;

    public void Close()
    {
        if (parentScene != null)
        {
            parentScene.OnPopupClosed(this);
        }
        Destroy(gameObject);
    }
}