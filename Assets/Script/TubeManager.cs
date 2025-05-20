using UnityEngine;
using DG.Tweening;

public class TubeManager : MonoBehaviour {
    public static TubeManager Instance;

    [Header("Prefab")]
    public GameObject tubePrefab;

    private Transform tubeGridLayoutParent; 

    [Header("Tube Tracking")]
    public TubeController tubeSpawned;
    private int pressButtonCount = 0;

    [Header("Nut Move State")]
    public GameObject liftedNut;
    public TubeController sourceTube;
    public bool IsAnimating { get; private set; }

    // ====== Undo tracking ======
    private GameObject lastMovedNut;
    private TubeController lastSourceTube;
    private TubeController lastTargetTube;

    void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        GameObject found = GameObject.Find("TubeGridLayout");
        if (found != null) {
            tubeGridLayoutParent = found.transform;
        } else {
            Debug.LogError("Không tìm thấy GameObject tên 'TubeGridLayout' trong scene!");
        }
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
        pressButtonCount++;

        if (pressButtonCount == 1) {
            if (tubeSpawned != null) {
                Debug.LogWarning("Tube đã được spawn.");
                return;
            }

            if (tubeGridLayoutParent == null) {
                Debug.LogError("Không tìm thấy TubeGridLayout.");
                return;
            }

            GameObject newTube = Instantiate(tubePrefab, Vector3.zero, Quaternion.identity, tubeGridLayoutParent);
            tubeSpawned = newTube.GetComponent<TubeController>();
            return;
        }

        var spawnManager = FindObjectOfType<SpawnPosOfNewTubeManager>();
        int indexToActivate = pressButtonCount - 2;
        if (spawnManager != null && indexToActivate < spawnManager.spawnPos.Length) {
            spawnManager.spawnPos[indexToActivate].SetActive(true);
        }
    }

    public void RevealNextSpawnPointForSpawnedTube() {
        if (tubeSpawned != null) {
            tubeSpawned.RevealNextSpawnPoint();
        } else {
            Debug.LogWarning("Chưa có tube nào được spawn.");
        }
    }
}
