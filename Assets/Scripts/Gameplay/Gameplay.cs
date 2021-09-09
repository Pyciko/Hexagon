using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gameplay : MonoBehaviour {

    public static Gameplay instance;
    public UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset pipelineAsset;

    public static int score;
    public static int misses;

    public float baseTime = 1;
    public float additionalDifficulty = 1.005f;
    public float hexLifetimeMult = 2;
    public int defaultReward = 5;

    public Hex[] Hexes;
    private List<int> possibleIndexes = new List<int>();

    private Coroutine hexesCoroutine;
    private Coroutine crystalCoroutine;

    public static float transitionTime = 0.3f;

    void Awake () {
        instance = this;
    }

    void Start () {
        BeginPlay();
    }

    public void BeginPlay () {
        if(Screen.dpi > 0) {
            pipelineAsset.renderScale = Mathf.Clamp(0.5f * 403 / Screen.dpi, 0.25f, 1);
        } else {
            pipelineAsset.renderScale = 0.5f;
        }
        
        baseTime = 1;
        score = 0;
        misses = 5;
        PlayUI.instance.previousMisses = misses;
        PlayUI.instance.UpdateScore();
        ContinuePlay();
    }

    public void EndPlay () {
        ClearField();
        //GIVE A REWARD AND STUFF
    }

    public void ClearField () {
        StopCoroutine(hexesCoroutine);
        StopCoroutine(crystalCoroutine);
        for (int i=0; i<19; i++) {
            Hexes[i].Hide();
        }
    }

    public void ContinuePlay () {
        hexesCoroutine = StartCoroutine(HexesLoop());
        crystalCoroutine = StartCoroutine(CrystalWait());
        CameraController.crystalMode = false;
    }

    public void Success (bool Crystal) {
        if (Crystal) {
            //Difficulty += AdditionalDifficulty * 2;
            baseTime /= additionalDifficulty;
            score += defaultReward * 100;
        } else {
            //Difficulty += AdditionalDifficulty;
            baseTime /= additionalDifficulty;
            score += defaultReward;
        }
    
        transitionTime = baseTime / 3;
        PlayUI.instance.UpdateScore();
    }

    public void Miss (bool Explosion, bool Crystal) {
        if (Crystal) {
            misses -= 2;
            baseTime /= additionalDifficulty;
        } else {
            misses--;
        }

        if (misses < 0) {
            EndPlay();
        } else {
            PlayUI.instance.UpdateMisses();
        }  
    }

    private IEnumerator CrystalWait () {
        float waitTime = Random.Range(baseTime * 30, baseTime * 60);
        yield return new WaitForSecondsRealtime (waitTime);

        ClearField();
        CameraController.crystalMode = true;
        Crystal.instance.Uprise();
    }

    private IEnumerator HexesLoop () {
        while (true) {
            for (int i=0; i<Hexes.Length; i++) {
                if (!Hexes[i].wasRecentlyUsed) possibleIndexes.Add(i);
            }

            if (possibleIndexes.Count > 0) {
                int hexIndex = Random.Range(0, possibleIndexes.Count);
                Hexes[possibleIndexes[hexIndex]].Raise();
                possibleIndexes.Clear();
            }
        
            yield return new WaitForSecondsRealtime (baseTime);
        }
    }
}