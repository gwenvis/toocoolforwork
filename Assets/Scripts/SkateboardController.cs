using UnityEngine;
using System.Collections;
using System.Linq;

public class SkateboardController : MonoBehaviour
{

    [SerializeField]
    private float baseSpeed = 2.0f;
    [SerializeField]
    private float jumpHeight = 5.0f;
    [SerializeField]
    private Transform playerPosition;
    public ParticleSystem[] particles;
    bool particlesPlaying = false;

    private bool PlayerOnSkateboard = true;
    private bool isDead = false;
    private bool isGrounded = false;
    private bool wasGrounded = false;
    private bool grinding = false;
    private bool wasGrinding = false;
    private bool newAnim = false;

    private float currentSpeed;
    private Vector3 lastPosition;
    private Vector3 speed;

    private Rigidbody2D rigid;

    //Player Sprite and Gameobject
    [SerializeField]
    private GameObject player_go;
    [SerializeField]
    private GameObject player_sprite;
    [SerializeField]
    private GameObject skateboard_sprite;
    [SerializeField]
    private GameObject ragdoll;

    private GameObject mainCamera;
    private GameObject currentlyGrindingObject;

    //Sounds
    [Header("Sounds")]
    public AudioClip SkateLoop;
    public AudioClip SkateJump;
    public AudioClip SkateLand;
    public AudioClip SkateGrindLoop;
    public AudioClip SkateGrindLand;
    private AudioSource audio;
    private AudioSource audio2;

    //Animations
    private HashIDs hash;
    private Animator sAnim;
    private Animator pAnim;

    //State Enum
    private enum State
    {
        Skating,
        Preparing,
        Trick,
        Grinding,
        Falling,
        Landing,
        ShortLanding,
        Dying,
        Dead
    }

    private enum GrindTrick
    {

        Grind,
        ForwardGrind,
        BackGrind
    }

    private enum Tricks
    {
        Ollie,
        Kickflip,
        Heelflip
    }

    private State currentState = State.Skating;
    private Tricks currentTrick = Tricks.Ollie;
    private GrindTrick grindTrick = GrindTrick.Grind;

    bool Stuck = false;
    Vector3 stuckPosition;

    void Awake()
    {
        pAnim = player_sprite.GetComponent<Animator>();
        sAnim = skateboard_sprite.GetComponent<Animator>();
        currentSpeed = baseSpeed;
        rigid = GetComponent<Rigidbody2D>();
        audio = GetComponents<AudioSource>().ElementAt(0);
        audio2 = GetComponents<AudioSource>().ElementAt(1);
        hash = GameObject.FindGameObjectWithTag("GameController").GetComponent<HashIDs>();
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        //Time.timeScale = 0.4f;
    }

    void Reset()
    {
        currentState = State.Skating;
        isDead = false;
        currentSpeed = baseSpeed;
    }

    public void StopMoving(bool stopMoving, Vector3 stuck)
    {
        Stuck = stopMoving;
        stuckPosition = stuck;
        
    }

    void Update()
    {
        if(Stuck)
        {
            transform.position = stuckPosition;
            rigid.velocity = new Vector2(0, 0);
            return;
        }

        if (isDead || rigid == null)
            return;

        //rigid.velocity = new Vector2(baseSpeed, rigid.velocity.y);
        Vector3 wantedPosition = transform.position;
        speed = (gameObject.transform.position - lastPosition) / Time.deltaTime;
        currentSpeed += 0.1f * Time.deltaTime;
        wantedPosition.x += currentSpeed * Time.deltaTime;
        transform.position = wantedPosition;

        var hit = Physics2D.RaycastAll(playerPosition.position, -Vector2.up, .20f);
        Debug.DrawLine(playerPosition.position, playerPosition.position + -Vector3.up * .20f);

        if (hit.Length != 0)
        {
            isGrounded = hit.Any(x => x.collider.tag == "ground");

            if (hit.Any(x => x.collider.tag == "death"))
                Die();
        }
        else
            isGrounded = false;

        #region input


        //OLLIE TRICK || GRIND TRICK
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            if (isGrounded)
            {
                SwitchState(State.Preparing);
                currentTrick = Tricks.Ollie;
            }
            else
            {
                currentTrick = Tricks.Ollie;
                grindTrick = GrindTrick.Grind;
                Grind(hit);
            }

            if(!isGrounded && !grinding)
            {
                rigid.velocity = new Vector2(rigid.velocity.x, rigid.velocity.y - 10 * Time.deltaTime);
            }
        }
        else if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            if (isGrounded)
            {
                SwitchState(State.Preparing);
                currentTrick = Tricks.Kickflip;
            }
            else
            {
                currentTrick = Tricks.Kickflip;
                grindTrick = GrindTrick.ForwardGrind;
                Grind(hit);
            }

            if (!isGrounded && !grinding)
            {
                rigid.velocity = new Vector2(rigid.velocity.x, rigid.velocity.y - 10 * Time.deltaTime);
            }
        }
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            if (isGrounded)
            {
                SwitchState(State.Preparing);
                currentTrick = Tricks.Heelflip;
            }
            else
            {
                currentTrick = Tricks.Heelflip;
                grindTrick = GrindTrick.BackGrind;
                Grind(hit);
            }

            if (!isGrounded && !grinding)
            {
                rigid.velocity = new Vector2(rigid.velocity.x, rigid.velocity.y - 10 * Time.deltaTime);
            }
        }
        else
        {
            if (currentTrick == Tricks.Ollie && (currentState == State.Preparing || currentState == State.Grinding))
            {
                if (isGrounded || grinding)
                {
                    SwitchState(State.Trick);
                    grinding = false;
                    Jump();
                }
            }else if (currentTrick == Tricks.Heelflip && (currentState == State.Preparing || currentState == State.Grinding))
            {
                if (isGrounded || grinding)
                {
                    SwitchState(State.Trick);
                    grinding = false;
                    Jump();
                }
            }else if (currentTrick == Tricks.Kickflip && (currentState == State.Preparing || currentState == State.Grinding))
            {
                if (isGrounded || grinding)
                {
                    SwitchState(State.Trick);
                    grinding = false;
                    Jump();
                }
            }

        }

        //IF GROUNDED
        if (isGrounded && rigid.velocity.y < 0.00001)
        {
            if (currentState == State.Falling)
                SwitchState(State.ShortLanding);
            else if (currentState == State.Preparing)
                SwitchState(State.Preparing);
            else if (currentState == State.ShortLanding)
            {
                var info = pAnim.GetCurrentAnimatorStateInfo(0);
                if (info.normalizedTime > 0.98f)
                {
                    SwitchState(State.Skating);
                }
            }
            else
                SwitchState(State.Skating);
        }
        //IF GRINDING
        else if(grinding)
            SwitchState(State.Grinding);
        //IF FALLING
        else if (!isGrounded && rigid.velocity.y < -1.99)
        {
            currentState = State.Falling;
        }
        //IF NOT GROUNDED AND PREPARING TRICK
        else if (!isGrounded && currentState == State.Preparing)
            SwitchState(State.Skating);

        if (Input.GetKeyDown(KeyCode.Q))
            Die();

        if (Input.GetKeyDown(KeyCode.R))
            Application.LoadLevel(Application.loadedLevel);

        #endregion

        #region Animations
        switch (currentState)
        {
            case State.Skating:
                pAnim.Play(hash.skatingState);
                sAnim.Play(hash.skateSkatingState);
                break;
            case State.Preparing:
                pAnim.Play(hash.preparingState);
                sAnim.Play(hash.skateSkatingState);
                break;
            case State.Trick:
                if (currentTrick == Tricks.Ollie && newAnim)
                {
                    pAnim.Play(hash.ollieState, -1, 0);
                    sAnim.Play(hash.skateOllieState, -1, 0);
                }
                else if (currentTrick == Tricks.Kickflip && newAnim)
                {
                    pAnim.Play(hash.ollieState, -1, 0);
                    sAnim.Play(hash.skateKickflip, -1, 0);
                }
                else if (currentTrick == Tricks.Heelflip && newAnim)
                {
                    pAnim.Play(hash.ollieState, -1, 0);
                    sAnim.Play(hash.skateHeelflip, -1, 0);
                }
                newAnim = false;
                break;
            case State.Falling:
                pAnim.Play(hash.fallingState);
                break;
            case State.Dying:

                break;
            case State.Dead:

                break;
            case State.Landing:

                break;
            case State.ShortLanding:
                if (newAnim)
                {
                    pAnim.Play(hash.shortLandState, -1, 0);
                    sAnim.Play(hash.skateSkatingState, -1, 0);
                }
                newAnim = false;
                break;
            case State.Grinding:
                if (grindTrick == GrindTrick.Grind)
                {
                    sAnim.Play(hash.skateGrind);
                    pAnim.Play(hash.grindState);
                }
                else if (grindTrick == GrindTrick.ForwardGrind)
                {
                    sAnim.Play(hash.skateGrind2);
                    pAnim.Play(hash.grindFront);
                }
                else if (grindTrick == GrindTrick.BackGrind)
                {
                    sAnim.Play(hash.skateGrind2);
                    pAnim.Play(hash.grindBack);
                }
                break;
            default:

                break;

        }

        if (grinding)
            rigid.isKinematic = true;
        else
            rigid.isKinematic = false;
        #endregion

        #region Sound

        if (isGrounded)
        {
            //if (speed.x < 0.1)
            //    Die();

            if (!wasGrounded)
                audio2.PlayOneShot(SkateLand, 1);
            rigid.isKinematic = false;
            if (!audio.clip == SkateLoop)
            {
                audio.loop = true;
                audio.clip = SkateLoop;
                audio.Play();
            }
        }
        else if (grinding)
        {
            if (!wasGrinding)
                audio2.PlayOneShot(SkateGrindLand, 1);

            if (!particlesPlaying)
            {
                for (int i = 0; i < particles.Length; i++)
                {
                    particles[i].Play(true);
                }
                particlesPlaying = true;
            }
            if (!audio.clip == SkateGrindLoop)
            {
                audio.loop = true;
                audio.clip = SkateGrindLoop;
                audio.Play();
            }
        }
        else
        {
            rigid.isKinematic = false;
            if (particlesPlaying)
            {
                for (int i = 0; i < particles.Length; i++)
                {
                    particles[i].Stop(true);
                }
                particlesPlaying = false;
            }
            audio.Stop();
            audio.clip = null;
        }

        #endregion

        lastPosition = gameObject.transform.position;
        wasGrounded = isGrounded;
        wasGrinding = grinding;
    }

    private void Grind(RaycastHit2D[] hit)
    {

        var grindObject = hit.Where(x => x.collider.tag == "grindable").FirstOrDefault();

        if (grindObject.collider != null)
        {
            grinding = true;
            SwitchState(State.Grinding);
            var wantedTransform = transform.position;
            var grindScript = grindObject.collider.gameObject.GetComponent<Grind>();
            wantedTransform.y = grindScript.GetYPosition(gameObject.transform.position.x);
            if ((transform.position.y > wantedTransform.y - 0.1 && transform.position.y < wantedTransform.y + 0.15f || currentlyGrindingObject == grindObject.collider.gameObject) && transform.position.x < grindScript.pointB.transform.position.x)
            {
                transform.position = wantedTransform;
                currentlyGrindingObject = grindObject.collider.gameObject;
                grinding = true;
                rigid.isKinematic = true;

            }
            else
            {
                rigid.isKinematic = false;
                grinding = false;
            }
        }
        else
        {
            rigid.isKinematic = false;
            grinding = false;
        }
    }

    private void Jump()
    {
        var currentVelocity = rigid.velocity;
        currentVelocity.y = jumpHeight;
        rigid.velocity = currentVelocity;
        audio2.PlayOneShot(SkateJump, 1);
    }

    private void SwitchState(State state)
    {
        currentState = state;
        newAnim = true;
    }

    public void Die()
    {
        isDead = true;
        Vector3 playerPos;

        if (player_go != null)
        {
            playerPos = player_go.transform.position;
            playerPos.y += 0.2f;
        }
        else
            playerPos = Vector3.zero;

        var ragdollobject = Instantiate(ragdoll, playerPos, player_go.transform.rotation) as GameObject;
        var physicsApplier = ragdollobject.GetComponent<Ragdoll>();
        var forceToApply = new Vector2(currentSpeed, rigid.velocity.y + Random.Range(0f, 3f));
        physicsApplier.forceToApply = forceToApply * 50;
        Destroy(gameObject);
    }

    //public void OnTriggerStay2D(Collider2D collision)
    //{
    //    if(collision.tag == "grindable")
    //    {
    //        var grind = collision.gameObject.GetComponent<Grind>();

    //        if(grind != null)
    //        {
                
    //        }
    //    }

    //}
}
