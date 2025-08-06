using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// It deactivates the Shop panel and activates MainMenu panel. While doing so, 
/// it ensures that the ShopMainMenu panel is active 
/// and the ShopCharactersMenu and ShopBoardsMenu panels are deactivated.
/// </summary>
public class ShopCloseButton : MonoBehaviour
{
    // white background for the shop panel
    [SerializeField] private GameObject background;

    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private GameObject shopMainMenuPanel;
    [SerializeField] private GameObject shopCharactersMenuPanel;
    [SerializeField] private GameObject shopBoardsMenuPanel;

    /// <summary>
    /// /// It deactivates the Shop panel and activates MainMenu panel. While doing so, 
    /// it ensures that the ShopMainMenu panel is active 
    /// and the ShopCharactersMenu and ShopBoardsMenu panels are deactivated.
    /// </summary>
    public void CloseShop()
    {
        // Deactivate the background
        background.SetActive(false);

        // Deactivate Shop panel
        shopPanel.SetActive(false);
        // Activate Main Menu panel
        mainMenuPanel.SetActive(true);
        // Activate ShopMainMenu panel
        shopMainMenuPanel.SetActive(true);
        // Deactivate ShopCharactersMenu and ShopBoardsMenu panels
        shopCharactersMenuPanel.SetActive(false);
        shopBoardsMenuPanel.SetActive(false);
    }
}
