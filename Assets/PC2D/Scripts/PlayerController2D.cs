using UnityEngine;

/// <summary>
/// This class is a simple example of how to build a controller that interacts with PlatformerMotor2D.
/// </summary>
[RequireComponent(typeof(PlatformerMotor2D)), RequireComponent(typeof(Player))]
public class PlayerController2D : MonoBehaviour
{
    private Player _player;
    private PlatformerMotor2D _motor;
    private bool _restored = true;
    private bool _enableOneWayPlatforms;
    private bool _oneWayPlatformsAreWalls;
    private bool _jumpBelowOneWay;

    public float _oneWayDropGrace = 0.35f;
    private float _curOneWayDropGrace;


    public KeyCode _left;
    public KeyCode _right;
    public KeyCode _jump;
        public KeyCode _up;
    public KeyCode _down;
    public KeyCode _dash;
    public KeyCode _attack;


    // Use this for initialization
    void Start()
    {
        _player = GetComponent<Player>();
        _motor = GetComponent<PlatformerMotor2D>();
        _enableOneWayPlatforms = _motor.enableOneWayPlatforms;
        _oneWayPlatformsAreWalls = _motor.oneWayPlatformsAreWalls;
    }

    // before enter en freedom state for ladders
    void FreedomStateSave(PlatformerMotor2D motor)
    {
        if (!_restored) // do not enter twice
            return;

        _restored = false;
        _enableOneWayPlatforms = _motor.enableOneWayPlatforms;
        _oneWayPlatformsAreWalls = _motor.oneWayPlatformsAreWalls;
    }
    // after leave freedom state for ladders
    void FreedomStateRestore(PlatformerMotor2D motor)
    {
        if (_restored) // do not enter twice
            return;

        _restored = true;
        _motor.enableOneWayPlatforms = _enableOneWayPlatforms;
        _motor.oneWayPlatformsAreWalls = _oneWayPlatformsAreWalls;
    }

    // Update is called once per frame
    void Update()
    {

        // use last state to restore some ladder specific values
        if (_motor.motorState != PlatformerMotor2D.MotorState.FreedomState)
        {
            // try to restore, sometimes states are a bit messy because change too much in one frame
            FreedomStateRestore(_motor);
        }

        if (_enableOneWayPlatforms)
        {
            if (_jumpBelowOneWay)
            {
                _curOneWayDropGrace -= Time.deltaTime;
                if (_curOneWayDropGrace <= 0)
                {
                    Debug.Log("Jump reset");
                    _jumpBelowOneWay = false;
                    _motor.oneWayPlatformsAreWalls = true;
                }
            }
        }
        float hori = 0;
        float vert = 0;

        if (Input.GetKey(_right)) hori += 1;
        if (Input.GetKey(_left)) hori -= 1;

        if (Input.GetKey(_up)) vert += 1;
        if (Input.GetKey(_down)) vert -= 1;
        // Jump?
        // If you want to jump in ladders, leave it here, otherwise move it down
        if (Input.GetKeyDown(_jump))
        {
            //drop from oneway
            if (_motor.IsOnGround() && _motor.IsOnOneWay && vert < 0)
            {
                if (_enableOneWayPlatforms )
                {

                    if (!_jumpBelowOneWay)
                    {
                        Debug.Log("Jump Below");
                        _jumpBelowOneWay = true;
                        _motor.oneWayPlatformsAreWalls = false;
                        _curOneWayDropGrace = _oneWayDropGrace;
                        _motor.NoHoldJump(0.1f);
                    } 
                    else
                    {
                        _jumpBelowOneWay = false;
                        _motor.oneWayPlatformsAreWalls = false;
                        _curOneWayDropGrace = 0;
                    }
                   
                    //_motor.ForceJump(-_motor.jumpHeight);
                    //FreedomStateSave(_motor);

                }
            }
            else
            {
                _motor.Jump();
                _motor.DisableRestrictedArea();
            }
        }

        _motor.jumpingHeld = Input.GetKey(_jump);

        // XY freedom movement
        if (_motor.motorState == PlatformerMotor2D.MotorState.FreedomState)
        {
            _motor.normalizedXMovement = hori;
            _motor.normalizedYMovement = vert;

            return; // do nothing more
        }

        // X axis movement
        if (Mathf.Abs(hori) > PC2D.Globals.INPUT_THRESHOLD)
        {
            _motor.normalizedXMovement = hori;
        }
        else
        {
            _motor.normalizedXMovement = 0;
        }

        if (vert != 0)
        {
            bool up_pressed = vert > 0;
            if (_motor.IsOnLadder())
            {
                if (
                    (up_pressed && _motor.ladderZone == PlatformerMotor2D.LadderZone.Top)
                    ||
                    (!up_pressed && _motor.ladderZone == PlatformerMotor2D.LadderZone.Bottom)
                 )
                {
                    // do nothing!
                }
                // if player hit up, while on the top do not enter in freeMode or a nasty short jump occurs
                else
                {
                    // example ladder behaviour

                    _motor.FreedomStateEnter(); // enter freedomState to disable gravity
                    _motor.EnableRestrictedArea();  // movements is retricted to a specific sprite bounds

                    // now disable OWP completely in a "trasactional way"
                    FreedomStateSave(_motor);
                    _motor.enableOneWayPlatforms = false;
                    _motor.oneWayPlatformsAreWalls = false;

                    // start XY movement
                    _motor.normalizedXMovement = hori;
                    _motor.normalizedYMovement = vert;
                }
            }
        }
        else if (vert < -PC2D.Globals.FAST_FALL_THRESHOLD)
        {
            _motor.fallFast = false;
        }

        if (Input.GetKeyDown(_dash))
        {
            _motor.Dash();
        }

        if (Input.GetKeyDown(_attack))
        {
            _player.Attack();
        }

    }
}
