using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Gameplay : MonoBehaviour {

    public static Gameplay instance;
    public UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset PipelineAsset;

    public static float Difficulty = 1;
    public static int Score;
    public static int Misses;

    public int HexGenTime = 2;
    public float AdditionalDifficulty = 0.02f;
    public float HexLiveMult = 2;
    public int DefaultReward = 5;

    public Hex[] Hexes;
    List<int> possibleIndexes = new List<int>();

    private Coroutine HexesCoroutine;
    private Coroutine CrystalCoroutine;

    public static float TransitionTime = 0.3f;

    void Awake () {
        instance = this;
    }

    void Start () {
        BeginPlay();
    }

    public void UpdateHexGenTime (int Value) {
        HexGenTime = Value;
        ClearField();
    }

    public void UpdateAdditionalDifficulty (float Value) {
        AdditionalDifficulty = Value;
        ClearField();
    }

    public void UpdateHexLiveMult (float Value) {
        HexLiveMult = Value;
        ClearField();
    }

    public void BeginPlay () {
        if(Screen.dpi > 0) {
            PipelineAsset.renderScale = Mathf.Clamp(0.5f * 403 / Screen.dpi, 0.25f, 1);
        } else {
            PipelineAsset.renderScale = 0.5f;
        }
        
        Difficulty = 1;
        Score = 0;
        Misses = 5;
        UIManager.instance.LastMisses = Misses;
        UIManager.instance.UpdateScore();
        ContinuePlay();
    }

    public void EndPlay () {
        ClearField();
        //GIVE A REWARD AND STUFF
    }

    public void ClearField () {
        StopCoroutine(HexesCoroutine);
        StopCoroutine(CrystalCoroutine);
        for (int i=0; i<19; i++) {
            Hexes[i].Hide();
        }
    }

    public void ContinuePlay () {
        HexesCoroutine = StartCoroutine(HexesLoop());
        CrystalCoroutine = StartCoroutine(CrystalWait());
        CameraController.CrystalMode = false;
    }

    public void Success (bool Crystal) {
        if (Crystal) {
            Difficulty += AdditionalDifficulty * 2;
            Score += DefaultReward * 100;
        } else {
            Difficulty += AdditionalDifficulty;
            Score += DefaultReward;
        }
    
        TransitionTime = 0.3f / Difficulty;
        UIManager.instance.UpdateScore();
    }

    public void Miss (bool Explosion, bool Crystal) {
        if (Crystal) {
            Misses -= 2;
            Difficulty += AdditionalDifficulty * 2;
        } else {
            Misses--;
        }

        if (Misses < 0) {
            EndPlay();
        } else {
            UIManager.instance.UpdateMisses();
        }  
    }

    IEnumerator CrystalWait () {
        int waitTime = Random.Range(HexGenTime * 60, HexGenTime * 90);
        //yield return new WaitForSecondsRealtime (waitTime / Difficulty);
        yield return new WaitForSecondsRealtime (5 / Difficulty);

        ClearField();
        CameraController.CrystalMode = true;
        Crystal.instance.Uprise();
    }

    IEnumerator HexesLoop () {
        while (true) {
            for (int i=0; i<Hexes.Length; i++) {
                if (!Hexes[i].wasRecentlyUsed) possibleIndexes.Add(i);
            }

            if (possibleIndexes.Count > 0) {
                int hexIndex = Random.Range(0, possibleIndexes.Count);
                Hexes[possibleIndexes[hexIndex]].Uprise();
                possibleIndexes.Clear();
            }
        
            yield return new WaitForSecondsRealtime (HexGenTime / Difficulty);
        }
    }
}