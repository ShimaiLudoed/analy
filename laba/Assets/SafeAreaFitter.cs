using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class SafeAreaFitter : MonoBehaviour
{
    private RectTransform rectTransform;
    private Rect lastSafeArea;
    private ScreenOrientation lastOrientation;

    [Header("Debug")]
    [SerializeField] private bool logSafeArea = true;
    [SerializeField] private Color gizmoColor = new Color(0, 1, 0, 0.5f);

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        ApplySafeArea();
    }

    void Update()
    {
        // Проверяем изменения safe area или ориентации
        if (Screen.safeArea != lastSafeArea || Screen.orientation != lastOrientation)
        {
            ApplySafeArea();
        }
    }

    public void ApplySafeArea()
    {
        Rect safeArea = Screen.safeArea;
        lastSafeArea = safeArea;
        lastOrientation = Screen.orientation;

        // Конвертируем пиксели в нормализованные координаты (0-1)
        Vector2 anchorMin = safeArea.position;
        Vector2 anchorMax = safeArea.position + safeArea.size;

        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        // Применяем к RectTransform
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;

        if (logSafeArea)
        {
            Debug.Log($"🟢 SafeArea Updated:\n" +
                      $"  Screen: {Screen.width} x {Screen.height}\n" +
                      $"  SafeArea: {safeArea}\n" +
                      $"  Orientation: {Screen.orientation}\n" +
                      $"  AnchorMin: {anchorMin}\n" +
                      $"  AnchorMax: {anchorMax}");
        }
    }

    #if UNITY_EDITOR
    // Визуализация safe area в редакторе
    void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        Rect safeArea = Screen.safeArea;
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);

        // Рисуем прямоугольник safe area
        Gizmos.color = gizmoColor;
        Vector3 center = (corners[0] + corners[2]) * 0.5f;
        Vector3 size = corners[2] - corners[0];
        Gizmos.DrawWireCube(center, size);
    }
    #endif
}