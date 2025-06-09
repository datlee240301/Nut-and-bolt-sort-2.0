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
    [SerializeField] Image bg;

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
            // Nếu là UI và KHÔNG phải tag "bg" thì coi như đang trỏ vào UI
            if (result.gameObject.GetComponent<Graphic>() != null && !result.gameObject.CompareTag("bg"))
            {
                return true;
            }
        }

        return false; // chỉ trúng "bg" hoặc không trúng UI nào
    }

    public void ShakeTube()
    {
        TubeCameraController cameraController = FindObjectOfType<TubeCameraController>();

        // Báo camera tube đang rung
        if (cameraController != null)
        {
            cameraController.isShaking = true;
        }

        // Ví dụ rung theo trục Y 0.2f trong 0.5 giây, rung 10 lần
        transform.DOShakePosition(0.5f, strength: new Vector3(0f, 0.2f, 0f), vibrato: 10).OnComplete(() =>
        {
            // Rung xong, báo camera cho phép cập nhật lại vị trí
            if (cameraController != null)
            {
                cameraController.isShaking = false;
            }
        });
    }
    void OnMouseDown()
{
    if (!isPointerOverUI)
    {
        if (TubeManager.Instance.IsAnimating) return;
        if (isMovingNut) return; // tránh spam click khi đang chuyển nut

        // Bắt đầu rung tube này khi được chạm
        TubeCameraController cameraController = FindObjectOfType<TubeCameraController>();
        if (cameraController != null)
        {
            cameraController.isShaking = true;
        }

        transform.DOShakePosition(0.5f, new Vector3(0f, 0.2f, 0f), 10).OnComplete(() =>
        {
            if (cameraController != null)
            {
                cameraController.isShaking = false;
            }
        });

        if (TubeManager.Instance.HasLiftedNut())
        {
            if (TubeManager.Instance.sourceTube == this)
            {
                // Nếu nut lifted đang từ tube này thì trả nut về
                ReturnNutToOriginal();
            }
            else
            {
                if (CanReceiveNut())
                {
                    GameObject liftedNut = TubeManager.Instance.liftedNut;

                    if (currentNuts.Count == 0)
                    {
                        // Tube rỗng, luôn nhận nut
                        ReceiveNut();
                    }
                    else
                    {
                        GameObject topNut = currentNuts[currentNuts.Count - 1];

                        if (topNut.tag == liftedNut.tag)
                        {
                            ReceiveNut();
                        }
                        else
                        {
                            // Tag khác → trả nut về source tube
                            TubeManager.Instance.sourceTube.ReturnNutToOriginal();

                            // Sau khi nut bị trả về, tube đích thử nâng nut của mình lên
                            // Dùng delay nhỏ để đảm bảo nut cũ hoàn tất trước
                            Invoke(nameof(TryLiftNut), 0.35f);
                        }
                    }
                }
                else
                {
                    // Không thể nhận nut → trả nut về source tube
                    TubeManager.Instance.sourceTube.ReturnNutToOriginal();

                    // Sau khi nut bị trả về, tube đích thử nâng nut của mình lên
                    Invoke(nameof(TryLiftNut), 0.35f);
                }
            }
        }
        else
        {
            // Không có nut đang lift → thử nâng nut trên cùng lên
            TryLiftNut();
        }
    }
}




    public void AutoLiftTopNutIfValid()
    {
        // Nếu tube này không trống và chưa có nut nào đang lift
        if (currentNuts.Count > 0 && TubeManager.Instance.liftedNut == null)
        {
            GameObject topNut = currentNuts[currentNuts.Count - 1];
            Vector3 liftPos = topNut.transform.position + Vector3.up * 1.5f;

            TubeManager.Instance.SetAnimating(true);
            TubeManager.Instance.SetLiftedNut(topNut, this);
            topNut.transform.DOMove(liftPos, 0.3f).SetEase(Ease.OutQuad).OnComplete(() =>
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
            bool allSame = currentNuts.TrueForAll(nut => nut.tag == tagCheck);
            if (allSame) return;
        }

        GameObject topNut = currentNuts[currentNuts.Count - 1];
        currentNuts.RemoveAt(currentNuts.Count - 1);

        isMovingNut = true; // khóa thao tác mới
        TubeManager.Instance.SetAnimating(true);
        topNut.transform.DOMove(waitPoint.position, 0.3f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            TubeManager.Instance.SetLiftedNut(topNut, this);
            TubeManager.Instance.SetAnimating(false);
            isMovingNut = false; // mở khóa thao tác
        });
    }


    public void ReturnNutToOriginal()
    {
        if (!TubeManager.Instance.HasLiftedNut() || TubeManager.Instance.sourceTube != this) return;

        GameObject nut = TubeManager.Instance.liftedNut;
        int targetIndex = currentNuts.Count;
        if (targetIndex >= spawnPoints.Length) return;

        isMovingNut = true; // khóa thao tác mới
        TubeManager.Instance.SetAnimating(true);
        Vector3 returnPos = spawnPoints[targetIndex].position;

        nut.transform.DOMove(returnPos, 0.3f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            nut.transform.SetParent(transform);
            nut.transform.localScale = originalScale;
            currentNuts.Add(nut);
            TubeManager.Instance.ClearLiftedNut();
            isMovingNut = false; // mở khóa thao tác
        });
    }

    private bool isMovingNut = false;

    bool CanReceiveNut()
    {
        return currentNuts.Count < spawnPoints.Length && spawnPoints[currentNuts.Count].gameObject.activeSelf;
    }


    void ReceiveNut()
    {
        TubeManager.Instance.SetAnimating(true);

        GameObject nut = TubeManager.Instance.liftedNut;
        TubeController source = TubeManager.Instance.sourceTube;

        // Kiểm tra xem tube đích có rỗng không (chỉ khi đích rỗng mới cho phép chuyển liên tiếp)
        bool allowChainMove = currentNuts.Count == 0;

        // Kiểm tra có thể move liên tiếp nếu các nut có cùng tag
        bool canMoveAll = false;
        if (currentNuts.Count > 0)
        {
            GameObject topNut = currentNuts[currentNuts.Count - 1];
            if (topNut.tag == nut.tag)
            {
                canMoveAll = true;
            }
        }
        else
        {
            canMoveAll = allowChainMove;
        }

        // Hàm di chuyển từng nut
        void MoveNextNut(GameObject movingNut)
        {
            movingNut.transform.DOMove(source.waitPoint.position, 0.3f).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                movingNut.transform.DOMove(waitPoint.position, 0.3f).SetEase(Ease.OutQuad).OnComplete(() =>
                {
                    int targetIndex = currentNuts.Count;
                    Vector3 targetPos = spawnPoints[targetIndex].position;

                    movingNut.transform.DOMove(targetPos, 0.3f).SetEase(Ease.OutQuad).OnComplete(() =>
                    {
                        movingNut.transform.SetParent(transform);

                        if (originalScale == Vector3.zero)
                            originalScale = movingNut.transform.localScale;

                        movingNut.transform.localScale = originalScale;
                        currentNuts.Add(movingNut);

                        if (collisionEffectPrefab != null)
                            Instantiate(collisionEffectPrefab, targetPos, Quaternion.identity);

                        if (currentNuts.Count == 4)
                        {
                            string tagCheck = currentNuts[0].tag;
                            bool allSame = currentNuts.TrueForAll(n => n.tag == tagCheck);
                            if (allSame && fullColumnEffectPrefab != null)
                                Instantiate(fullColumnEffectPrefab, waitPoint.position, Quaternion.identity);
                        }

                        int fromIndex = source.currentNuts.Count;
                        if (fromIndex > 0 && fromIndex - 1 < source.spawnedSpecialNuts.Count)
                        {
                            GameObject specialNutAbove = source.spawnedSpecialNuts[fromIndex - 1];
                            if (specialNutAbove != null)
                                specialNutAbove.SetActive(false);
                        }

                        TubeManager.Instance.RegisterMove(movingNut, source, this);

                        // Chỉ tiếp tục nếu được phép chain move và còn nut cùng tag
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

                        // Kết thúc
                        TubeManager.Instance.ClearLiftedNut();
                        TubeManager.Instance.SetAnimating(false);
                        GameManager.Instance.CheckWinCondition();
                    });
                });
            });
        }

        MoveNextNut(nut);
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