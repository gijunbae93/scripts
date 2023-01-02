using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;

public class RifleTen : Rifles
{
    public RifleAction<RifleTen, BulletTen> rifleAction;
    [SerializeField] BulletTen bullet;

    Transform muzzleFlashPosition;
    ParticleSystem muzzleFlashParticle;

    MuzzleFlash muzzleFlash;

    bool isZoomed = false;
    float zoomTimer;
    float zoomCoolDown = 0.5f;
    public event EventHandler<ZoomArgs> OnZoom;
    public class ZoomArgs : EventArgs
    {
        public bool isZoomed;
        public float zoomDuration;
    }

    GameObject[] visualComponents = new GameObject[5];

    RifleTenSoundEffects rifleTenSoundEffect = new RifleTenSoundEffects();

    Action onReload;

    public override void Awake()
    { 
        base.Awake();

        bulletFiringPoints = new Transform[1];
        bulletFiringPoints[0] = transform.Find("RifleTenBulletFiringPoint");

        muzzleFlashPosition = transform.Find("MuzzleFlashPosition");
        muzzleFlashParticle = transform.Find("MuzzleFlashParticle").GetComponent<ParticleSystem>();
        muzzleFlash = new MuzzleFlash(muzzleFlashPosition, muzzleFlashParticle);

        visualComponents[0] = transform.Find("Cube").gameObject;
        visualComponents[1] = transform.Find("Cube.001").gameObject;
        visualComponents[2] = transform.Find("Cube.002").gameObject;
        visualComponents[3] = transform.Find("Cube.003").gameObject;
        visualComponents[4] = transform.Find("Cube.004").gameObject;

        foreach (GameObject visualComponent in visualComponents) visualComponent.SetActive(true);
    }
    // Start is called before the first frame update
    void Start()
    {
        rifleType = Rifle.RifleType.RifleTen;

        rifleAction = new RifleAction<RifleTen, BulletTen>(this, bullet);
        rifleAction.OnFire += RifleAction_OnFire;

        currentAmmo = RifleStatAsset.Instance.rifle10CurrentAmmo;
        maxAmmo = RifleStatAsset.Instance.GetMaxAmmo(rifleType);
        bulletFireCoolDown = RifleStatAsset.Instance.rifle10FireCoolDown;
        bulletTravelingForceMagnitude = RifleStatAsset.Instance.rifle10FireForceMagnitude;

        defaultCurrentAmmo = currentAmmo;
        defaultMaxAmmo = maxAmmo;

        OnZoom += RifleTen_OnZoom;
        PlayerStats.Instance.OnPlayerDeath += Instance_OnPlayerDeath;
    }

    private void Instance_OnPlayerDeath(object sender, EventArgs e) => ExitZoom();
    public override void OnDisable()
    {
        ExitZoom();

        base.OnDisable();
    }

    private void RifleTen_OnZoom(object sender, ZoomArgs e)
    {
        if (isZoomed)
        {
            foreach (GameObject visualComponent in visualComponents) visualComponent.SetActive(false);

            zoomTimer = Time.time;

            AudioManager.Play2DSoundEffect(AudioManager.SoundEffect.SniperZoomIn, AudioAsset.Instance.player2DSoundSource);
        }
        else
        {
            foreach (GameObject visualComponent in visualComponents) visualComponent.SetActive(true);

            zoomTimer = Time.time;

            AudioManager.Play2DSoundEffect(AudioManager.SoundEffect.SniperZoomOut, AudioAsset.Instance.player2DSoundSource);
        }
    }

    public void ExitZoom()
    {
        if (isZoomed)
        {
            isZoomed = false;
            AudioManager.Play2DSoundEffect(AudioManager.SoundEffect.SniperZoomOut, AudioAsset.Instance.player2DSoundSource);
            OnZoom?.Invoke(this, new ZoomArgs
            {
                isZoomed = isZoomed,
                zoomDuration = zoomCoolDown - 0.2f
            });
        }
    }

    private void RifleAction_OnFire(object sender, EventArgs e)
    {
        muzzleFlash.SpawnMuzzleFlash();

        animator.SetTrigger("Fire");

        rifleTenSoundEffect.Fire();
    }
    bool CanZoom()
    {
        return (PlayerAction.currentGunMotion == PlayerAction.GunMotion.GunIpose | PlayerAction.currentGunMotion == PlayerAction.GunMotion.GunFire)
        && Time.time > zoomTimer + zoomCoolDown && Mouse.current.rightButton.wasPressedThisFrame;
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        if (CanZoom())
        {
            isZoomed = isZoomed ? isZoomed = false : isZoomed = true;

            OnZoom?.Invoke(this, new ZoomArgs {
                isZoomed = isZoomed,
                zoomDuration = zoomCoolDown - 0.3f,
            });
        }

        if (isReloading && isZoomed)
            ExitZoom();
    }

    public void ReloadOneSoundEffect() => rifleTenSoundEffect.ReloadOne(); // used in unity animation event;
    public void ReloadTwoSoundEffect() => rifleTenSoundEffect.ReloadTwo(); // used in unity animation event;
    public void SniperReloadSoundEffect() => rifleTenSoundEffect.ReloadThree(); // used in unity animation event;
    public void UpdateAmmoAfterReload() => rifleAction.UpdateAmmoAfterReload(); // used in unity animation event;

    public override void FireBullet() => rifleAction.FireBullet();
    public override void Reload() => rifleAction.Reload();
    public override void ResetAllAmmo() => rifleAction.ResetAllAmmo();
    public override void SetWeaponMotion()
    {
        rifleAction.SetRunState();
        rifleAction.SetCurrentGunState();
    }
    public override int GetCurrentAmmo() { return currentAmmo; }
    public override int GetDefaultCurrentAmmo() { return defaultCurrentAmmo; }
    public override int GetMaxAmmo() { return maxAmmo; }
    public override int GetDefaultMaxAmmo() { return defaultMaxAmmo; }

    class RifleTenSoundEffects 
    {
        public void Fire() => AudioManager.Play2DSoundEffect(AudioManager.SoundEffect.RifleTenFire, AudioAsset.Instance.player2DSoundSource, 1f);
        public void ReloadOne() => AudioManager.Play2DSoundEffect(AudioManager.SoundEffect.RifleReloadOne, AudioAsset.Instance.player2DSoundSource, 0.6f); // used in unity animation event;
        public void ReloadTwo() => AudioManager.Play2DSoundEffect(AudioManager.SoundEffect.RifleReloadTwo, AudioAsset.Instance.player2DSoundSource, 0.8f); // used in unity animation event;
        public void ReloadThree() => AudioManager.Play2DSoundEffect(AudioManager.SoundEffect.RifleTenReload, AudioAsset.Instance.player2DSoundSource, 1f); // used in unity animation event;
    }

}
