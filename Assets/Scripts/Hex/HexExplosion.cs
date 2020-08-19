using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexExplosion : MonoBehaviour {

    IEnumerator Start () {
        float Lifetime = 0.4f;
        Lifetime /= Gameplay.Difficulty;
        GetComponent<ParticleSystem>().startLifetime = Lifetime;

        yield return new WaitForSecondsRealtime (Lifetime + 0.1f);
        Destroy(gameObject);
    }
}