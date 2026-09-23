using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;

public class PointerArrow : MonoBehaviour
{
    public RectTransform canvasRect;
    public Image pointerPrefab;

    public float borderOffset = 50f;

    private Camera mainCam;
    private List<Image> pointerPool = new List<Image>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainCam = Camera.main;
        if (canvasRect == null)
            canvasRect = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (mainCam == null) return;

        var targets = GetOffScreenTargets();
        ResetPointers();

        for (int i = 0; i < targets.Count; i++)
        {
            Image pointer = SpawnPointer(i);
            PositioningPointer(pointer, targets[i]);
        }
    }

    private Vector2 GetScreenEdge(Vector2 center, Vector2 direction)
    {
        Rect rect = canvasRect.rect;

        float tLeft = (rect.xMin - center.x) / direction.x;
        float tRight = (rect.xMax - center.x) / direction.x;
        float tBottom = (rect.yMin - center.y) / direction.y;
        float tTop = (rect.yMax - center.y) / direction.y;

        float t = float.MaxValue;
        if (tLeft > 0 && tLeft < t)
            t = tLeft;
        if (tRight > 0 && tRight < t)
            t = tRight;
        if (tBottom > 0 && tBottom < t)
            t = tBottom;
        if (tTop > 0 && tTop < t)
            t = tTop;

        return center + direction * t;
    }

    private float GetScreenRadius(Transform target)
    {
        float worldRadius = 0f;

        Collider collider = target.GetComponent<Collider>();
        if (collider != null)
            worldRadius = collider.bounds.extents.magnitude;
        else
        {
            Renderer rend = target.GetComponent<Renderer>();
            if (rend != null)
                worldRadius = rend.bounds.extents.magnitude;
        }

        if (worldRadius <= 0f) return 0f;

        Vector3 worldPos = target.position;
        Vector3 screenCenter = mainCam.WorldToScreenPoint(worldPos);
        Vector3 screenRight = mainCam.WorldToScreenPoint(worldPos + mainCam.transform.right * worldRadius);

        return Mathf.Abs(screenRight.x - screenCenter.x);
    }

    private bool IsObjectOffScreen(Vector3 screenPos, float screenRadius)
    {
        float astMinX = screenPos.x - screenRadius;
        float astMaxX = screenPos.x + screenRadius;
        float astMinY = screenPos.y - screenRadius;
        float astMaxY = screenPos.y + screenRadius;

        float scrMinX = 0;
        float scrMaxX = Screen.width;
        float scrMinY = 0;
        float scrMaxY = Screen.height;

        bool overlaps = astMinX < scrMaxX && astMaxX > scrMinX &&
                        astMinY < scrMaxY && astMaxY > scrMinY;

        return !overlaps;
    }

    private List<Vector2> GetOffScreenTargets()
    {
        List<Vector2> result = new List<Vector2>();

        var asteroidTransform = FindObjectsByType<Asteroid>()
            .Where(ast => ast != null)
            .Select(ast => ast.transform);

        var fragmentTransforms = FindObjectsByType<Fragment>()
            .Where(frag => frag != null)
            .Select(frag => frag.transform);


        foreach (var target in asteroidTransform.Concat(fragmentTransforms))
        {
            Vector3 screenPos = mainCam.WorldToScreenPoint(target.position);
            if (screenPos.z < 0) continue;

            float screenRadius = GetScreenRadius(target);

            if (!IsObjectOffScreen(screenPos, screenRadius)) continue;

            Vector2 localPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect, screenPos, null, out localPos
                );

            result.Add(localPos);
        }

        return result;
    }

    private Image SpawnPointer(int index)
    {
        while (pointerPool.Count <= index)
        {
            Image newPointer = Instantiate(pointerPrefab, canvasRect);



            pointerPool.Add(newPointer);
        }

        Image pointer = pointerPool[index];
        pointer.gameObject.SetActive(true);

        return pointer;
    }

    private void PositioningPointer(Image pointer, Vector2 targetLocalPos)
    {
        Rect rect = canvasRect.rect;
        Vector2 center = rect.center;

        Vector2 direction = (targetLocalPos - center).normalized;

        Vector2 pointerPos = GetScreenEdge(center, direction);
        pointerPos -= direction * borderOffset;

        pointer.rectTransform.anchoredPosition = pointerPos;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        pointer.rectTransform.localRotation = Quaternion.Euler(0, 0, angle - 90f);
    }

    private void ResetPointers()
    {
        foreach (var pointer in pointerPool)
            pointer.gameObject.SetActive(false);
    }
}