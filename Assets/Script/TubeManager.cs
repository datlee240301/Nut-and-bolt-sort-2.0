using System;
using System.Collections.Generic;
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
    private Stack<MoveInfo> moveStack = new Stack<MoveInfo>();

    // Replace lastMovedNut, lastSourceTube, lastTargetTube with MoveInfo
    

    public class MoveInfo
    {
        public GameObject nut;
        public TubeController fromTube;
        public TubeController toTube;
    }

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
        moveStack.Push(new MoveInfo
        {
            nut = nut,
            fromTube = fromTube,
            toTube = toTube
        });
    }

    public void UndoLastMove()
    {
        if (PlayerPrefs.GetInt(StringManager.undoNumber) > 0 && !undoFeePanel.activeSelf)
        {
            if (IsAnimating || moveStack.Count == 0) return;

            MoveInfo move = moveStack.Pop();
            if (!move.toTube.GetCurrentNuts().Contains(move.nut)) return;

            SetAnimating(true);
            move.toTube.RemoveNut(move.nut);

            int returnIndex = move.fromTube.GetCurrentNuts().Count;
            Vector3 returnPos = move.fromTube.spawnPoints[returnIndex].position;

            move.nut.transform.DOMove(returnPos, 0.3f).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                move.nut.transform.SetParent(move.fromTube.transform);
                move.nut.transform.localScale = move.fromTube.GetOriginalScale();
                move.fromTube.AddNut(move.nut);

                ClearLiftedNut();
                // Trừ undo nếu hoàn tác thành công
                if (move.fromTube != move.toTube && uiManager != null)
                {
                    uiManager.MinusUndoNumber(1);
                    if (PlayerPrefs.GetInt(StringManager.undoNumber) <= 0)
                        undoFeePanel.SetActive(true);
                }
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