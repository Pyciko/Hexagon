using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hex : MonoBehaviour {
    
    public Image Indicator;

    public Gradient GradientIndicator;
    public Color32 DefaultLineColor;

    public bool isUprised;
    public bool wasRecentlyUsed;

    float TimePlanned;

    public GameObject ExplosionPrefab;

    public void Uprise() {
        if (!isUprised && !wasRecentlyUsed) {
            StartCoroutine(ChangeHexState(1));
            isUprised = true;
            wasRecentlyUsed = true;
            
            TimePlanned = Gameplay.instance.HexGenTime * Gameplay.instance.HexLiveMult / Gameplay.Difficulty;
            StartCoroutine (HexLife());
        }
    }

    IEnumerator HexLife () {
        float TimePassed = 0;
        while (isUprised) {
            TimePassed += Time.deltaTime;
            Indicator.fillAmount = TimePassed / TimePlanned;
            Indicator.color = GradientIndicator.Evaluate(TimePassed / TimePlanned);

            if (TimePassed >= TimePlanned) {
                Explode();
            }
            yield return null;
        }
    }

    public void Explode () {
        StartCoroutine(ChangeHexState(0.1f));
        Instantiate(ExplosionPrefab, transform.localPosition + (Vector3.up / 2), Quaternion.identity);
        Gameplay.instance.Miss(true);
    }

    public void Hide () {
        StartCoroutine(ChangeHexState(0.1f));
    }

    public void CoolDown () {
        StartCoroutine(ChangeHexState(0.1f));
        Gameplay.instance.Success();
    }

    IEnumerator ChangeHexState (float newHeight) {
        if (newHeight != 1)
            isUprised = false;

        float originalHeight = transform.localScale.y;
        float originalIndFill = Indicator.fillAmount;
        Color originalColor = Indicator.color;
        float TimePassed = 0;

        while (TimePassed < Gameplay.TransitionTime) {
            TimePassed += Time.deltaTime;
            transform.localScale = new Vector3 (1, Mathf.Lerp(originalHeight, newHeight, TimePassed / Gameplay.TransitionTime), 1);
            if (newHeight != 1) {
                Indicator.fillAmount = Mathf.Lerp(originalIndFill, 1, TimePassed / Gameplay.TransitionTime);
                Indicator.color = Color.Lerp(originalColor, DefaultLineColor, TimePassed / Gameplay.TransitionTime);
            }

            yield return null;
        }

        if (newHeight != 1) {
            yield return new WaitForSecondsRealtime (2/Gameplay.Difficulty);
            wasRecentlyUsed = false;
        }
    }
}