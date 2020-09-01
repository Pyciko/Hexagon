using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexExplosion : MonoBehaviour {

    IEnumerator Start () {
        float Lifetime = Gameplay.instance.BaseTime / 3;
        GetComponent<ParticleSystem>().startLifetime = Lifetime;

        yield return new WaitForSecondsRealtime (Lifetime + 0.1f);
        Destroy(gameObject);
    }
}