using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Crystal : MonoBehaviour {

    public static Crystal instance;

    public Image TimeImage;
    public GameObject TimeLine;
    public TextMeshProUGUI Header;
    public GameObject ScoreText;
    public Gradient ColorGradient;
    
    public Material CrystalMat;
    
    float HealthLeft;
    float TimeLeft;
    float TimePlanned;

    void Awake () {
        instance = this;
    }

    public void Uprise () {
        CrystalMat.SetFloat("Vector1_11351534", 0);
        CrystalMat.SetFloat("Vector1_CF4763B6", 0);
        HealthLeft = 100;
        TimePlanned = Gameplay.instance.BaseTime * 25;
        TimeLeft = TimePlanned;
        StartCoroutine(CrystalLife());
        Header.text = "TIME LEFT";
        TimeLine.SetActive(true);
        TimeImage.fillAmount = 0;
        ScoreText.SetActive(false);
    }

    public void Damage () {
        HealthLeft = Mathf.Clamp(HealthLeft - 1, 0, 100);
    }

    IEnumerator CrystalLife () {
        CameraController.instance.MoveAway();
        float TimePassed = 0;
        while (TimePassed < Gameplay.TransitionTime * 3.333f) {
            TimePassed += Time.deltaTime;
            transform.localPosition = new Vector3 (0, Mathf.Lerp(-8, 0, TimePassed / (Gameplay.TransitionTime * 3.333f)), 0);
            yield return null;
        }

        while (TimeLeft > 0 && HealthLeft > 0) {
            TimeLeft -= Time.deltaTime;
            CrystalMat.SetFloat("Vector1_11351534", (100 - HealthLeft)/100);
            CrystalMat.SetFloat("Vector1_CF4763B6", 1 - TimeLeft / TimePlanned);
            TimeImage.fillAmount = TimeLeft / TimePlanned;
            TimeImage.color = ColorGradient.Evaluate(1 - TimeLeft / TimePlanned);
            yield return null;
        }

        if (HealthLeft > 0) {
            Gameplay.instance.Miss(true, true);
        } else {
            Gameplay.instance.Success(true);
        }

        Header.text = "SCORE";
        TimeImage.fillAmount = 0;

        CameraController.instance.MoveCloser();
        TimePassed = 0;
        while (TimePassed < Gameplay.TransitionTime * 3.333f) {
            TimePassed += Time.deltaTime;
            transform.localPosition = new Vector3 (0, Mathf.Lerp(0, -8, TimePassed / (Gameplay.TransitionTime * 3.333f)), 0);
            yield return null;
        }

        ScoreText.SetActive(true);
        TimeLine.SetActive(false);
        Gameplay.instance.ContinuePlay();
        yield return new WaitForSecondsRealtime (Gameplay.TransitionTime);
        PlayUI.instance.ScoreSize();
    }
}