using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    Camera Cam;
    public static bool CrystalMode;

    public static CameraController instance;

    public Vector3 ClosePos;
    public Vector3 CloseRot;
    public float CloseFov;

    public Vector3 FarPos;
    public Vector3 FarRot;
    public float FarFov;

    public AnimationCurve MovementCurve;

    void Awake () {
        instance = this;
    }

    void Start () {
        Cam = GetComponent<Camera>();
    }

    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            RaycastHit hit;
            if (Physics.Raycast(Cam.ScreenPointToRay(Input.mousePosition), out hit)) {
                if (CrystalMode) {
                    if (hit.collider.GetComponent<Crystal>() != null) {
                        Crystal.instance.Damage();
                    }
                } else {
                    if (hit.collider.GetComponent<Hex>().isUprised) {
                        hit.collider.GetComponent<Hex>().CoolDown();
                    } else {
                        Gameplay.instance.Miss(false, false);
                    }
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.E)) {
            MoveAway();
        }
        if (Input.GetKeyDown(KeyCode.Q)) {
            MoveCloser();
        }
    }

    public void MoveAway () {
        StartCoroutine(CamTransition(true));
    }

    public void MoveCloser () {
        StartCoroutine(CamTransition(false));
    }

    IEnumerator CamTransition (bool Away) {
        float TimePassed = 0;
        float TimePlanned = Gameplay.TransitionTime * 3.333f;
        float evalValue = 0;
        while (TimePassed < TimePlanned) {
            TimePassed += Time.deltaTime;
            evalValue = MovementCurve.Evaluate(TimePassed / TimePlanned);
            if (Away) {
                transform.position = Vector3.Lerp(ClosePos, FarPos, evalValue);
                transform.rotation = Quaternion.Euler(Vector3.Lerp(CloseRot, FarRot, evalValue));
                Cam.orthographicSize = Mathf.Lerp(CloseFov, FarFov, evalValue);
            } else {
                transform.position = Vector3.Lerp(FarPos, ClosePos, evalValue);
                transform.rotation = Quaternion.Euler(Vector3.Lerp(FarRot, CloseRot, evalValue));
                Cam.orthographicSize = Mathf.Lerp(FarFov, CloseFov, evalValue);
            }
            yield return null;
        }
    }
}