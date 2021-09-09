using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayUI : MonoBehaviour {

    public static PlayUI instance;

    public TextMeshProUGUI header;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI missesText;
    public RectTransform missesRect;
    public RectTransform scoreRect;

    public AnimationCurve ScoreSizeCurve;

    public int previousMisses;
    int nextMisses;

    bool missesShown = false;
    float shownTime = 0;

    void Awake () {
        instance = this;
    }

    public void UpdateScore() {
        StartCoroutine(SmoothScoreChange());
    }

    public void ScoreSize () {
        StartCoroutine(ScoreSizeChange());
    }

    IEnumerator ScoreSizeChange () {
        float TimePassed = 0;
        while (TimePassed < Gameplay.transitionTime * 2) {
            TimePassed += Time.deltaTime;
            scoreRect.localScale = Vector3.one * ScoreSizeCurve.Evaluate(TimePassed/(Gameplay.transitionTime*2));
            yield return null;
        }
    }

    IEnumerator SmoothScoreChange () {
        int NextScore = Gameplay.score;
        int LastScore = int.Parse(scoreText.text);

        float TimePassed = 0;
        while (TimePassed < Gameplay.transitionTime) {
            TimePassed += Time.deltaTime;
            scoreText.text = Mathf.RoundToInt(Mathf.Lerp(LastScore, NextScore, TimePassed / Gameplay.transitionTime)).ToString();
            yield return null;
        }
    }

    public void UpdateMisses() {
        if (!missesShown) {
            StartCoroutine(SmoothMissesChange());
        } else {
            StartCoroutine(MissesBlink());
        }
    }

    private IEnumerator MissesFade () {
        if (!missesShown) {
            header.text = "MISSES";
            scoreText.color = new Color (0, 0, 0, 0);
            missesText.color = new Color (0, 0, 0, 1);
            missesShown = true;

            float TimePassed = 0;

            while (TimePassed < Gameplay.transitionTime) {
                TimePassed += Time.deltaTime;
                missesText.color = new Color(1, 0, 0, TimePassed / Gameplay.transitionTime);
                yield return null;
            }
        }
    }

    private IEnumerator MissesBlink () {
        nextMisses = Gameplay.misses;
        Vector3 oldPos = new Vector3 (-38.6f * previousMisses, -292, 0);
        Vector3 newPos = new Vector3 (-38.6f * nextMisses, -292, 0);
        missesRect.anchoredPosition = oldPos;
        int missesDelta = previousMisses - nextMisses;
        shownTime = 0;
        for (int i=0; i<3; i++) {
            missesText.text = new string('⬡', previousMisses);
            yield return new WaitForSecondsRealtime (Gameplay.instance.baseTime/10);
            if (previousMisses > nextMisses) missesText.text = new string('⬡', previousMisses - missesDelta) + new string('⬢', missesDelta);
            yield return new WaitForSecondsRealtime (Gameplay.instance.baseTime/10);
        }
        previousMisses = Gameplay.misses;
        missesText.text = new string('⬡', nextMisses);

        float TimePassed = 0;
        while (TimePassed < Gameplay.transitionTime) {
            TimePassed += Time.deltaTime;
            missesRect.anchoredPosition = Vector3.Lerp(oldPos, newPos, TimePassed/Gameplay.transitionTime);
            yield return null;
        }
    }

    private IEnumerator SmoothMissesChange () {
        yield return MissesFade();
        yield return new WaitForSecondsRealtime (Gameplay.instance.baseTime / 2);
        yield return MissesBlink();
        
        while (shownTime < Gameplay.instance.baseTime) {
            shownTime += Time.deltaTime;
            yield return null;
        }

        float TimePassed = 0;
        while (TimePassed < Gameplay.transitionTime) {
            TimePassed += Time.deltaTime;
            missesText.color = new Color(1, 0, 0, 1 - (TimePassed / Gameplay.transitionTime));
            yield return null;
        }
        
        shownTime = 0;
        missesShown = false;
        scoreText.color = new Color (0, 0, 0, 1);
        header.text = "SCORE";
    }
}