using System.IO;
using UnityEngine;

namespace Universal.Loading.UI
{
    [System.Serializable]
    public class LoadTexturesCommand : LoadUICommand<Texture2D[]>
    {
        public override Texture2D[] Execute()
        {
            if(CommandFailed()) return null;
            
            Texture2D[] files = new Texture2D[fileNames.Length];
            for (int i = 0; i < files.Length; i++)
                files[i] = Resources.Load<Texture2D>(path+fileNames[i]);
            
            return files;
        }
    }

    public abstract class LoadUICommand<T>
    {
        [SerializeField] internal string basePath;
        [SerializeField] internal string[] fileNames;
        internal LoadUIManager manager;
        internal string path;

        public abstract T Execute();

        internal virtual bool CommandFailed()
        {
            bool failed = basePath == "";
            failed |= (fileNames == null || fileNames.Length == 0);
            if (!manager)
            {
                manager = LoadUIManager.inst;
                path = basePath + manager.platform;
                failed |= !manager;
            }
            
            if(failed)
                Debug.LogError("LoadUICommand failed to initialize");

            return failed;
        }
    }
}
