using UnityEngine;
using System.Collections;
using DG.Tweening;

public abstract class Asteroid : MonoBehaviour
{
    [Header("Health")]
    public float baseHealth = 1;
    protected float currentHealth;

    [Header("Movement")]
    public float baseSpeed = 2f;
    public Vector3 rotationSpeed = new Vector3(30f, 45f, 20f);

    [Header("Life Time")]
    public float bigCircleRadius = 45f;
    public float timeOutsideBigRadius = 10f;

    public float appearDuration = 0.3f;
    public float disappearDuration = 0.25f;

    protected float currentSize = 1f;
    protected Rigidbody rb;
    private float timeOutside = 0f;

    private bool isDisapear = false;

    protected virtual void Awake() { 
        rb = GetComponent<Rigidbody>();

        if (rb == null)
            Debug.LogError("Rigidbody not found!");
    }

    protected virtual void OnEnable() { 
        transform.DOKill();
        isDisapear = false;
        timeOutside = 0f;

        Outline outline = GetComponent<Outline>();
        if (outline != null)
            outline.enabled = true;

        if (rb != null) { 
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        AsteroidPath path = FindAnyObjectByType<AsteroidPath>();
        if (path != null) path.ClearOffset(GetHashCode());
    }

    protected virtual void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime);

        Transform player = PlayerFinder.Player;
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (distanceToPlayer > bigCircleRadius)
        {
            timeOutside += Time.deltaTime;
            if (timeOutside >= timeOutsideBigRadius)
            {
                DisableOutline();
                DisappearAndDestroy();
                return;
            }
        }
        else
        {
            timeOutside = 0f;
        }
    }

    protected virtual float GetCalculatedSpeed()
    {
        return baseSpeed / Mathf.Max(currentSize, 0.75f);
    }

    protected virtual float GetCalculatedHealth()
    {
        return Mathf.CeilToInt(baseHealth * currentSize * GetHealthMultiplier());
    }

    protected virtual float GetHealthMultiplier()
    {
        return 1f;
    }

    public virtual void OnShieldHit()
    {
        DisappearAndDestroy();
    }

    protected abstract void Die();

    private void GiveExp()
    {
        int expGain = Mathf.RoundToInt(GetCalculatedHealth());
        ExpManager.GiveExp(expGain, transform.position);
    }

    protected void DisableOutline()
    {
        Outline outline = GetComponent<Outline>();
        if (outline != null)
            outline.enabled = false;
    }

    public virtual void Activate()
    {
        AppearAnimation();

        if (rb != null)
            rb.linearVelocity = transform.forward * GetCalculatedSpeed();
    }

    public virtual void TakeDamage(float damage)
    {
        if (currentHealth <= 0) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            GiveExp();
            HighlightManager.Instance?.OnAsteroidDestroyed(gameObject);
            DisappearAndDestroy();
        }
    }

    public void AppearAnimation() 
    {
        transform.localScale = Vector3.zero;
        transform.DOScale(Vector3.one * currentSize, appearDuration).SetEase(Ease.OutBack);
    }

    public void DisappearAndDestroy() 
    {
        if (isDisapear) return;
        isDisapear = true;

        if (rb != null) 
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        transform.DOKill();

        transform.DOScale(Vector3.zero, disappearDuration)
            .SetEase(Ease.InBack)
            .OnComplete(() => {
            Die();
            ReturnToPool();
        });
    }

    public virtual void SetSize(float size)
    {
        currentSize = Mathf.Max(size, 0.1f);
        transform.localScale = Vector3.one * currentSize;

        float calculatedHealth = GetCalculatedHealth();
        currentHealth = calculatedHealth;

        if (rb != null)
            rb.linearVelocity = transform.forward * GetCalculatedSpeed();
    }

    public float GetSize()
    {
        return currentSize;
    }

    public virtual void ReturnToPool() {
        Destroy(gameObject);
    }
}
