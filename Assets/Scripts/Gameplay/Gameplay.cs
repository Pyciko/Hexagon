using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Gameplay : MonoBehaviour {

    public static Gameplay instance;

    public static float Difficulty = 1;
    public static int Score;
    public static int Misses;

    public int HexGenTime = 2;
    public float AdditionalDifficulty = 0.02f;
    public float HexLiveMult = 2;
    public UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset PipelineAsset;

    public Hex[] Hexes;
    List<int> possibleIndexes = new List<int>();

    private Coroutine HexesCoroutine;

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

    public void ChangeRenderScale (float Value) {
        PipelineAsset.renderScale = Value;
    }

    public void BeginPlay () {
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
        for (int i=0; i<19; i++) {
            Hexes[i].Hide();
        }
    }

    public void ContinuePlay () {
        HexesCoroutine = StartCoroutine(HexesLoop());
    }

    public void Success () {
        Difficulty += AdditionalDifficulty;
        Score += 5;
        TransitionTime = 0.3f / Difficulty;
        UIManager.instance.UpdateScore();
    }

    public void Miss (bool Explosion) {
        Misses--;
        if (Misses < 0) {
            EndPlay();
        } else {
            UIManager.instance.UpdateMisses();
        }   
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