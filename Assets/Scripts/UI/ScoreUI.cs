using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreUI : MonoBehaviour
{
    #region Fields

    [SerializeField] private Image scoreBackground;  // Arkadaki Image
    [SerializeField] private TMP_Text scoreText;

    private RectTransform bgRect;

    #endregion

    #region Unity Methods

    void Awake()
    {
        // RectTransform’ý al
        bgRect = scoreBackground.GetComponent<RectTransform>();
    }

    private void Start()
    {
        int initialScore = Blackboard.Instance.GetValue<int>(BlackboardKey.Score);
        scoreText.text = initialScore.ToString();
        OnScoreChanged(initialScore);

        Blackboard.Instance.Subscribe<int>(BlackboardKey.Score, OnScoreChanged);
    }

    // test
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ScoreManager.Instance.Add(1111); // test için puan ekle
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            ScoreManager.Instance.Add(100000); // test için puaný sýfýrla
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
            ScoreManager.Instance.Add(1000000); // test için puaný sýfýrla
        }
        else if (Input.GetKeyDown(KeyCode.Y))
        {
            ScoreManager.Instance.Add(10000000); // test için puaný sýfýrla
        }

    }

    #endregion

    private void OnScoreChanged(int newScore)
    {
        scoreText.text = newScore.ToString();
        int digitCount = scoreText.text.Length;

        // Arka plan geniþliði: 1 digit için 150, her ek digit için +50
        float newWidth = 150f + (digitCount - 1) * 50f;
        bgRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newWidth);

        // Margin sabit
        Vector4 currentMargins = scoreText.margin;
        scoreText.margin = new Vector4(0f, currentMargins.y, currentMargins.z, currentMargins.w);

        // Text pozisyonu
        float newX;
        if (digitCount == 1)
        {
            bgRect.anchoredPosition = new Vector2(-25f, bgRect.anchoredPosition.y);
            newX = 0f;
        }
        else if (digitCount == 2)
        {
            bgRect.anchoredPosition = new Vector2(-20f, bgRect.anchoredPosition.y);
            newX = -20f;
        }
        else if (digitCount == 3)
        {
            bgRect.anchoredPosition = new Vector2(-20f, bgRect.anchoredPosition.y);
            newX = -42f;
        }
        else if (digitCount == 4)
        {
            bgRect.anchoredPosition = new Vector2(-20f, bgRect.anchoredPosition.y);
            newX = -65f; 
        }
        else if (digitCount < 6)
        {
            bgRect.anchoredPosition = new Vector2(-5f, bgRect.anchoredPosition.y);
            newX = digitCount * -15f;
        }
        else
        {
            newX = -100f - ((digitCount - 6) * 20f);
        }

        scoreText.rectTransform.anchoredPosition = new Vector2(newX, scoreText.rectTransform.anchoredPosition.y);
    }
}
