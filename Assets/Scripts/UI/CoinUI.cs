using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CoinUI : MonoBehaviour
{
    #region Fields
    [SerializeField] private Image coinBackground;  // Arka plan g�rseli
    [SerializeField] private TMP_Text coinText;     // TextMeshPro metni

    private RectTransform bgRect;
    #endregion

    #region Unity Methods
    void Awake()
    {
        bgRect = coinBackground.GetComponent<RectTransform>();
    }

    void Start()
    {
        int initial = Blackboard.Instance.GetValue<int>(BlackboardKey.Coin);
        coinText.text = initial.ToString();
        OnCoinChanged(initial);

        // De�i�iklik oldu�unda UI g�ncellensin
        Blackboard.Instance.Subscribe<int>(BlackboardKey.Coin, OnCoinChanged);
    }

    // test
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CoinManager.Instance.Add(1111); // test i�in puan ekle
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            CoinManager.Instance.Add(100000); // test i�in puan� s�f�rla
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
            CoinManager.Instance.Add(1000000); // test i�in puan� s�f�rla
        }
        else if (Input.GetKeyDown(KeyCode.Y))
        {
            CoinManager.Instance.Add(10000000); // test i�in puan� s�f�rla
        }
        else if (Input.GetKeyDown(KeyCode.U))
        {
            CoinManager.Instance.Add(10); // test i�in puan� s�f�rla
        }

    }

    #endregion

    /// <summary>
    /// Updates the background and text when a new coin value arrives.
    /// </summary>
    private void OnCoinChanged(int newCoin)
    {
        coinText.text = newCoin.ToString();
        int digitCount = coinText.text.Length;

        // Geni�lik: 1 digit i�in 150, her ek digit +50
        float newWidth = 150f + (digitCount - 1) * 50f;
        bgRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newWidth);

        // Margin sabit tut
        Vector4 m = coinText.margin;
        coinText.margin = new Vector4(0f, m.y, m.z, m.w);

        // Pozisyonu ScoreUI mant���yla ayarla
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

        coinText.rectTransform.anchoredPosition = new Vector2(newX, coinText.rectTransform.anchoredPosition.y);
    }
}
