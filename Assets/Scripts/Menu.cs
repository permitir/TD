using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] TextMeshProUGUI currencyUI;
    [SerializeField] Animator anim;
    [SerializeField] private GameObject clickBlocker;

    private bool isMenuOpen = true;

    public void ToggleMenu()
    {
        isMenuOpen = !isMenuOpen;
        anim.SetBool("MenuOpen", isMenuOpen);

        if (clickBlocker != null)
        {
            clickBlocker.SetActive(isMenuOpen);
        }
    }

    private void OnGUI()
    {
        currencyUI.text = "$" + LevelManager.main.currency.ToString();

    }

    public void SetSelected()
    {

    }
    
}
