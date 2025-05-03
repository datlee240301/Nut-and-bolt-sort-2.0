using UnityEngine;

public class TubeManager : MonoBehaviour {
    public static TubeManager Instance;

    [HideInInspector] public GameObject liftedNut = null;
    [HideInInspector] public TubeController sourceTube = null;

    void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public bool HasLiftedNut() {
        return liftedNut != null;
    }

    public void SetLiftedNut(GameObject nut, TubeController tube) {
        liftedNut = nut;
        sourceTube = tube;
    }

    public void ClearLiftedNut() {
        liftedNut = null;
        sourceTube = null;
    }
}
