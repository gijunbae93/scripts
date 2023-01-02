using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class Rifles : MonoBehaviour
{
    protected Rifle.RifleType rifleType;

    [HideInInspector] public Animator animator;

    [HideInInspector] public Transform[] bulletFiringPoints;

    [HideInInspector] public int currentAmmo, maxAmmo;

    [HideInInspector] public int defaultCurrentAmmo, defaultMaxAmmo; 

    [HideInInspector] public float bulletFireCoolDown, bulletTravelingForceMagnitude;

    [HideInInspector] public PlayerUi playerUi;

    [HideInInspector] public bool isReloading = false, isFiring = false, isDrawing = false, isRunning = false;

    [HideInInspector] public string clipName;

    [HideInInspector] public AnimatorClipInfo[] clipInfoArray;

    private void OnEnable()
    {
        PlayerAction.currentGunMotion = PlayerAction.GunMotion.GunDraw;

        NextFrame.Create(() => UpdateAmmo());
    }

    public virtual void Awake()
    {
        animator = GetComponent<Animator>();
        playerUi = GameObject.Find("Player").GetComponent<PlayerUi>();
    }

    public void UpdateAmmo()
    {
        if (defaultMaxAmmo != RifleStatAsset.Instance.GetMaxAmmo(rifleType))
        {
            int changedAmount = RifleStatAsset.Instance.GetMaxAmmo(rifleType) - defaultMaxAmmo;

            //defaultCurrentAmmo += changedAmount;
            defaultMaxAmmo += changedAmount;
            //currentAmmo += changedAmount;
            maxAmmo += changedAmount;

            playerUi.UpdateAmmoTMPText();
        }
        else
            playerUi.UpdateAmmoTMPText();
    }

    public abstract void FireBullet();
    public abstract void Reload();
    public abstract void ResetAllAmmo();
    public abstract void SetWeaponMotion();
    public abstract int GetCurrentAmmo();
    public abstract int GetDefaultCurrentAmmo();
    public abstract int GetMaxAmmo();
    public abstract int GetDefaultMaxAmmo();
}

public class MuzzleFlash
{
    Transform muzzleFlashPoint;
    ParticleSystem muzzleFlashParticle;

    public MuzzleFlash(Transform muzzleFlashPoint, ParticleSystem muzzleFlashParticle)
    {
        this.muzzleFlashPoint = muzzleFlashPoint;
        this.muzzleFlashParticle = muzzleFlashParticle;
    }

    public void SpawnMuzzleFlash()
    {
        GameObject muzzleFlash = GameObject.Instantiate(RifleAsset.Instance.muzzleFlashPrefab, muzzleFlashPoint, false);
        muzzleFlash.transform.localPosition = new Vector3(0f, 0f, 0f);
        muzzleFlash.transform.localRotation = Quaternion.Euler(0f, 0, 0f);
        muzzleFlashParticle.Play();
        UnityEngine.GameObject.Destroy(muzzleFlash, 0.05f);
    }
}
