namespace QuietNoize.SimpleSOStaticAccess.Editor
{
    using UnityEditor;
    using UnityEngine;
    using System.IO;

    /// <summary>
    /// Creates new StaticDataSO script templates directly from the Unity
    /// Project window context menu.
    /// </summary>
    public static class StaticDataSOScriptCreator
    {
        private const string TEMPLATE =
    @"using QuietNoize.SimpleSOStaticAccess;
using UnityEngine;

[CreateAssetMenu(menuName = ""Static Data/#SCRIPTNAME#"")]
public class #SCRIPTNAME# : StaticDataSO
{

}";

        /// <summary>
        /// Creates a new C# script that inherits from StaticDataSO
        /// in the currently selected project folder.
        /// </summary>
        [MenuItem("Assets/Create/Scripting/Static Data SO", false, 80)]
        public static void Create()
        {
            string folder = GetSelectedFolder();

            string fileName = "NewStaticDataSO.cs";
            string fullPath = Path.Combine(folder, fileName);

            string script = TEMPLATE.Replace("#SCRIPTNAME#", "NewStaticDataSO");

            File.WriteAllText(fullPath, script);

            AssetDatabase.Refresh();

            var asset = AssetDatabase.LoadAssetAtPath<Object>(fullPath);
            Selection.activeObject = asset;
        }

        /// <summary>
        /// Returns the currently selected folder in the Project window.
        /// Falls back to the Assets root folder if nothing is selected.
        /// </summary>
        private static string GetSelectedFolder()
        {
            Object selected = Selection.activeObject;

            if (selected == null)
                return "Assets";

            string path = AssetDatabase.GetAssetPath(selected);

            if (File.Exists(path))
                return Path.GetDirectoryName(path);

            return path;
        }
    }
}