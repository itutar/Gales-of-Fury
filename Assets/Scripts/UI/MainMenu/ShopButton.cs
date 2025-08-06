using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Adjusts the behavior of the shop button
/// </summary>
public class ShopButton : MonoBehaviour
{
    // white background for the shop panel
    [SerializeField] private GameObject background;

    [SerializeField] private GameObject mainMenuPanel; 
    [SerializeField] private GameObject shopPanel;     

    /// <summary>
    /// This method will switch from Main Menu to Shop
    /// </summary>
    public void OpenShop()
    {
        // Activate the background
        background.SetActive(true);

        // Deactivate Main Menu panel
        mainMenuPanel.SetActive(false);

        // Activate Shop panel
        shopPanel.SetActive(true);
    }
}
