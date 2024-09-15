using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Universal.Loading.Images
{
    public class LoadSpecs
    {
        public LoadSpecs(bool isMobile)
        {
            this.isMobile = isMobile;
            
            path = isMobile ? "Mobile/" : "Computer/";
        }
        
        public string path { get; private set; }
        public bool isMobile { get; private set; }
    }
    
    public class ImgLoadersManager : MonoBehaviour
    {
        //[Header("Set Values")]
        [SerializeField] bool forceMobile;
        //[Header("Runtime Values")]
        LoadSpecs loadSpecs;

        //Unity Events
        void Awake()
        {
            bool isMobile = forceMobile ? true : Application.isMobilePlatform;
            
            loadSpecs = new LoadSpecs(isMobile);
            
            //Get roots objs
            GameObject[] roots = SceneManager.GetActiveScene().GetRootGameObjects();
            
            List<IImgLoaderUser> users = new List<IImgLoaderUser>();
            for (int i = 0; i < roots.Length; i++)
                users.AddRange(roots[i].GetComponentsInChildren<IImgLoaderUser>(true));
            
            for (int i = 0; i < users.Count; i++)
                users[i].SetImgLoaders(loadSpecs);
        }

        //Methods
    }
}