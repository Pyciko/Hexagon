using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hex : MonoBehaviour {
    
    public Image indicator;

    public Gradient gradientIndicator;
    public Color32 defaultLineColor;

    public bool isRaised;
    public bool wasRecentlyUsed;

    private float timePlanned;

    public GameObject explosionPrefab;

    public void Raise() {
        if (!isRaised && !wasRecentlyUsed) {
            StartCoroutine(ChangeHexState(1));
            isRaised = true;
            wasRecentlyUsed = true;
            
            timePlanned = Gameplay.instance.baseTime * Gameplay.instance.hexLifetimeMult;
            StartCoroutine (HexLife());
        }
    }

    IEnumerator HexLife () {
        float TimePassed = 0;
        while (isRaised) {
            TimePassed += Time.deltaTime;
            indicator.fillAmount = TimePassed / timePlanned;
            indicator.color = gradientIndicator.Evaluate(TimePassed / timePlanned);

            if (TimePassed >= timePlanned) {
                Explode();
            }
            yield return null;
        }
    }

    public void Explode () {
        StartCoroutine(ChangeHexState(0.1f));
        Instantiate(explosionPrefab, transform.localPosition + (Vector3.up / 2), Quaternion.identity);
        Gameplay.instance.Miss(true, false);
    }

    public void Hide () {
        StartCoroutine(ChangeHexState(0.1f));
    }

    public void CoolDown () {
        StartCoroutine(ChangeHexState(0.1f));
        Gameplay.instance.Success(false);
    }

    IEnumerator ChangeHexState (float newHeight) {
        if (newHeight != 1)
            isRaised = false;

        float originalHeight = transform.localScale.y;
        float originalIndFill = indicator.fillAmount;
        Color originalColor = indicator.color;
        float timePassed = 0;

        while (timePassed < Gameplay.transitionTime) {
            timePassed += Time.deltaTime;
            transform.localScale = new Vector3 (1, Mathf.Lerp(originalHeight, newHeight, timePassed / Gameplay.transitionTime), 1);
            if (newHeight != 1) {
                indicator.fillAmount = Mathf.Lerp(originalIndFill, 1, timePassed / Gameplay.transitionTime);
                indicator.color = Color.Lerp(originalColor, defaultLineColor, timePassed / Gameplay.transitionTime);
            }

            yield return null;
        }

        if (newHeight != 1) {
            yield return new WaitForSecondsRealtime (Gameplay.instance.baseTime);
            wasRecentlyUsed = false;
        }
    }
}