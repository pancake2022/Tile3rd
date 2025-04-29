using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CSFramework
{
    public class AudioContext
    {
        public string MusicResourceRootPath;
        public string SoundResourceRootPath;
        public Transform DefaultAudioSourceTransform;
        public int SameSoundMaxPlayingCount = 1;
    }

    public class AudioManager : Module<Framework>
    {
        public class Audio 
        {
            public string Path;
            public int ID;
            public float Volume;
            public bool Loop;
            public AudioSource Source;
            public AudioClip Clip;
            public float FadeInTime;
            public float FadeOutTime;
            public bool InFadeIn;
            public bool InFadeOut;
            public float FadeEscapeTime;
            public bool Pausing;
            public bool IsMusic;
            //test
            public float Pitch;
        }

        public bool IsMusicOpen { get; private set; }
        public bool IsSoundOpen { get; private set; }
        public Action<string> AudioPlayCallback;
        public Action<string> AudioStopCallback;
        public Action<bool> MusicOpenStatusUpdateCallback;
        public Action<bool> SoundOpenStatusUpdateCallback;
        private AudioContext _context;
        private Dictionary<int, Audio> _audio_dict;
        private Dictionary<string, int> _audio_playing_dict;
        private int _newest_audio_id;
        private int _last_music_id;

        protected override IEnumerator on_init (params object[] param_list)
        {
            _context = _main_module.Context.AudioContext;
            _audio_dict = new Dictionary<int, Audio>();
            _audio_playing_dict = new Dictionary<string, int>();
            _newest_audio_id = 0;
            IsMusicOpen = true;
            IsSoundOpen = true;
            yield return null;
        }

        protected int play_music (string path, float volume, bool loop, float fade_in_seconds, float fade_out_seconds)
        {
            if (try_create_audio(path, true, volume, loop, out var audio))
            {
                audio.FadeInTime = fade_in_seconds;
                audio.FadeOutTime = fade_out_seconds;
                if (IsMusicOpen)
                    audio.Source.Play();
                else
                    audio.Pausing = true;

                if (audio.FadeInTime > 0)
                {
                    audio.Source.volume = 0;
                    audio.InFadeIn = true;
                    audio.InFadeOut = false;
                }

                _last_music_id = audio.ID;
                return audio.ID;
            }
            return -1;
        }

        protected int play_sound (string path, bool loop)
        {
            if (!IsSoundOpen)
                return -1;

            if (_audio_playing_dict.TryGetValue(path, out var playing_count))
            {
                if (playing_count >= _context.SameSoundMaxPlayingCount)
                    return -1;
            }
            else
            {
                playing_count = 0;
            }

            if (try_create_audio(path, false, 1.0f, loop, out var audio))
            {
                audio.Source.Play();
                _audio_playing_dict[path] = playing_count + 1;

                if (playing_count == 0)
                    AudioPlayCallback?.Invoke(path);

                return audio.ID;
            }

            CSFramework.Logger.Log($"AudioManager: not found audio [{path}]");
            return -1;
        }

        protected bool stop_audio (int audio_id)
        {
            if (_audio_dict.TryGetValue(audio_id, out var audio))
            {
                on_audio_stop(audio);
                return true;
            }
            return false;
        }

        protected void on_audio_stop (Audio audio)
        {
            if (audio.IsMusic && audio.FadeOutTime > 0)
            {
                audio.InFadeIn = false;
                audio.InFadeOut = true;
                audio.FadeEscapeTime = 0;
            }
            else
            {
                real_stop_audio(audio);
            }
        }

        protected void real_stop_audio (Audio audio)
        {
            Destroy(audio.Source);
            _audio_dict.Remove(audio.ID);
            if (_audio_playing_dict.TryGetValue(audio.Path, out var playing_count))
            {
                if (playing_count <= 1)
                {
                    _audio_playing_dict.Remove(audio.Path);
                    AudioStopCallback?.Invoke(audio.Path);
                }
                else
                {
                    _audio_playing_dict[audio.Path] = playing_count - 1;
                }
            }
        }

        protected bool try_create_audio (string path, bool is_music, float volume, bool loop, out Audio audio)
        {
            var audio_clip = _main_module.ResourcesManager.LoadResource<AudioClip>(path);
            if (audio_clip != null)
            {
                audio = new Audio
                {
                    Path = path,
                    ID = ++_newest_audio_id,
                    Volume = volume,
                    Loop = loop,
                    Source = _context.DefaultAudioSourceTransform.gameObject.AddComponent<AudioSource>(),
                    Clip = audio_clip,
                    IsMusic = is_music,
                    Pausing = false,
                };
                audio.Source.clip = audio.Clip;
                audio.Source.loop = audio.Loop;
                audio.Source.volume = audio.Volume;
                _audio_dict[audio.ID] = audio;
                return true;
            }
            audio = null;
            return false;
        }

        protected override void on_tick (float dt)
        {
            var wait_remove_audio_id_list = new List<int>();

            foreach (var audio in _audio_dict.Values)
            {
                // fade in & fade out
                if (audio.IsMusic)
                {
                    if (audio.InFadeIn)
                    {
                        audio.FadeEscapeTime += dt;
                        if (audio.FadeEscapeTime >= audio.FadeInTime)
                        {
                            audio.InFadeIn = false;
                            audio.Source.volume = audio.Volume;
                        }
                        else
                        {
                            audio.Source.volume = (audio.FadeEscapeTime /audio.FadeInTime) * audio.Volume;
                        }
                    }
                    else if (audio.InFadeOut)
                    {
                        audio.FadeEscapeTime += dt;
                        if (audio.FadeEscapeTime >= audio.FadeOutTime)
                        {
                            audio.InFadeOut = false;
                            audio.Source.volume = 0;
                            wait_remove_audio_id_list.Add(audio.ID);
                        }
                        else
                        {
                            audio.Source.volume = (1.0f - (audio.FadeEscapeTime / audio.FadeOutTime)) * audio.Volume;
                        }
                    }
                }

                if (!audio.Source.isPlaying && !audio.Pausing)
                    wait_remove_audio_id_list.Add(audio.ID);
            }

            foreach (var audio_id in wait_remove_audio_id_list)
            {
                if (_audio_dict.TryGetValue(audio_id, out var audio))
                    real_stop_audio(audio);
            }
        }

        #region public interface
        public int PlayMusic (string name, float volume = 1.0f, bool loop = true, float fade_in_seconds = 0.64f, float fade_out_seconds = 0.32f)
        {
            return PlayMusicWithFullPath(string.Format("{0}/{1}", _context.MusicResourceRootPath, name), volume, loop, fade_in_seconds, fade_out_seconds);
        }

        public int PlayMusicWithFullPath (string path, float volume = 1.0f, bool loop = true, float fade_in_seconds = 0.64f, float fade_out_seconds = 0.32f)
        {
            return play_music(path, volume, loop, fade_in_seconds, fade_out_seconds);
        }

        public bool StopMusic (int audio_id)
        {
            return stop_audio(audio_id);
        }

        public int PlaySound (string name, bool loop = false)
        {
            return PlaySoundWithFullPath(string.Format("{0}/{1}", _context.SoundResourceRootPath, name), loop);
        }

        public int PlaySoundWithFullPath (string path, bool loop)
        {
            return play_sound(path, loop);
        }

        public bool StopSound (int audio_id)
        {
            return stop_audio(audio_id);
        }

        public void StopAllSound ()
        {
            var audio_list = new List<Audio>(_audio_dict.Values);
            foreach (var audio in audio_list)
            {
                if (!audio.IsMusic)
                    on_audio_stop(audio);
            }
        }

        public void PauseMusic ()
        {
            if (IsMusicOpen)
            {
                if (_audio_dict.TryGetValue(_last_music_id, out var audio))
                {
                    audio.Pausing = true;
                    audio.Source.Pause();
                }
            }
        }

        public void ResumeMusic ()
        {
            if (IsMusicOpen)
            {
                if (_audio_dict.TryGetValue(_last_music_id, out var audio))
                {
                    audio.Pausing = false;
                    audio.Source.UnPause();
                }
            }
        }

        public void SetMusicOpen (bool is_open)
        {
            IsMusicOpen = is_open;
            MusicOpenStatusUpdateCallback?.Invoke(is_open);
            if (IsMusicOpen)
            {
                if (_audio_dict.TryGetValue(_last_music_id, out var audio))
                {
                    audio.Pausing = false;
                    audio.Source.Play();
                }
            }
            else
            {
                if (_audio_dict.TryGetValue(_last_music_id, out var audio))
                {
                    audio.Pausing = true;
                    audio.Source.Stop();
                }
            }
        }

        public float SetMusicVolume (float volume)
        {
            if (volume < 0)
               volume = 0;
            else if (volume > 1.0f)
                volume = 1.0f;
            if (_audio_dict.TryGetValue(_last_music_id, out var audio))
            {
                var previous_volume = audio.Source.volume;
                audio.Volume = volume;
                audio.Source.volume = volume;
                return previous_volume;
            }
            return 0;
        }
        //随时删除用pitch
        public float SetMusicPitch(float pitch)
        {
            //if (volume < 0)
            //    volume = 0;
            //else if (volume > 1.0f)
            //    volume = 1.0f;
            if (_audio_dict.TryGetValue(_last_music_id, out var audio))
            {
                var previous_pitch = audio.Source.pitch;
                audio.Pitch = pitch;
                audio.Source.pitch = pitch;
                return previous_pitch;
            }
            return 0;
        }

        public bool IsAudioPlaying (int audio_id)
        {
            if (_audio_dict.TryGetValue(audio_id, out var audio))
            {
                return audio.Source.isPlaying;
            }
            return false;
        }

        public void SetSoundOpen (bool is_open)
        {
            IsSoundOpen = is_open;
            SoundOpenStatusUpdateCallback?.Invoke(is_open);
            if (!IsSoundOpen)
            {
                foreach (var audio_item in _audio_dict)
                {
                    if (audio_item.Key != _last_music_id)
                        audio_item.Value.Source.Stop();
                }               
            }
        }
        #endregion
    }
}