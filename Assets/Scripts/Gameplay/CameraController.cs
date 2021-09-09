using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    private Camera cam;
    public static bool crystalMode;

    public static CameraController instance;

    public Vector3 closePos;
    public Vector3 closeRot;
    public float closeFov;

    public Vector3 farPos;
    public Vector3 farRot;
    public float farFov;

    public AnimationCurve movementCurve;

    void Awake () {
        instance = this;
        cam = GetComponent<Camera>();
    }

    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            RaycastHit hit;
            if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit)) {
                if (crystalMode) {
                    if (hit.collider.GetComponent<Crystal>() != null) {
                        Crystal.instance.Damage();
                    }
                } else {
                    if (hit.collider.GetComponent<Hex>().isRaised) {
                        hit.collider.GetComponent<Hex>().CoolDown();
                    } else {
                        Gameplay.instance.Miss(false, false);
                    }
                }
            }
        }
    }

    public void MoveAway () {
        StartCoroutine(CamTransition(true));
    }

    public void MoveCloser () {
        StartCoroutine(CamTransition(false));
    }

    private IEnumerator CamTransition (bool Away) {
        float timePassed = 0;
        float timePlanned = Gameplay.transitionTime * 3.333f;
        float evalValue = 0;
        while (timePassed < timePlanned) {
            timePassed += Time.deltaTime;
            evalValue = movementCurve.Evaluate(timePassed / timePlanned);
            if (Away) {
                transform.position = Vector3.Lerp(closePos, farPos, evalValue);
                transform.rotation = Quaternion.Euler(Vector3.Lerp(closeRot, farRot, evalValue));
                cam.orthographicSize = Mathf.Lerp(closeFov, farFov, evalValue);
            } else {
                transform.position = Vector3.Lerp(farPos, closePos, evalValue);
                transform.rotation = Quaternion.Euler(Vector3.Lerp(farRot, closeRot, evalValue));
                cam.orthographicSize = Mathf.Lerp(farFov, closeFov, evalValue);
            }
            yield return null;
        }
    }
}