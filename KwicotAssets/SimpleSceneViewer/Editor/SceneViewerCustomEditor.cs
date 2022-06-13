using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace KwicotAssets.SimpleSceneViewer
{
    public class SceneViewerCustomEditor : UnityEditor.Editor
    {
        static void StartInitialize()
        {
            UpdateScenes();
        }

        /// <summary>
        /// Update scenes after Add/Delete scenes
        /// </summary>
        public static void UpdateScenes()
        {
            var paths = GetScenes();
            DeleteAllScripts();
            GenerateScripts(paths);
            Debug.Log("Scenes update done");
        }
/// <summary>
/// Find all scenes on project and get theirs paths
/// </summary>
/// <returns>Scenes paths</returns>
        static List<string> GetScenes()
        {
            var assetsFolderPath = Application.dataPath;
            var folders = GetSubFolders(assetsFolderPath);
            List<string> scenes = new List<string>();
            foreach (var folder in folders)
                scenes.AddRange(GetScenesOnFolder(folder));

            var allScenes = new List<string>();
            foreach (var scene in scenes)
            {
                var newScenePath = scene.Replace(assetsFolderPath + @"\", "");
                allScenes.Add(newScenePath);
            }

            return allScenes;
        }
        /// <summary>
        /// Get all scenes on folder
        /// </summary>
        /// <param name="folderPath"></param>
        /// <returns>scenes paths</returns>
        static List<string> GetScenesOnFolder(string folderPath)
        {
            List<string> paths = new List<string>();
            var files = Directory.GetFiles(folderPath);
            foreach (var file in files) 
            {
                if(file.Contains(".unity") && !file.Contains(".meta"))
                    paths.Add(file);
            }

            return paths;
        }
        /// <summary>
        /// Get all sub folders paths
        /// </summary>
        /// <param name="folderPath"></param>
        /// <returns>sub folders paths</returns>
        static List<string> GetSubFolders(string folderPath)
        {
            List<string> allFolders = new List<string>();
            var folders = Directory.GetDirectories(folderPath);
            foreach (var folder in folders)
            {
                allFolders.Add(folder);
                
                var subFolders = Directory.GetDirectories(folder);
                if (subFolders.Length > 0)
                    allFolders.AddRange(GetSubFolders(folder));
            }

            return allFolders;
        }
        /// <summary>
        /// Generate editorScript for all finded scenes
        /// </summary>
        /// <param name="scenes"></param>
        static void GenerateScripts(List<string> scenes)
        {
            foreach (var _scenePath in scenes)
            {
                var scriptName = string.Empty;
                for (int i = _scenePath.Length - 1; i >= 0; i--)
                {
                    if(_scenePath[i] == '/' || _scenePath[i] == '\\')
                        break;

                    scriptName += _scenePath[i];
                }

                scriptName = scriptName.Replace(".unity", "");
                scriptName = scriptName.Replace('.', '_');
                scriptName = scriptName.Replace(',', '_');
                scriptName = scriptName.Replace(' ', '_');
                scriptName = scriptName.Replace(" ", "_");
                scriptName = scriptName.Replace('-', '_');


                var scenePath = Application.dataPath + "/" + _scenePath;
                
                scenePath = scenePath.Replace(@"\", "/");
                
                
                var windowName = _scenePath;
                
                windowName = windowName.Replace(".unity", "");
                windowName = windowName.Replace(@"\", "/");
                
                var text = $"using UnityEditor; " +
                           "using UnityEditor.SceneManagement; " +
                           $"public class {scriptName} : UnityEditor.Editor " +
                           "{ " +
                           $"private static string path = \"{scenePath}\"; " +
                           $"[MenuItem(\"Tools/ScenesManager/{windowName}\")] " +
                           "public static void Open() " +
                           "{ " +
                           "if (!EditorSceneManager.GetActiveScene().isDirty)" +
                "{" +
                    "EditorSceneManager.OpenScene(path);" +
                    "return;" +
                "}" +
                           "if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())" +
                           "EditorSceneManager.SaveOpenScenes();" +
                           " " +
                           "EditorSceneManager.OpenScene(path); " +
                           "} " +
                           "}";
                var folderPath = Application.dataPath + "/Plugins/KwicotAssets/SimpleSceneViewer/Editor/Scenes";

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                var path = folderPath + $"/{scriptName}.cs";
                CreateScript(path, text);
            }

            AssetDatabase.Refresh();
        }
        
        static void CreateScript(string path, string text)
        {
            if (File.Exists(path))
                File.Delete(path);
            
            var fs = File.Create(path);
            fs.Close();

            File.WriteAllText(path,text);
            
        }

        static void DeleteAllScripts()
        {
            var folder = Application.dataPath + "/Plugins/KwicotAssets/SimpleSceneViewer/Editor/Scenes";
            if (Directory.Exists(folder))
            {
                var files = Directory.GetFiles(folder);
                foreach (var file in files)
                    File.Delete(file);
            }
        }
    }

    // public class SceneViewer_Scene : UnityEditor.Editor
    // {
    //     private static string path = "";
    //     public static void OpenScene()
    //     {
    //         if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
    //             EditorSceneManager.SaveOpenScenes();
    //         else
    //             EditorSceneManager.OpenScene(path);
    //     }
    // }
}