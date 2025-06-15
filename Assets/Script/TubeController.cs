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
    public Transform visual; // Gán visual tại đây trong Inspector

    private List<GameObject> currentNuts = new List<GameObject>();
    public List<GameObject> spawnedSpecialNuts = new List<GameObject>();

    private Vector3 originalScale;
    private int activeSpawnCount = 0;
    private bool isPointerOverUI;
    private bool isMovingNut = false;

    [SerializeField] private GameObject collisionEffectPrefab;
    [SerializeField] private GameObject fullColumnEffectPrefab;
    [SerializeField] private Image bg;

    [SerializeField] private float nutMoveDuration = 0.15f; // 💡 tốc độ di chuyển nut

    void Start()
    {
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
            nut.transform.SetParent(visual);
            originalScale = nut.transform.localScale;
            currentNuts.Add(nut);
        }

        for (int i = 0; i < Mathf.Min(spawnPoints.Length, specialNutsPrefabs.Length); i++)
        {
            GameObject special = Instantiate(specialNutsPrefabs[i], spawnPoints[i].position, Quaternion.identity);
            special.transform.SetParent(visual);
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
            if (result.gameObject.GetComponent<Graphic>() != null && !result.gameObject.CompareTag("bg"))
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
            if (visual != null)
            {
                visual.DOComplete();
                visual.DOShakePosition(0.15f, new Vector3(0f, 0.2f, 0f), 10, 90, false, false);
            }

            if (TubeManager.Instance.IsAnimating || isMovingNut) return;

            if (TubeManager.Instance.HasLiftedNut())
            {
                if (TubeManager.Instance.sourceTube == this)
                {
                    ReturnNutToOriginal();
                }
                else
                {
                    if (CanReceiveNut())
                    {
                        GameObject liftedNut = TubeManager.Instance.liftedNut;

                        if (currentNuts.Count == 0 || currentNuts[currentNuts.Count - 1].tag == liftedNut.tag)
                        {
                            ReceiveNut();
                        }
                        else
                        {
                            TubeManager.Instance.sourceTube.ReturnNutToOriginal();
                            Invoke(nameof(TryLiftNut), 0.35f);
                        }
                    }
                    else
                    {
                        TubeManager.Instance.sourceTube.ReturnNutToOriginal();
                        Invoke(nameof(TryLiftNut), 0.35f);
                    }
                }
            }
            else
            {
                TryLiftNut();
            }
        }
    }

    public void AutoLiftTopNutIfValid()
    {
        if (currentNuts.Count > 0 && TubeManager.Instance.liftedNut == null)
        {
            GameObject topNut = currentNuts[currentNuts.Count - 1];
            Vector3 liftPos = topNut.transform.position + Vector3.up * 1.5f;

            TubeManager.Instance.SetAnimating(true);
            TubeManager.Instance.SetLiftedNut(topNut, this);
            topNut.transform.DOMove(liftPos, nutMoveDuration).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                TubeManager.Instance.SetAnimating(false);
            });
        }
    }

    void TryLiftNut()
    {
        if (currentNuts.Count == 0) return;

        if (currentNuts.Count == 4)
        {
            string tagCheck = currentNuts[0].tag;
            if (currentNuts.TrueForAll(n => n.tag == tagCheck)) return;
        }

        GameObject topNut = currentNuts[currentNuts.Count - 1];
        currentNuts.RemoveAt(currentNuts.Count - 1);

        isMovingNut = true;
        TubeManager.Instance.SetAnimating(true);
        topNut.transform.DOMove(waitPoint.position, nutMoveDuration).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            TubeManager.Instance.SetLiftedNut(topNut, this);
            TubeManager.Instance.SetAnimating(false);
            isMovingNut = false;
        });
    }

    public void ReturnNutToOriginal()
    {
        if (!TubeManager.Instance.HasLiftedNut() || TubeManager.Instance.sourceTube != this) return;

        GameObject nut = TubeManager.Instance.liftedNut;
        int targetIndex = currentNuts.Count;
        if (targetIndex >= spawnPoints.Length) return;

        isMovingNut = true;
        TubeManager.Instance.SetAnimating(true);
        nut.transform.DOMove(spawnPoints[targetIndex].position, nutMoveDuration).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            nut.transform.SetParent(visual);
            nut.transform.localScale = originalScale;
            currentNuts.Add(nut);
            TubeManager.Instance.ClearLiftedNut();
            isMovingNut = false;
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
        bool allowChainMove = currentNuts.Count == 0;
        bool canMoveAll = currentNuts.Count == 0 || currentNuts[currentNuts.Count - 1].tag == nut.tag;

        void MoveNextNut(GameObject movingNut)
        {
            movingNut.transform.DOMove(source.waitPoint.position, nutMoveDuration).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                movingNut.transform.DOMove(waitPoint.position, nutMoveDuration).SetEase(Ease.OutQuad).OnComplete(() =>
                {
                    int targetIndex = currentNuts.Count;
                    movingNut.transform.DOMove(spawnPoints[targetIndex].position, nutMoveDuration).SetEase(Ease.OutQuad).OnComplete(() =>
                    {
                        movingNut.transform.SetParent(visual);
                        if (originalScale == Vector3.zero) originalScale = movingNut.transform.localScale;
                        movingNut.transform.localScale = originalScale;
                        currentNuts.Add(movingNut);

                        if (collisionEffectPrefab) Instantiate(collisionEffectPrefab, spawnPoints[targetIndex].position, Quaternion.identity);

                        if (currentNuts.Count == 4)
                        {
                            string tagCheck = currentNuts[0].tag;
                            if (currentNuts.TrueForAll(n => n.tag == tagCheck) && fullColumnEffectPrefab)
                                Instantiate(fullColumnEffectPrefab, waitPoint.position, Quaternion.identity);
                        }

                        int fromIndex = source.currentNuts.Count;
                        if (fromIndex > 0 && fromIndex - 1 < source.spawnedSpecialNuts.Count)
                        {
                            GameObject specialNut = source.spawnedSpecialNuts[fromIndex - 1];
                            if (specialNut != null) specialNut.SetActive(false);
                        }

                        TubeManager.Instance.RegisterMove(movingNut, source, this);

                        if (canMoveAll && currentNuts.Count < spawnPoints.Length && source.currentNuts.Count > 0)
                        {
                            GameObject nextNut = source.currentNuts[source.currentNuts.Count - 1];
                            if (nextNut.tag == nut.tag)
                            {
                                source.currentNuts.RemoveAt(source.currentNuts.Count - 1);
                                MoveNextNut(nextNut);
                                return;
                            }
                        }

                        TubeManager.Instance.ClearLiftedNut();
                        TubeManager.Instance.SetAnimating(false);
                        GameManager.Instance.CheckWinCondition();
                    });
                });
            });
        }

        MoveNextNut(nut);
    }

    public List<GameObject> GetCurrentNuts() => currentNuts;
    public void RemoveNut(GameObject nut) => currentNuts.Remove(nut);
    public void AddNut(GameObject nut)
    {
        if (originalScale == Vector3.zero)
            originalScale = nut.transform.localScale;

        currentNuts.Add(nut);
    }

    public Vector3 GetOriginalScale() => originalScale;

    public void RevealNextSpawnPoint()
    {
        if (activeSpawnCount >= spawnPoints.Length) return;

        Transform pointToEnable = spawnPoints[activeSpawnCount];
        pointToEnable.gameObject.SetActive(true);
        activeSpawnCount++;
    }
}
