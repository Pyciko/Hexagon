using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexExplosion : MonoBehaviour {

    IEnumerator Boom () {
        float lifetime = Gameplay.instance.baseTime / 3;
        GetComponent<ParticleSystem>().startLifetime = lifetime;

        yield return new WaitForSecondsRealtime (lifetime + 0.1f);
        Destroy(gameObject);
    }
}