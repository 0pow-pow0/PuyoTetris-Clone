using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;


public enum DetectedTouchInput
{
    NONE,
    STILL,
    LONG_STILL_TOUCH,
    SWIPE_UP,
    SWIPE_LEFT,
    SWIPE_RIGHT,
    SWIPE_DOWN
}


public class TouchManager : MonoBehaviour
{
    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;

    [SerializeField] 
    private float swipeTolerance = 50f;

    // ? --- Tempo da fermi prima di attivare il long press
    [SerializeField] 
    private float stillTimeTolerance;
    private float stillTimeTimer;

    // ? --- Viene ucciso ogni frame
    public static DetectedTouchInput thisFrameTouchInput { get; private set; } 
    //DetectedSwipe lastSwipe;

    void Awake()
    {
        InitSingleton();
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }


    // Update is called once per frame
    void Update()
    {
        thisFrameTouchInput = DetectedTouchInput.NONE;

        if (Touch.activeTouches.Count > 0)
        {
            Debug.Log("Touches: " + Touch.activeTouches.Count);
            Touch touch = Touch.activeTouches[0];

            switch (touch.phase)
            {
                case UnityEngine.InputSystem.TouchPhase.Began:
                    startTouchPosition = touch.screenPosition;
                    break;

                case UnityEngine.InputSystem.TouchPhase.Stationary:
                stillTimeTimer += Time.deltaTime;
                if(stillTimeTimer >= stillTimeTolerance)
                {
                    thisFrameTouchInput = DetectedTouchInput.LONG_STILL_TOUCH;
                }
                break;

                case UnityEngine.InputSystem.TouchPhase.Ended:
                    endTouchPosition = touch.screenPosition;
                    DetectSwipe();
                    break;
            }
        }
    }
    
    void DetectSwipe()
    {
        Vector2 delta = endTouchPosition - startTouchPosition;

        // ? --- Ignora swipe con spostamento minimo
        if (delta.magnitude < swipeTolerance)
            return;

        // ? --- Orizzontale
        if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
        {
            if (delta.x > 0)
            {
                thisFrameTouchInput = DetectedTouchInput.SWIPE_RIGHT;
                Debug.Log("Swipe Destra");
            }
            else
            {
                thisFrameTouchInput = DetectedTouchInput.SWIPE_LEFT;
                Debug.Log("Swipe Sinistra");
            }
        }
        // ? --- Verticale
        else
        {
            if (delta.y > 0)
            {
                thisFrameTouchInput = DetectedTouchInput.SWIPE_UP;
                Debug.Log("Swipe Alto");
            }
            else
            {
                thisFrameTouchInput = DetectedTouchInput.SWIPE_DOWN;
                Debug.Log("Swipe Basso");
            }
        }
    }
    
    // -------------------------------------------
    // ! Singleton shit
    // -------------------------------------------
    private static TouchManager inst;

    public static TouchManager Get()
    {
        if(inst == null)
        {
            Debug.LogError("TouchManager non instanziato!");    
            return null;
        }

        return inst;
    }

    void InitSingleton()
    {
        if(inst != null)
        {
            Debug.LogError("TouchManager gia' instanziato");
        }
        else
        {
            inst = this;
        }
    }


}
