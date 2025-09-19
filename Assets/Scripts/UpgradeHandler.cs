using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class UpgradeHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool mouse_over = false;

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouse_over = true;
        UIManager.main.SetHoveringState(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouse_over = false;
        UIManager.main.SetHoveringState(false);
    }

}
