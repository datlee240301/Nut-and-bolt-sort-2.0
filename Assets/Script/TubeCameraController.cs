using UnityEngine;

[RequireComponent(typeof(Camera))]
public class TubeCameraController : MonoBehaviour
{
    public string tubeTag = "Tube";
    public float screenPadding = 0.1f;
    public float minDistance = 10f;
    public float maxDistance = 60f;

    [Header("Manual Y Offset")] public float yOffset = 0f; // <-- Thêm dòng này để điều chỉnh độ cao Y

    private Camera cam;

    void Awake() => cam = GetComponent<Camera>();

    void Update() => FrameAllTubes();

    /* ---------- Core ---------- */
    void FrameAllTubes()
    {
        GameObject[] tubes = GameObject.FindGameObjectsWithTag(tubeTag);
        if (tubes.Length == 0) return;

        Bounds bounds = new Bounds(tubes[0].transform.position, Vector3.zero);
        for (int i = 1; i < tubes.Length; i++)
            bounds.Encapsulate(tubes[i].transform.position);

        Vector3 centre = bounds.center;
        centre.y += yOffset; // <-- Áp dụng yOffset tại đây

        if (cam.orthographic)
        {
            float sizeX = bounds.extents.x / cam.aspect;
            float sizeY = bounds.extents.y;
            float targetSize = Mathf.Max(sizeX, sizeY);
            targetSize *= (1f + screenPadding);

            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize,
                targetSize,
                10f * Time.unscaledDeltaTime);
            transform.position = new Vector3(centre.x,
                centre.y,
                transform.position.z);
        }
        else
        {
            float frustumHeight = Mathf.Max(bounds.size.y,
                bounds.size.x / cam.aspect);
            frustumHeight *= (1f + screenPadding);
            float distance = frustumHeight * 0.5f / Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
            distance = Mathf.Clamp(distance, minDistance, maxDistance);
            Vector3 desiredPos = centre - transform.forward * distance;
            transform.position = Vector3.Lerp(transform.position,
                desiredPos,
                10f * Time.unscaledDeltaTime);
        }
    }
}