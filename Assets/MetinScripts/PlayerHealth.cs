using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private AudioClip damageSound;
    [SerializeField] private float growthFactor = 0.05f; // hasar baþýna büyüme katsayýsý
    [SerializeField] private float scaleLerpSpeed = 5f; // büyüme hýzýný yumuþatmak için

    private int _current;
    private Vector3 _targetScale;
    public event Action OnPlayerDamaged;

    public bool IsAlive => _current > 0;

    public static PlayerHealth Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // sahnede 2 tane varsa önle
            return;
        }
        Instance = this;
        _current = maxHealth;
        _targetScale = transform.localScale;
    }
    void Update()
    {
        // scale animasyonu FPS dostu bir þekilde yumuþat
        if (transform.localScale != _targetScale)
        {
            transform.localScale = Vector3.Lerp(
                transform.localScale,
                _targetScale,
                Time.deltaTime * scaleLerpSpeed
            );
        }
    }

    public void TakeDamage(int damage, Vector3 hitPoint, Vector3 hitNormal = default)
    {
        if (!IsAlive) return;
        _current -= damage;
        if (_current < 0) _current = 0;

        if (damageSound != null)
            AudioPoolManager.Instance.PlayAt(damageSound, hitPoint);

        ScreenDamageEffect.Instance?.Flash();

        PlayerUI.Instance?.UpdateHealth(_current, maxHealth);
        //Debug.Log($"Player took {damage} damage. Current health: {_current}");

        // Event tetikleme
        OnPlayerDamaged?.Invoke();

        // Kara delik efekti ? hasar aldýkça büyüsün
        float growthAmount = damage * growthFactor;
        _targetScale += new Vector3(growthAmount, growthAmount, growthAmount);

        if (!IsAlive)
            GameOverManager.Instance.TriggerGameOver();
    }

    public void ResetHealth()
    {
        _current = maxHealth;
        PlayerUI.Instance?.UpdateHealth(_current, maxHealth);
        Debug.Log($"Player health reset. Current health: {_current}");

        // resetlenince scale’i de eski haline getir
        _targetScale = Vector3.one;
        transform.localScale = Vector3.one;
    }

    // GunShoot ile uyumlu overload
    public void TakeDamage(int damage, Vector3 hitPoint)
        => TakeDamage(damage, hitPoint, Vector3.up);
}

