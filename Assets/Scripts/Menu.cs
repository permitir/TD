using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] TextMeshProUGUI currencyUI;
    [SerializeField] Animator anim;

    private bool isMenuOpen = true;

    private void Start()
    {
        UIManager.main.MenuOpen(isMenuOpen);
    }

    public void ToggleMenu()
    {
        isMenuOpen = !isMenuOpen;
        anim.SetBool("MenuOpen", isMenuOpen);

        UIManager.main.MenuOpen(isMenuOpen);
    }

    private void OnGUI()
    {
        currencyUI.text = "$" + LevelManager.main.currency.ToString();

    }    
}
