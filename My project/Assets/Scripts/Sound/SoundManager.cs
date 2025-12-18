using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource backgroundMusicSource;
    [SerializeField] private AudioSource ambientSoundSource;
    [SerializeField] private AudioSource sfxSourcePrefab;
    [SerializeField] private int maxConcurrentSFX = 10;

    [Header("Sound Clips")]
    [SerializeField] private List<SoundClipData> soundClips = new List<SoundClipData>();

    [Header("Sound Pitch Settings")]
    [SerializeField] private float defaultPitch = 1f;
    [SerializeField] private List<SoundPitchData> pitchOverrides = new List<SoundPitchData>();

    private Queue<AudioSource> availableSFXSources = new Queue<AudioSource>();
    private List<AudioSource> activeSFXSources = new List<AudioSource>();
    private Dictionary<SoundType, AudioClip[]> soundClipDictionary = new Dictionary<SoundType, AudioClip[]>();
    private Dictionary<MaterialType, SoundType> materialToSoundType = new Dictionary<MaterialType, SoundType>();
    private Dictionary<SoundType, float> soundPitchDictionary = new Dictionary<SoundType, float>();

    [System.Serializable]
    public class SoundClipData
    {
        public SoundType soundType;
        [Tooltip("여러 개의 사운드 클립을 등록하면 랜덤으로 재생됩니다")]
        public AudioClip[] clips;
    }

    [System.Serializable]
    public class SoundPitchData
    {
        public SoundType soundType;
        [Range(0.5f, 2f)]
        [Tooltip("1.0 = 원래 속도, 1.5 = 50% 빠름, 2.0 = 2배 빠름")]
        public float pitch = 1f;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeSoundManager();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeSoundManager()
    {
        // 배경음악 AudioSource 초기화
        if (backgroundMusicSource == null)
        {
            GameObject bgmObject = new GameObject("BackgroundMusicSource");
            bgmObject.transform.SetParent(transform);
            backgroundMusicSource = bgmObject.AddComponent<AudioSource>();
            backgroundMusicSource.loop = true;
            backgroundMusicSource.playOnAwake = false;
        }

        // 환경음 AudioSource 초기화
        if (ambientSoundSource == null)
        {
            GameObject ambientObject = new GameObject("AmbientSoundSource");
            ambientObject.transform.SetParent(transform);
            ambientSoundSource = ambientObject.AddComponent<AudioSource>();
            ambientSoundSource.loop = true;
            ambientSoundSource.playOnAwake = false;
        }

        // 사운드 클립 딕셔너리 초기화
        foreach (var soundData in soundClips)
        {
            if (soundData.clips != null && soundData.clips.Length > 0)
            {
                // null이 아닌 클립만 필터링
                List<AudioClip> validClips = new List<AudioClip>();
                foreach (var clip in soundData.clips)
                {
                    if (clip != null)
                    {
                        validClips.Add(clip);
                    }
                }
                
                if (validClips.Count > 0)
                {
                    soundClipDictionary[soundData.soundType] = validClips.ToArray();
                }
            }
        }

        // 재질별 타격 소리 매핑
        materialToSoundType[MaterialType.Flesh] = SoundType.WeaponHitFlesh;
        materialToSoundType[MaterialType.Metal] = SoundType.WeaponHitMetal;
        materialToSoundType[MaterialType.Wood] = SoundType.WeaponHitWood;
        materialToSoundType[MaterialType.Stone] = SoundType.WeaponHitStone;

        // 사운드 피치 딕셔너리 초기화 (기본값)
        soundPitchDictionary[SoundType.PlayerFootstep] = 1.5f;      // 발소리 50% 빠르게
        soundPitchDictionary[SoundType.WeaponSwing] = 1.4f;         // 무기 휘두르는 소리 40% 빠르게
        soundPitchDictionary[SoundType.WeaponHitFlesh] = 1.2f;      // 타격 소리 20% 빠르게
        soundPitchDictionary[SoundType.WeaponHitMetal] = 1.2f;
        soundPitchDictionary[SoundType.WeaponHitWood] = 1.2f;
        soundPitchDictionary[SoundType.WeaponHitStone] = 1.2f;

        // 인스펙터에서 설정한 오버라이드 적용
        foreach (var pitchData in pitchOverrides)
        {
            soundPitchDictionary[pitchData.soundType] = pitchData.pitch;
        }

        // SFX AudioSource 풀 초기화
        if (sfxSourcePrefab == null)
        {
            GameObject sfxPrefab = new GameObject("SFXSource");
            sfxSourcePrefab = sfxPrefab.AddComponent<AudioSource>();
            sfxSourcePrefab.playOnAwake = false;
        }

        for (int i = 0; i < maxConcurrentSFX; i++)
        {
            AudioSource source = Instantiate(sfxSourcePrefab, transform);
            source.gameObject.name = $"SFXSource_{i}";
            availableSFXSources.Enqueue(source);
        }
    }

    /// <summary>
    /// 효과음 재생 (여러 개 등록 시 랜덤으로 선택)
    /// pitch가 -1이면 사운드 타입별 기본 pitch 사용, 그렇지 않으면 지정된 pitch 사용
    /// </summary>
    public void PlaySFX(SoundType soundType, float volume = 1f, float pitch = -1f)
    {
        if (!soundClipDictionary.ContainsKey(soundType))
        {
            Debug.LogWarning($"사운드 클립이 등록되지 않았습니다: {soundType}");
            return;
        }

        AudioClip[] clips = soundClipDictionary[soundType];
        if (clips == null || clips.Length == 0)
        {
            Debug.LogWarning($"사운드 클립 배열이 비어있습니다: {soundType}");
            return;
        }

        // 여러 개의 클립 중 랜덤으로 선택
        AudioClip clip = clips[Random.Range(0, clips.Length)];
        
        // pitch가 -1이면 기본값 사용, 그렇지 않으면 지정된 pitch 사용
        if (pitch < 0)
        {
            pitch = soundPitchDictionary.ContainsKey(soundType) 
                ? soundPitchDictionary[soundType] 
                : defaultPitch;
        }
        
        PlaySFX(clip, volume, pitch);
    }

    /// <summary>
    /// AudioClip으로 직접 효과음 재생
    /// </summary>
    public void PlaySFX(AudioClip clip, float volume = 1f, float pitch = 1f)
    {
        if (clip == null) return;

        AudioSource source = GetAvailableSFXSource();
        if (source != null)
        {
            source.clip = clip;
            source.volume = volume;
            source.pitch = pitch;
            source.Play();
            StartCoroutine(ReturnSFXSourceToPool(source, clip.length / pitch));
        }
    }

    /// <summary>
    /// 재질에 따른 타격 소리 재생
    /// pitch가 -1이면 사운드 타입별 기본 pitch 사용, 그렇지 않으면 지정된 pitch 사용
    /// </summary>
    public void PlayWeaponHitSound(MaterialType materialType, float volume = 1f, float pitch = -1f)
    {
        if (materialToSoundType.ContainsKey(materialType))
        {
            PlaySFX(materialToSoundType[materialType], volume, pitch);
        }
        else
        {
            Debug.LogWarning($"재질 타입에 대한 사운드가 매핑되지 않았습니다: {materialType}");
        }
    }

    /// <summary>
    /// 배경음악 재생 (여러 개 등록 시 랜덤으로 선택)
    /// </summary>
    public void PlayBackgroundMusic(SoundType soundType, float volume = 1f, bool fadeIn = false)
    {
        if (!soundClipDictionary.ContainsKey(soundType))
        {
            Debug.LogWarning($"배경음악 클립이 등록되지 않았습니다: {soundType}");
            return;
        }

        AudioClip[] clips = soundClipDictionary[soundType];
        if (clips == null || clips.Length == 0)
        {
            Debug.LogWarning($"배경음악 클립 배열이 비어있습니다: {soundType}");
            return;
        }

        // 여러 개의 클립 중 랜덤으로 선택
        AudioClip clip = clips[Random.Range(0, clips.Length)];
        backgroundMusicSource.clip = clip;
        backgroundMusicSource.volume = volume;
        backgroundMusicSource.Play();
    }

    /// <summary>
    /// 배경음악 중지
    /// </summary>
    public void StopBackgroundMusic(bool fadeOut = false)
    {
        if (fadeOut)
        {
            StartCoroutine(FadeOutAudioSource(backgroundMusicSource, 1f));
        }
        else
        {
            backgroundMusicSource.Stop();
        }
    }

    /// <summary>
    /// 환경음 재생 (여러 개 등록 시 랜덤으로 선택)
    /// </summary>
    public void PlayAmbientSound(SoundType soundType, float volume = 1f)
    {
        if (!soundClipDictionary.ContainsKey(soundType))
        {
            Debug.LogWarning($"환경음 클립이 등록되지 않았습니다: {soundType}");
            return;
        }

        AudioClip[] clips = soundClipDictionary[soundType];
        if (clips == null || clips.Length == 0)
        {
            Debug.LogWarning($"환경음 클립 배열이 비어있습니다: {soundType}");
            return;
        }

        // 여러 개의 클립 중 랜덤으로 선택
        AudioClip clip = clips[Random.Range(0, clips.Length)];
        ambientSoundSource.clip = clip;
        ambientSoundSource.volume = volume;
        ambientSoundSource.Play();
    }

    /// <summary>
    /// 환경음 중지
    /// </summary>
    public void StopAmbientSound()
    {
        ambientSoundSource.Stop();
    }

    /// <summary>
    /// 배경음악 볼륨 설정
    /// </summary>
    public void SetBackgroundMusicVolume(float volume)
    {
        backgroundMusicSource.volume = Mathf.Clamp01(volume);
    }

    /// <summary>
    /// 효과음 볼륨 설정
    /// </summary>
    public void SetSFXVolume(float volume)
    {
        foreach (var source in activeSFXSources)
        {
            if (source != null)
            {
                source.volume = Mathf.Clamp01(volume);
            }
        }
    }

    private AudioSource GetAvailableSFXSource()
    {
        // 사용 가능한 소스가 있으면 반환
        if (availableSFXSources.Count > 0)
        {
            AudioSource source = availableSFXSources.Dequeue();
            activeSFXSources.Add(source);
            return source;
        }

        // 사용 가능한 소스가 없으면 가장 오래된 소스 재사용
        if (activeSFXSources.Count > 0)
        {
            AudioSource oldestSource = activeSFXSources[0];
            activeSFXSources.RemoveAt(0);
            activeSFXSources.Add(oldestSource);
            return oldestSource;
        }

        return null;
    }

    private System.Collections.IEnumerator ReturnSFXSourceToPool(AudioSource source, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        if (source != null && activeSFXSources.Contains(source))
        {
            activeSFXSources.Remove(source);
            availableSFXSources.Enqueue(source);
        }
    }

    private System.Collections.IEnumerator FadeOutAudioSource(AudioSource source, float duration)
    {
        float startVolume = source.volume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
            yield return null;
        }

        source.Stop();
        source.volume = startVolume;
    }

    /// <summary>
    /// 모든 사운드 일시정지
    /// </summary>
    public void PauseAllSounds()
    {
        backgroundMusicSource.Pause();
        ambientSoundSource.Pause();
        foreach (var source in activeSFXSources)
        {
            if (source != null && source.isPlaying)
            {
                source.Pause();
            }
        }
    }

    /// <summary>
    /// 모든 사운드 재개
    /// </summary>
    public void ResumeAllSounds()
    {
        backgroundMusicSource.UnPause();
        ambientSoundSource.UnPause();
        foreach (var source in activeSFXSources)
        {
            if (source != null)
            {
                source.UnPause();
            }
        }
    }
}

