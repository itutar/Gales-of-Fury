using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverUIManager : MonoBehaviour
{
    [SerializeField] private GameObject ScoreUI;
    [SerializeField] private GameObject CoinUI;
    [SerializeField] private GameObject GameOverUI;
    [SerializeField] private GameObject GameOverBackground;

    private void Start()
    {
        if (GameOverEvent.instance == null)
        {
            Debug.LogError("ShowGameOverUI metodu listener eklenemedi. GameOverEvent instance null");
            return;
        }

        GameOverEvent.instance.OnGameOver.AddListener(ShowGameOverUI);

    }

    private void OnDisable()
    {
        if (GameOverEvent.instance != null)
        {
            GameOverEvent.instance.OnGameOver.RemoveListener(ShowGameOverUI);
        }
    }

    private void ShowGameOverUI()
    {
        Time.timeScale = 0f; // Oyun durdurulsun
        if (ScoreUI != null) ScoreUI.SetActive(false);
        if (CoinUI != null) CoinUI.SetActive(false);

        StartCoroutine(ActivateGameOverUIWithDelay(0.3f));
    }

    private IEnumerator ActivateGameOverUIWithDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        if (GameOverUI != null) GameOverUI.SetActive(true);
        if (GameOverBackground != null) GameOverBackground.SetActive(true);
    }
}
