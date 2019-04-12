using BS_Utils.Utilities;
using IPA;
using IPA.Config;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NoArrowsRandom
{
    public class Plugin : IBeatSaberPlugin
    {
        private const string MenuScene = "MenuCore";
        private const string GameScene = "GameCore";

        private NoArrowsRandom noArrowsRandom;

        public string Name => NoArrowsRandom.Name;
        public string Version => "1.0.0";

        public void OnActiveSceneChanged(Scene prevScene, Scene nextScene)
        {
            if (nextScene.name == GameScene && this.noArrowsRandom.NoArrowsOption == true)
            {
                SharedCoroutineStarter.instance.StartCoroutine(this.noArrowsRandom.TransformBeatMap());
            }
        }

        public void OnApplicationQuit()
        {
        }

        public void OnApplicationStart()
        {
            noArrowsRandom = new GameObject("NoArrowsRandom").AddComponent<NoArrowsRandom>();
            noArrowsRandom.Init();
        }

        public void OnFixedUpdate()
        {
        }

        public void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)
        {
            if (scene.name == MenuScene)
            {
                Logging.Info("Creating NoArrowsRandom Option");
                this.noArrowsRandom.CreateNoArrowsRandomOption();
            }
        }

        public void OnSceneUnloaded(Scene scene)
        {
        }

        public void OnUpdate()
        {
        }
    }
}