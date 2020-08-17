using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchManager : MonoBehaviour {

    public Camera MainCam;

    void Update() {
        // for (int i = 0; i < Input.touchCount; ++i) {
        //      if (Input.GetTouch(i).phase == TouchPhase.Began) {

        //          RaycastHit hit;
        //          if (Physics.Raycast(MainCam.ScreenPointToRay(Input.GetTouch(i).position), out hit))
        //             hit.collider.GetComponent<Hex>().Hide(false);
        //      }
        // }

        if (Input.GetMouseButtonDown(0)) {
            RaycastHit hit;
            if (Physics.Raycast(MainCam.ScreenPointToRay(Input.mousePosition), out hit)) {
                if (hit.collider.GetComponent<Hex>().isUprised) {
                    hit.collider.GetComponent<Hex>().CoolDown();
                } else {
                    Gameplay.instance.Miss(false);
                }
            }
        }
    }
}