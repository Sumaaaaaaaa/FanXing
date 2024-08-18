using System;
using System.IO;
using UnityEngine;
namespace AVG_System.Scripts
{
    public enum AudioSystemTarget { Music, Voice, SoundEffect, All }
    internal class AudioSystemSettingData
    {
        // 变量
        public float AllLoudness = 1;
        public float MusicLoudness = 1;
        public float VoiceLoudness = 1;
        public float SoundEffectLoudness = 1;
        
        /// <summary>
        /// 实例化
        /// </summary>
        public AudioSystemSettingData()
        {
            if (PlayerPrefs.HasKey(nameof(AllLoudness)))
            {
                AllLoudness = PlayerPrefs.GetFloat(nameof(AllLoudness));
                MusicLoudness = PlayerPrefs.GetFloat(nameof(MusicLoudness));
                VoiceLoudness = PlayerPrefs.GetFloat(nameof(VoiceLoudness));
                SoundEffectLoudness = PlayerPrefs.GetFloat(nameof(SoundEffectLoudness));
                Debug.Log($"<{nameof(AudioSystemSettingData)}>...数据读取..." +
                                                            $"[{nameof(AllLoudness)}:{AllLoudness}]" +
                                                              $"[{nameof(MusicLoudness)}:{MusicLoudness}]" +
                                                              $"[{nameof(VoiceLoudness)}:{VoiceLoudness}]" +
                                                              $"[{nameof(SoundEffectLoudness)}:{SoundEffectLoudness}]");
            }
            else
            {
                Save();
            }
        }
        public void Save()
        {
            PlayerPrefs.SetFloat(nameof(AllLoudness),AllLoudness);
            PlayerPrefs.SetFloat(nameof(MusicLoudness),MusicLoudness);
            PlayerPrefs.SetFloat(nameof(VoiceLoudness),VoiceLoudness);
            PlayerPrefs.SetFloat(nameof(SoundEffectLoudness),SoundEffectLoudness);
            Debug.Log($"<{nameof(AudioSystemSettingData)}>...数据存储..." +
                                                            $"[{nameof(AllLoudness)}:{AllLoudness}]" +
                                                            $"[{nameof(MusicLoudness)}:{MusicLoudness}]" +
                                                            $"[{nameof(VoiceLoudness)}:{VoiceLoudness}]" +
                                                            $"[{nameof(SoundEffectLoudness)}:{SoundEffectLoudness}]");
        }

        public override string ToString()
        {
            return $"[{nameof(AllLoudness)}:{AllLoudness}]" +
                $"[{nameof(MusicLoudness)}:{MusicLoudness}]" +
                $"[{nameof(VoiceLoudness)}:{VoiceLoudness}]" +
                $"[{nameof(SoundEffectLoudness)}:{SoundEffectLoudness}]";
        }
    }
    [RequireComponent(typeof(AudioSource))]
    public class AudioSystem : MonoBehaviour
    {
        // 单例
        private static AudioSystem _instance;
        // 组件
        private static AudioSource _sourceMusic;
        private static AudioSource _sourceVoice;
        private static AudioSource _sourceSoundEffect;
        // 数据
        private static AudioSystemSettingData _settingData = new AudioSystemSettingData();
        
        // 初始化
        [RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            // 生成所有组件
            
            var instanceOwner = new GameObject(nameof(AudioSystem));
            _instance = instanceOwner.AddComponent<AudioSystem>();
            
            var go = new GameObject(nameof(_sourceMusic))
            {
                transform =
                {
                    parent = instanceOwner.transform
                }
            };
            _sourceMusic = go.AddComponent<AudioSource>();
            
            go = new GameObject(nameof(_sourceVoice))
            {
                transform =
                {
                    parent = instanceOwner.transform
                }
            };
            _sourceVoice = go.AddComponent<AudioSource>();
            
            go = new GameObject(nameof(_sourceSoundEffect))
            {
                transform =
                {
                    parent = instanceOwner.transform
                }
            };
            _sourceSoundEffect = go.AddComponent<AudioSource>();
            
            // 赋值所有响度数据到组件
            AudioListener.volume = _settingData.AllLoudness;
            _sourceMusic.volume = _settingData.MusicLoudness;
            _sourceVoice.volume = _settingData.VoiceLoudness;
            _sourceSoundEffect.volume = _settingData.SoundEffectLoudness;
            
            // 循环设定
            _sourceMusic.loop = true;
        }
        
        // 播放和暂停
        
        public static void PlayMusic(AudioClip audioClip)
        {
            _sourceMusic.clip = audioClip;
            _sourceMusic.Play();
        }
        public static void StopMusic()
        {
            _sourceMusic.Stop();
        }

        public static void PlayVoice(AudioClip audioClip)
        {
            _sourceVoice.clip = audioClip;
            _sourceVoice.Play();
        }
        public static void StopVoice()
        {
            _sourceVoice.Stop();
        }

        public static void PlaySoundEffect(AudioClip audioClip)
        {
            _sourceSoundEffect.PlayOneShot(audioClip);
        }
        public static void StopSoundEffect()
        {
            _sourceSoundEffect.Stop();
        }

        public static void StopAll()
        {
            StopMusic();
            StopVoice();
            StopSoundEffect();
        }
        
        
        public static void Save()
        {
            _settingData.Save();
        }
        
        // 获取音量
        public static float GetLoudness(AudioSystemTarget target)
        {
            return target switch
            {
                AudioSystemTarget.All => _settingData.AllLoudness,
                AudioSystemTarget.Music => _settingData.MusicLoudness,
                AudioSystemTarget.Voice => _settingData.VoiceLoudness,
                AudioSystemTarget.SoundEffect => _settingData.SoundEffectLoudness,
                _ => -1
            };
        }
        // 设置音量
        public static void SetLoudness(AudioSystemTarget target, float loudness)
        {
            switch (target)
            {
                case AudioSystemTarget.All:
                    _settingData.AllLoudness = loudness;
                    AudioListener.volume = loudness;
                    return;
                case AudioSystemTarget.Music:
                    _settingData.MusicLoudness = loudness;
                    _sourceMusic.volume = loudness;
                    return;
                case AudioSystemTarget.Voice:
                    _settingData.VoiceLoudness = loudness;
                    _sourceVoice.volume = loudness;
                    return;
                case AudioSystemTarget.SoundEffect:
                    _settingData.SoundEffectLoudness = loudness;
                    _sourceSoundEffect.volume = loudness;
                    return;
                default:
                    return;
            }
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.L))
            {
                print(_settingData);
            }
        }
    }
}