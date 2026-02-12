using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class BackgroundFitter : MonoBehaviour
{
  private RectTransform rectTransform;

  void Awake()
  {
    rectTransform = GetComponent<RectTransform>();
    StretchToFullScreen();
  }

  void Update()
  {
    // Фон всегда на весь экран, даже при изменении ориентации
    StretchToFullScreen();
  }

  void StretchToFullScreen()
  {
    rectTransform.anchorMin = Vector2.zero;
    rectTransform.anchorMax = Vector2.one;
    rectTransform.offsetMin = Vector2.zero;
    rectTransform.offsetMax = Vector2.zero;
  }
}