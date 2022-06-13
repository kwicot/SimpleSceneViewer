using UnityEditor;
using UnityEngine;

namespace KwicotAssets.SimpleSceneViewer
{
    public class SceneViewerCustomAssetCallbacks : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            bool needUpdate = false;
            //Проверка не был ли добавлен новый файл разширения .unity
            foreach (var importedAsset in importedAssets)
            {
                if (importedAsset.Contains(".unity") && !importedAsset.Contains(".meta"))
                {
                    Debug.Log("One or more scenes was created, start update SceneManager");
                    needUpdate = true;
                }
            }
            
            //Проверка не был ли удалён файл разширения .unity
            foreach (var deletedAsset in deletedAssets)
            {
                if (deletedAsset.Contains(".unity") && !deletedAsset.Contains(".meta"))
                {
                    Debug.Log("One or more scenes was deleted, start update SceneManager");

                    needUpdate = true;
                }
            }
            
            //Проверка не был ли перемещён файл разширения .unity
            foreach (var movedAsset in movedAssets)
            {
                if (movedAsset.Contains(".unity") && !movedAsset.Contains(".meta"))
                {
                    Debug.Log("One or more scenes was moved, start update SceneManager");
                    needUpdate = true;
                }
            }
            
            if(needUpdate)
                SceneViewerCustomEditor.UpdateScenes();
        }
        
    }
}