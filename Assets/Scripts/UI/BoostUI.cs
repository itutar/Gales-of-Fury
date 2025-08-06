using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoostUI : MonoBehaviour
{
    [SerializeField] private Image fillImage;   // iste�e ba�l� � scriptten alabilirsiniz
    private float duration;
    private float timeLeft;

    public void Init(Sprite sprite, float duration)
    {
        fillImage = fillImage ?? GetComponent<Image>();
        fillImage.sprite = sprite;
        fillImage.type = Image.Type.Filled;
        fillImage.fillMethod = Image.FillMethod.Vertical;
        fillImage.fillOrigin = (int)Image.OriginVertical.Top;

        this.duration = duration;
        timeLeft = duration;
        StartCoroutine(UpdateFill());
    }

    private IEnumerator UpdateFill()
    {
        while (timeLeft > 0f)
        {
            timeLeft -= Time.deltaTime;
            fillImage.fillAmount = timeLeft / duration;
            yield return null;
        }

        Destroy(gameObject);          // s�re bitti�inde UI�dan sil
    }

    // E�er ikon s�resini yenilemek isterseniz:
    public void Refresh(float newDuration)
    {
        duration = newDuration;
        timeLeft = newDuration;
    }
}
