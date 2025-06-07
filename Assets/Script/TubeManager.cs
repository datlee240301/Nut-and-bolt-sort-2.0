using System;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.Serialization;

public class TubeManager : MonoBehaviour
{
    public static TubeManager Instance;

    [Header("Prefab")] public GameObject tubePrefab;

    private Transform tubeGridLayoutParent;

    [Header("Tube Tracking")] public TubeController tubeSpawned;
    private int pressButtonCount = 0;

    [Header("Nut Move State")] public GameObject liftedNut;
    public TubeController sourceTube;
    public bool IsAnimating { get; private set; }

    // ====== Undo tracking ======
    private GameObject lastMovedNut;
    private TubeController lastSourceTube;
    private TubeController lastTargetTube;
    UiManager uiManager;

    [FormerlySerializedAs("undoFee")] [Header("BoosterZone")] [SerializeField]
    GameObject undoFeePanel;

    [SerializeField] int undoFeeCost;

    [FormerlySerializedAs("addTubeFee")] [SerializeField]
    GameObject addTubeFeePanel;

    [SerializeField] int addTubeFeeCost;
    [SerializeField] TextMeshProUGUI coinNumberText;
    [SerializeField] UiPanelDotween noticePanel;
    [SerializeField] UiPanelDotween limitSpawnTubePanel;


    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // GameObject found = GameObject.Find("TubeGridLayout");
        // if (found != null)
        // {
        //     tubeGridLayoutParent = found.transform;
        // }
        // else
        // {
        //     Debug.LogError("Không tìm thấy GameObject tên 'TubeGridLayout' trong scene!");
        // }

        uiManager = FindObjectOfType<UiManager>();
    }

    private void Start()
    {
        if (undoFeePanel != null)
        {
            if (PlayerPrefs.GetInt(StringManager.undoNumber) > 0)
            {
                undoFeePanel.SetActive(false);
            }
            else
            {
                undoFeePanel.SetActive(true);
            }
        }

        if (addTubeFeePanel != null)
        {
            if (PlayerPrefs.GetInt(StringManager.addTubeNumber) > 0)
            {
                addTubeFeePanel.SetActive(false);
            }
            else
            {
                addTubeFeePanel.SetActive(true);
            }
        }
    }

    public bool HasLiftedNut() => liftedNut != null;

    public void SetLiftedNut(GameObject nut, TubeController tube)
    {
        liftedNut = nut;
        sourceTube = tube;
    }

    public void ClearLiftedNut()
    {
        liftedNut = null;
        sourceTube = null;
        IsAnimating = false;
    }

    public void SetAnimating(bool value)
    {
        IsAnimating = value;
    }

    // ====== Undo Support ======
    public void RegisterMove(GameObject nut, TubeController fromTube, TubeController toTube)
    {
        lastMovedNut = nut;
        lastSourceTube = fromTube;
        lastTargetTube = toTube;
    }

    public void UndoLastMove()
    {
        if (PlayerPrefs.GetInt(StringManager.undoNumber) > 0 && !undoFeePanel.activeSelf)
        {
            if (IsAnimating || lastMovedNut == null || lastTargetTube == null || lastSourceTube == null) return;
            if (!lastTargetTube.GetCurrentNuts().Contains(lastMovedNut)) return;

            SetAnimating(true);
            lastTargetTube.RemoveNut(lastMovedNut);

            int returnIndex = lastSourceTube.GetCurrentNuts().Count;
            Vector3 returnPos = lastSourceTube.spawnPoints[returnIndex].position;

            lastMovedNut.transform.DOMove(returnPos, 0.3f).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                lastMovedNut.transform.SetParent(lastSourceTube.transform);
                lastMovedNut.transform.localScale = lastSourceTube.GetOriginalScale();
                lastSourceTube.AddNut(lastMovedNut);

                ClearLiftedNut();
                // Chỉ trừ undo nếu hạt đã chuyển sang tube khác
                if (lastSourceTube != lastTargetTube && uiManager != null)
                {
                    uiManager.MinusUndoNumber(1);
                    if (PlayerPrefs.GetInt(StringManager.undoNumber) <= 0)
                        undoFeePanel.SetActive(true);
                }

                lastMovedNut = null;
                lastSourceTube = null;
                lastTargetTube = null;
            });
        }
        else
        {
            if (PlayerPrefs.GetInt(StringManager.ticketNumber) >= undoFeeCost)
            {
                PlayerPrefs.SetInt(StringManager.ticketNumber,
                    PlayerPrefs.GetInt(StringManager.ticketNumber) - undoFeeCost);
                if (coinNumberText != null)
                {
                    coinNumberText.text = PlayerPrefs.GetInt(StringManager.ticketNumber).ToString();
                }

                undoFeePanel.SetActive(false);
                if (uiManager != null)
                {
                    uiManager.PlusUndoNumber(5);
                }
            }
            else
            {
                noticePanel.PanelFadeIn();
            }
        }
    }

    // ====== Tube Spawn + Reveal Support ======
    public void SpawnNewTube()
    {
        GameObject found = GameObject.Find("TubeGridLayout");
        if (found != null)
        {
            tubeGridLayoutParent = found.transform;
        }
        else
        {
            Debug.LogError("Không tìm thấy GameObject tên 'TubeGridLayout' trong scene!");
        }

        if (PlayerPrefs.GetInt(StringManager.addTubeNumber) > 0 && !addTubeFeePanel.activeSelf)
        {
            pressButtonCount++;
            if (pressButtonCount <= 4)
            {
                if (pressButtonCount <= 4)
                    uiManager.MinusAddTubeNumber(1);
                if (PlayerPrefs.GetInt(StringManager.addTubeNumber) <= 0)
                    addTubeFeePanel.SetActive(true);

                if (pressButtonCount == 1)
                {
                    if (tubeSpawned != null)
                    {
                        Debug.LogWarning("Tube đã được spawn.");
                        return;
                    }

                    if (tubeGridLayoutParent == null)
                    {
                        Debug.LogError("Không tìm thấy TubeGridLayout.");
                        return;
                    }

                    GameObject newTube =
                        Instantiate(tubePrefab, Vector3.zero, Quaternion.identity, tubeGridLayoutParent);
                    tubeSpawned = newTube.GetComponent<TubeController>();
                    return;
                }

                var spawnManager = FindObjectOfType<SpawnPosOfNewTubeManager>();
                int indexToActivate = pressButtonCount - 2;
                if (spawnManager != null && indexToActivate < spawnManager.spawnPos.Length)
                {
                    spawnManager.spawnPos[indexToActivate].SetActive(true);
                }
            }
            else if (pressButtonCount >= 4)
            {
                limitSpawnTubePanel.PanelFadeIn();
            }
        }
        else
        {
            if (PlayerPrefs.GetInt(StringManager.ticketNumber) >= addTubeFeeCost)
            {
                PlayerPrefs.SetInt(StringManager.ticketNumber,
                    PlayerPrefs.GetInt(StringManager.ticketNumber) - addTubeFeeCost);
                if (coinNumberText != null)
                {
                    coinNumberText.text = PlayerPrefs.GetInt(StringManager.ticketNumber).ToString();
                }

                addTubeFeePanel.SetActive(false);
                if (uiManager != null)
                {
                    uiManager.PlusAddTubeNumber(2);
                }
            }
            else
            {
                noticePanel.PanelFadeIn();
            }
        }
    }

    public void RevealNextSpawnPointForSpawnedTube()
    {
        if (tubeSpawned != null)
        {
            tubeSpawned.RevealNextSpawnPoint();
        }
        else
        {
            Debug.LogWarning("Chưa có tube nào được spawn.");
        }
    }
}