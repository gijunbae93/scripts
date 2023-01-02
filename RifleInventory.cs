using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RifleInventory : MonoBehaviour
{
    public static RifleInventory Instance { get; private set; }

    RifleInventory() { }

    List<Rifle> rifleList;

    Rifle activeRifle;
    Rifle previousActiveRifle;

    public event Action<Rifle> OnRifleActivation;

    PlayerAction playerAction;
    private void Awake()
    {
        Instance = this;

        rifleList = new List<Rifle>();

        playerAction = GetComponent<PlayerAction>();
    }

    private void Start()
    {
        OnRifleActivation += PlayerRifleInventory_OnRifleActivation;

        playerAction.OnKeycodeOnePress += (s, args) => SwapRifle();

        AddRifleToInventory(new Rifle { rifleType = Rifle.RifleType.RifleOne });
    }

    private void PlayerRifleInventory_OnRifleActivation(Rifle obj)
    {
        SetActiveRifle(obj);
        obj.ActivateRifle();

        AudioManager.Play2DSoundEffect(AudioManager.SoundEffect.RifleSwapSound, AudioAsset.Instance.player2DSoundSource, 0.3f);
    }

    void SetActiveRifle(Rifle rifle) => activeRifle = rifle;

    public Rifle GetActiveRifle()
    {
        return activeRifle;
    }

    void SetPreviousActiveRifle(Rifle rifle) => previousActiveRifle = rifle;

    void RemoveRifle(Rifle rifle)
    {
        rifleList.Remove(rifle);
    }
    void AddRifle(Rifle rifle)
    {
        rifleList.Add(rifle);
    }
    public void AddRifleToInventory(Rifle rifle, bool inventoryReloadSound = false)
    {
        if(rifleList.Count < PlayerStats.Instance.GetRifleMaxSlotNumber())
        {
            foreach (Rifle inventoryRifle in rifleList)
            {
                if (inventoryRifle.rifleType == rifle.rifleType)
                {
                    inventoryRifle.ResetAmmo();
                    if(inventoryReloadSound)
                        AudioManager.Play2DSoundEffect(AudioManager.SoundEffect.InventoryReload, AudioAsset.Instance.player2DSoundSource, 1f);
                    return;
                }
            }
            // when it does not have matching rifle
            AddRifle(rifle);
            if (rifleList.Count != 1)
                SetPreviousActiveRifle(activeRifle);
            OnRifleActivation(rifle);
        }
        else if(rifleList.Count == PlayerStats.Instance.GetRifleMaxSlotNumber())
        {
            foreach (Rifle inventoryRifle in rifleList)
            {
                if (inventoryRifle.rifleType == rifle.rifleType)
                {
                    inventoryRifle.ResetAmmo();
                    if (inventoryReloadSound)
                        AudioManager.Play2DSoundEffect(AudioManager.SoundEffect.InventoryReload, AudioAsset.Instance.player2DSoundSource, 1f);
                    return;
                }
            }
            // when it does not have matching rifle
            activeRifle.ResetAmmo();
            SetPreviousActiveRifle(activeRifle);
            RemoveRifle(activeRifle);
            AddRifle(rifle);
            OnRifleActivation(rifle);
        }
    }
    public void SwapRifle()
    {
        if (!PlayerStats.Instance.GetIsUsingRifle())
        {
            OnRifleActivation(activeRifle);
            GrenadeInventory.Instance.DisableAllGrenadeVisual();
        }
        else
        {
            if (rifleList.Count == 1)
            {
                return;
            }
            else if (rifleList.Count == 2)
            {
                Rifle tempRifle = previousActiveRifle;

                SetPreviousActiveRifle(activeRifle);
                OnRifleActivation(tempRifle);
            }
            else if (rifleList.Count == 3)
            {
                foreach (Rifle inventoryRifle in rifleList)
                {
                    if (inventoryRifle.rifleType != activeRifle.rifleType && inventoryRifle.rifleType != previousActiveRifle.rifleType)
                    {
                        SetPreviousActiveRifle(activeRifle);
                        OnRifleActivation(inventoryRifle);
                        return;
                    }
                }
            }
            else Debug.Log("Error");
        }

        AudioManager.Play2DSoundEffect(AudioManager.SoundEffect.RifleSwapSound, AudioAsset.Instance.player2DSoundSource, 0.3f);
    }

    public void DisableAllRifle()  // used when player is using grenade
    {
        foreach(Rifles rifle in RifleAsset.Instance.rifleDictionary.Values)
        {
            rifle.gameObject.SetActive(false);
        }
    }
}
