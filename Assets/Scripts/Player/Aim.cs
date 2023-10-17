using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Aim : MonoBehaviour
{
    public RectTransform RectTransformAim;
    public Image ImageAim;

    private void Start()
    {
       // Cursor.visible = false;
    }

    void Update()
    {
        RectTransformAim.anchoredPosition = Input.mousePosition;

        ImageAim.enabled = !EventSystem.current.IsPointerOverGameObject();
        Cursor.visible = EventSystem.current.IsPointerOverGameObject();
    }
}
