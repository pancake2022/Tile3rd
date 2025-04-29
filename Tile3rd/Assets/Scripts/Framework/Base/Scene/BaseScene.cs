using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CSFramework
{
    public class BaseScene : Module<SceneManager>
    {
        public Framework Framework { get; private set; }
        public string SceneMusicName { get; protected set; }

        protected int _scene_music_id;
        protected bool _clean_all_sound;

        public override IEnumerator Init (CSBehaviour main_module, params object[] param_list)
        {
            Framework = param_list[0] as Framework;
            _scene_music_id = -1;
            _clean_all_sound = true;
            var new_param_list = new List<object>(param_list);
            new_param_list.RemoveAt(0);
            yield return base.Init(main_module, new_param_list.ToArray());
            yield return new WaitForEndOfFrame(); // For WindowUI
            PlayMusic();
        }

        public void PlayMusic ()
        {
            if (!string.IsNullOrEmpty(SceneMusicName))
                _scene_music_id = Framework.AudioManager.PlayMusic(SceneMusicName);
        }

        public void StopMusic ()
        {
            if (_scene_music_id >= 0)
            {
                Framework.AudioManager.StopMusic(_scene_music_id);
                _scene_music_id = -1;
            }
        }

        public override IEnumerator Cleanup ()
        {
            yield return base.Cleanup();
            StopMusic();

            if (_clean_all_sound)
                Framework.AudioManager.StopAllSound();
                
            DestroyImmediate(gameObject);
        }
    }
}