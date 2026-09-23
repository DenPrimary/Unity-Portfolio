using UnityEngine;
using TMPro;
using DG.Tweening;

public class FloatingFloats : MonoBehaviour
{
    public TextMeshProUGUI textMesh;

    public float lifetime = 1.2f;
    public float dropDuration = 0.15f;
    public float dropDistance = 1.5f;
    public float riseDuration = 0.8f;
    public float riseDistance = 5f;

    public void Setup(string text, Vector3 worldPosition, Transform playerTransform) {
        if (textMesh == null)
            return;

        textMesh.text = text;
        transform.position = worldPosition;

        Vector3 toPlayer = playerTransform.position - worldPosition;
        toPlayer.y = 0;
        toPlayer.Normalize();

        Vector3 dropTarget = worldPosition + Vector3.down * dropDistance;
        Vector3 riseTarget = dropTarget + Vector3.up * riseDistance + toPlayer * riseDistance;

        Sequence seq = DOTween.Sequence();

        seq.Append(transform.DOMove(dropTarget, dropDuration).SetEase(Ease.OutCubic));
        seq.Append(transform.DOMove(riseTarget, riseDuration).SetEase(Ease.OutCubic));
        seq.Join(textMesh.DOFade(0f, riseDuration * 0.6f).SetDelay(riseDuration * 0.4f));
        seq.OnComplete(() => Destroy(gameObject));
    }
}
