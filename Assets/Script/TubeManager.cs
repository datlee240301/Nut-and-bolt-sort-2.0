using UnityEngine;

public class TubeManager : MonoBehaviour {
    public static TubeManager Instance;

    public GameObject liftedNut;
    public TubeController sourceTube;

    public bool IsAnimating { get; private set; }

    void Awake() {
        Instance = this;
    }

    public void SetAnimating(bool value) {
        IsAnimating = value;
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
        IsAnimating = false; // ✅ Cho phép nhấn lại tube sau khi hoàn tất
    }
}
