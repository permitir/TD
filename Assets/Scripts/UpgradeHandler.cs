using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class UpgradeHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public void OnPointerEnter(PointerEventData eventData)
    {
        UIManager.main.SetHoveringState(true); // Whenever mouse cursor enters UI, setting HoveringState as true
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UIManager.main.SetHoveringState(false); // Whenever mouse cursor exits UI, setting HoveringState as false
    }

}
