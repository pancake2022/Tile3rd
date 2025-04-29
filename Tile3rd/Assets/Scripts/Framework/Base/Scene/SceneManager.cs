using System;
using System.Collections;
using System.Collections.Generic;

namespace CSFramework
{
    public class SceneManager : Module<Framework>
    {
        private BaseScene _current_scene;
        public BaseScene CurrentScene { get { return _current_scene; } }
        public Type PreviousSceneType { get; private set; }
        private BaseLoadingScene _current_loading_scene;

        public void StartLoadScene<TScene> (params object[] param_list) where TScene : BaseScene 
        {
            StartCoroutine(LoadScene<TScene, BaseLoadingScene>(_main_module.Context.DefaultScenePath, _main_module.Context.DefaultLoadingScenePath, param_list));
        }

        public void StartLoadScene<TScene, TLoadingScene> (params object[] param_list) where TScene : BaseScene where TLoadingScene : BaseLoadingScene
        {
            StartCoroutine(LoadScene<TScene, TLoadingScene>(_main_module.Context.DefaultScenePath, _main_module.Context.DefaultLoadingScenePath, param_list));
        }

        public void StartLoadScene<TScene> (string scene_path, params object[] param_list) where TScene : BaseScene
        {
            StartCoroutine(LoadScene<TScene, BaseLoadingScene>(scene_path, _main_module.Context.DefaultLoadingScenePath, param_list));
        }

        public void StartLoadScene<TScene, TLoadingScene> (string scene_path, params object[] param_list) where TScene : BaseScene where TLoadingScene : BaseLoadingScene
        {
            StartCoroutine(LoadScene<TScene, TLoadingScene>(scene_path, _main_module.Context.DefaultLoadingScenePath, param_list));
        }

        public void StartLoadScene<TScene, TLoadingScene> (string scene_path, string loading_scene_path, params object[] param_list) where TScene : BaseScene where TLoadingScene : BaseLoadingScene
        {
            StartCoroutine(LoadScene<TScene, TLoadingScene>(scene_path, loading_scene_path, param_list));
        }

        public IEnumerator LoadScene<TScene, TLoadingScene> (string scene_path, string loading_scene_path, params object[] param_list) where TScene : BaseScene where TLoadingScene : BaseLoadingScene
        {
            var new_param_list = new List<object>(param_list);
            new_param_list.Insert(0, _main_module);
            
            // first disable current scene
            if (_current_scene)
            {
                PreviousSceneType = _current_scene.GetType();
                _current_scene.SetEnable(false);
            }

            // load loading scene
            var loading_prefab = _main_module.GameObjectPoolManager.Spawn(scene_path, _main_module.Context.SceneRoot);
            _current_loading_scene = Utils.GetOrCreateComponent<TLoadingScene>(loading_prefab);
            yield return _current_loading_scene.Init(this, new_param_list.ToArray());

            // unload current scene
            if (_current_scene)
            {
                _main_module.UIManager.OnCleanupScene();
                yield return _current_scene.Cleanup();
                _current_scene = null;
            }

            yield return _current_loading_scene.LoadingProcess();

            var scene_obj = _main_module.GameObjectPoolManager.Spawn(scene_path, _main_module.Context.SceneRoot);
            _current_scene = Utils.GetOrCreateComponent<TScene>(scene_obj);
            yield return _current_scene.Init(this, new_param_list.ToArray());

            // unload loading scene
            var loading_scene = _current_loading_scene;
            _current_loading_scene = null;
            yield return loading_scene.Cleanup();
        }

        protected override void on_tick (float dt) 
        {
            if (_current_loading_scene)
                _current_loading_scene.Tick(dt);
            else if (_current_scene)
                _current_scene.Tick(dt);
        }

        protected override void on_millisecond_tick (long ms_dt)
        {
            if (_current_loading_scene)
                _current_loading_scene.MillisecondTick(ms_dt);
            else if (_current_scene)
                _current_scene.MillisecondTick(ms_dt);
        }
    }
}