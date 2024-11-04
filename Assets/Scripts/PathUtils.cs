using System.IO;
using UnityEngine;

public static class PathUtils
{
    public static string ApplicationPath
    {
        get
        {
            var root = Application.persistentDataPath;
#if UNITY_EDITOR
            root = Path.Combine(Application.dataPath, "..", "SaveData");
#endif
            return root;
        }
    }
}