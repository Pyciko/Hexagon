using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Crystal : MonoBehaviour {

    public static Crystal instance;

    public Image timeImage;
    public GameObject timeLine;
    public TextMeshProUGUI header;
    public GameObject scoreText;
    public Gradient colorGradient;
    
    public Material crystalMat;
    
    private float healthLeft;
    private float timeLeft;
    private float timePlanned;

    void Awake () {
        instance = this;
    }

    public void Uprise () {
        crystalMat.SetFloat("Vector1_11351534", 0);
        crystalMat.SetFloat("Vector1_CF4763B6", 0);
        healthLeft = 100;
        timePlanned = Gameplay.instance.baseTime * 35;
        timeLeft = timePlanned;
        StartCoroutine(CrystalLife());
        header.text = "TIME LEFT";
        timeLine.SetActive(true);
        timeImage.fillAmount = 0;
        scoreText.SetActive(false);
    }

    public void Damage () {
        healthLeft = Mathf.Clamp(healthLeft - 1, 0, 100);
    }

    private IEnumerator CrystalLife () {
        CameraController.instance.MoveAway();
        float TimePassed = 0;
        while (TimePassed < Gameplay.transitionTime * 3.333f) {
            TimePassed += Time.deltaTime;
            transform.localPosition = new Vector3 (0, Mathf.Lerp(-8, 0, TimePassed / (Gameplay.transitionTime * 3.333f)), 0);
            yield return null;
        }

        while (timeLeft > 0 && healthLeft > 0) {
            timeLeft -= Time.deltaTime;
            crystalMat.SetFloat("Vector1_11351534", (100 - healthLeft)/100);
            crystalMat.SetFloat("Vector1_CF4763B6", 1 - timeLeft / timePlanned);
            timeImage.fillAmount = timeLeft / timePlanned;
            timeImage.color = colorGradient.Evaluate(1 - timeLeft / timePlanned);
            yield return null;
        }

        if (healthLeft > 0) {
            Gameplay.instance.Miss(true, true);
        } else {
            Gameplay.instance.Success(true);
        }

        header.text = "SCORE";
        timeImage.fillAmount = 0;

        CameraController.instance.MoveCloser();
        TimePassed = 0;
        while (TimePassed < Gameplay.transitionTime * 3.333f) {
            TimePassed += Time.deltaTime;
            transform.localPosition = new Vector3 (0, Mathf.Lerp(0, -8, TimePassed / (Gameplay.transitionTime * 3.333f)), 0);
            yield return null;
        }

        scoreText.SetActive(true);
        timeLine.SetActive(false);
        Gameplay.instance.ContinuePlay();
        yield return new WaitForSecondsRealtime (Gameplay.transitionTime);
        PlayUI.instance.ScoreSize();
    }
}