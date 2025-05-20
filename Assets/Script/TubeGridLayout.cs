using UnityEngine;

[ExecuteAlways]
public class TubeGridLayout : MonoBehaviour {
    [Header("Grid Settings")]
    public int maxTubesPerRow = 4;
    public float spacingX = 2.5f;
    public float spacingY = 3.0f;

    void Update() {
        UpdateTubePositions();
    }

    void OnTransformChildrenChanged() {
        UpdateTubePositions();
    }

    void UpdateTubePositions() {
        int totalTubes = transform.childCount;

        for (int i = 0; i < totalTubes; i++) {
            Transform tube = transform.GetChild(i);

            // Vị trí trong grid
            int rowIndex = i / maxTubesPerRow;
            int colIndex = i % maxTubesPerRow;

            // Đảo chiều xếp theo yêu cầu: phải -> trái
            int col = maxTubesPerRow - 1 - colIndex;
            int row = rowIndex;

            Vector3 targetPos = new Vector3(col * spacingX, row * spacingY, 0);
            tube.localPosition = targetPos;
        }
    }
}
