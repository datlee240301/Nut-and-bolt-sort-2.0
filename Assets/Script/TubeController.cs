using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class TubeController : MonoBehaviour {
    public Transform[] spawnPoints;
    public GameObject[] nutPrefabs;
    public GameObject[] specialNutsPrefabs;
    public Transform waitPoint;

    private List<GameObject> currentNuts = new List<GameObject>();
    private Vector3 originalScale;

    void Start() {
        SpawnNuts();
    }
    private GameObject[] spawnedSpecialNuts; 

    void SpawnNuts() {
        spawnedSpecialNuts = new GameObject[spawnPoints.Length]; 

        for (int i = 0; i < Mathf.Min(spawnPoints.Length, nutPrefabs.Length); i++) {
            GameObject nut = Instantiate(nutPrefabs[i], spawnPoints[i].position, Quaternion.identity);
            nut.transform.SetParent(transform);
            originalScale = nut.transform.localScale;
            currentNuts.Add(nut);
        }

        for (int i = 0; i < Mathf.Min(spawnPoints.Length, specialNutsPrefabs.Length); i++) {
            if (specialNutsPrefabs[i] == null) continue;
            GameObject specialNut = Instantiate(specialNutsPrefabs[i], spawnPoints[i].position, Quaternion.identity);
            specialNut.transform.SetParent(transform);
            specialNut.transform.localScale = originalScale;
            spawnedSpecialNuts[i] = specialNut;
        }
    }

    void OnMouseDown() {
        if (TubeManager.Instance.IsAnimating) return; // 🚫 Đang animating, không cho nhấn

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

        if (currentNuts.Count == 4) {
            string tagCheck = currentNuts[0].tag;
            bool allSame = currentNuts.TrueForAll(nut => nut.tag == tagCheck);
            if (allSame) return;
        }
        GameObject topNut = currentNuts[currentNuts.Count - 1];
        currentNuts.RemoveAt(currentNuts.Count - 1);
        topNut.transform.DOMove(waitPoint.position, 0.3f).SetEase(Ease.OutQuad);
        TubeManager.Instance.SetLiftedNut(topNut, this);
    }


    public void ReturnNutToOriginal() {
        if (!TubeManager.Instance.HasLiftedNut() || TubeManager.Instance.sourceTube != this) return;

        GameObject nut = TubeManager.Instance.liftedNut;
        int targetIndex = currentNuts.Count;
        if (targetIndex >= spawnPoints.Length) return;

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
        TubeManager.Instance.SetAnimating(true); // ✅ Bắt đầu animating

        GameObject nut = TubeManager.Instance.liftedNut;
        TubeController source = TubeManager.Instance.sourceTube;

        nut.transform.DOMove(waitPoint.position, 0.3f).SetEase(Ease.OutQuad).OnComplete(() => {
            int targetIndex = currentNuts.Count;
            Vector3 targetPos = spawnPoints[targetIndex].position;

            nut.transform.DOMove(targetPos, 0.3f).SetEase(Ease.OutQuad).OnComplete(() => {
                nut.transform.SetParent(transform);
                nut.transform.localScale = originalScale;
                currentNuts.Add(nut);

                int fromIndex = source.currentNuts.Count;
                if (fromIndex > 0) {
                    GameObject specialNutAbove = source.spawnedSpecialNuts[fromIndex - 1];
                    if (specialNutAbove != null) {
                        specialNutAbove.SetActive(false);
                    }
                }

                TubeManager.Instance.ClearLiftedNut(); // ✅ Cho phép nhấn lại tube
                GameManager.Instance.CheckWinCondition();
            });
        });
    }


    public List<GameObject> GetCurrentNuts() {
        return currentNuts;
    }

}
