using System.Collections.Generic;
using UnityEngine;

public class GunShoot : MonoBehaviour
{
    [Header("Weapon Settings")]
    public float fireRate = 0.25f;
    public float weaponRange = 100f;

    [Header("Transforms & Effects")]
    public Transform gunEnd;
    public ParticleSystem muzzleFlash;
    public ParticleSystem cartridgeEjection;

    [Header("Audio Clips")]

    public AudioClip gunSound;

    private float nextFire;
    private Animator anim;
    private GunAim gunAim;

    private Camera _camPlayer;
 
    void Start()
    {
        anim = GetComponent<Animator>();
        gunAim = GetComponentInParent<GunAim>();
        _camPlayer = gunAim?.GetComponentInChildren<Camera>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1") && Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;
            muzzleFlash?.Play();
            cartridgeEjection?.Play();
            anim?.SetTrigger("Fire");

            ShootRay();
        }
    }
    void ShootRay()
    {
        if (gunAim == null || gunEnd == null || _camPlayer == null) return;

        // Kamera crosshair ile hedef
        Ray camRay = _camPlayer.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Vector3 targetPoint = camRay.GetPoint(weaponRange);

        // Her zaman ateş sesi çalsın (2D olarak UI’den almak için spatialBlend=0f)
        if (gunSound != null)
        {
            AudioPoolManager.Instance.PlayAt(gunSound, Vector3.zero, 1f, 0f, AudioPoolManager.Instance.defaultSFXMixerGroup);
        }

        // Silah ucundan raycast
        Vector3 direction = (targetPoint - gunEnd.position).normalized;
        Debug.DrawRay(gunEnd.position, direction * weaponRange, Color.red, 1f);

        if (Physics.Raycast(gunEnd.position, direction, out RaycastHit hit, weaponRange))
        {
            HandleHit(hit);
        }
        else
        {
            // Hiçbir şeye vurmadı -> sadece fireSound çaldı
            Debug.Log("Boşa ateş edildi!");
        }
    }

    private static readonly Dictionary<string, string> materialEffectMap = new()
    {
        { "Metal", "FX_Hit_Metal" },
        { "Wood", "FX_Hit_Wood" },
        { "Stone", "FX_Hit_Stone" },
        { "Sand", "FX_Hit_Sand" },
        { "Water", "FX_Hit_Water" },
        { "Flesh", "FX_Hit_Flesh"},
        {" Fire","FX_Hit_Fire"  }, 
        { "WaterFilledExtinguish", "FX_Hit_WaterExt" }
    };
 
    void HandleHit(RaycastHit hit)
    {
        if (hit.collider.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.TakeDamage(1, hit.point, hit.normal);
            Debug.Log("Ray hit: " + hit.collider.name);
        }

        var targetObj = hit.collider.GetComponent<TargetObject>();
        if (targetObj != null && targetObj.Data != null)
        {
            var data = targetObj.Data;

            if (!string.IsNullOrEmpty(data.hitEffectKey))
                EffectPoolManager.Instance?.SpawnEffect(data.hitEffectKey, hit.point, hit.normal, 1.25f);

            return;
        }

        if (hit.collider.sharedMaterial != null)
        {
            string mat = hit.collider.sharedMaterial.name;

            foreach (var kvp in materialEffectMap)
            {
                if (mat.Contains(kvp.Key, System.StringComparison.OrdinalIgnoreCase))
                {
                    EffectPoolManager.Instance?.SpawnEffect(kvp.Value, hit.point, hit.normal, 1.25f);
                    return;
                }
            }
        }
    }
}
