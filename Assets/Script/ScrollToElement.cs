using UnityEngine;
using UnityEngine.UI;

public class ScrollToTargetByLevel : MonoBehaviour
{
    public ScrollRect scrollRect;
    public RectTransform content;
    public RectTransform[] targetElement;

    void Start()
    {
        StartCoroutine(ScrollToCurrentLevel());
    }

    System.Collections.IEnumerator ScrollToCurrentLevel()
    {
        yield return null; // chờ layout tính xong

        int levelId = PlayerPrefs.GetInt(StringManager.currentLevelId, 0); // mặc định là 0
        levelId = Mathf.Clamp(levelId, 0, targetElement.Length - 1);

        RectTransform target = targetElement[levelId];

        float contentHeight = content.rect.height;
        float viewportHeight = scrollRect.viewport.rect.height;
        float scrollableHeight = contentHeight - viewportHeight;

        float targetY = Mathf.Abs(target.anchoredPosition.y);

        // Nếu muốn focus giữa Viewport:
        float centerOffset = (viewportHeight - target.rect.height) / 2f;
        float adjustedTargetY = targetY - centerOffset;

        float targetNormalized = 1f - (adjustedTargetY / scrollableHeight);
        targetNormalized = Mathf.Clamp01(targetNormalized); // giới hạn từ 0 -> 1

        scrollRect.verticalNormalizedPosition = targetNormalized;
    }
}