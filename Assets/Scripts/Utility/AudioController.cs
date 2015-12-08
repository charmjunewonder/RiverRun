using UnityEngine;
using System.Collections;

public class AudioController : MonoBehaviour {
    public static AudioController Singleton;
    public AudioSource menuAus;
    public AudioSource gameAus;
    public AudioSource voiceOverAus;

    public AudioClip goodFeedBack;
    public AudioClip badFeedBack;

    public AudioClip bloodLowSound;
    public AudioClip bossComeSound;
    public AudioClip bossDieSound;
    public AudioClip cystalFail;
    public AudioClip cystalSuccess;

    //public AudioClip strickerSkill1Sound;
    public AudioClip strickerUtliStartSound;
    public AudioClip strickerUtliExplosionSound;

    public AudioClip defenderUltiStartSound;
    public AudioClip defenderUltiExplosionSound;

    public AudioClip engineerHealingSound;
    public AudioClip engineerCystalProductionSound;

    public AudioClip newWaveOfEnemySound;
    public AudioClip bossComingVoiceOverSound;

    public AudioClip skill1SuccessSound;
    void Start()
    {
        Singleton = this;
	}
    #region iPad
    public void PlayGoodFeedBack()
    {
        PlaySound(menuAus, goodFeedBack);
    }

    public void PlayBadFeedBack()
    {
        PlaySound(menuAus, badFeedBack);
    }

    public void PlayBloodLowSound()
    {
        PlaySound(menuAus, bloodLowSound);
    }
    public void PlayCystalFail()
    {
        PlaySound(menuAus, cystalFail);
    }

    public void PlaySkill1Success()
    {
        PlaySound(menuAus, skill1SuccessSound);
    }

    public void PlayCystalSuccess()
    {
        PlaySound(menuAus, cystalSuccess);
    }

    public void PlayEngineerHealingSound()
    {
        PlaySound(gameAus, engineerHealingSound);
    }

    public void PlayEngineerCystalProductionSound()
    {
        PlaySound(gameAus, engineerCystalProductionSound);
    }
    #endregion

    #region Server

    public void PlayBossDieSound()
    {
        PlaySound(gameAus, bossDieSound);
    }

    public void PlayBossComing()
    {
        PlaySound(menuAus, bossComeSound);
        Invoke("StopBossComing", 12);
    }

    public void StopBossComing()
    {
        menuAus.Stop();
    }

    public void PlayStrickerUtliStartSound()
    {
        PlaySound(gameAus, strickerUtliStartSound);
    }
    public void StopStrickerUtliStartSound()
    {
        gameAus.Stop();
    }
    public void PlayStrickerUtliExplosionSound()
    {
        PlaySound(gameAus, strickerUtliExplosionSound);
    }

    public void PlayDefenderUltiStartSound()
    {
        PlaySound(gameAus, defenderUltiExplosionSound);
    }

    public void PlayDefenderUltiExplosionSound()
    {
        PlaySound(gameAus, defenderUltiExplosionSound);
    }

    #endregion

    private void PlaySound(AudioSource aus, AudioClip auc)
    {
        aus.Stop();
        aus.clip = auc;
        aus.Play();
    }

    public void PlayNewWaveOfEnemySound()
    {
        PlaySound(voiceOverAus, newWaveOfEnemySound);
    }

    public void PlayBossCommingVoiceOverSound()
    {
        PlaySound(voiceOverAus, bossComingVoiceOverSound);
    }

    public void ChangeVolume(float value)
    {
        value = Mathf.Clamp01(value);
        gameAus.volume = value;
        menuAus.volume = value;
        voiceOverAus.volume = value;
    }
}
