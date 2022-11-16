using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [Header("オプション")]
    public GameData data;
    public Options options;
    [Header("ステージBGM")]
    public AudioClip stageMusic;
	public float loopBegin;
	public float loopEnd;
    [Header("その他BGM")]
	public AudioClip speedUpMusic;
	public AudioClip invincibilityMusic;
	public AudioClip cSonicInvincibilityMusic;
	public AudioClip sonicInvincibilityMusic;
	public AudioClip metalMarioMusic;
	public AudioClip deathMusic;
	public AudioClip drowningMusic;
	public AudioClip clearMusic;

    private AudioClip currentMusic;
    public static int musicIndex = 0;
    public static bool musicFade = false;
    private float musicOpacity = 1f;
	private AudioSource musicManager;
    float Ttime;

    float[] loopValues = new float[2];
    // Start is called before the first frame update
    void Awake()
    {
        /*
        "All-Star Rest Area" = Begin : 0.582, End : 24.372

        変数名：musicIndex
        ０＝通常
        １＝やられBGM
        ２＝止める
        ３＝溺れる！！
        ４＝スピードアップ
        ５＝無敵
        ６＝無敵
        ７＝メタルマリオ
        ８＝クリア
        */
        musicIndex = 0;
        musicFade = false;
        musicManager = GetComponent<AudioSource>();
		musicManager.clip = stageMusic;
		musicManager.Play();

        currentMusic = stageMusic;
        loopValues[0] = loopBegin;
        loopValues[1] = loopEnd;
    }

    // Update is called once per frame
    void Update()
    {
        if (musicManager.loop) {
            if (musicManager.pitch > 0) {
                if (musicManager.time >= loopEnd){
                    musicManager.time = loopBegin;
                    musicManager.Play();
                }
            } else {
                if (musicManager.time > loopEnd && loopEnd > 0){
                    musicManager.time = loopEnd;
                    musicManager.Play();
                } else if (musicManager.time <= loopBegin){
                    musicManager.time = loopEnd;
                    musicManager.Play();
                }
            }
            Ttime = musicManager.time;
            if (musicIndex == 1) {
                DeathMusic();
            } else if (musicIndex == 2) {
                MusicStop();
            } else if (musicIndex == 3) {
                DrowningMusic();
            } else if (musicIndex == 4) {
                SpeedUpMusic();
            } else if (musicIndex == 5) {
                InvincibilityMusic();
            } else if (musicIndex == 6) {
                InvincibilityMusic(true);
            } else if (musicIndex == 7) {
                MetalMusic();
            } else if (musicIndex == 8) {
                ClearMusic();
            }
        } else {
            if (musicIndex == 0) {
                musicManager.clip = currentMusic;
                musicManager.Play();
                musicManager.loop = true;
            }
        }

        if (musicIndex == 0) {
            if (musicFade) {
                musicOpacity -= 2 * Time.deltaTime;
            } else {
                musicOpacity += 2 * Time.deltaTime;
            }
        }
        musicOpacity = Mathf.Clamp(musicOpacity, 0f, 1f);

        musicManager.volume = 0.5f * musicOpacity * options.musicVolume;
    }

    void DeathMusic(){
        musicManager.clip = deathMusic;
        if (musicManager.pitch > 0) {
            musicManager.time = 0f;
        } else {
            musicManager.time = deathMusic.length;
        }
        musicManager.Play();
        musicManager.loop = false;
    }

    void MusicStop() {
        musicManager.Stop();
        musicManager.loop = false;
    }

    void DrowningMusic(){
        musicManager.clip = drowningMusic;
        if (musicManager.pitch > 0) {
            musicManager.time = 0f;
        } else {
            musicManager.time = drowningMusic.length;
        }
        musicManager.Play();
        musicManager.loop = false;
    }

    void SpeedUpMusic(){
        musicManager.clip = speedUpMusic;
        if (musicManager.pitch > 0) {
            musicManager.time = 0f;
        } else {
            musicManager.time = speedUpMusic.length;
        }
        musicManager.Play();
        musicManager.loop = false;
    }
    void InvincibilityMusic(bool sonic = false){
        if (data.HUDType == GameType.Sonic || data.HUDType == GameType.SonicBoss) {
            musicManager.clip = sonicInvincibilityMusic;
        } else {
            musicManager.clip = (sonic ? cSonicInvincibilityMusic : invincibilityMusic);
        }
        if (musicManager.pitch > 0) {
            musicManager.time = 0f;
        } else {
            if (data.HUDType == GameType.Sonic || data.HUDType == GameType.SonicBoss) {
                musicManager.time = sonicInvincibilityMusic.length;
            } else {
                musicManager.time = (sonic ? cSonicInvincibilityMusic.length : invincibilityMusic.length);
            }
        }
        musicManager.Play();
        musicManager.loop = false;
    }
    void MetalMusic(){
        musicManager.clip = metalMarioMusic;
        if (musicManager.pitch > 0) {
            musicManager.time = 0f;
        } else {
            musicManager.time = metalMarioMusic.length;
        }
        musicManager.Play();
        musicManager.loop = false;
    }

    void ClearMusic(){
        musicOpacity = 1f;
        musicFade = false;
        musicManager.clip = clearMusic;
        if (musicManager.pitch > 0) {
            musicManager.time = 0f;
        } else {
            musicManager.time = clearMusic.length;
        }
        musicManager.Play();
        musicManager.loop = false;
    }

    public void ChangeMusic(AudioClip music, float begin, float end) {
        currentMusic = music;
        loopBegin = begin;
        loopEnd = end;
        musicOpacity = 1f;
        musicFade = false;
        if (musicManager.loop) {
            musicManager.clip = music;
            musicManager.Play();
        }
    }
    public void ReturnMusic() {
        musicOpacity = 1f;
        musicFade = false;

        currentMusic = stageMusic;
        loopBegin = loopValues[0];
        loopEnd = loopValues[1];
        if (musicManager.loop) {
            musicManager.clip = stageMusic;
            musicManager.Play();
        }
    }
}
