using CustomUI.GameplaySettings;
using CustomUI.Utilities;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace NoArrowsRandom
{
    public class NoArrowsRandom : MonoBehaviour
    {
        // NoArrowsRandom mod names
        public const string Name = "NoArrowsRandom";
        public const string Option = "Option";

        // Level serialized names
        private const string StandardName = "Standard";
        private const string OneSaberName = "OneSaber";
        private const string NoArrowsName = "NoArrows";

        private BS_Utils.Utilities.Config config;

        /// <summary>
        /// Indicates whether the user has selected to use NoArrowsRandom or not
        /// </summary>
        public bool NoArrowsOption { get; private set; }

        /// <summary>
        /// Called before Start or Updates by Unity infrastructure
        /// </summary>
        public void Awake()
        {
            config = new BS_Utils.Utilities.Config(Name);
            NoArrowsOption = config.GetBool(Name, Option, false, true);
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
            noArrowsRandomOption.AddConflict("MODIFIER_NO_ARROWS");
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
                // Check for game mode and early exit on One Saber or NoArrows
                GameplayCoreSceneSetupData data = BS_Utils.Plugin.LevelData?.GameplayCoreSceneSetupData;
                var beatmap = data.difficultyBeatmap;
                string serializedName = beatmap.parentDifficultyBeatmapSet.beatmapCharacteristic.serializedName;
                if (serializedName == OneSaberName || serializedName == NoArrowsName)
                {
                    // Do not transform for One Saber or legitimate No Arrows mode
                    Logging.Info($"Cannot transform song: {beatmap.level.songName} due to being a One Saber or No Arrows map");
                    yield break;
                }

                // Transform the loaded in-memory map
                GameplayCoreSceneSetup gameplayCoreSceneSetup = Resources.FindObjectsOfTypeAll<GameplayCoreSceneSetup>().First();
                if (gameplayCoreSceneSetup == null) yield break;

                BeatmapDataModel dataModel = gameplayCoreSceneSetup.GetField<BeatmapDataModel>("_beatmapDataModel");
                BeatmapData beatmapData = dataModel.beatmapData;

                Logging.Info("Disabling submission on NoArrowsOption turned on.");
                BS_Utils.Gameplay.ScoreSubmission.DisableSubmission("NoArrowsRandom");

                // Applying NoArrowsRandom transformation
                Logging.Info($"Transforming song: {beatmap.level.songName}");
                var transformedBeatmap = BeatmapDataNoArrowsTransform.CreateTransformedData(beatmapData, true);
                var beatmapDataModel = gameplayCoreSceneSetup.GetPrivateField<BeatmapDataModel>("_beatmapDataModel");
                beatmapDataModel.SetPrivateField("_beatmapData", transformedBeatmap);
            }
        }

        private void OnNoArrowsRandomOptionToggle(bool option)
        {
            NoArrowsOption = option;
            config.SetBool(Name, Option, option);
        }
    }
}
