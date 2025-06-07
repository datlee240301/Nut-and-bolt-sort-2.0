using System;
using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TubeController : MonoBehaviour
{
    public Transform[] spawnPoints;
    public GameObject[] nutPrefabs;
    public GameObject[] specialNutsPrefabs;
    public Transform waitPoint;

    private List<GameObject> currentNuts = new List<GameObject>();
    public List<GameObject> spawnedSpecialNuts = new List<GameObject>();

    private Vector3 originalScale;

    private int activeSpawnCount = 0;
    private bool isPointerOverUI;
    [SerializeField] GameObject collisionEffectPrefab;
    [SerializeField] GameObject fullColumnEffectPrefab;

    void Start()
    {
        // Disable tất cả spawnPoints ban đầu
        //foreach (Transform spawnPoint in spawnPoints) {
        //    spawnPoint.gameObject.SetActive(false);
        //}
        SpawnNuts();
    }

    private void Update()
    {
        isPointerOverUI = IsPointerOverUIElement();
    }

    void SpawnNuts()
    {
        for (int i = 0; i < Mathf.Min(spawnPoints.Length, nutPrefabs.Length); i++)
        {
            GameObject nut = Instantiate(nutPrefabs[i], spawnPoints[i].position, Quaternion.identity);
            nut.transform.SetParent(transform);
            originalScale = nut.transform.localScale;
            currentNuts.Add(nut);
        }

        // Spawn specialNuts (sẽ nằm lên trên nut)
        for (int i = 0; i < Mathf.Min(spawnPoints.Length, specialNutsPrefabs.Length); i++)
        {
            GameObject special = Instantiate(specialNutsPrefabs[i], spawnPoints[i].position, Quaternion.identity);
            special.transform.SetParent(transform);
            special.transform.localScale = originalScale;
            spawnedSpecialNuts.Add(special);
        }
    }

    private bool IsPointerOverUIElement()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        foreach (RaycastResult result in results)
        {
            if (result.gameObject.GetComponent<Graphic>() != null)
            {
                return true;
            }
        }

        return false;
    }

    void OnMouseDown()
    {
        if (!isPointerOverUI)
        {
            if (TubeManager.Instance.IsAnimating) return;

            if (TubeManager.Instance.HasLiftedNut())
            {
                if (CanReceiveNut())
                {
                    ReceiveNut();
                }
                else
                {
                    TubeManager.Instance.sourceTube.ReturnNutToOriginal();
                }
            }
            else
            {
                TryLiftNut();
            }
        }
    }

    void TryLiftNut()
    {
        if (currentNuts.Count == 0) return;

        if (currentNuts.Count == 4)
        {
            string tagCheck = currentNuts[0].tag;
            bool allSame = currentNuts.TrueForAll(nut => nut.tag == tagCheck);
            if (allSame) return;
        }

        GameObject topNut = currentNuts[currentNuts.Count - 1];
        currentNuts.RemoveAt(currentNuts.Count - 1);

        TubeManager.Instance.SetAnimating(true);
        topNut.transform.DOMove(waitPoint.position, 0.3f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            TubeManager.Instance.SetLiftedNut(topNut, this);
            TubeManager.Instance.SetAnimating(false);
        });
    }

    public void ReturnNutToOriginal()
    {
        if (!TubeManager.Instance.HasLiftedNut() || TubeManager.Instance.sourceTube != this) return;

        GameObject nut = TubeManager.Instance.liftedNut;
        int targetIndex = currentNuts.Count;
        if (targetIndex >= spawnPoints.Length) return;

        TubeManager.Instance.SetAnimating(true);
        Vector3 returnPos = spawnPoints[targetIndex].position;

        nut.transform.DOMove(returnPos, 0.3f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            nut.transform.SetParent(transform);
            nut.transform.localScale = originalScale;
            currentNuts.Add(nut);
            TubeManager.Instance.ClearLiftedNut();
        });
    }

    bool CanReceiveNut()
    {
        return currentNuts.Count < spawnPoints.Length && spawnPoints[currentNuts.Count].gameObject.activeSelf;
    }


    void ReceiveNut()
    {
        TubeManager.Instance.SetAnimating(true);

        GameObject nut = TubeManager.Instance.liftedNut;
        TubeController source = TubeManager.Instance.sourceTube;

        nut.transform.DOMove(waitPoint.position, 0.3f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            int targetIndex = currentNuts.Count;
            Vector3 targetPos = spawnPoints[targetIndex].position;

            nut.transform.DOMove(targetPos, 0.3f).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                nut.transform.SetParent(transform);

                if (originalScale == Vector3.zero)
                    originalScale = nut.transform.localScale;

                nut.transform.localScale = originalScale;
                currentNuts.Add(nut);

                // Spawn collision effect at the target position
                if (collisionEffectPrefab != null)
                {
                    Instantiate(collisionEffectPrefab, targetPos, Quaternion.identity);
                }

                // Check for 4 nuts with the same tag and spawn fullColumnEffect
                if (currentNuts.Count == 4)
                {
                    string tagCheck = currentNuts[0].tag;
                    bool allSame = currentNuts.TrueForAll(n => n.tag == tagCheck);
                    if (allSame && fullColumnEffectPrefab != null)
                    {
                        Instantiate(fullColumnEffectPrefab, waitPoint.position, Quaternion.identity);
                    }
                }

                int fromIndex = source.currentNuts.Count;
                if (fromIndex > 0 && fromIndex - 1 < source.spawnedSpecialNuts.Count)
                {
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


    public List<GameObject> GetCurrentNuts()
    {
        return currentNuts;
    }

    public void RemoveNut(GameObject nut)
    {
        currentNuts.Remove(nut);
    }

    public void AddNut(GameObject nut)
    {
        if (originalScale == Vector3.zero)
        {
            originalScale = nut.transform.localScale;
        }

        currentNuts.Add(nut);
    }


    public Vector3 GetOriginalScale()
    {
        return originalScale;
    }

    public void RevealNextSpawnPoint()
    {
        if (activeSpawnCount >= spawnPoints.Length) return;

        Transform pointToEnable = spawnPoints[activeSpawnCount];
        pointToEnable.gameObject.SetActive(true);
        activeSpawnCount++;
    }
}