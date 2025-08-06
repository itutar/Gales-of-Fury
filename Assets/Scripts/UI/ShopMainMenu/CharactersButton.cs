using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Adjusts the behavior of the Characters button
/// </summary>
public class CharactersButton : MonoBehaviour
{
    [SerializeField] private GameObject shopMainMenuPanel;
    [SerializeField] private GameObject shopCharactersMenu;

    /// <summary>
    /// This method will switch from Shop Main Menu to Shop Characters Menu
    /// </summary>
    public void OpenShopCharactersMenu()
    {
        // Deactivate Main Menu panel
        shopMainMenuPanel.SetActive(false);
        // Activate Characters panel
        shopCharactersMenu.SetActive(true);
    }
}
