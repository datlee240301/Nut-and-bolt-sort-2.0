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
    public Transform visual;

    private List<GameObject> currentNuts = new List<GameObject>();
    public List<GameObject> spawnedSpecialNuts = new List<GameObject>();

    private Vector3 originalScale;
    private int activeSpawnCount = 0;
    private bool isPointerOverUI;
    private bool isMovingNut = false;

    [SerializeField] private GameObject collisionEffectPrefab;
    [SerializeField] private GameObject fullColumnEffectPrefab;
    [SerializeField] private Image bg;

    [SerializeField] private float nutMoveDuration = 0.15f;

    // Khoảng sai lệch vị trí cho phép khi tìm specialNut trùng vị trí.
    [SerializeField] private float specialNutPositionTolerance = 0.05f;

    private void Start()
    {
        SpawnNuts();
    }

    private void Update()
    {
        isPointerOverUI = IsPointerOverUIElement();
    }

    private void SpawnNuts()
    {
        for (int i = 0; i < Mathf.Min(spawnPoints.Length, nutPrefabs.Length); i++)
        {
            GameObject nut = Instantiate(
                nutPrefabs[i],
                spawnPoints[i].position,
                Quaternion.identity
            );

            nut.transform.SetParent(visual);
            originalScale = nut.transform.localScale;
            currentNuts.Add(nut);
        }

        for (int i = 0; i < Mathf.Min(spawnPoints.Length, specialNutsPrefabs.Length); i++)
        {
            GameObject special = Instantiate(
                specialNutsPrefabs[i],
                spawnPoints[i].position,
                Quaternion.identity
            );

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
            if (result.gameObject.GetComponent<Graphic>() != null &&
                !result.gameObject.CompareTag("bg"))
            {
                return true;
            }
        }

        return false;
    }

    private void OnMouseDown()
    {
        if (isPointerOverUI)
        {
            return;
        }

        if (visual != null)
        {
            visual.DOComplete();
            visual.DOShakePosition(
                0.15f,
                new Vector3(0f, 0.2f, 0f),
                10,
                90,
                false,
                false
            );
        }

        if (TubeManager.Instance.IsAnimating || isMovingNut)
        {
            return;
        }

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

                    if (currentNuts.Count == 0 ||
                        currentNuts[currentNuts.Count - 1].tag == liftedNut.tag)
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

    public void AutoLiftTopNutIfValid()
    {
        if (currentNuts.Count > 0 && TubeManager.Instance.liftedNut == null)
        {
            GameObject topNut = currentNuts[currentNuts.Count - 1];
            Vector3 liftPos = topNut.transform.position + Vector3.up * 1.5f;

            TubeManager.Instance.SetAnimating(true);
            TubeManager.Instance.SetLiftedNut(topNut, this);

            topNut.transform
                .DOMove(liftPos, nutMoveDuration)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    TubeManager.Instance.SetAnimating(false);
                });
        }
    }

    private void TryLiftNut()
    {
        if (currentNuts.Count == 0)
        {
            return;
        }

        if (currentNuts.Count == 4)
        {
            string tagCheck = currentNuts[0].tag;

            if (currentNuts.TrueForAll(n => n.tag == tagCheck))
            {
                return;
            }
        }

        GameObject topNut = currentNuts[currentNuts.Count - 1];
        currentNuts.RemoveAt(currentNuts.Count - 1);

        isMovingNut = true;
        TubeManager.Instance.SetAnimating(true);

        topNut.transform
            .DOMove(waitPoint.position, nutMoveDuration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                TubeManager.Instance.SetLiftedNut(topNut, this);
                TubeManager.Instance.SetAnimating(false);
                isMovingNut = false;
            });
    }

    public void ReturnNutToOriginal()
    {
        if (!TubeManager.Instance.HasLiftedNut() ||
            TubeManager.Instance.sourceTube != this)
        {
            return;
        }

        GameObject nut = TubeManager.Instance.liftedNut;
        int targetIndex = currentNuts.Count;

        if (targetIndex >= spawnPoints.Length)
        {
            return;
        }

        isMovingNut = true;
        TubeManager.Instance.SetAnimating(true);

        nut.transform
            .DOMove(spawnPoints[targetIndex].position, nutMoveDuration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                nut.transform.SetParent(visual);
                nut.transform.localScale = originalScale;
                currentNuts.Add(nut);

                TubeManager.Instance.ClearLiftedNut();
                isMovingNut = false;
            });
    }

    private bool CanReceiveNut()
    {
        return currentNuts.Count < spawnPoints.Length &&
               spawnPoints[currentNuts.Count].gameObject.activeSelf;
    }

    private void ReceiveNut()
    {
        TubeManager.Instance.SetAnimating(true);

        GameObject nut = TubeManager.Instance.liftedNut;
        TubeController source = TubeManager.Instance.sourceTube;
        string nutTag = nut.tag;

        List<GameObject> nutsToMove = new List<GameObject> { nut };

        /*
         * Nut đầu tiên đã được remove khỏi source.currentNuts trong TryLiftNut().
         * Vì vậy source.currentNuts.Count chính là index ban đầu của nut đó.
         */
        int firstMovedNutOriginalIndex = source.currentNuts.Count;

        // Lấy thêm các nut cùng màu liên tiếp.
        for (int i = source.currentNuts.Count - 1; i >= 0; i--)
        {
            if (source.currentNuts[i].tag == nutTag)
            {
                nutsToMove.Add(source.currentNuts[i]);
            }
            else
            {
                break;
            }
        }

        int availableSpace = spawnPoints.Length - currentNuts.Count;
        int moveCount = Mathf.Min(availableSpace, nutsToMove.Count);

        if (moveCount <= 0)
        {
            TubeManager.Instance.SetAnimating(false);
            source.ReturnNutToOriginal();
            return;
        }

        float sequenceDelayStep = 0.15f;

        for (int i = 0; i < moveCount; i++)
        {
            GameObject movingNut = nutsToMove[i];
            int targetIndex = currentNuts.Count + i;
            int moveIndex = i;

            /*
             * Nut đang di chuyển vốn nằm ở originalIndex.
             * Nut ngay sát bên dưới nó nằm ở originalIndex - 1.
             */
            int originalIndex = firstMovedNutOriginalIndex - i;
            int belowNutIndex = originalIndex - 1;

            bool hasNutDirectlyBelow =
                belowNutIndex >= 0 &&
                belowNutIndex < source.spawnPoints.Length &&
                source.spawnPoints[belowNutIndex] != null;

            /*
             * Lưu lại vị trí trước khi animation bắt đầu.
             * Nhờ vậy, kể cả nut bên dưới cũng được chuyển đi sau đó,
             * specialNut vẫn được tìm theo đúng vị trí cũ của nó.
             */
            Vector3 belowNutOriginalPosition = hasNutDirectlyBelow
                ? source.spawnPoints[belowNutIndex].position
                : Vector3.zero;

            // Nut đầu tiên đã bị remove trong TryLiftNut.
            if (i != 0)
            {
                source.currentNuts.Remove(movingNut);
            }

            float delay = i * sequenceDelayStep;

            DOVirtual.DelayedCall(delay, () =>
            {
                Sequence moveSeq = DOTween.Sequence();

                moveSeq.Append(
                    movingNut.transform
                        .DOMove(source.waitPoint.position, nutMoveDuration)
                        .SetEase(Ease.OutQuad)
                );

                moveSeq.Append(
                    movingNut.transform
                        .DOMove(waitPoint.position, nutMoveDuration)
                        .SetEase(Ease.OutQuad)
                );

                moveSeq.Append(
                    movingNut.transform
                        .DOMove(spawnPoints[targetIndex].position, nutMoveDuration)
                        .SetEase(Ease.OutQuad)
                );

                moveSeq.OnComplete(() =>
                {
                    movingNut.transform.SetParent(visual);

                    if (originalScale == Vector3.zero)
                    {
                        originalScale = movingNut.transform.localScale;
                    }

                    movingNut.transform.localScale = originalScale;
                    currentNuts.Add(movingNut);

                    if (collisionEffectPrefab != null)
                    {
                        SoundManager.instance.PlayMoveDiamondSound();

                        Instantiate(
                            collisionEffectPrefab,
                            spawnPoints[targetIndex].position,
                            Quaternion.identity
                        );
                    }

                    if (currentNuts.Count == 4)
                    {
                        string tagCheck = currentNuts[0].tag;

                        if (currentNuts.TrueForAll(n => n.tag == tagCheck) &&
                            fullColumnEffectPrefab != null)
                        {
                            SoundManager.instance.PlayColumnDoneSound();

                            Instantiate(
                                fullColumnEffectPrefab,
                                waitPoint.position,
                                Quaternion.identity
                            );
                        }
                    }

                    /*
                     * LOGIC SPECIAL NUT:
                     * Khi movingNut đã di chuyển thành công,
                     * tìm specialNut đang trùng với vị trí của nut
                     * ngay sát bên dưới movingNut ở source tube và ẩn nó.
                     */
                    if (hasNutDirectlyBelow)
                    {
                        source.HideSpecialNutAtPosition(
                            belowNutOriginalPosition
                        );
                    }

                    TubeManager.Instance.RegisterMove(
                        movingNut,
                        source,
                        this
                    );

                    if (moveIndex == moveCount - 1)
                    {
                        TubeManager.Instance.ClearLiftedNut();
                        TubeManager.Instance.SetAnimating(false);
                        GameManager.Instance.CheckWinCondition();
                    }
                });
            });
        }
    }

    private void HideSpecialNutAtPosition(Vector3 targetPosition)
    {
        float toleranceSqr =
            specialNutPositionTolerance * specialNutPositionTolerance;

        for (int i = 0; i < spawnedSpecialNuts.Count; i++)
        {
            GameObject specialNut = spawnedSpecialNuts[i];

            if (specialNut == null || !specialNut.activeSelf)
            {
                continue;
            }

            float distanceSqr =
                (specialNut.transform.position - targetPosition).sqrMagnitude;

            if (distanceSqr <= toleranceSqr)
            {
                specialNut.SetActive(false);
                return;
            }
        }
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
        if (activeSpawnCount >= spawnPoints.Length)
        {
            return;
        }

        Transform pointToEnable = spawnPoints[activeSpawnCount];
        pointToEnable.gameObject.SetActive(true);
        activeSpawnCount++;
    }
}
