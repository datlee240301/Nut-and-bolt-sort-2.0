using UnityEngine;

[ExecuteAlways]
public class TubeGridLayout : MonoBehaviour
{
    [Header("Grid Settings")]
    public int maxTubesPerRow = 5;
    public float spacingX = 2.5f;
    public float spacingY = 3.0f;

    void Update()
    {
        UpdateTubePositions();
    }

    void OnTransformChildrenChanged()
    {
        UpdateTubePositions();
    }

    void UpdateTubePositions()
    {
        int totalTubes = transform.childCount;

        for (int i = 0; i < totalTubes; i++)
        {
            Transform tube = transform.GetChild(i);

            int rowIndex = i / maxTubesPerRow;
            int colIndex = i % maxTubesPerRow;

            // Số lượng tube trong hàng này
            int tubesInThisRow = Mathf.Min(maxTubesPerRow, totalTubes - rowIndex * maxTubesPerRow);

            // Căn giữa theo hàng
            float rowOffsetX = (tubesInThisRow - 1) * spacingX / 2f;

            // Tính vị trí: giữa → trái → phải, hàng từ dưới lên
            float posX = colIndex * spacingX - rowOffsetX;
            float posY = rowIndex * spacingY;  // <-- từ dưới lên

            tube.localPosition = new Vector3(posX, posY, 0);
        }
    }
}