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

    float charWidth = 50f;
    float padding = 50f;
    float newWidth;

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
    /*
    private void OnScoreChanged(int newScore)
    {
        scoreText.text = newScore.ToString();
        int digitCount = scoreText.text.Length;
        Vector4 currentMargins = scoreText.margin;

        
        // Update the width of the background image and adjust the text position
        
        //textRect.anchoredPosition = new Vector2(-padding / 2f, textRect.anchoredPosition.y);
        if (digitCount == 1)
        {
            newWidth = charWidth * digitCount + 100f;
            scoreText.margin = new Vector4(0f, currentMargins.y, currentMargins.z, currentMargins.w);
        }
        else if (digitCount == 2)
        {
            newWidth = charWidth * digitCount + 150f;
            scoreText.margin = new Vector4(0f, currentMargins.y, currentMargins.z, currentMargins.w);
        }
        else if (digitCount == 3)
        {
            newWidth = charWidth * digitCount + 200f;
            scoreText.margin = new Vector4(0f, currentMargins.y, currentMargins.z, currentMargins.w);
        }
        else if (digitCount == 4)
        {
            scoreText.margin = new Vector4(0f, currentMargins.y, currentMargins.z, currentMargins.w);
        }
        else if (digitCount == 5)
        {
            scoreText.margin = new Vector4(0f, currentMargins.y, currentMargins.z, currentMargins.w);
            scoreText.rectTransform.anchoredPosition = new Vector2(-50f, scoreText.rectTransform.anchoredPosition.y);
        }
        else if (digitCount == 6)
        {
            scoreText.margin = new Vector4(0f, currentMargins.y, currentMargins.z, currentMargins.w);
        }
        else if (digitCount == 7)
        {
            scoreText.margin = new Vector4(0f, currentMargins.y, currentMargins.z, currentMargins.w);
        }
        else if (digitCount == 8)
        {
            scoreText.margin = new Vector4(0f, currentMargins.y, currentMargins.z, currentMargins.w);
        }
        else if (digitCount == 9)
        {
            scoreText.margin = new Vector4(0f, currentMargins.y, currentMargins.z, currentMargins.w);
        }
        else
        {
            scoreText.margin = new Vector4(0f, currentMargins.y, currentMargins.z, currentMargins.w);
        }
        bgRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newWidth);
    }
    */

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
        if (digitCount < 6)
        {
            newX = digitCount * -10f;
        }
        else
        {
            newX = -100f - ((digitCount - 6) * 20f);
        }

        scoreText.rectTransform.anchoredPosition = new Vector2(newX, scoreText.rectTransform.anchoredPosition.y);
    }
}
