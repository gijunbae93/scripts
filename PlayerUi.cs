using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PlayerUi : MonoBehaviour
{
    #region interact
    TextMeshProUGUI playerInteractMessageTMPGUI;
    public Action<string> updateInteractMessage;
    #endregion

    public bool NeedToSkipUnpause { get; set; }
    

    PlayerStatusEffectUi playerStatusEffectUi;

    readonly Action<EffectUpgradeLevelUi, Transform, Transform, int> effectUpgradeLevelUiAction = (t1, v1, w1, x1) => t1.UpdateFireUpgardeUi(v1, w1, x1);

    [SerializeField] RifleTen rifleTen;

    // Start is called before the first frame update
    void Start()
    {
        #region SetActive
        UiAsset.Instance.GetMiniMap().SetActive(true);
        UiAsset.Instance.GetCrossHair().SetActive(true);
        UiAsset.Instance.GetZoom().SetActive(false);
        UiAsset.Instance.GetMoney().SetActive(true);
        UiAsset.Instance.GetBulletInfo().SetActive(true);
        UiAsset.Instance.GetPlayerInteractMessage().SetActive(true);
        UiAsset.Instance.GetHpAndAmor().SetActive(true);
        UiAsset.Instance.GetBloodOverLay().gameObject.SetActive(true);
        UiAsset.Instance.GetEffectUpgrade().SetActive(true);
        UiAsset.Instance.GetUpgrade().SetActive(true);
        UiAsset.Instance.GetStatusEffect().SetActive(true);
        UiAsset.Instance.GetRoundInfo().SetActive(true);
        UiAsset.Instance.GetGameOver().SetActive(false);
        UiAsset.Instance.GetLeaderBoard().SetActive(true);
        CanvasGroup leaderboardCanvasGroup = UiAsset.Instance.GetLeaderBoard().GetComponent<CanvasGroup>();
        UiAsset.Instance.GetESCMenu().SetActive(false);
        UiAsset.Instance.GetSnowGageUi().SetActive(true);
        #endregion

        playerInteractMessageTMPGUI = UiAsset.Instance.GetPlayerInteractMessage().GetComponent<TextMeshProUGUI>();
        updateInteractMessage = t => playerInteractMessageTMPGUI.text = t;

        UiAsset.Instance.GetBloodOverLay().color = new Color(255f, 255f, 255f, 0); // trasnparent at beginning

        PlayerStats.Instance.OnHpChange += Instance_OnHpChange;
        PlayerStats.Instance.OnArmorChange += Instance_OnArmorChange;
        PlayerStats.Instance.OnMoneyChange += Instance_OnMoneyChange;

        //NextFrame.Create(() => UpdateAmmoTMPText());             // update ammo Ui after one frame from start, give time for rifle to initalize at start function
        UpdateArmorTMPText(0);
        UpdateHpTMPText(100);
        UpdatePlayerMoneyTMPText(1200);

        NextFrame.Create(() =>  // ui update needs to run after upgrade level has been updated
        {
            FireButton.OnBurnUpgrade += FireButton_OnFireDamageUpgrade;
            ElectrocuteButton.OnElectrocuteUpgrade += ElectrocuteButton_OnElectrocuteUpgrade;
            PoisonGrenadePurchase.OnPoisonUpgrade += PoisonGrenadePurchase_OnPoisonUpgrade;
            ExplosiveGrenadePurchase.OnExplosionUpgrade += ExplosiveGrenadePurchase_OnExplosionUpgrade;
        });

        playerStatusEffectUi = new PlayerStatusEffectUi(UiAsset.Instance.GetStatusEffectContainer(), UiAsset.Instance.GetStatusEffectUi());

        PlayerStats.Instance.OnPlayerDeath += Instance_OnPlayerDeath;

        rifleTen.OnZoom += RifleTen_OnZoom;
    }

    private void OnDestroy()
    {
        FireButton.OnBurnUpgrade -= FireButton_OnFireDamageUpgrade;
        ElectrocuteButton.OnElectrocuteUpgrade -= ElectrocuteButton_OnElectrocuteUpgrade;
        PoisonGrenadePurchase.OnPoisonUpgrade -= PoisonGrenadePurchase_OnPoisonUpgrade;
        ExplosiveGrenadePurchase.OnExplosionUpgrade -= ExplosiveGrenadePurchase_OnExplosionUpgrade;
    }

    private void RifleTen_OnZoom(object sender, RifleTen.ZoomArgs e)
    {
        Action<bool> OnZoom = (t) =>
        {
            UiAsset.Instance.GetCrossHair().SetActive(!t);
            UiAsset.Instance.GetZoom().SetActive(t);
        };

        if (e.isZoomed)
            OnZoom(true);
        else
            OnZoom(false);
    }

    private void Instance_OnPlayerDeath(object sender, EventArgs e)
    {
        DisplayUi(); // incase player dies during camera animatino;

        UiAsset.Instance.GetBloodOverLay().gameObject.SetActive(true);

        UiAsset.Instance.GetMiniMap().SetActive(false);
        UiAsset.Instance.GetCrossHair().SetActive(false);
        UiAsset.Instance.GetZoom().SetActive(false);
        UiAsset.Instance.GetPlayerInteractMessage().SetActive(false);
        UiAsset.Instance.GetHpAndAmor().SetActive(false);
        UiAsset.Instance.GetEffectUpgrade().SetActive(false);
        UiAsset.Instance.GetStatusEffect().SetActive(false);
        UiAsset.Instance.GetRoundInfo().SetActive(false);
        UiAsset.Instance.GetSnowGageUi().SetActive(false);

        FunctionTimer.Create(() => UiAsset.Instance.GetGameOver().SetActive(true), 3f);
    }

    private void PoisonGrenadePurchase_OnPoisonUpgrade(object sender, EventArgs e) => effectUpgradeLevelUiAction(new EffectUpgradeLevelUi(), UiAsset.Instance.GetPoisonSymbolContainerTransform(),
        UiAsset.Instance.GetPoisonSymbolTransform(), Poison.Instance.CurrentLevel);
    private void ExplosiveGrenadePurchase_OnExplosionUpgrade(object sender, EventArgs e) => effectUpgradeLevelUiAction(new EffectUpgradeLevelUi(), UiAsset.Instance.GetExplosionSymbolContainerTransform(),
        UiAsset.Instance.GetExplosionSymbolTransform(), Explosion.Instance.CurrentLevel);
    void ElectrocuteButton_OnElectrocuteUpgrade(object sender, EventArgs e) => effectUpgradeLevelUiAction(new EffectUpgradeLevelUi(), UiAsset.Instance.GetElectrocuteSymbolContainerTransform(), 
        UiAsset.Instance.GetElectrocuteSymbolTransform(), Electricity.Instance.CurrentLevel);
    void FireButton_OnFireDamageUpgrade(object sender, EventArgs e) => effectUpgradeLevelUiAction(new EffectUpgradeLevelUi(), UiAsset.Instance.GetFireSymbolContainerTransform(), 
        UiAsset.Instance.GetFireSymbolTransform(), Fire.Instance.CurrentLevel);

    private void Instance_OnMoneyChange(object sender, PlayerStats.MoneyChangeArgs e)
    {
        UpdatePlayerMoneyTMPText(e.currentMoney);
        InstantiateMoneyChangeText(e.changeAmount);
    }

    private void Instance_OnArmorChange(object sender, PlayerStats.ArmorChangeArgs e)
    {
        UpdateArmorTMPText(e.currentArmor);
        InstantiateArmorChangeText(e.armorChangeAmount);
    }

    private void Instance_OnHpChange(object sender, PlayerStats.HpChangeArgs e)
    {
        UpdateHpTMPText(e.currentHp);
        InstantiateHpChangeText(e.hpChangeAmount);
        SetBloodOverLayTransparency(e.currentHp, e.maxHp);
    }

    private void Update()
    {
        OnEscPress();
    }

    #region Ammo Infomation
    public void UpdateAmmoTMPText()
    {
        if (PlayerStats.Instance.GetIsUsingRifle())
        {
            Rifles rifle = RifleInventory.Instance.GetActiveRifle().GetRifles();
            if (rifle != null)
            {
                string currentAmmoString;
                string maxAmmoString;

                currentAmmoString = rifle.GetCurrentAmmo().ToString();
                maxAmmoString = rifle.GetMaxAmmo().ToString();

                UiAsset.Instance.GetAmmoTMPUGUI().text = currentAmmoString + "/" + maxAmmoString;
                UiAsset.Instance.GetAmmoImage().sprite = UiAsset.Instance.GetBulletSprite();
            }
            else Debug.Log("Error");
        }
        else
        {
            int grenadeStorageNumber = GrenadeInventory.Instance.GetGrenadeQueue().Count - 1;

            UiAsset.Instance.GetAmmoTMPUGUI().text = 1 + "/" + grenadeStorageNumber;
            UiAsset.Instance.GetAmmoImage().sprite = UiAsset.Instance.GetGrenadeSprite();
        }
    }

    #endregion

    #region Hp And Armor information
    public void UpdateHpTMPText(float currentHp)
    {
        string currentHpString = currentHp.ToString();

        UiAsset.Instance.GetHpTMPUGUI().text = currentHpString;
    }
    void InstantiateHpChangeText(float hpChangeAmount)
    {
        RectTransform hpChangeTextRectTransform = Instantiate(UiAsset.Instance.GetHpChangeTMPTransform(), UiAsset.Instance.GetHpChangeContainerTransform()).GetComponent<RectTransform>();
        hpChangeTextRectTransform.gameObject.SetActive(true);

        HpAndArmorChangeText hpAndRmorChangeText = hpChangeTextRectTransform.GetComponent<HpAndArmorChangeText>();
        hpAndRmorChangeText.SetText(hpChangeAmount);

    }
    public void UpdateArmorTMPText(float currentArmor)
    {
        string currentArmorString = currentArmor.ToString();

        UiAsset.Instance.GetArmorTMPUGUI().text = currentArmorString;
    }
    void InstantiateArmorChangeText(float armorChangeAmount)
    {
        RectTransform armorChangeTextRectTransform = Instantiate(UiAsset.Instance.GetArmorChangeTMPTransform(), UiAsset.Instance.GetArmorChangeContinerTransform()).GetComponent<RectTransform>();
        armorChangeTextRectTransform.gameObject.SetActive(true);

        HpAndArmorChangeText hpAndArmorChangeText = armorChangeTextRectTransform.GetComponent<HpAndArmorChangeText>();
        hpAndArmorChangeText.SetText(armorChangeAmount);
    }
    void SetBloodOverLayTransparency(float currentHp, float maxHp)
    {
        float currentHpPercentage = (currentHp / maxHp) * 100;

        if(currentHpPercentage > 40)
        {
            UiAsset.Instance.GetBloodOverLay().color = new Color(255f, 255f, 255f, 0);     // invisible
        }
        else
        {
            float transparency, startValue = 1f, endValue = 0f;
            transparency = Mathf.Lerp(startValue, endValue, currentHpPercentage / 40);

            UiAsset.Instance.GetBloodOverLay().color = new Color(255f, 255f, 255f, transparency);
        }
    }

    #endregion

    #region Player Money Information
    public void UpdatePlayerMoneyTMPText(int currentMoney)
    {
        UiAsset.Instance.GetPlayerMoneyTMPUGUI().text = currentMoney.ToString() + "$";
    }
    void InstantiateMoneyChangeText(float moneyChangeAmount)
    {
        RectTransform moneyChangeTextRectTransform = Instantiate(UiAsset.Instance.GetMoneyChangeTMPTransform(), UiAsset.Instance.GetMoneyChangeContainerTransform()).GetComponent<RectTransform>();
        moneyChangeTextRectTransform.gameObject.SetActive(true);

        HpAndArmorChangeText moneyChangeText = moneyChangeTextRectTransform.GetComponent<HpAndArmorChangeText>();
        moneyChangeText.SetText(moneyChangeAmount);
    }
    #endregion

    #region PlayerUpgrade
    public void OnUpgrade(HashSet<Upgrade> upgradeHashSet)
    {
        PlayerUpgradeUi playerUpgradeUi = new PlayerUpgradeUi(UiAsset.Instance.GetUpgradeContainer(), UiAsset.Instance.GetUpgradeUi(), this);
        playerUpgradeUi.ActivateUpgradeUi(upgradeHashSet);
    }
    #endregion

    #region PlayerStatusEffect
    public void AddPlayerStatusEffectList(PlayerStatusEffect playerStatusEffect, float totalDuration)
    {
        playerStatusEffectUi.AddPlayerStatusEffectUi(playerStatusEffect, totalDuration);
    }

    public void RemovePlayerStatusEffectList(PlayerStatusEffect playerStatusEffect)
    {
        playerStatusEffectUi.RemovePlayerStatusEffectUi(playerStatusEffect);
    }
    #endregion

    #region EscMenu
    void OnEscPress()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (UiAsset.Instance.GetESCMenu().activeSelf)
                UiAsset.Instance.GetESCMenu().SetActive(false);
            else
                UiAsset.Instance.GetESCMenu().SetActive(true);
        }
    }
    #endregion

    #region Set
    public void SetRoundInfoTMPText(int roundNumber)
    {
        UiAsset.Instance.GetRoundInfoTMPUGUI().text = roundNumber.ToString();
    }
    #endregion

    class EffectUpgradeLevelUi
    {
        public void UpdateFireUpgardeUi(Transform SymbolContainerTransform, Transform SymbolTransform, int currentUpgradeLevel)
        {
            foreach (Transform symbol in SymbolContainerTransform)
            {
                if (symbol == SymbolTransform) continue;

                Destroy(symbol.gameObject);
            }

            int x = 0;
            int y = 0;
            float SymbolSize = 33f;

            int numberOfUi = currentUpgradeLevel + 1;
            for (int i = 0; i < numberOfUi; i++)
            {
                x = i;

                RectTransform fireUpgradeSymbolUiRectTransform = Instantiate(SymbolTransform, SymbolContainerTransform).GetComponent<RectTransform>();
                fireUpgradeSymbolUiRectTransform.gameObject.SetActive(true);
                fireUpgradeSymbolUiRectTransform.anchoredPosition = new Vector2(x * -SymbolSize, y * SymbolSize);  // watch out for negative sign
            }
        }
    }

    class PlayerUpgradeUi
    {
        Transform upgradeContainer;
        Transform upgradeUi;
        PlayerUi playerUi;
        public PlayerUpgradeUi(Transform upgradeContainer, Transform upgradeUi, PlayerUi playerUi)
        {
            this.upgradeContainer = upgradeContainer;
            this.upgradeUi = upgradeUi;
            this.playerUi = playerUi;
        }

        void OnUpgradeComplete()
        {
            playerUi.NeedToSkipUnpause = false;

            upgradeContainer.gameObject.SetActive(false);

            foreach (Transform upgradeUi in upgradeContainer)
            {
                if (upgradeUi == this.upgradeUi) continue;

                Destroy(upgradeUi.gameObject);
            }

            PauseManager.Instance.UnPause();
        }

        public void ActivateUpgradeUi(HashSet<Upgrade> upgradeHashset)
        {
            int y = 0;
            float upgradeUiSizeY = 186f;
            int yPosAtStart = 100;
            int spaceBetweenCell = 10;

            upgradeContainer.gameObject.SetActive(true);
            AudioManager.PlayUiSoundEffect(AudioManager.UiSoundEffect.UpgradePopUp);
            foreach (Upgrade upgrade in upgradeHashset)
            {
                RectTransform upgradeRectransform = Instantiate(upgradeUi, upgradeContainer).GetComponent<RectTransform>();
                upgradeRectransform.gameObject.SetActive(true);
                if(y > 0)
                    upgradeRectransform.anchoredPosition = new Vector2(0, -y * upgradeUiSizeY + yPosAtStart - spaceBetweenCell); //only changing Y and watch out negative sign
                else
                    upgradeRectransform.anchoredPosition = new Vector2(0, -y * upgradeUiSizeY + yPosAtStart); //only changing Y and watch out negative sign

                Image iconImage = upgradeRectransform.Find("IconBackground(Image)").Find("Icon(Image)").GetComponent<Image>();
                iconImage.sprite = upgrade.GetUpgradeSprite();

                TextMeshProUGUI descriptText = upgradeRectransform.Find("DescriptionText (TMP)").GetComponent<TextMeshProUGUI>();
                descriptText.text = upgrade.GetUpgradeDescription();

                TextMeshProUGUI titleText = upgradeRectransform.Find("TitleText (TMP)").GetComponent<TextMeshProUGUI>();
                titleText.text = upgrade.GetUpgradeTitle();

                TextMeshProUGUI upgradeLevelInfoText = upgradeRectransform.Find("UpgradeLevelInfo (TMP)").GetComponent<TextMeshProUGUI>();
                if (upgrade.GetUpgradeLevelInfo() == 0)
                    upgradeLevelInfoText.text = "New!";
                else
                    upgradeLevelInfoText.text = upgrade.GetUpgradeLevelInfo().ToString();

                Button upgradeButton = upgradeRectransform.Find("Button").GetComponent<Button>();
                upgradeButton.onClick.AddListener(() =>
                {
                    AudioManager.PlayUiSoundEffect(AudioManager.UiSoundEffect.Click);
                    upgrade.UpgradePlayer();
                    OnUpgradeComplete();
                });

                y++;
            }
        }
    }

    class PlayerStatusEffectUi
    {
        Transform statusEffectContainer;
        Transform statusEffectUi;

        const int cellSize = 30;

        List<RectTransform> rectTransformList = new List<RectTransform>();
        List<PlayerStatusEffect> playerStatusEffectList = new List<PlayerStatusEffect>();
        public PlayerStatusEffectUi(Transform statusEffectContainer, Transform statusEffectUi)
        {
            this.statusEffectContainer = statusEffectContainer;
            this.statusEffectUi = statusEffectUi;
        }

        public void AddPlayerStatusEffectUi(PlayerStatusEffect playerStatusEffect, float totalDuration)
        {
            for (int i = 0; i < playerStatusEffectList.Count; i++)
            {
                if(playerStatusEffectList[i] == playerStatusEffect)
                {
                    RemovePlayerStatusEffectUi(playerStatusEffect);
                }
            }

            RectTransform statusEffectUiRectTransform = Instantiate(statusEffectUi, statusEffectContainer).GetComponent<RectTransform>();
            statusEffectUiRectTransform.gameObject.SetActive(true);
            statusEffectUiRectTransform.anchoredPosition = new Vector2(rectTransformList.Count * cellSize, 0);

            Image image = statusEffectUiRectTransform.Find("IconImage(Image)").GetComponent<Image>();
            image.sprite = playerStatusEffect.GetSpirte();

            playerStatusEffect.Add(image, totalDuration);

            rectTransformList.Add(statusEffectUiRectTransform);
            playerStatusEffectList.Add(playerStatusEffect);
        }

        public void RemovePlayerStatusEffectUi(PlayerStatusEffect playerStatusEffect)
        {
            int index = 0;
            bool found = false;
            for (int i = 0; i < playerStatusEffectList.Count; i++)
            {
                if (playerStatusEffectList[i] == playerStatusEffect)
                {
                    index = i;
                    found = true;
                }
            }
            if (!found) return; // this will only run upon player's death

            RectTransform tempRectTransform = rectTransformList[index];
            rectTransformList.RemoveAt(index);
            Destroy(tempRectTransform.gameObject);

            PlayerStatusEffect tempPlayerStatusEffect = playerStatusEffectList[index];
            playerStatusEffectList.RemoveAt(index);
            tempPlayerStatusEffect.Remove();

            for (int i = index; i < rectTransformList.Count; i++)
            {
                rectTransformList[i].anchoredPosition = new Vector2(i * cellSize, 0);
            }
        }
    }
}
