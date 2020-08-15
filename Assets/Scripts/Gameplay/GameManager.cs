using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour {

    public static GameManager instance;

    public GameUIManager UIManager;

    public float Difficulty;
    public int Score;
    public int Misses;

    public Hex[] Hexes;
    
    void Awake () {
        instance = this;
    }

    void Start () {
        StartCoroutine(MainLoop());
    }

    IEnumerator MainLoop () {
        List<int> possibleIndexes = new List<int>();
        for (int i=0; i<Hexes.Length; i++) {
            if (!Hexes[i].wasRecentlyUsed) possibleIndexes.Add(i);
        }

        if (possibleIndexes.Count > 0) {
            int hexIndex = Random.Range(0, possibleIndexes.Count);
            Hexes[possibleIndexes[hexIndex]].Uprise();
        }
    
        yield return new WaitForSecondsRealtime (2 / Difficulty);
        StartCoroutine (MainLoop());
    }

    public void Success () {
        Difficulty += 0.02f;
        Score += 10;
        UIManager.UpdateScore();
    }

    public void Miss (bool Explosion) {
        Misses--;
        if (Misses < 0) 
            Application.Quit();
    }
}