using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexExplosion : MonoBehaviour {

    IEnumerator Start () {
        yield return new WaitForSecondsRealtime (1);
        Destroy(gameObject);
    }
}