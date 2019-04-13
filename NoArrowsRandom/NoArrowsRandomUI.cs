using CustomUI.GameplaySettings;
using CustomUI.Utilities;
using IPA.Config;
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

        /// <summary>
        /// Called before Start or Updates by Unity infrastructure
        /// </summary>
        public void Awake()
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

            noArrowsRandomOption.GetValue = NoArrowsOption;
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
                var beatmap = data.difficultyBeatmap;

                string characteristic = beatmap.parentDifficultyBeatmapSet.beatmapCharacteristic.characteristicName;
                if (characteristic == ("One Saber") || characteristic == ("No Arrows"))
                {
                    // Do not transform for One Saber or legitimate No Arrows mode
                    Logging.Info($"Cannot transform song: {beatmap.level.songName} due to being a One Saber or No Arrows map");
                    yield break;
                }

                Logging.Info("Disabling submission on NoArrowsOption turned on.");
                BS_Utils.Gameplay.ScoreSubmission.DisableSubmission("NoArrowsRandom");

                var gameplayCore = Resources.FindObjectsOfTypeAll<GameplayCoreSceneSetup>().FirstOrDefault();
                if (gameplayCore == null) yield break;

                // Applying NoArrowsRandom transformation
                Logging.Info($"Transforming song: {beatmap.level.songName}");
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
