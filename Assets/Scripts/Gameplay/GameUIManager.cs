using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameUIManager : MonoBehaviour {

    public TextMeshProUGUI ScoreText;
    public TextMeshProUGUI MissesText;

    int LastScore = 0;

    public void UpdateScore() {
        StartCoroutine(SmoothScoreChange());
    }

    IEnumerator SmoothScoreChange () {
        int NextScore = GameManager.instance.Score;
        float currentMovementTime = 0;
        int scoreToDraw = 0;
        float animTime = 0.1f / GameManager.instance.Difficulty;
        while (scoreToDraw < NextScore) {
            currentMovementTime += Time.deltaTime;
            scoreToDraw = Mathf.RoundToInt(Mathf.Lerp(LastScore, NextScore, currentMovementTime / animTime));
            ScoreText.text = scoreToDraw.ToString();
            yield return null;
        }
        LastScore = scoreToDraw;
        MissesText.text = GameManager.instance.Misses.ToString();
    }
}