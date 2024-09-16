using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Universal.Loading.UI
{
    public class LoadUIManager : MonoBehaviour
    {
        public static LoadUIManager inst;
        
        //[Header("Set Values")]
        [SerializeField] bool forceMobile;
        //[Header("Runtime Values")]

        public string platform { get; private set; }
        
        //Unity Events
        void Awake()
        {
            //Singleton initialization
            if(inst) 
            {
                Destroy(this);
                return;
            }
            inst = this;
            
            //Get platform
            bool isMobile = Application.isMobilePlatform || forceMobile;
            platform = (isMobile ? "Mobile" : "Computer") + '/';
        }
        void OnDestroy()
        {
            if(inst == this)
                inst = null;
        }

        //Methods
    }
}