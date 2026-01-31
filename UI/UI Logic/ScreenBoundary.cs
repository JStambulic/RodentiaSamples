using UnityEngine;

public class ScreenBoundary : MonoBehaviour
{
    [SerializeField] RectTransform canvasRectangle;

    Vector2 screenBoundary;
    RectTransform rect;
    [SerializeField] float rectSizeDivision = 2.0f;

    [SerializeField, Range(0, 1920)] float xBufferAmount = 0.0f;
    [SerializeField, Range(0, 1080)] float yBufferAmount = 0.0f;
    float imageWidth, imageHeight;

    // Start is called before the first frame update
    void Start()
    {
        screenBoundary = new Vector2(canvasRectangle.sizeDelta.x, canvasRectangle.sizeDelta.y);
        rect = GetComponent<RectTransform>();

        imageWidth = GetComponent<RectTransform>().sizeDelta.x / rectSizeDivision;
        imageHeight = GetComponent<RectTransform>().sizeDelta.y / rectSizeDivision;
    }

    void LateUpdate()
    {
        Vector2 viewPos = rect.anchoredPosition;

        viewPos.x = Mathf.Clamp(viewPos.x, 0 + imageWidth + xBufferAmount,
            screenBoundary.x - imageWidth - xBufferAmount);

        viewPos.y = Mathf.Clamp(viewPos.y, 0 + imageHeight + yBufferAmount,
            screenBoundary.y - imageHeight - yBufferAmount);

        rect.anchoredPosition = viewPos;
    }
}
