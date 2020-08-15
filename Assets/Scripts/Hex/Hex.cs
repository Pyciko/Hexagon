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
    float TimePassed;

    public void Uprise() {
        if (!isUprised && !wasRecentlyUsed) {
            StartCoroutine(SmoothMove(1));
            isUprised = true;
            wasRecentlyUsed = true;
            
            TimePlanned = 3 / GameManager.instance.Difficulty;
            TimePassed = 0;
        }
    }

    void Update () {
        if (isUprised) {
            TimePassed += Time.deltaTime;
            Indicator.fillAmount = TimePassed / TimePlanned;
            Indicator.color = GradientIndicator.Evaluate(TimePassed / TimePlanned);

            if (TimePassed >= TimePlanned) {
                //EXPLODE
                Explode();
            }
        }
    }

    public void Explode () {
        GameManager.instance.Miss(true);
        gameObject.SetActive(false);
    }

    public void Hide () {
        if (isUprised) {
            StartCoroutine(SmoothMove(0.1f));
            isUprised = false;
            Indicator.color = DefaultLineColor;
            Indicator.fillAmount = 1;

            //ADD SOME SCORE POINTS AND SPEED UP THE GAME
            GameManager.instance.Success();
            StartCoroutine (Hiding());
        } else {
            //REMOVE MISSES
            GameManager.instance.Miss(false);
        }
    }

    IEnumerator Hiding () {
        yield return new WaitForSecondsRealtime (2/GameManager.instance.Difficulty);
        wasRecentlyUsed = false;
    }

    IEnumerator SmoothMove (float newHeight) {
        // float currentMovementTime = 0;
        // float originalHeight = transform.localScale.y;
        // float Height = 1;
        // float animTime = 0.1f / GameManager.instance.Difficulty;
        // while (transform.localScale.y != newHeight) {
        //     currentMovementTime += Time.deltaTime;
        //     Height = Mathf.Lerp(originalHeight, newHeight, currentMovementTime / animTime);
        //     transform.localScale = new Vector3 (1, Height, 1);
        //     yield return null;
        // }
        transform.localScale = new Vector3 (1, newHeight, 1);
        yield return null;
    }
}