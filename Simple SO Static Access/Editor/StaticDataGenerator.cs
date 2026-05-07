namespace QuietNoize.SimpleSOStaticAccess.Editor
{
#if UNITY_EDITOR
    using System.IO;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// Context for static data code generation.
    /// Contains source StaticDataSO and generation settings.
    /// </summary>
    public struct StaticDataGenerationContext
    {
        public StaticDataSO Source;
        public StaticDataCodegenSettings Settings;
    }

    /// <summary>
    /// Generates and syncs static C# wrapper code from StaticDataSO assets.
    /// Handles file output and Unity AssetDatabase refresh.
    /// </summary>
    public static class StaticDataGenerator
    {
        #region Public API
        /// <summary>
        /// Generates C# static wrapper class from the provided StaticDataSO
        /// and writes it to disk, then refreshes Unity AssetDatabase.
        /// </summary>
        public static void SyncConstants(StaticDataSO so)
        {
            if (so == null)
            {
                Debug.LogError("[StaticDataGenerator.SyncConstants] Settings is null");
                return;
            }

            var ctx = new StaticDataGenerationContext
            {
                Source = so,
                Settings = so.codegenSettings
            };

            if (string.IsNullOrWhiteSpace(ctx.Settings.WrapperClassName))
            {
                Debug.LogError(
                    $"[StaticDataGenerator] Cannot generate code for '{ctx.Source.name}'. " +
                    $"WrapperClassName is not set in StaticDataCodegenSettings."
                    );
                return;
            }

            string folder = GetOutputFolder(ctx);
            string outputPath = Path.Combine(folder, $"{ctx.Settings.WrapperClassName}.cs");

            string code = StaticDataCodeBuilder.GetCode(ctx);
            code = NormalizeNewLines(code);
            File.WriteAllText(outputPath, code);
            AssetDatabase.Refresh();

            Debug.Log($"[StaticDataGenerator.SyncConstants] Data code synced at: {outputPath}");
        }
        #endregion

        #region Helpers
        private static string GetOutputFolder(StaticDataGenerationContext ctx)
        {
            if (string.IsNullOrEmpty(ctx.Settings.WrapperCodePath))
            {
                string assetPath = AssetDatabase.GetAssetPath(ctx.Source);
                return Path.GetDirectoryName(assetPath);
            }

            return ctx.Settings.WrapperCodePath;
        }

        private static string NormalizeNewLines(string s)
        {
            return s.Replace("\r\n", "\n").Replace("\r", "\n");
        }
        #endregion
    }
#endif
}

