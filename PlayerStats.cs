using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO.Pipes;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance { get; private set; }

    PlayerStats() { }

    float currentHp = 100f, maxHp = 100f;
    public float MaxHp
    {
        get => maxHp;
        set
        {
            if (value > 999) //////////////////////////////////////////////////////////
                maxHp = 999;//////////////////////////////////////////////////////////
            else
                maxHp = value;
        }
    }
    float currentArmor, maxArmor;
    const int maxArmorLimit = 999;
    public float MaxArmor
    {
        get => maxArmor;
        set
        {
            if (value > maxArmorLimit)
                maxArmor = maxArmorLimit;
            else
                maxArmor = value;
        }
    }

    int money;
    const int maxMoneyAmount = 999999;
    int Money
    {
        get => money;
        set
        {
            if (value > maxMoneyAmount)
                money = maxMoneyAmount;
            else
                money = value;
        }
    }

    int rifleMaxSlotNumber = 3;
    int grenadeMaxSlotNumber = 4;

    bool isUsingRifle;

    PlayerUi playerUi;

    #region Smoke
    AffectPlayer affectPlayer;

    bool isInSmoke = false;
    float smokeTimer = 0;
    const float smokeEndTimer = 1f;
    PlayerStatusEffect playerStatusEffect = new PlayerStatusEffect { statusEffectType = PlayerStatusEffect.StatusEffectType.Smoke };
    #endregion

    bool isPlayerDead;
    public event EventHandler OnPlayerDeath;

    public event EventHandler<HpChangeArgs> OnHpChange;
    public event EventHandler<ArmorChangeArgs> OnArmorChange;
    public event EventHandler<MoneyChangeArgs> OnMoneyChange;

    public class HpChangeArgs : EventArgs
    {
        public float currentHp;
        public float maxHp;
        public float hpChangeAmount;
    }

    public class ArmorChangeArgs : EventArgs
    {
        public float currentArmor;
        public float armorChangeAmount;
    }

    public class MoneyChangeArgs : EventArgs
    {
        public int currentMoney;
        public int changeAmount;
    }

    #region Regenerations
    HealthRegeneration healthRegeneration;
    ArmorRegeration armorRegeneration;
    #endregion

    #region MoneyGain
    float moneyGainPercentage;
    public float MoneyGainPercentage
    {
        get => moneyGainPercentage;
        set
        {
            if ((value % 10) == 0)
                moneyGainPercentage = value;
            else
                Debug.Log("Error");
        }
    }
    #endregion

    #region Invincible
    Invincible invincible;
    #endregion

    #region InstantKill
    public InstantKill instantKill;
    #endregion

    #region Explosion
    [SerializeField] GameObject explosionPrefab;
    Explosion explosion;
    #endregion

    #region Burn
    [SerializeField] GameObject firePrefab;
    Burn burn;
    #endregion

    #region Poison
    [SerializeField] GameObject poisonPrefab;
    Poison poison;
    #endregion

    #region MoneyRegeneration
    MoneyRegeneration moneyRegeneration = new MoneyRegeneration();
    #endregion

    GameObject smokeSoundSource;

    float playerTotalTimePlayed;

    SpawnEnemy spawnEnemy;
    Snow snow = new Snow();
    [SerializeField] SnowHandler snowHandler;

    private void Awake()
    {
        Instance = this;

        isUsingRifle = true;

        affectPlayer = GetComponent<AffectPlayer>();

        playerUi = GetComponent<PlayerUi>();

        spawnEnemy = transform.Find("SpawnEnemy").GetComponent<SpawnEnemy>();
    }

    private void Start()
    {
        RifleInventory.Instance.OnRifleActivation += Instance_OnRifleActivation;
        GrenadeInventory.Instance.OnGrenadeActivation += Instance_OnGrenadeActivation;
        OnPlayerDeath += PlayerStats_OnPlayerDeath;

        MaxHp = 110f + (AdditionalUpgradeData.StartHpLevel * 15); currentHp = 110f + (AdditionalUpgradeData.StartHpLevel * 15); currentArmor = 0f; MaxArmor = 100f; //////////////////////////////////////////////////////////
        isPlayerDead = false;
        Money = 600 + (AdditionalUpgradeData.StartMoneyLevel * 600);
        playerUi.UpdateArmorTMPText(currentArmor);
        playerUi.UpdateHpTMPText(currentHp);
        playerUi.UpdatePlayerMoneyTMPText(money);

        affectPlayer.smoke += AffectPlayer_smoke;

        healthRegeneration = new HealthRegeneration();
        armorRegeneration = new ArmorRegeration();
        invincible = new Invincible(playerUi);
        instantKill = new InstantKill(playerUi);
        explosion = new Explosion(explosionPrefab);
        burn = new Burn(firePrefab);
        poison = new Poison(poisonPrefab);

        GameObject playerSoundSources = transform.Find("PlayerSoundSources").gameObject;
        smokeSoundSource = playerSoundSources.transform.Find("PlayerSmokeSoundSource").gameObject;

        Debug.Log(AdditionalUpgradeData.StartHpLevel + " AdditionalUpgradeData.StartHpLevel");
    }

    #region event Subscribe
    private void Instance_OnGrenadeActivation(object sender, System.EventArgs e)
    {
        SetIsUsingRifle(false);
    }

    private void Instance_OnRifleActivation(Rifle obj)
    {
        SetIsUsingRifle(true);
    }
    private void PlayerStats_OnPlayerDeath(object sender, EventArgs e)
    {
        SetPlayerDead(true);

        PlayerRecords.totalTimePlayed = playerTotalTimePlayed;
        PlayerRecords.CalculateTotalDamage();
        PlayerRecords.CalculateBulletAccuracy();
        PlayerRecords.CalculateHeadShotPercentage();
    }

    private void AffectPlayer_smoke(object sender, System.EventArgs e)
    {
        if (!isPlayerDead)
        {
            SetIsInSmokeTrue();
            playerUi.AddPlayerStatusEffectList(playerStatusEffect, smokeEndTimer);
        }
    }
    #endregion

    private void Update()
    {
        if (isPlayerDead)
            return;

        playerTotalTimePlayed += Time.deltaTime;

        smokeUpdate();
        healthRegeneration.Update();
        armorRegeneration.Update();
        invincible.Update();
        instantKill.Update();
        explosion.Update();
        burn.Update();
        poison.Update();
        moneyRegeneration.Update();
        snow.Update();
    }

    void smokeUpdate()
    {
        if (!isInSmoke)
            return;

        if (smokeTimer <= smokeEndTimer)
        {
            smokeTimer += Time.deltaTime;

            AudioManager.Play2DSoundEffect(AudioManager.SoundEffect.PlayerSmoke, smokeSoundSource, 0.3f);
        }
        else if (smokeTimer > smokeEndTimer && isInSmoke)
        {
            isInSmoke = false;
            playerUi.RemovePlayerStatusEffectList(playerStatusEffect);

            AudioManager.StopPlayingSoundEffect(AudioManager.SoundEffect.PlayerSmoke, smokeSoundSource);
        }
    }


    #region Change
    public void ChangeCurrentHp(float damageAmount)
    {
        if (currentHp <= 0)
            return;

        if (damageAmount < 0 && invincible.GetInvincible())
            return;

        float hpAmountBeforeChange = currentHp;
        float armorAmountBeforeChange = currentArmor;

        float halfOfDamage = damageAmount / 2;
        float newHalfOfDamage = Mathf.Ceil(halfOfDamage);       // ceil is used because this is only used for calculating armor damage

        if (damageAmount > 0)  //            hp increase
        {
            if (currentHp == MaxHp)
            {
                Debug.Log("Error");
                return;
            }
            else if ((currentHp + damageAmount) > MaxHp)
            {
                currentHp = MaxHp;
            }
            else if ((currentHp + damageAmount) <= MaxHp)
            {
                currentHp += damageAmount;
            }
            else Debug.Log("Error");
        }
        else if (damageAmount < 0)            // hp loss
        {
            if (currentArmor <= 0)
            {
                if ((currentHp + damageAmount) <= 0)
                {
                    currentHp = 0;
                }
                else if ((currentHp + damageAmount) > 0)
                {
                    currentHp += damageAmount;
                }
                else Debug.Log("Error");
            }
            else if (currentArmor > 0)
            {
                if (currentArmor >= -newHalfOfDamage) // newHalfoFDmage is nEgagive
                {
                    currentArmor += newHalfOfDamage;

                    if (currentHp > -newHalfOfDamage)
                    {
                        currentHp += newHalfOfDamage;
                    }
                    else if (currentHp <= -newHalfOfDamage)
                    {
                        currentHp = 0;
                    }
                }
                else if (currentArmor < -newHalfOfDamage)
                {
                    float newDamageAmount = (damageAmount + currentArmor);

                    currentArmor = 0f;

                    if (currentHp <= -newDamageAmount)
                    {
                        currentHp = 0;
                    }
                    else if (currentHp > -newDamageAmount)
                    {
                        currentHp += newDamageAmount;
                    }
                }
            }
        }

        currentHp = Mathf.Ceil(currentHp);
        currentArmor = Mathf.Ceil(currentArmor);

        float hpAmountAfterChange = currentHp;
        float hpChangeAmount = hpAmountBeforeChange - hpAmountAfterChange;

        OnHpChange?.Invoke(this, new HpChangeArgs
        {
            currentHp = currentHp,
            maxHp = MaxHp,
            hpChangeAmount = -hpChangeAmount,           // Watch out for negative sign
        });

        float armorAmountAfterChange = currentArmor;                       // armor might not change if armor is 0 or player was affected by effects;
        float armorChangeAmount = armorAmountBeforeChange - armorAmountAfterChange;
        if (armorChangeAmount != 0)
        {
            OnArmorChange?.Invoke(this, new ArmorChangeArgs
            {
                currentArmor = currentArmor,
                armorChangeAmount = -armorChangeAmount,
            });
        }

        if (currentHp <= 0)
            OnPlayerDeath?.Invoke(this, EventArgs.Empty);
    }

    public void ChangeOnlyCurrentHp(float damageAmount)    // used by affects
    {
        if (currentHp <= 0)
            return;

        if (damageAmount < 0 && invincible.GetInvincible())
            return;

        float hpAmountBeforeChange = currentHp;

        float newCurrentHp = currentHp + damageAmount;

        if (newCurrentHp <= 0)
            currentHp = 0;
        else
            currentHp = newCurrentHp;

        currentHp = Mathf.Ceil(currentHp); // incase it becomes decimal

        float hpAmountAfterChange = currentHp;
        float hpChangeAmount = hpAmountBeforeChange - hpAmountAfterChange;

        OnHpChange?.Invoke(this, new HpChangeArgs
        {
            currentHp = currentHp,
            maxHp = MaxHp,
            hpChangeAmount = -hpChangeAmount,
        });

        if (currentHp <= 0)
            OnPlayerDeath?.Invoke(this, EventArgs.Empty);
    }

    public void ChangeCurrentArmor(float armorAmount)
    {
        if (armorAmount < 0 && invincible.GetInvincible())
            return;

        float armorAmountBeforeChange = currentArmor;

        if (armorAmount > 0)
        {
            if (armorAmount == MaxArmor)
            {
                return;
            }
            else if (armorAmount < MaxArmor)
            {
                if ((armorAmount + currentArmor) > MaxArmor)
                {
                    currentArmor = MaxArmor;
                }
                else if ((armorAmount + currentArmor) < MaxArmor)
                {
                    currentArmor += armorAmount;
                }
            }
        }
        else Debug.Log("Error");

        float armorAmountAfterChange = currentArmor;
        float armorChangeAmount = armorAmountBeforeChange - armorAmountAfterChange;

        OnArmorChange?.Invoke(this, new ArmorChangeArgs
        {
            currentArmor = currentArmor,
            armorChangeAmount = -armorChangeAmount,
        });
    }

    public void ChangeMoney(int moneyAmount)
    {
        if (moneyAmount > 0 && Money == maxMoneyAmount) // so does not invoke money change animation;
            return;

        int newMoneyAmount = moneyAmount;
        if (moneyAmount > 0)
        {
            float tempMoney = moneyAmount * MoneyGainPercentage / 100;
            newMoneyAmount += (int)Math.Ceiling(tempMoney);

            PlayerRecords.totalMoneyEarned += newMoneyAmount;
        }

        Money += newMoneyAmount;

        OnMoneyChange?.Invoke(this, new MoneyChangeArgs
        {
            currentMoney = Money,
            changeAmount = newMoneyAmount,
        });
    }
    #endregion

    #region Get
    public float GetCurrentArmor()
    {
        return currentArmor;
    }
    public float GetMaxArmor()
    {
        return MaxArmor;
    }
    public float GetCurrentHp()
    {
        return currentHp;
    }
    public int GetMoney()
    {
        return Money;
    }
    public int GetRifleMaxSlotNumber()
    {
        return rifleMaxSlotNumber;
    }
    public int GetGrenadeMaxSlotNumber()
    {
        return grenadeMaxSlotNumber;
    }
    public bool GetIsUsingRifle()
    {
        return isUsingRifle;
    }
    public bool GetIsInSmoke()
    {
        return isInSmoke;
    }
    #endregion

    #region Set
    void SetPlayerDead(bool b) => isPlayerDead = b;
    public void SetIsUsingRifle(bool b) => isUsingRifle = b;
    public void SetIsInSmokeTrue()
    {
        isInSmoke = true;
        smokeTimer = 0;

        AudioManager.Play2DSoundEffect(AudioManager.SoundEffect.PlayerSmoke, smokeSoundSource, 0.3f);
    }
    public void SetHealthRegeneration(bool b) => healthRegeneration.SetIsHeathRegenerationOn(b);
    public void SetArmorRegeneration(bool b) => armorRegeneration.SetIsArmorGenerationOn(b);
    public void SetInvincibleOn(bool b) => invincible.SetIsInvincibleOn(b);
    public void ReduceHealthRegenerationCoolDown(int reduceCoolDownBy) => healthRegeneration.ReduceRegenerationCoolDown(reduceCoolDownBy);
    public void ReduceArmorRegenerationCoolDown(int reduceCoolDownBy) => armorRegeneration.ReduceRegenerationCoolDown(reduceCoolDownBy);
    public void IncreaseHealthRegenerationAmount(int increaseAmount) => healthRegeneration.IncreaseHealthRegenerationAmount(increaseAmount);
    public void IncreaseArmorRegenerationAmount(int increaseAmount) => armorRegeneration.IncreaseArmorRegenerationAmount(increaseAmount);
    public void IncreaseInvincibleDuration(int durationAmount) => invincible.IncreaseInvincibleDuration(durationAmount);
    #endregion
}
