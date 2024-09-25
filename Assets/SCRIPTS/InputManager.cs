using Terresquall;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager inst;
    
    [SerializeField] VirtualJoystick Joystick1;
    [SerializeField] VirtualJoystick Joystick2;
    [SerializeField] bool forceMobile = false;
    bool isMobile;

    public bool has2Players;
    public Vector2 Axis1 { get; private set; }
    public Vector2 Axis2 { get; private set; }
    
    void Awake()
    {
        if (inst != null)
        {
            Destroy(this);
            return;
        }
        
        inst = this;
    }
    void OnDestroy()
    {
        if(inst == this)
            inst = null;
    }
    void Start()
    {
        isMobile = Application.isMobilePlatform || forceMobile;
        
        if(!isMobile)
        {
            Joystick1.gameObject.SetActive(false);
            if(has2Players)
                Joystick2.gameObject.SetActive(false);
        }
    }
    void Update()
    {
        if (isMobile)
        {
            Axis1 = Joystick1.GetAxis();
            if(has2Players)
                Axis2 = Joystick2.GetAxis();
        }
        else
        {
            //Get axis of player 1
            Vector2 axis = new Vector2();
            axis.x = Input.GetKey(KeyCode.A) ? -1 : Input.GetKey(KeyCode.D) ? 1 : 0;
            axis.y = Input.GetKey(KeyCode.W) ? 1 : Input.GetKey(KeyCode.S) ? -1 : 0;
            Axis1 = axis;
            
            if(!has2Players) return;
                
            //Get axis of player 2
            axis = new Vector2();
            axis.x = Input.GetKey(KeyCode.LeftArrow) ? -1 : 
                        Input.GetKey(KeyCode.RightArrow) ? 1 : 0;
            axis.y = Input.GetKey(KeyCode.UpArrow) ? 1 : 
                        Input.GetKey(KeyCode.DownArrow) ? -1 : 0;
            Axis2 = axis;
        }
    }
}