using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class TubeController : MonoBehaviour {
    public Transform[] spawnPoints;
    public GameObject[] nutPrefabs;
    public GameObject[] specialNutsPrefabs;
    public Transform waitPoint;

    private List<GameObject> currentNuts = new List<GameObject>();
    public List<GameObject> spawnedSpecialNuts = new List<GameObject>();

    private Vector3 originalScale;

    void Start() {
        SpawnNuts();
    }

    void SpawnNuts() {
        for (int i = 0; i < Mathf.Min(spawnPoints.Length, nutPrefabs.Length); i++) {
            GameObject nut = Instantiate(nutPrefabs[i], spawnPoints[i].position, Quaternion.identity);
            nut.transform.SetParent(transform);
            originalScale = nut.transform.localScale;
            currentNuts.Add(nut);
        }

        // Spawn specialNuts (sẽ nằm lên trên nut)
        for (int i = 0; i < Mathf.Min(spawnPoints.Length, specialNutsPrefabs.Length); i++) {
            GameObject special = Instantiate(specialNutsPrefabs[i], spawnPoints[i].position, Quaternion.identity);
            special.transform.SetParent(transform);
            special.transform.localScale = originalScale;
            spawnedSpecialNuts.Add(special);
        }
    }

    void OnMouseDown() {
        if (TubeManager.Instance.IsAnimating) return;

        if (TubeManager.Instance.HasLiftedNut()) {
            if (CanReceiveNut()) {
                ReceiveNut();
            } else {
                TubeManager.Instance.sourceTube.ReturnNutToOriginal();
            }
        } else {
            TryLiftNut();
        }
    }

    void TryLiftNut() {
        if (currentNuts.Count == 0) return;

        // Nếu đủ 4 nut và tất cả giống tag thì không cho lift
        if (currentNuts.Count == 4) {
            string tagCheck = currentNuts[0].tag;
            bool allSame = currentNuts.TrueForAll(nut => nut.tag == tagCheck);
            if (allSame) return;
        }

        GameObject topNut = currentNuts[currentNuts.Count - 1];
        currentNuts.RemoveAt(currentNuts.Count - 1);

        TubeManager.Instance.SetAnimating(true);
        topNut.transform.DOMove(waitPoint.position, 0.3f).SetEase(Ease.OutQuad).OnComplete(() => {
            TubeManager.Instance.SetLiftedNut(topNut, this);
            TubeManager.Instance.SetAnimating(false);
        });
    }

    public void ReturnNutToOriginal() {
        if (!TubeManager.Instance.HasLiftedNut() || TubeManager.Instance.sourceTube != this) return;

        GameObject nut = TubeManager.Instance.liftedNut;
        int targetIndex = currentNuts.Count;
        if (targetIndex >= spawnPoints.Length) return;

        TubeManager.Instance.SetAnimating(true);
        Vector3 returnPos = spawnPoints[targetIndex].position;

        nut.transform.DOMove(returnPos, 0.3f).SetEase(Ease.OutQuad).OnComplete(() => {
            nut.transform.SetParent(transform);
            nut.transform.localScale = originalScale;
            currentNuts.Add(nut);
            TubeManager.Instance.ClearLiftedNut();
        });
    }

    bool CanReceiveNut() {
        return currentNuts.Count < spawnPoints.Length;
    }

    void ReceiveNut() {
        TubeManager.Instance.SetAnimating(true);

        GameObject nut = TubeManager.Instance.liftedNut;
        TubeController source = TubeManager.Instance.sourceTube;

        nut.transform.DOMove(waitPoint.position, 0.3f).SetEase(Ease.OutQuad).OnComplete(() => {
            int targetIndex = currentNuts.Count;
            Vector3 targetPos = spawnPoints[targetIndex].position;

            nut.transform.DOMove(targetPos, 0.3f).SetEase(Ease.OutQuad).OnComplete(() => {
                nut.transform.SetParent(transform);
                nut.transform.localScale = originalScale;
                currentNuts.Add(nut);

                // Ẩn specialNut phía trên nut vừa bị di chuyển từ tube cũ
                int fromIndex = source.currentNuts.Count;
                if (fromIndex > 0 && fromIndex - 1 < source.spawnedSpecialNuts.Count) {
                    GameObject specialNutAbove = source.spawnedSpecialNuts[fromIndex - 1];
                    if (specialNutAbove != null)
                        specialNutAbove.SetActive(false);
                }

                TubeManager.Instance.RegisterMove(nut, source, this);
                TubeManager.Instance.ClearLiftedNut();
                GameManager.Instance.CheckWinCondition();
            });
        });
    }

    public List<GameObject> GetCurrentNuts() {
        return currentNuts;
    }

    public void RemoveNut(GameObject nut) {
        currentNuts.Remove(nut);
    }

    public void AddNut(GameObject nut) {
        currentNuts.Add(nut);
    }

    public Vector3 GetOriginalScale() {
        return originalScale;
    }
}
