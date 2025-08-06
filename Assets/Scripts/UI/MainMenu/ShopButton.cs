using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Adjusts the behavior of the shop button
/// </summary>
public class ShopButton : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuPanel; 
    [SerializeField] private GameObject shopPanel;     

    /// <summary>
    /// This method will switch from Main Menu to Shop
    /// </summary>
    public void OpenShop()
    {
        // Deactivate Main Menu panel
        mainMenuPanel.SetActive(false);

        // Activate Shop panel
        shopPanel.SetActive(true);
    }
}
