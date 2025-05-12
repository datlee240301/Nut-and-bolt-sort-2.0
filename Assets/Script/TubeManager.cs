using DG.Tweening;
using UnityEngine;

public class TubeManager : MonoBehaviour {
    public static TubeManager Instance;

    public GameObject liftedNut;
    public TubeController sourceTube;

    public bool IsAnimating { get; private set; }

    // ====== Thông tin để Undo ======
    private GameObject lastMovedNut;
    private TubeController lastSourceTube;
    private TubeController lastTargetTube;

    void Awake() {
        Instance = this;
    }

    public void SetAnimating(bool value) {
        IsAnimating = value;
    }

    public bool HasLiftedNut() => liftedNut != null;

    public void SetLiftedNut(GameObject nut, TubeController tube) {
        liftedNut = nut;
        sourceTube = tube;
    }

    public void ClearLiftedNut() {
        liftedNut = null;
        sourceTube = null;
        IsAnimating = false;
    }

    // ====== Ghi lại thao tác để Undo ======
    public void RegisterMove(GameObject nut, TubeController fromTube, TubeController toTube) {
        lastMovedNut = nut;
        lastSourceTube = fromTube;
        lastTargetTube = toTube;
    }

    // ====== Hàm Undo để gắn vào button ======
    public void UndoLastMove() {
        if (IsAnimating || lastMovedNut == null || lastTargetTube == null || lastSourceTube == null) return;

        if (!lastTargetTube.GetCurrentNuts().Contains(lastMovedNut)) return;

        SetAnimating(true);
        lastTargetTube.RemoveNut(lastMovedNut);

        int returnIndex = lastSourceTube.GetCurrentNuts().Count;
        Vector3 returnPos = lastSourceTube.spawnPoints[returnIndex].position;

        lastMovedNut.transform.DOMove(returnPos, 0.3f).SetEase(Ease.OutQuad).OnComplete(() => {
            lastMovedNut.transform.SetParent(lastSourceTube.transform);
            lastMovedNut.transform.localScale = lastSourceTube.GetOriginalScale();
            lastSourceTube.AddNut(lastMovedNut);

            ClearLiftedNut();
            lastMovedNut = null;
            lastSourceTube = null;
            lastTargetTube = null;
        });
    }
}
