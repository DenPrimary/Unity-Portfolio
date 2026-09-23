using UnityEngine;
using DG.Tweening;

public class BorderAnimation : MonoBehaviour
{
    [Header("Animation Settings")]
    public float moveDistance = 5f;
    public float moveDuration = 2f;
    public Vector3 moveDirection = Vector3.right;

    private Vector3 startPos;
    private Tween moveTween;

    void Start()
    {
        startPos = transform.position;
        StartAnimation();
    }

    void OnDestroy()
    {
        moveTween?.Kill();
    }

    public void StartAnimation()
    {
        moveTween?.Kill();

        moveTween = transform.DOMove(startPos + moveDirection * moveDistance, moveDuration)
            .SetEase(Ease.InOutQuad)
            .SetLoops(-1, LoopType.Yoyo);
    }

    public void StopAnimation()
    {
        moveTween?.Kill();
        transform.position = startPos;
    }
}
