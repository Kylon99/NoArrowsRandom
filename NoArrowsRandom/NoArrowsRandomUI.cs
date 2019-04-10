using CustomUI.GameplaySettings;
using CustomUI.Utilities;
using IPA.Config;
using SongLoaderPlugin.OverrideClasses;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace NoArrowsRandom
{
    public class NoArrowsRandom : MonoBehaviour
    {
        public const string Name = "NoArrowsRandom";
        public const string Option = "Option";

        /// <summary>
        /// Indicates whether the user has selected to use NoArrowsRandom or not
        /// </summary>
        public bool NoArrowsOption { get; private set; }

        public void Init()
        {
            NoArrowsOption = ModPrefs.GetBool(Name, Option, false, true);
        }

        /// <summary>
        /// Creates the Toggle UI option
        /// </summary>
        public void CreateNoArrowsRandomOption()
        {
            var noArrowsIcon = UIUtilities.LoadSpriteFromResources("NoArrowsRandom.Resources.NoArrowsRandom.png");

            var noArrowsRandomOption = GameplaySettingsUI.CreateToggleOption(
                "No Arrows Random",
                "Turn every song into the classic randomized No Arrows mode.",
                noArrowsIcon);

            noArrowsRandomOption.GetValue = false;
            noArrowsRandomOption.AddConflict("No Arrows");
            noArrowsRandomOption.OnToggle += OnNoArrowsRandomOptionToggle;
        }

        /// <summary>
        /// Performs the transformation
        /// </summary>
        public IEnumerator TransformBeatMap()
        {
            yield return new WaitForSecondsRealtime(0.1f);
            if (this.NoArrowsOption && BS_Utils.Plugin.LevelData.IsSet)
            {
                GameplayCoreSceneSetupData data = BS_Utils.Plugin.LevelData?.GameplayCoreSceneSetupData;
                var beatmap = data.difficultyBeatmap as CustomLevel.CustomDifficultyBeatmap;

                if (beatmap == null) yield break; // Possibly not a custom map so abort

                var charactersticList = beatmap.customLevel.beatmapCharacteristics.Select(c => c.characteristicName).ToList();
                if (charactersticList.Contains("One Saber") || charactersticList.Contains("No Arrows"))
                {
                    // Do not transform for One Saber or legitimate No Arrows mode
                    Logging.Info($"Cannot transform song: {SongLoaderPlugin.SongLoader.CurrentLevelPlaying.customLevel.songName} in {String.Join(", ", charactersticList)} mode.");
                    yield break;
                }

                Logging.Info("Disabling submission on NoArrowsOption turned on.");
                BS_Utils.Gameplay.ScoreSubmission.DisableSubmission("NoArrowsRandom");

                var gameplayCore = Resources.FindObjectsOfTypeAll<GameplayCoreSceneSetup>().FirstOrDefault();
                if (gameplayCore == null) yield break;

                // Applying NoArrowsRandom transformation
                Logging.Info($"Transforming song: {beatmap.customLevel.songName}");
                var transformedBeatmap = BeatmapDataNoArrowsTransform.CreateTransformedData(beatmap.beatmapData, true);
                var beatmapDataModel = gameplayCore.GetPrivateField<BeatmapDataModel>("_beatmapDataModel");
                beatmapDataModel.SetPrivateField("_beatmapData", transformedBeatmap);
            }
        }

        private void OnNoArrowsRandomOptionToggle(bool option)
        {
            NoArrowsOption = option;
            ModPrefs.SetBool(Name, Option, option);
        }
    }
}
