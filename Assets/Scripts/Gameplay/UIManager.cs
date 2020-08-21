using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour {

    public static UIManager instance;

    public TextMeshProUGUI Header;
    public TextMeshProUGUI ScoreText;
    public TextMeshProUGUI MissesText;
    public RectTransform MissesRect;

    public int LastMisses;
    int NextMisses;

    bool MissesShown = false;
    float ShownTime = 0;

    void Awake () {
        instance = this;
    }

    public void UpdateScore() {
        StartCoroutine(SmoothScoreChange());
    }

    IEnumerator SmoothScoreChange () {
        int NextScore = Gameplay.Score;
        int LastScore = int.Parse(ScoreText.text);

        float TimePassed = 0;
        while (TimePassed < Gameplay.TransitionTime) {
            TimePassed += Time.deltaTime;
            ScoreText.text = Mathf.RoundToInt(Mathf.Lerp(LastScore, NextScore, TimePassed / Gameplay.TransitionTime)).ToString();
            yield return null;
        }
    }

    public void UpdateMisses() {
        if (!MissesShown) {
            StartCoroutine(SmoothMissesChange());
        } else {
            StartCoroutine(MissesBlink());
        }
    }

    IEnumerator MissesFade () {
        if (!MissesShown) {
            Header.text = "MISSES";
            ScoreText.color = new Color (0, 0, 0, 0);
            MissesText.color = new Color (0, 0, 0, 1);
            MissesShown = true;

            float TimePassed = 0;

            while (TimePassed < Gameplay.TransitionTime) {
                TimePassed += Time.deltaTime;
                MissesText.color = new Color(1, 0, 0, TimePassed / Gameplay.TransitionTime);
                yield return null;
            }
        }
    }

    IEnumerator MissesBlink () {
        NextMisses = Gameplay.Misses;
        Vector3 oldPos = new Vector3 (-38.6f * LastMisses, -298, 0);
        Vector3 newPos = new Vector3 (-38.6f * NextMisses, -298, 0);
        MissesRect.anchoredPosition = oldPos;
        int missesDelta = LastMisses - NextMisses;
        ShownTime = 0;
        for (int i=0; i<3; i++) {
            MissesText.text = new string('⬡', LastMisses);
            yield return new WaitForSecondsRealtime (0.1f/Gameplay.Difficulty);
            if (LastMisses > NextMisses) MissesText.text = new string('⬡', LastMisses - missesDelta) + new string('⬢', missesDelta);
            yield return new WaitForSecondsRealtime (0.1f/Gameplay.Difficulty);
        }
        LastMisses = Gameplay.Misses;
        MissesText.text = new string('⬡', NextMisses);

        float TimePassed = 0;
        while (TimePassed < Gameplay.TransitionTime) {
            TimePassed += Time.deltaTime;
            MissesRect.anchoredPosition = Vector3.Lerp(oldPos, newPos, TimePassed/Gameplay.TransitionTime);
            yield return null;
        }
    }

    IEnumerator SmoothMissesChange () {
        yield return MissesFade();
        yield return new WaitForSecondsRealtime (0.5f/Gameplay.Difficulty);
        yield return MissesBlink();

        while (ShownTime < 1 / Gameplay.Difficulty) {
            ShownTime += Time.deltaTime;
            yield return null;
        }

        float TimePassed = 0;
        while (TimePassed < Gameplay.TransitionTime) {
            TimePassed += Time.deltaTime;
            MissesText.color = new Color(1, 0, 0, 1 - (TimePassed / Gameplay.TransitionTime));
            yield return null;
        }
        
        ShownTime = 0;
        MissesShown = false;
        ScoreText.color = new Color (0, 0, 0, 1);
        Header.text = "SCORE";
    }
}