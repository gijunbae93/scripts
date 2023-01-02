using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class AudioManager
{
    static Dictionary<SoundEffect, float> updateSoundEffectTimerDictionary = new Dictionary<SoundEffect, float>();

    public static Dictionary<SoundEffect, float> soundDelayTimerDictionary { get; private set; }
    static HashSet<SoundDelayTimerMonobehaviour> soundDelayTimerMonobehaviourHashset = new HashSet<SoundDelayTimerMonobehaviour>();
    static GameObject checkIfSoundDelayGameObjectIsPresent;

    class SoundDelayTimerMonobehaviour : MonoBehaviour
    {
        bool startDelayTimer = false;
        bool isDelayed = true;
        float delayTimer;
        float soundEffectDelayTimer;
        float lastTimePlayed;
        float soundEffectUpdateTimer;
        AudioManager.SoundEffect soundEffectType;

        public void Initalize(float soundEffectUpdateTimer, float soundEffectDelayTimer, AudioManager.SoundEffect soundEffectType)
        {
            this.soundEffectDelayTimer = soundEffectDelayTimer;
            this.soundEffectType = soundEffectType;
            this.soundEffectUpdateTimer = soundEffectUpdateTimer;
        }

        private void Update()
        {
            if (isDelayed && startDelayTimer)
            {
                if(delayTimer + soundEffectDelayTimer <= Time.time)
                {
                    isDelayed = false;
                    startDelayTimer = false;
                    lastTimePlayed = Time.time;
                }
            }
            else if(!isDelayed)
            {
                if(Time.time >= lastTimePlayed + soundEffectUpdateTimer + 0.1f)
                {
                    isDelayed = true;
                }
            }
        }

        public bool GetIsDelayed()
        {
            return isDelayed;
        }
        public bool GetStartDelayTimer()
        {
            return startDelayTimer;
        }
        public AudioManager.SoundEffect GetSoundEffectType()
        {
            return soundEffectType;
        }
        public void SetLastTimePlayed(float lastTimePlayed) => this.lastTimePlayed = lastTimePlayed;
        public void SetStartDelayTimer(bool b)
        {
            startDelayTimer = b;
            delayTimer = Time.time;
        }
    }

    static Dictionary<BackGroundMusic, float> backGroundMusicVolumeDictionary = new Dictionary<BackGroundMusic, float>();
    static Dictionary<UiSoundEffect, float> uiSoundEffectVolumeDictionary = new Dictionary<UiSoundEffect, float>();

    public enum SoundEffect
    {
        PlayerWalk,
        PlayerLand,
        PlayerRun,
        PlayerJump, // 2d
        PlayerGetHit, // 2d
        PlayerBurn, // 2d
        PlayerPoison, // 2d
        PlayerElectrocute, // 2d
        PlayerSmoke, // 2d
        PlayerExplosionHit, // 2d
        PlayerInvincible, // 2d
        PlayerInstantKill, // 2d
        RifleSwapSound, // 2d
        RifleOneFire, // 2d
        RifleReloadOne, // 2d
        RifleReloadTwo, // 2d
        RifleReloadThree, // 2d
        RifleTwoFire, // 2d
        ShotGunReload, // 2d
        ShotGunReloadTwo, // 2d
        RifleFourFire, // 2d
        RifleFourReloadOne, // 2d
        RifleFourReloadTwo, // 2d
        RifleFourReloadThree, // 2d
        RifleFiveFire, // 2d
        RifleFiveDuringReload, // 2d
        RifleSixFire, // 2d
        RifleSixReloadOne, // 2d
        RifleSixReloadTwo, // 2d
        RifleNineFire, // 2d
        RifleNineReloadOne, // 2d
        RifleNineReloadTwo, // 2d
        GrenadeThrowOne, // 2d
        GrenadeThrowTwo, // 2d
        CenterRifleFire,
        FireGrenade,
        GasLeak,
        Purchase, // 2d
        Explosion,
        PickUp, // 2d
        Electricity2D, // 2d
        Electricity3D,
        FireCollider2D, // 2d
        FireCollider3D,
        Electricity2DTwo, // 2d
        Electricity3DTwo,
        Electricity2DThree, // 2d
        Electricity3DThree,
        EnemyGroan,
        EnemyOneAttackTwo,
        EnemyFall,
        EnemyTwoAttackOne1,
        EnemyTwoAttackOne2,
        EnemyThreeAttackOneBody,
        EnemyThreeAttackOneArm,
        DoorDragSoundEffectOne,
        DoorDragSoundEffectTwo,
        DoorDragSoundEffectThree,
        DoorDragSoundEffectFour,
        PlayerHeartBeat,    // 2d
        WeaponBox,
        WeaponBoxOpen,
        WeaponBoxClose,
        WeaponBoxBlankOne,
        WeaponBoxBlankTwo,
        WeaponBoxBlankThree,
        WeaponBoxVanish,
        EnemyBulletHit,
        EnemyDeath,
        InventoryReload,   // 2d
        UnRestriction,  // 2d
        PurchaseError, // 2d
        SniperZoomOut, // 2d
        SniperZoomIn, // 2d
        RifleTenFire, //2d
        RifleTenReload, //2d
        WeaponBoxJump,
        CinematicWhoosh, // 2d
        FireSkillCast, // 2d
        Poison,
        PoisonSkillCast, // 2d
        coldExhale, // 2d
        Infection, // 2d
    }
    public enum UiSoundEffect
    {
        GameOver,
        PopUp,
        Click,
        RoundChange,
        UpgradePopUp,
    }
    public enum BackGroundMusic
    {
        One,
        Two,
        Three,
    }
    public static void Initalize()
    {
        SetUpdateSoundEffectTimer();

        soundDelayTimerDictionary = new Dictionary<SoundEffect, float>();
        SetSoundDelayTimer();

        SetBackGroundMusicVolume();

        SetUiSoundEffectVolume();
    }

    static void SetUpdateSoundEffectTimer()
    {
        float initalValue = -999f;

        updateSoundEffectTimerDictionary[SoundEffect.PlayerWalk] = initalValue;
        updateSoundEffectTimerDictionary[SoundEffect.PlayerRun] = initalValue;
        updateSoundEffectTimerDictionary[SoundEffect.PlayerBurn] = initalValue;
        updateSoundEffectTimerDictionary[SoundEffect.PlayerPoison] = initalValue;
        updateSoundEffectTimerDictionary[SoundEffect.PlayerElectrocute] = initalValue;
        updateSoundEffectTimerDictionary[SoundEffect.PlayerSmoke] = initalValue;
        updateSoundEffectTimerDictionary[SoundEffect.FireCollider2D] = initalValue;
        updateSoundEffectTimerDictionary[SoundEffect.FireCollider3D] = initalValue;
        updateSoundEffectTimerDictionary[SoundEffect.Electricity2D] = initalValue;
        updateSoundEffectTimerDictionary[SoundEffect.Electricity3D] = initalValue;
        updateSoundEffectTimerDictionary[SoundEffect.Electricity2DTwo] = initalValue;
        updateSoundEffectTimerDictionary[SoundEffect.Electricity3DTwo] = initalValue;
        updateSoundEffectTimerDictionary[SoundEffect.Electricity2DThree] = initalValue;
        updateSoundEffectTimerDictionary[SoundEffect.Electricity3DThree] = initalValue;
        updateSoundEffectTimerDictionary[SoundEffect.DoorDragSoundEffectOne] = initalValue;
        updateSoundEffectTimerDictionary[SoundEffect.DoorDragSoundEffectTwo] = initalValue;
        updateSoundEffectTimerDictionary[SoundEffect.DoorDragSoundEffectThree] = initalValue;
        updateSoundEffectTimerDictionary[SoundEffect.DoorDragSoundEffectFour] = initalValue;
        updateSoundEffectTimerDictionary[SoundEffect.PlayerHeartBeat] = initalValue;
        updateSoundEffectTimerDictionary[SoundEffect.Infection] = initalValue;
    }

    static void SetSoundDelayTimer()
    {
        soundDelayTimerDictionary[SoundEffect.PlayerWalk] = 0.3f;
        soundDelayTimerDictionary[SoundEffect.PlayerRun] = 0.15f;
        soundDelayTimerDictionary[SoundEffect.PlayerBurn] = 0.15f;
        soundDelayTimerDictionary[SoundEffect.PlayerPoison] = 0.15f;
        soundDelayTimerDictionary[SoundEffect.PlayerElectrocute] = 0.15f;
        soundDelayTimerDictionary[SoundEffect.PlayerSmoke] = 0.15f;
        soundDelayTimerDictionary[SoundEffect.PlayerHeartBeat] = 1.5f;
        soundDelayTimerDictionary[SoundEffect.Infection] = 0.15f;

        InitializeSoundDelayGameObjects();
    }
    public static void InitializeSoundDelayGameObjects() // since gameobject gets destroyed i need to initialize in main scene again
    {
        soundDelayTimerMonobehaviourHashset.Clear();
        checkIfSoundDelayGameObjectIsPresent = new GameObject("CheckIfSoundDelayGameObjectIsPresent");
        foreach (SoundEffect soundEffect in soundDelayTimerDictionary.Keys)
        {
            GameObject gameObject = new GameObject(soundEffect.ToString(), typeof(SoundDelayTimerMonobehaviour));
            SoundDelayTimerMonobehaviour soundDelayTimerMonobehaviour = gameObject.GetComponent<SoundDelayTimerMonobehaviour>();
            soundDelayTimerMonobehaviour.Initalize(GetUpdateSoundEffectTimer(soundEffect), soundDelayTimerDictionary[soundEffect], soundEffect);
            soundDelayTimerMonobehaviour.transform.SetParent(checkIfSoundDelayGameObjectIsPresent.transform);
            soundDelayTimerMonobehaviourHashset.Add(soundDelayTimerMonobehaviour);
        }
    }
    static void SetBackGroundMusicVolume()
    {
        backGroundMusicVolumeDictionary[BackGroundMusic.One] = 0.25f;
        backGroundMusicVolumeDictionary[BackGroundMusic.Two] = 0.25f;
        backGroundMusicVolumeDictionary[BackGroundMusic.Three] = 0.2f;
    }
    static void SetUiSoundEffectVolume()
    {
        uiSoundEffectVolumeDictionary[UiSoundEffect.GameOver] = 0.5f;
        uiSoundEffectVolumeDictionary[UiSoundEffect.PopUp] = 0.5f;
        uiSoundEffectVolumeDictionary[UiSoundEffect.Click] = 0.5f;
        uiSoundEffectVolumeDictionary[UiSoundEffect.RoundChange] = 1f;
        uiSoundEffectVolumeDictionary[UiSoundEffect.UpgradePopUp] = 0.5f;
    }


    public static void Play2DSoundEffect(SoundEffect soundEffect, GameObject soundSourceGameObject, float volume = 0.5f)
    {
        if (NeedDelay(soundEffect))
            return;

        if (!CanPlaySoundEffect(soundEffect))
            return;

        Action<AudioSource> OnSoundEffectPlay = (t) =>
        {
            t.volume = volume;

            t.minDistance = 500f;
            t.maxDistance = 500f;
            t.spatialBlend = 0f;

            t.outputAudioMixerGroup = AudioAsset.Instance.audioMixer.FindMatchingGroups(AudioAsset.Instance.soundEffectMixerString)[1];
            t.PlayOneShot(GetAudioClip(soundEffect));
        };

        if (soundSourceGameObject.transform.TryGetComponent<AudioSource>(out AudioSource audioSource))
        {
            OnSoundEffectPlay(audioSource);
        }
        else
        {
            AudioSource gameObjectAudioSource = soundSourceGameObject.AddComponent<AudioSource>();
            OnSoundEffectPlay(gameObjectAudioSource);
        }

        if (soundDelayTimerDictionary.ContainsKey(soundEffect))
            foreach (SoundDelayTimerMonobehaviour soundDelayTimerMonobehaviour in soundDelayTimerMonobehaviourHashset)
                if (soundDelayTimerMonobehaviour.GetSoundEffectType() == soundEffect)
                    soundDelayTimerMonobehaviour.SetLastTimePlayed(Time.time);
    }
    public static void Play3DSoundEffect(SoundEffect soundEffect, GameObject soundSourceGameObject, float volume = 0.5f)
    {
        if (NeedDelay(soundEffect))
            return;

        if (!CanPlaySoundEffect(soundEffect))
            return;

        AudioSource audioSource = soundSourceGameObject.transform.GetComponent<AudioSource>();
        if (audioSource)
        {
            audioSource.volume = volume;

            audioSource.spatialBlend = 1f;

            audioSource.outputAudioMixerGroup = AudioAsset.Instance.audioMixer.FindMatchingGroups(AudioAsset.Instance.soundEffectMixerString)[1];
            audioSource.PlayOneShot(GetAudioClip(soundEffect));
        }
        else
        {
            Debug.Log("Error"); // 3d sound effect must have audioSource, set 3d sound graph
            return;
        }


        if (soundDelayTimerDictionary.ContainsKey(soundEffect))
            foreach (SoundDelayTimerMonobehaviour soundDelayTimerMonobehaviour in soundDelayTimerMonobehaviourHashset)
                if (soundDelayTimerMonobehaviour.GetSoundEffectType() == soundEffect)
                    soundDelayTimerMonobehaviour.SetLastTimePlayed(Time.time);
    }
    public static void StopPlayingSoundEffect(SoundEffect soundEffect, GameObject soundSourceGameObject)
    {
        if (soundSourceGameObject.transform.TryGetComponent<AudioSource>(out AudioSource audioSource))
        {
            audioSource.Stop();
        }
        else
        {
            Debug.Log(soundEffect.ToString() + " Error");
            return;
        }

        float resetTimer = -999f;

        if (updateSoundEffectTimerDictionary.ContainsKey(soundEffect))
            updateSoundEffectTimerDictionary[soundEffect] = resetTimer;

        if (checkIfSoundDelayGameObjectIsPresent == null)
        {
            Debug.Log("Error");
            InitializeSoundDelayGameObjects();
        }
        if (soundDelayTimerDictionary.ContainsKey(soundEffect))
            foreach (SoundDelayTimerMonobehaviour soundDelayTimerMonobehaviour in soundDelayTimerMonobehaviourHashset)
                if (soundDelayTimerMonobehaviour.GetSoundEffectType() == soundEffect)
                    soundDelayTimerMonobehaviour.SetLastTimePlayed(resetTimer);
    }
    static float GetUpdateSoundEffectTimer(SoundEffect soundEffect)
    {
        switch (soundEffect)
        {
            default:
                Debug.Log("Error");
                return 0f;
            case SoundEffect.PlayerWalk: return 0.7f;
            case SoundEffect.PlayerRun: return 0.5f;
            case SoundEffect.PlayerBurn: return 54f;
            case SoundEffect.PlayerPoison: return 1.3f;
            case SoundEffect.PlayerElectrocute: return 10f;
            case SoundEffect.PlayerSmoke: return 1.2f;
            case SoundEffect.FireCollider2D: return 35f;
            case SoundEffect.FireCollider3D: return 35f;
            case SoundEffect.Electricity2D: return 27f;
            case SoundEffect.Electricity3D: return 27f;
            case SoundEffect.Electricity2DTwo: return 27f;
            case SoundEffect.Electricity3DTwo: return 27f;
            case SoundEffect.Electricity2DThree: return 27f;
            case SoundEffect.Electricity3DThree: return 27f;
            case SoundEffect.DoorDragSoundEffectOne: return 8f;
            case SoundEffect.DoorDragSoundEffectTwo: return 8f;
            case SoundEffect.DoorDragSoundEffectThree: return 8f;
            case SoundEffect.DoorDragSoundEffectFour: return 8f;
            case SoundEffect.PlayerHeartBeat: return 10.5f;
            case SoundEffect.Infection: return 26f;
        }
    }
    static bool CanPlaySoundEffect(SoundEffect soundEffect)
    {
        if (updateSoundEffectTimerDictionary.ContainsKey(soundEffect))
        {
            float lastTimePlayed = updateSoundEffectTimerDictionary[soundEffect];
            float playSoundEffectAfter = GetUpdateSoundEffectTimer(soundEffect);
            if (playSoundEffectAfter + lastTimePlayed < Time.time)
            {
                updateSoundEffectTimerDictionary[soundEffect] = Time.time;
                return true;
            }
            else
                return false;
        }
        else
            return true;
    }
    static bool NeedDelay(SoundEffect soundEffect)
    {
        if (!soundDelayTimerDictionary.ContainsKey(soundEffect))
            return false;

        if (checkIfSoundDelayGameObjectIsPresent == null)
        {
            InitializeSoundDelayGameObjects();
            return true;
        }

        foreach (SoundDelayTimerMonobehaviour soundDelayTimerMonobehaviour in soundDelayTimerMonobehaviourHashset)
        {
            if (soundDelayTimerMonobehaviour.GetSoundEffectType() == soundEffect)
            {
                if (soundDelayTimerMonobehaviour.GetIsDelayed() && !soundDelayTimerMonobehaviour.GetStartDelayTimer())
                {
                    soundDelayTimerMonobehaviour.SetStartDelayTimer(true);
                    return true;
                }
                else if (soundDelayTimerMonobehaviour.GetIsDelayed())
                {
                    return true;
                }
                else if (!soundDelayTimerMonobehaviour.GetIsDelayed())
                {
                    return false;
                }
            }
        }

        Debug.Log("Error");
        return false;
    }

    public static void PlayUiSoundEffect(UiSoundEffect uiSoundEffect)
    {
        AudioAsset.Instance.uiSoundEffectAudioSource.volume = uiSoundEffectVolumeDictionary[uiSoundEffect];

        AudioAsset.Instance.uiSoundEffectAudioSource.spatialBlend = 0f;

        AudioAsset.Instance.uiSoundEffectAudioSource.outputAudioMixerGroup = AudioAsset.Instance.audioMixer.FindMatchingGroups(AudioAsset.Instance.uiSoundEffectMixerString)[0];
        AudioAsset.Instance.uiSoundEffectAudioSource.PlayOneShot(GetAudioClip(uiSoundEffect));

        Debug.Log(uiSoundEffect);
    }

    public static void PlayBackGroundMusic(BackGroundMusic backGroundMusic)
    {
        AudioAsset.Instance.backGroundMusicAudioSource.volume = backGroundMusicVolumeDictionary[backGroundMusic];

        AudioAsset.Instance.backGroundMusicAudioSource.spatialBlend = 0f;

        AudioAsset.Instance.backGroundMusicAudioSource.clip = GetAudioClip(backGroundMusic);

        AudioAsset.Instance.backGroundMusicAudioSource.outputAudioMixerGroup = AudioAsset.Instance.audioMixer.FindMatchingGroups(AudioAsset.Instance.backGroundMusicMixerString)[0];

        AudioAsset.Instance.backGroundMusicAudioSource.Play();

        Debug.Log(AudioAsset.Instance.backGroundMusicAudioSource.clip.name);
        Debug.Log(backGroundMusic);

        float delayBetweenBackGroundMusic = 15f;
        UnscaleFunctionTimer.Create(() => BackGroundManager.PlayBackGroundMusic(), delayBetweenBackGroundMusic + AudioAsset.Instance.backGroundMusicAudioSource.clip.length);
    }

    static AudioClip GetAudioClip(SoundEffect soundEffect)
    {
        foreach (SoundEffectAudioClip soundEffectAudioClip in SoundAsset.Instance.soundEffectAudioClip)
        {
            if (soundEffectAudioClip.soundEffect == soundEffect)
                return soundEffectAudioClip.audioClip;
        }
        Debug.Log("Sound " + soundEffect + " Not Found");
        return null;
    }
    static AudioClip GetAudioClip(UiSoundEffect uiSoundEffect)
    {
        foreach (UiSoundEffectAudioClip uiSoundEffectAudioClip in SoundAsset.Instance.uiSoundEffectAudioClip)
        {
            if (uiSoundEffectAudioClip.uiSoundEffect == uiSoundEffect)
                return uiSoundEffectAudioClip.audioClip;
        }
        Debug.Log("Sound " + uiSoundEffect + " Not Found");
        return null;
    }
    static AudioClip GetAudioClip(BackGroundMusic backGroundMusic)
    {
        foreach (BackGroundMusicAudioClip backGroundMusicAudioClip in SoundAsset.Instance.backGroundMusicAudioClip)
        {
            if (backGroundMusicAudioClip.backGroundMusic == backGroundMusic)
                return backGroundMusicAudioClip.audioClip;
        }
        Debug.Log("Sound " + backGroundMusic + " Not Found");
        return null;
    }
}

[Serializable]
public class SoundEffectAudioClip
{
    public AudioManager.SoundEffect soundEffect;
    public AudioClip audioClip;
}
[Serializable]
public class UiSoundEffectAudioClip
{
    public AudioManager.UiSoundEffect uiSoundEffect;
    public AudioClip audioClip;
}
[Serializable]
public class BackGroundMusicAudioClip
{
    public AudioManager.BackGroundMusic backGroundMusic;
    public AudioClip audioClip;
}
