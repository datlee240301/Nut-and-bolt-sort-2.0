using UnityEngine;
using DG.Tweening;

public class TubeManager : MonoBehaviour {
    public static TubeManager Instance;

    public GameObject liftedNut;
    public TubeController sourceTube;

    public GameObject tubePrefab;
    public Transform tubeSpawnPoint;

    public TubeController tubeSpawned;

    public bool IsAnimating { get; private set; }

    // ====== Undo tracking ======
    private GameObject lastMovedNut;
    private TubeController lastSourceTube;
    private TubeController lastTargetTube;

    void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
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

    public void SetAnimating(bool value) {
        IsAnimating = value;
    }

    // ====== Undo Support ======
    public void RegisterMove(GameObject nut, TubeController fromTube, TubeController toTube) {
        lastMovedNut = nut;
        lastSourceTube = fromTube;
        lastTargetTube = toTube;
    }

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

    // ====== Tube Spawn + Reveal Support ======
    public void SpawnNewTube() {
        if (tubeSpawned != null) {
            Debug.LogWarning("Tube đã được spawn.");
            return;
        }

        GameObject newTube = Instantiate(tubePrefab, tubeSpawnPoint.position, Quaternion.identity);
        tubeSpawned = newTube.GetComponent<TubeController>();
    }

    public void RevealNextSpawnPointForSpawnedTube() {
        if (tubeSpawned != null) {
            tubeSpawned.RevealNextSpawnPoint();
        } else {
            Debug.LogWarning("Chưa có tube nào được spawn.");
        }
    }
}
