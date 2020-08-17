using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gameplay : MonoBehaviour {

    public static Gameplay instance;

    public static float Difficulty = 1;
    public static int Score;
    public static int Misses;

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
        Difficulty += 0.02f;
        Score += 5;
        TransitionTime = 0.3f / Difficulty;
        UIManager.instance.UpdateScore();
    }

    public void Miss (bool Explosion) {
        Misses--;
        UIManager.instance.UpdateMisses();
        if (Misses < 0) 
            EndPlay();
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
        
            yield return new WaitForSecondsRealtime (1 / Difficulty);
        }
    }
}