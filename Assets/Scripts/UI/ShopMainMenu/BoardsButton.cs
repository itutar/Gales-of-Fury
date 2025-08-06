using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Adjusts the behavior of the Boards button
/// </summary>
public class BoardsButton : MonoBehaviour
{
    [SerializeField] private GameObject shopMainMenuPanel; 
    [SerializeField] private GameObject shopBoardsMenu;     
    
    /// <summary>
    /// This method will switch from Shop Main Menu to Shop Boards Menu
    /// </summary>
    public void OpenShopBoardsMenu()
    {
        // Deactivate Main Menu panel
        shopMainMenuPanel.SetActive(false);
        // Activate Boards panel
        shopBoardsMenu.SetActive(true);
    }
}
