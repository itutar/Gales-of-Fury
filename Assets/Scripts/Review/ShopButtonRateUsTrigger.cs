using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Play.Review;

public class ShopButtonRateUsTrigger : MonoBehaviour
{
    private int threshold = 500; // Trigger every x clicks

    private const string ClickCountKey = "rate.shop.clickCount";
    private bool _inFlight = false; // Prevents double calls while a flow is running

    private void Awake()
    {
        // Ensure persistent counter key exists
        if (!ES3.KeyExists(ClickCountKey))
            ES3.Save(ClickCountKey, 0);
    }

    /// <summary>
    /// Called from Shop button's onClick.
    /// </summary>
    public void OnShopButtonClicked()
    {
        // 1) Increase persistent click counter
        int count = ES3.Load<int>(ClickCountKey, 0);
        count++;
        ES3.Save(ClickCountKey, count);

        // 2) If threshold reached, try showing the review card, then reset counter
        if (count >= threshold)
        {
            // Reset immediately as requested (even if Google suppresses the dialog)
            ES3.Save(ClickCountKey, 0);

#if UNITY_ANDROID && !UNITY_EDITOR
            if (!_inFlight)
                StartCoroutine(TryInAppReview());
#endif
        }
    }

    private IEnumerator TryInAppReview()
    {
        _inFlight = true;

        // Create the ReviewManager
        var reviewManager = new ReviewManager();

        // Step A: Request the review flow (may fail due to network/quota/Play Store missing)
        var req = reviewManager.RequestReviewFlow();
        yield return req;

        if (req.Error != ReviewErrorCode.NoError)
        {
            // Could not obtain review info; just stop gracefully.
            _inFlight = false;
            yield break;
        }

        // Step B: Launch the review flow. Google renders a native, uncustomizable card if allowed.
        var playReviewInfo = req.GetResult();
        var launch = reviewManager.LaunchReviewFlow(playReviewInfo);
        yield return launch;
        playReviewInfo = null; // Single-use object

        // The API never tells whether the dialog actually appeared or if the user submitted a review.
        // Just continue your game flow normally.
        _inFlight = false;
    }
}
