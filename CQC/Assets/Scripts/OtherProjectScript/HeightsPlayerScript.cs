//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class PlayerScript : MonoBehaviour
//{
//    //Player Controller
//    [Header("Player Movement")]

//    public GameObject player;
//    public CapsuleCollider playerHB;
//    public Rigidbody rig, shipRig;
//    public float moveSpeed, sprintMultiplier, airstrafeSpeed, strafeForwardBias, jumpForce, doubleJumpVerticalForce, doubleJumpHorizontalForce, steepestAngle, jumpCoolDown, decelSpeed, interactionDelay, interactionBodyLock;
//    public float maxFallDistance;
//    public bool piloting, onBoat, okToJump, okToDoubleJump;
//    private Vector3 moveVec = Vector3.zero, airstrafeVec = Vector3.zero;
//    private Vector3 velocityCapVec;
//    private float groundAngle, m1HoldLength, m1Sine, correctedMaxFallDistance;
//    private int groundedLoop;
//    public float maxRCDistance;
//    public float maxGCRadius, maxGCDistance, overlapSphereOffset, jumpBuffer;
//    private bool playerFreeze, isSprinting;
//    public Animator anim;
//    public float animMoveFloat, animReelFloat;
//    public float animMoveScaleSpeed, animReelScaleSpeed;

//    [HideInInspector]
//    public float playerFreezeTimer;

//    //Shooting
//    [Header("Utilities")]
//    public GameObject impactMarker;
//    public Ray rayOrigin;
//    public RaycastHit hit;
//    public GameObject[] toolBar = new GameObject[3];
//    public enum EquipSpace { One, Two, Three, Four };
//    public EquipSpace equippedSlot;
//    public bool firstEquipBool = true;

//    //Fishing
//    [Header("Fishing")]
//    public FishMinigame fishingMini;
//    public InventoryManager invManager;
//    public GameObject hotbarOutline;
//    public HotBarItem[] hotbarSlots;
//    public GameObject fishingHook;
//    public GameObject hookedFish;
//    private Rigidbody fishingHookRig;
//    private HookScript hookScript;
//    public float castForce, castUpForce, castChargeMin, castChargeMax, castChargeAccel, reelSpeed, grappleSpeed, minimumReelDistance, minimumGrappleDistance, shockMaxTime;
//    private float unpauseDelay = 0.3f, unpauseNewTime, shockCurrentTime;
//    public bool reelingIn = false, reelOfShame = false, isFishing = false, isGrappling = false, isShocked = false;
//    private bool capBool;
//    private bool powerRising = true;//Jake
//    public LearnUIFill powerGauge;
//    public GameObject powerMeter;
//    public GameObject ChargePromptTextGO;

//    //Camera Controller
//    [Header("Camera Controller")]

//    public GameObject fPC;
//    public GameObject tPC, cC;
//    public bool invertedCamera;
//    public float camCooldown, firstPersonMinYCamLock, firstPersonMaxYCamLock, thirdPersonMinYCamLock, thirdPersonMaxYCamLock, mSpeedX, mSpeedY;
//    public bool fBool, tBool;
//    [HideInInspector]
//    public float yaw, pitch = 0.0f;

//    //Audio
//    [Header("Audio")]
//    public AudioSource audioSource;
//    public AudioClip castLineSound;
//    public AudioClip catchCollectableSound;
//    public AudioClip caughtFishSound;
//    public AudioClip fishReelSound;
//    public AudioClip fishPullFailSound;
//    private bool reelOnce = false;
//    public AudioClip hookLatchSound;
//    public AudioClip furnaceOpeningSound;
//    [Header("Menu")]
//    public GameObject pauseMenuGO;
//    public Menu_Play menuScript;
//    public bool isPaused;

//    private bool canPause = true;
//    public GameObject flowChart;
//    public GameObject dropPoint;
//    public GameObject TPGO;

//    public GameObject furnaceTextGO;
//    public GameObject furnaceFullTextGO;

//    public GameObject playerHolder; // Jake
//    public Sprite defaultSlotImage;
//    public ParticleSystem fireBurst1;
//    public ParticleSystem fireBurst2;


//    // Start is called before the first frame update
//    void Start()
//    {

//        rig = GetComponent<Rigidbody>();
//        try
//        {
//            fPC = GameObject.Find("First-Person Camera");
//        }
//        catch { }
//        try
//        {
//            tPC = GameObject.Find("Third-Person Camera");
//        }
//        catch { }
//        try
//        {
//            cC = GameObject.Find("Camera Case");
//        }
//        catch
//        {
//            Debug.LogWarning("I couldn't find a camera case!");
//        }
//        try
//        {
//            anim = GetComponent<Animator>();
//        }
//        catch
//        {
//            Debug.LogWarning("I couldn't find an Animator!");
//        }


//        try
//        {
//            fishingHook = GameObject.Instantiate(fishingHook);
//            fishingHookRig = fishingHook.GetComponent<Rigidbody>();
//            fishingHook.SetActive(false);
//        }
//        catch
//        {
//            Debug.LogWarning("I couldn't find a fishing hook!");
//        }


//        fPC.SetActive(true);

//        fBool = true;
//        tBool = false;
//        Cursor.lockState = CursorLockMode.Locked;
//        Cursor.visible = false;
//        piloting = false;
//        playerFreeze = false;

//        hookScript = fishingHook.GetComponent<HookScript>();

//        try
//        {
//            if (pauseMenuGO != null)
//            {
//                pauseMenuGO.SetActive(false);
//                isPaused = false;
//            }
//        }
//        catch
//        {
//            Debug.LogWarning("Either no pause menu has been attached or something is preventing it from being disabled");
//        }
//        correctedMaxFallDistance = player.transform.position.y + maxFallDistance;

//        hotbarOutline.transform.position = new Vector3(hotbarSlots[invManager.GetHotBarLocation()].GetObject().transform.position.x,
//                                        hotbarSlots[invManager.GetHotBarLocation()].GetObject().transform.position.y,
//                                        hotbarSlots[invManager.GetHotBarLocation()].GetObject().transform.position.z);
//    }

//    private void FixedUpdate()
//    {
//        if (!isPaused)
//        {
//            #region Player Movement
//            if (!piloting)
//            {
//                switch (okToJump && !isGrappling)
//                {
//                    case true:
//                        if (onBoat)
//                        {
//                            if (!playerFreeze && !isShocked)
//                            {
//                                if (Input.GetAxisRaw("Horizontal") != 0 && Input.GetAxisRaw("Vertical") != 0)
//                                {
//                                    moveVec = transform.right * Input.GetAxisRaw("Horizontal") + transform.forward * Input.GetAxisRaw("Vertical");
//                                    if (isSprinting)
//                                    {
//                                        moveVec = new Vector3(moveVec.x * ((moveSpeed * sprintMultiplier) * 1 - strafeForwardBias) * Time.fixedDeltaTime, rig.velocity.y, moveVec.z * ((moveSpeed * sprintMultiplier) * strafeForwardBias) * Time.fixedDeltaTime);
//                                        if (animMoveFloat > 1)
//                                        {
//                                            animMoveFloat += animMoveScaleSpeed;
//                                        }
//                                        else if (animMoveFloat < 1)
//                                        {
//                                            animMoveFloat = 1;
//                                        }
//                                    }
//                                    else
//                                    {
//                                        moveVec = new Vector3(moveVec.x * (moveSpeed * 1 - strafeForwardBias) * Time.fixedDeltaTime, rig.velocity.y, moveVec.z * (moveSpeed * strafeForwardBias) * Time.fixedDeltaTime);
//                                        if (animMoveFloat < 0.4f)
//                                        {
//                                            animMoveFloat += animMoveScaleSpeed;
//                                        }
//                                        else if (animMoveFloat > 0.6f)
//                                        {
//                                            animMoveFloat -= animMoveScaleSpeed;
//                                        }
//                                        else
//                                        {
//                                            animMoveFloat = 0.5f;

//                                        }
//                                    }
//                                    anim.SetFloat("Move", animMoveFloat);
//                                    anim.SetFloat("Jumping", animMoveFloat);

//                                    //rig.velocity = moveVec + new Vector3(shipRig.velocity.x, 0, shipRig.velocity.z);
//                                    rig.velocity = moveVec + shipRig.velocity;

//                                }
//                                else if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
//                                {
//                                    moveVec = transform.right * Input.GetAxisRaw("Horizontal") + transform.forward * Input.GetAxisRaw("Vertical");

//                                    if (isSprinting)
//                                    {
//                                        moveVec = new Vector3(moveVec.x * ((moveSpeed * sprintMultiplier)) * Time.fixedDeltaTime, rig.velocity.y, moveVec.z * ((moveSpeed * sprintMultiplier)) * Time.fixedDeltaTime);
//                                        if(animMoveFloat > 1)
//                                        {
//                                            animMoveFloat += animMoveScaleSpeed;
//                                        }
//                                        else if(animMoveFloat < 1)
//                                        {
//                                            animMoveFloat = 1;
//                                        }
//                                    }
//                                    else
//                                    {
//                                        moveVec = new Vector3(moveVec.x * (moveSpeed) * Time.fixedDeltaTime, rig.velocity.y, moveVec.z * (moveSpeed) * Time.fixedDeltaTime);
//                                        if (animMoveFloat < 0.4f)
//                                        {
//                                            animMoveFloat += animMoveScaleSpeed;
//                                        }
//                                        else if (animMoveFloat > 0.6f)
//                                        {
//                                            animMoveFloat -= animMoveScaleSpeed;
//                                        }
//                                        else
//                                        {
//                                            animMoveFloat = 0.5f;

//                                        }
//                                    }
//                                    //rig.velocity = moveVec + new Vector3(shipRig.velocity.x, 0, shipRig.velocity.z);
//                                    rig.velocity = moveVec + shipRig.velocity;

//                                }
//                                else
//                                {
//                                    //rig.velocity = new Vector3(shipRig.velocity.x, 0, shipRig.velocity.z);
//                                    rig.velocity = shipRig.velocity;

//                                    if (animMoveFloat > 0)
//                                    {
//                                        animMoveFloat -= animMoveScaleSpeed;
//                                    }
//                                    else if (animMoveFloat < 0)
//                                    {
//                                        animMoveFloat = 0;
//                                    }
//                                }

//                            }
//                            else
//                            {
//                                //rig.velocity = new Vector3(shipRig.velocity.x, 0, shipRig.velocity.z);
//                                if (animMoveFloat > 0)
//                                {
//                                    animMoveFloat -= animMoveScaleSpeed;
//                                }
//                                else if (animMoveFloat < 0)
//                                {
//                                    animMoveFloat = 0;
//                                }
//                            }

//                        }
//                        else
//                        {
//                            if (!playerFreeze && !isShocked)
//                            {
//                                if (Input.GetAxisRaw("Horizontal") != 0 && Input.GetAxisRaw("Vertical") != 0)
//                                {
//                                    //print("Horizontal: " + Input.GetAxisRaw("Horizontal") + " Vertical: " + Input.GetAxisRaw("Vertical"));
//                                    moveVec = transform.right * Input.GetAxisRaw("Horizontal") + transform.forward * Input.GetAxisRaw("Vertical");

//                                    //moveVec = (transform.right * Input.GetAxisRaw("Horizontal") * (moveSpeed * 1 - strafeForwardBias)) + (transform.forward * Input.GetAxisRaw("Vertical") * (moveSpeed * strafeForwardBias));
//                                    if (isSprinting)
//                                    {
//                                        moveVec = new Vector3(moveVec.x * ((moveSpeed * sprintMultiplier) * 1 - strafeForwardBias) * Time.fixedDeltaTime, rig.velocity.y, moveVec.z * ((moveSpeed * sprintMultiplier) * strafeForwardBias) * Time.fixedDeltaTime);
//                                        if (animMoveFloat > 1)
//                                        {
//                                            animMoveFloat += animMoveScaleSpeed;
//                                        }
//                                        else if (animMoveFloat < 1)
//                                        {
//                                            animMoveFloat = 1;
//                                        }
//                                    }
//                                    else
//                                    {
//                                        moveVec = new Vector3(moveVec.x * (moveSpeed * 1 - strafeForwardBias) * Time.fixedDeltaTime, rig.velocity.y, moveVec.z * (moveSpeed * strafeForwardBias) * Time.fixedDeltaTime);
//                                        if (animMoveFloat < 0.4f)
//                                        {
//                                            animMoveFloat += animMoveScaleSpeed;
//                                        }
//                                        else if (animMoveFloat > 0.6f)
//                                        {
//                                            animMoveFloat -= animMoveScaleSpeed;
//                                        }
//                                        else
//                                        {
//                                            animMoveFloat = 0.5f;

//                                        }
//                                    }
//                                    rig.velocity = moveVec;
//                                }
//                                else if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
//                                {
//                                    moveVec = transform.right * Input.GetAxisRaw("Horizontal") + transform.forward * Input.GetAxisRaw("Vertical");

//                                    if (isSprinting)
//                                    {
//                                        moveVec = new Vector3(moveVec.x * ((moveSpeed * sprintMultiplier)) * Time.fixedDeltaTime, rig.velocity.y, moveVec.z * ((moveSpeed * sprintMultiplier)) * Time.fixedDeltaTime);
//                                        if (animMoveFloat > 1)
//                                        {
//                                            animMoveFloat += animMoveScaleSpeed;
//                                        }
//                                        else if (animMoveFloat < 1)
//                                        {
//                                            animMoveFloat = 1;
//                                        }
//                                    }
//                                    else
//                                    {
//                                        moveVec = new Vector3(moveVec.x * (moveSpeed) * Time.fixedDeltaTime, rig.velocity.y, moveVec.z * (moveSpeed) * Time.fixedDeltaTime);
//                                        if (animMoveFloat < 0.4f)
//                                        {
//                                            animMoveFloat += animMoveScaleSpeed;
//                                        }
//                                        else if (animMoveFloat > 0.6f)
//                                        {
//                                            animMoveFloat -= animMoveScaleSpeed;
//                                        }
//                                        else
//                                        {
//                                            animMoveFloat = 0.5f;

//                                        }
//                                    }
//                                    rig.velocity = moveVec;
//                                }
//                                else
//                                {
//                                    rig.velocity *= decelSpeed;
//                                    if (animMoveFloat > 0)
//                                    {
//                                        animMoveFloat -= animMoveScaleSpeed;
//                                    }
//                                    else if(animMoveFloat < 0)
//                                    {
//                                        animMoveFloat = 0;
//                                    }
//                                }
//                            }
//                            else
//                            {
//                                rig.velocity *= decelSpeed;
//                                if (isShocked)
//                                {
//                                    //Shock animation trigger here
//                                }
//                            }
//                        }
//                        break;
//                    case false:
//                        if (!playerFreeze)
//                        {
//                            airstrafeVec = transform.right * Input.GetAxisRaw("Horizontal") + transform.forward * Input.GetAxisRaw("Vertical");
//                            rig.AddForce(airstrafeVec * airstrafeSpeed * Time.fixedDeltaTime);
//                            if (animMoveFloat > 1)
//                            {
//                                animMoveFloat = 1;
//                            }
//                            else if(animMoveFloat < 1)
//                            {
//                                animMoveFloat += animMoveScaleSpeed;
//                            }
//                        }
//                        break;

//                }
//                anim.SetFloat("Move", animMoveFloat);
//                anim.SetFloat("Jumping", animMoveFloat);

//            }
//            #endregion
//        }
//    }

//    // Update is called once per frame
//    void Update()
//    {

//        if (!isPaused)
//        {
//            playerFreeze = playerFreezeTimer >= Time.time;
//            shockCurrentTime -= Time.deltaTime * Time.timeScale;
//            isShocked = shockCurrentTime > 0;


//            rayOrigin = Camera.main.ScreenPointToRay(Input.mousePosition);

//            #region Ground Check
//            //Checks to see if the player is standing on level (or level enough) ground
//            Physics.SphereCast(player.transform.position, maxGCRadius, -player.transform.up, out hit, maxGCDistance, ~8, QueryTriggerInteraction.Ignore);
//            groundAngle = Vector3.Angle(player.transform.up, hit.normal);
//            bool lastJumpState = okToJump;
//            okToJump = steepestAngle >= groundAngle && hit.transform != null;
//            anim.SetBool("IsFalling", !okToJump);
//            if (okToJump)
//            {
//                anim.SetBool("IsJumping", !okToJump);
//            }
//            if (lastJumpState != okToJump && !Input.GetKeyDown(KeyCode.Space))
//            {
//                animMoveFloat = .5f;
//            }


//            if (hit.transform != null)
//            {
//                if (hit.transform.CompareTag("Ship"))
//                    onBoat = true;
//            }
//            else
//            {
//                onBoat = false;
//            }
//            if (okToJump)
//            {
//                correctedMaxFallDistance = player.transform.position.y + maxFallDistance;
//                if (groundedLoop == 0)
//                {
//                    okToDoubleJump = true;
//                    //rig.velocity = new Vector3(0,rig.velocity.y,0);
//                    groundedLoop++;
//                }
//            }
//            else if (!okToJump && groundedLoop > 0)
//            {
//                groundedLoop = 0;
//            }
//            #endregion

//            #region Camera Rotation
//            //Gets the horizontal and vertical movement of the mouse, translates it into camera movement.
//            yaw += mSpeedX * Input.GetAxis("Mouse X");
//            if (fBool)
//            {
//                if (invertedCamera)
//                {
//                    pitch = Mathf.Min(firstPersonMaxYCamLock, Mathf.Max(firstPersonMinYCamLock, pitch + (mSpeedY * Input.GetAxis("Mouse Y"))));
//                }
//                else
//                {
//                    pitch = Mathf.Min(firstPersonMaxYCamLock, Mathf.Max(firstPersonMinYCamLock, pitch + (mSpeedY * -Input.GetAxis("Mouse Y"))));
//                }
//            }
//            else if (tBool)
//            {
//                if (invertedCamera)
//                {
//                    pitch = Mathf.Min(thirdPersonMaxYCamLock, Mathf.Max(thirdPersonMinYCamLock, pitch + (mSpeedY * Input.GetAxis("Mouse Y"))));
//                }
//                else
//                {
//                    pitch = Mathf.Min(thirdPersonMaxYCamLock, Mathf.Max(thirdPersonMinYCamLock, pitch + (mSpeedY * -Input.GetAxis("Mouse Y"))));
//                }
//            }
//            cC.transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
//            rig.MoveRotation(Quaternion.Euler(new Vector3(0, yaw, 0)));
//            #endregion

//            #region Inputs
//            if (!piloting)
//            {
//                if (!isFishing && unpauseNewTime <= Time.time)
//                {

//                    if (Input.GetKey(KeyCode.E))
//                    {
//                        if (Physics.Raycast(rayOrigin, out hit, maxRCDistance, ~8, QueryTriggerInteraction.Ignore))
//                        {
//                            //pick up here
//                            if (hit.collider.CompareTag("ShipWheel"))
//                            {
//                                if (interactionDelay <= Time.time)
//                                {
//                                    //hit.transform.GetComponent<ShipScript>().PlayerPiloting(player);
//                                    try { hit.transform.GetComponent<ShipScript>().PlayerPiloting(player); }
//                                    catch { hit.transform.GetComponent<ShipScriptV2>().PlayerPiloting(player); }

//                                    //interactionDelay += Time.time + 3;
//                                }
//                            }
//                            if (hit.transform.CompareTag("DisabledFish"))
//                            {
//                                AddToInv(hit.transform.gameObject);
//                            }
//                        }
//                    }

//                    if (Input.GetKey(KeyCode.LeftShift))
//                    {
//                        isSprinting = true;
//                    }
//                    else
//                    {
//                        isSprinting = false;
//                    }
//                    if (Input.GetKeyDown(KeyCode.Mouse0))
//                    {
//                        m1HoldLength = 0;
//                        m1Sine = castChargeMin;
//                        powerGauge.slider.minValue = castChargeMin;
//                        powerGauge.slider.maxValue = castChargeMax * 100;
//                        capBool = false;
//                        ChargePromptTextGO.SetActive(false);
//                        anim.SetBool("IsFishing", true);
//                        anim.SetFloat("Reeling", 0);
//                        animReelFloat = 0;
//                    }

//                    if (Input.GetKey(KeyCode.Mouse0))
//                    {
//                        float tempFloat = m1Sine * 100;
//                        powerGauge.SetPower((int)tempFloat);//Jake
//                        if (powerMeter.activeSelf == false)
//                        {
//                            powerMeter.SetActive(true);
//                        }
//                        if (m1Sine > castChargeMax)
//                        {
//                            capBool = true;
//                            //powerRising = false;
//                        }
//                        else if (m1Sine < 0)
//                        {
//                            capBool = false;
//                            //powerRising = true;
//                        }

//                        if (capBool)
//                        {
//                            m1Sine -= Time.deltaTime * castChargeAccel;

//                        }
//                        else
//                        {
//                            m1Sine += Time.deltaTime * castChargeAccel;
//                        }
//                        //print(m1Sine);
//                        //m1Sine = (Mathf.Sin(m1HoldLength) * Time.fixedDeltaTime);
//                        //print(Mathf.Abs(m1Sine * 10000));
//                        if(animReelFloat < 1)
//                        {
//                            animReelFloat += animReelScaleSpeed;
//                        }
//                        else if(animReelFloat > 1)
//                        {
//                            animReelFloat = 1;
//                        }
//                    }
//                    if (Input.GetKeyUp(KeyCode.Mouse0))
//                    {
//                        //CastLine();
//                        isFishing = true;
//                        fishingHookRig.isKinematic = false;
//                        reelingIn = false;
//                        CastLineCharge(m1Sine);
//                        if (powerMeter.activeSelf == true)
//                        {
//                            powerMeter.SetActive(false);
//                        }
//                        ChargePromptTextGO.SetActive(false);
//                        //if(animReelFloat < 2)
//                        //{
//                        //    animReelFloat += animReelScaleSpeed;
//                        //}
//                        //else if(animReelFloat > 2)
//                        //{
//                        //    animReelFloat = 2;
//                        //}
//                    }
//                }
//                else
//                {
//                    //if (fishingMini.IsFishCaught())
//                    if (!fishingMini.IsMiniGameActive() && fishingMini.miniGameStatus == FishMinigame.MiniGameStatus.Won)
//                    {
//                        hookScript.ReelFish(hookScript.target);
//                    }
//                    else if (fishingMini.IsMiniGameActive() && fishingMini.miniGameStatus == FishMinigame.MiniGameStatus.Lost)
//                    {
//                        hookScript.FreeTarget(hookScript.target);
//                        audioSource.PlayOneShot(fishPullFailSound);
//                        Debug.Log("FailToCatch");
//                    }
//                    else if (!fishingMini.IsMiniGameActive() && fishingMini.miniGameStatus == FishMinigame.MiniGameStatus.Lost)
//                    {
//                        hookScript.FreeTarget(hookScript.target);
//                        audioSource.PlayOneShot(fishPullFailSound);
//                        Debug.Log("FailToCatch");
//                    }
//                    //else
//                    //{
//                    //    isFishing = false;
//                    //}
//                    //if (Input.GetKey(KeyCode.Mouse1) && (fishingMini.IsMiniGameActive() == false))
//                    if (Input.GetKey(KeyCode.Mouse1) && (fishingMini.miniGameStatus == FishMinigame.MiniGameStatus.Off))
//                    {
//                        reelOfShame = true;
//                        isGrappling = false;
//                    }
//                    if (reelOfShame || reelingIn)
//                    {
//                        if (animReelFloat < 3)
//                        {
//                            animReelFloat += animReelScaleSpeed;
//                        }
//                        else if (animReelFloat > 3)
//                        {
//                            animReelFloat = 3;
//                        }
//                    }
//                    else
//                    {
//                        if (animReelFloat < 2)
//                        {
//                            animReelFloat += animReelScaleSpeed;
//                        }
//                        else if (animReelFloat > 2)
//                        {
//                            animReelFloat = 2;
//                        }
//                    }
//                }
//                //if(reelOfShame || reelOfShame)
//                //{
//                //    if (animReelFloat < 2)
//                //    {
//                //        animReelFloat += animReelScaleSpeed;
//                //    }
//                //    else if (animReelFloat > 2)
//                //    {
//                //        animReelFloat = 2;
//                //    }
//                //}

//                switch (Input.GetKeyDown(KeyCode.Space) && okToJump)
//                {
//                    case true:
//                        rig.AddForce(transform.up * jumpForce);
//                        anim.SetBool("IsJumping", true);
//                        if (lastJumpState != okToJump)
//                        {
//                            animMoveFloat = 0;
//                        }
//                        break;
//                }
//                switch (Input.GetKeyDown(KeyCode.Space) && okToDoubleJump && !okToJump)
//                {
//                    case true:
//                        if (rig.velocity.y > 0)
//                        {
//                            rig.AddForce(transform.up * doubleJumpVerticalForce);
//                        }
//                        else
//                        {
//                            //rig.velocity.y = 0;
//                            rig.velocity = new Vector3(rig.velocity.x, 0, rig.velocity.z);
//                            rig.AddForce(transform.up * doubleJumpVerticalForce);
//                        }

//                        okToDoubleJump = false;
//                        if (Input.GetKey(KeyCode.W))
//                        {
//                            rig.AddForce(transform.forward * doubleJumpHorizontalForce);
//                        }
//                        if (Input.GetKey(KeyCode.S))
//                        {
//                            rig.AddForce(-transform.forward * doubleJumpHorizontalForce);
//                        }
//                        if (Input.GetKey(KeyCode.A))
//                        {
//                            rig.AddForce(-transform.right * doubleJumpHorizontalForce);
//                        }
//                        if (Input.GetKey(KeyCode.D))
//                        {
//                            rig.AddForce(transform.right * doubleJumpHorizontalForce);
//                        }

//                        if (lastJumpState != okToJump)
//                        {
//                            animMoveFloat = 0;
//                        }
//                        break;
//                }


//                if (Input.GetKeyDown(KeyCode.Escape))
//                {
//                    if (pauseMenuGO.activeSelf)
//                    {
//                        pauseMenuGO.SetActive(false);
//                        UnPausePlayer();
//                        Debug.Log("Off");

//                    }
//                    else if (!pauseMenuGO.activeSelf)
//                    {
//                        pauseMenuGO.SetActive(true);
//                        PausePlayer();
//                        Debug.Log("On");
//                    }
//                }

//                //Hotbar navigation
//                if (Input.GetKeyDown(KeyCode.X))
//                {
//                    if (invManager.GetHotBarLocation() + 1 < hotbarSlots.Length)
//                    {
//                        invManager.SetHotBarLocation(invManager.GetHotBarLocation() + 1, hotbarSlots);
//                    }
//                    else
//                    {
//                        invManager.SetHotBarLocation(0, hotbarSlots);
//                    }
//                    hotbarOutline.transform.position = new Vector3(hotbarSlots[invManager.GetHotBarLocation()].GetObject().transform.position.x,
//                                        hotbarSlots[invManager.GetHotBarLocation()].GetObject().transform.position.y,
//                                        hotbarSlots[invManager.GetHotBarLocation()].GetObject().transform.position.z);
//                }

//                if (Input.GetKeyDown(KeyCode.Z))
//                {
//                    if (invManager.GetHotBarLocation() - 1 >= 0)
//                    {
//                        invManager.SetHotBarLocation(invManager.GetHotBarLocation() - 1, hotbarSlots);
//                    }
//                    else
//                    {
//                        invManager.SetHotBarLocation(hotbarSlots.Length - 1, hotbarSlots);
//                    }
//                    hotbarOutline.transform.position = new Vector3(hotbarSlots[invManager.GetHotBarLocation()].GetObject().transform.position.x,
//                                        hotbarSlots[invManager.GetHotBarLocation()].GetObject().transform.position.y,
//                                        hotbarSlots[invManager.GetHotBarLocation()].GetObject().transform.position.z);
//                }

//                //Drop fish from hotbar
//                if (Input.GetKeyDown(KeyCode.C))
//                {
//                    int curSlot = invManager.GetHotBarLocation();
//                    if (hotbarSlots[curSlot].GetItem() != null)
//                    {
//                        Instantiate(hotbarSlots[curSlot].GetItem().prefab, dropPoint.transform.position, dropPoint.transform.rotation, null);
//                        invManager.Drop(1, hotbarSlots);
//                        if (hotbarSlots[curSlot].GetItem() != null)
//                        {
//                            hotbarSlots[curSlot].SetSprite(hotbarSlots[curSlot].GetItem().image);
//                        }
//                        else
//                        {
//                            hotbarSlots[curSlot].SetSprite(defaultSlotImage);
//                        }
//                        hotbarSlots[curSlot].SetItemCountText(hotbarSlots[curSlot].GetItemCount().ToString());
//                    }
//                }
//            }
//            #endregion
//            if (reelingIn)
//            {
//                ReelIn(hookedFish);
//                if (reelOnce == false)
//                {
//                    audioSource.PlayOneShot(fishReelSound);
//                    reelOnce = true;
//                }

//            }
//            else if (reelOfShame)
//            {
//                ReelIn();
//                if (!fishingHookRig.gameObject.activeSelf)
//                {
//                    reelOfShame = false;
//                    isFishing = false;
//                }
//            }
//            if (isGrappling)
//            {
//                CatchHookLatch();
//                if (reelOnce == false)
//                {
//                    audioSource.PlayOneShot(hookLatchSound);
//                    reelOnce = true;
//                }

//                //might need to set up a delay for this so it plays once;
//            }
//            anim.SetFloat("Reeling", animReelFloat);

//        }
//        else
//        {
//            if (Input.GetKeyDown(KeyCode.Escape))
//            {
//                UnPausePlayer();
//            }
//        }

//        if (correctedMaxFallDistance > player.transform.position.y)
//        {
//            //TPToDeck();
//            //TPToPos(rig, TPGO.transform);
//        }

//    }

//    public void StartStruggleMinigame()
//    {
//        fishingMini.SetMiniGame(false);

//    }

//    public void CastLine()
//    {
//        try
//        {
//            fishingHook.GetComponent<HookScript>().target.transform.SetParent(null);
//        }
//        catch { }
//        fishingHook.SetActive(false);
//        fishingHook.GetComponent<BoxCollider>().enabled = true;
//        fishingHookRig.angularVelocity = Vector3.zero;
//        fishingHookRig.velocity = Vector3.zero;
//        fishingHook.transform.position = rig.transform.position + rig.transform.forward;
//        fishingHook.transform.rotation = rig.transform.rotation;
//        fishingHook.SetActive(true);
//        fishingHookRig.AddForce((cC.transform.forward * castForce) + (fishingHookRig.transform.up * castUpForce));


//    }

//    public void CastLineCharge(float charge)
//    {
//        try
//        {
//            fishingHook.GetComponent<HookScript>().target.transform.SetParent(null);
//            fishingHook.GetComponent<HookScript>().firstCastCheck = true;
//        }
//        catch { }
//        fishingHook.SetActive(false);
//        fishingHook.GetComponent<BoxCollider>().enabled = true;
//        fishingHookRig.angularVelocity = Vector3.zero;
//        fishingHookRig.velocity = Vector3.zero;
//        fishingHook.transform.position = cC.transform.position + rig.transform.forward;
//        fishingHook.transform.rotation = rig.transform.rotation;
//        fishingHook.SetActive(true);
//        fishingHookRig.AddForce(((cC.transform.forward * castForce) + (fishingHookRig.transform.up * castUpForce)) * charge);
//        audioSource.PlayOneShot(castLineSound);
//        if (reelOnce == true)
//        {
//            reelOnce = false;//for the reeling in audio to reset
//        }
//    }

//    public void ReelIn()
//    {
//        if(animReelFloat < 3)
//        {
//            animReelFloat += animReelScaleSpeed;
//        }
//        else if(animReelFloat > 3)
//        {
//            animReelFloat = 3;
//        }
//        fishingHook.GetComponent<BoxCollider>().enabled = false;
//        //fishingHookRig.AddForce((player.transform.position - fishingHookRig.position) * reelSpeed * Time.fixedDeltaTime);
//        fishingHookRig.velocity = (player.transform.position - fishingHookRig.position) * reelSpeed * Time.fixedDeltaTime;
//        if (Vector3.Distance(player.transform.position, fishingHookRig.position) < minimumReelDistance)
//        {
//            fishingHook.SetActive(false);
//            reelOfShame = false;
//            reelingIn = false;
//            isFishing = false;
//            fishingMini.SetMiniGame(false);
//            fishingMini.miniGameStatus = FishMinigame.MiniGameStatus.Off;
//            anim.SetBool("IsFishing", false);
//        }
//    }

//    public void ReelIn(GameObject target)
//    {
//        if (animReelFloat < 3)
//        {
//            animReelFloat += animReelScaleSpeed;
//        }
//        else if (animReelFloat > 3)
//        {
//            animReelFloat = 3;
//        }
//        fishingHook.GetComponent<BoxCollider>().enabled = false;
//        fishingHookRig.velocity = (player.transform.position - fishingHookRig.position) * reelSpeed * Time.fixedDeltaTime;
//        if (Vector3.Distance(player.transform.position, fishingHookRig.position) <= minimumReelDistance)
//        {
//            //Add caught target to inventory here
//            target.SetActive(false);
//            target.transform.SetParent(null);
//            fishingHook.SetActive(false);
//            reelingIn = false;
//            reelOfShame = false;
//            isFishing = false;
//            if (target.transform.CompareTag("Fish"))
//            {
//                fishingMini.SetMiniGame(false, hookedFish);
//                fishingMini.miniGameStatus = FishMinigame.MiniGameStatus.Off;
//                if (target.GetComponent<Item>().GetItem().itemName != "Eel" || target.GetComponent<Item>().GetItem().itemName != "Jellyfish")
//                {
//                    AddToInv(target);
//                    audioSource.PlayOneShot(caughtFishSound);
//                }
//                else
//                {
//                    ShockPlayer();
//                    hookScript.FreeTarget(target);
//                }
//            }
//            else if (target.transform.CompareTag("Collectable"))
//            {
//                Destroy(target);
//                audioSource.PlayOneShot(catchCollectableSound);
//            }
//            else if (target.transform.CompareTag("DisabledFish"))
//            {
//                AddToInv(target);
//            }
//            animReelFloat = 0;
//            anim.SetBool("IsFishing", false);

//        }
//    }

//    public void CatchHookLatch()
//    {
//        if (animReelFloat < 3)
//        {
//            animReelFloat += animReelScaleSpeed;
//        }
//        else if (animReelFloat > 3)
//        {
//            animReelFloat = 3;
//        }
//        transform.SetParent(null);
//        rig.velocity = (fishingHookRig.position - player.transform.position) * grappleSpeed * Time.fixedDeltaTime;
//        if (Vector3.Distance(player.transform.position, fishingHookRig.position) < minimumGrappleDistance)
//        {
//            rig.velocity = new Vector3(0, rig.velocity.y, 0);
//            isGrappling = false;
//            reelOfShame = false;
//            isFishing = false;
//            fishingHook.SetActive(false);
//            animReelFloat = 0;
//            anim.SetBool("IsFishing", false);
//        }
//    }

//    public void TeleportToShip()
//    {
//        player.transform.position = shipRig.transform.position + new Vector3(0, 2);
//    }

//    //private void OnDrawGizmos()
//    //{
//    //    Gizmos.color = Color.green;
//    //    Gizmos.DrawWireSphere(player.transform.position + new Vector3(0, -maxGCDistance, 0), maxGCRadius);
//    //}

//    public void TPToDeck()
//    {
//        rig.transform.position = shipRig.GetComponentInChildren<ShipScript>().gameObject.transform.position
//           - shipRig.GetComponentInChildren<ShipScript>().gameObject.transform.forward + new Vector3(0, 1, 0);

//    }
//    public void TPToDeck(Rigidbody _rig)
//    {
//        _rig.transform.position = shipRig.GetComponentInChildren<ShipScript>().gameObject.transform.position
//           - shipRig.GetComponentInChildren<ShipScript>().gameObject.transform.forward + new Vector3(0, 1, 0);

//    }

//    public void TPToPos(Rigidbody _rig, Transform _NewPos)
//    {
//        _rig.transform.position = _NewPos.position;
//    }

//    public void ShockPlayer()
//    {
//        shockCurrentTime = shockMaxTime;
//        isShocked = true;
//    }

//    public void PausePlayer()
//    {
//        if (canPause == true)
//        {
//            Cursor.visible = true;
//            Cursor.lockState = CursorLockMode.None;

//            if (pauseMenuGO != null)
//            {
//                //if(pauseMenuGO == true)
//                isPaused = true;
//                pauseMenuGO.SetActive(true);
//            }
//            Debug.Log("Paused");
//        }

//    }
//    public void UnPausePlayer()
//    {
//        //Cursor.visible = false;
//        //Cursor.lockState = CursorLockMode.Locked;
//        if (pauseMenuGO != null)
//        {
//            //if(pauseMenuGO == true)
//            if (canPause == true)
//            {
//                isPaused = false;
//                Cursor.visible = false;
//                Cursor.lockState = CursorLockMode.Locked;
//                pauseMenuGO.SetActive(false);
//                unpauseNewTime = unpauseDelay + Time.time;
//                Debug.Log("Unpaused");
//            }

//        }
//    }

//    public void TalkManager()
//    {
//        Debug.Log("TalkManager");
//        if (isPaused == false)
//        {
//            isPaused = true;
//            Cursor.visible = true;
//            Cursor.lockState = CursorLockMode.None;
//        }
//        else if (isPaused == true)
//        {
//            isPaused = false;
//            Cursor.visible = false;
//            Cursor.lockState = CursorLockMode.Locked;
//            unpauseNewTime = unpauseDelay + Time.time;
//        }
//        if (canPause == true)
//        {
//            canPause = false;
//        }
//        else if (canPause == false)
//        {
//            canPause = true;
//        }
//        //Jake
//        //This should deactivate the player without activating the pauseMenu, and thusly not pause the Fungus Script
//    }

//    public void AddToInv(GameObject target)
//    {
//        if (!target.CompareTag("Harpoon"))
//        {
//            int curSlot;
//            Items item = target.GetComponent<Item>().GetItem();
//            if (invManager.Add(item, 1, hotbarSlots, out curSlot))
//            {
//                hotbarSlots[curSlot].SetSprite(hotbarSlots[curSlot].GetItem().image);
//                hotbarSlots[curSlot].SetItemCountText(hotbarSlots[curSlot].GetItemCount().ToString());
//            }
//            else
//            {
//                Instantiate(hotbarSlots[invManager.GetHotBarLocation()].GetItem().prefab, dropPoint.transform.position, dropPoint.transform.rotation, null);
//                invManager.Drop(1, hotbarSlots);
//                hotbarSlots[invManager.GetHotBarLocation()].SetItemCount(hotbarSlots[invManager.GetHotBarLocation()].GetItemCount() + 1);
//                hotbarSlots[invManager.GetHotBarLocation()].SetItem(item);
//                hotbarSlots[invManager.GetHotBarLocation()].SetSprite(hotbarSlots[invManager.GetHotBarLocation()].GetItem().image);
//                hotbarSlots[invManager.GetHotBarLocation()].SetItemCountText(hotbarSlots[invManager.GetHotBarLocation()].GetItemCount().ToString());
//            }
//        }
//    }

//    private void PPDInteraction(PPDScipt _pPDScipt)
//    {
//        ShipScriptV2 tempShipScript = shipRig.GetComponent<ShipScriptV2>();

//        tempShipScript.PPDToggle(_pPDScipt.pPDPosition);
//    }

//    private void OnTriggerStay(Collider other)
//    {
//        if (other.gameObject.tag == "Furnace")
//        {
//            int curSlot = invManager.GetHotBarLocation();
//            if (hotbarSlots[curSlot].GetItem() != null && shipRig.GetComponent<ShipScriptV2>().currentShipHealth < shipRig.GetComponent<ShipScriptV2>().maxHealth)
//            {
//                furnaceTextGO.SetActive(true);
//            }
//            else if(hotbarSlots[curSlot].GetItem() != null && shipRig.GetComponent<ShipScriptV2>().currentShipHealth == shipRig.GetComponent<ShipScriptV2>().maxHealth)
//            {
//                furnaceFullTextGO.SetActive(true);
//            }
//            //Set this up to only show if you have a fish in hand.
//            if (Input.GetKey(KeyCode.E))
//            {

//                if (hotbarSlots[curSlot].GetItem() != null)
//                {
//                    //Instantiate(hotbarSlots[curSlot].GetItem().prefab, dropPoint.transform.position, dropPoint.transform.rotation, null);
//                    if (shipRig.GetComponent<ShipScriptV2>().currentShipHealth + hotbarSlots[curSlot].GetItem().damage <= shipRig.GetComponent<ShipScriptV2>().maxHealth)
//                    {
//                        fireBurst1.Play();
//                        fireBurst2.Play();
//                        audioSource.PlayOneShot(furnaceOpeningSound);
//                        shipRig.GetComponent<ShipScriptV2>().currentShipHealth += hotbarSlots[curSlot].GetItem().damage;
//                        invManager.Drop(0, hotbarSlots);
//                        if (hotbarSlots[curSlot].GetItem() != null)
//                        {
//                            hotbarSlots[curSlot].SetSprite(hotbarSlots[curSlot].GetItem().image);

//                        }
//                        else
//                        {
//                            hotbarSlots[curSlot].SetSprite(defaultSlotImage);
//                            furnaceTextGO.SetActive(false);
//                        }
//                        hotbarSlots[curSlot].SetItemCountText(hotbarSlots[curSlot].GetItemCount().ToString());
//                    }
//                    else if (shipRig.GetComponent<ShipScriptV2>().currentShipHealth + hotbarSlots[curSlot].GetItem().damage > shipRig.GetComponent<ShipScriptV2>().maxHealth)
//                    {
//                        fireBurst1.Play();
//                        fireBurst2.Play();
//                        audioSource.PlayOneShot(furnaceOpeningSound);
//                        shipRig.GetComponent<ShipScriptV2>().currentShipHealth = shipRig.GetComponent<ShipScriptV2>().maxHealth;
//                        invManager.Drop(0, hotbarSlots);
//                        if (hotbarSlots[curSlot].GetItem() != null)
//                        {
//                            hotbarSlots[curSlot].SetSprite(hotbarSlots[curSlot].GetItem().image);
//                        }
//                        else
//                        {
//                            hotbarSlots[curSlot].SetSprite(defaultSlotImage);
//                            furnaceTextGO.SetActive(false);
//                        }
//                        hotbarSlots[curSlot].SetItemCountText(hotbarSlots[curSlot].GetItemCount().ToString());
//                    }


//                }

//            }

//        }
//        if (gameObject.transform.parent == null && other.gameObject.tag == "Ship")
//        {
//            gameObject.transform.SetParent(shipRig.transform);

//        }
//    }
//    private void OnTriggerExit(Collider other)
//    {
//        if (other.transform.CompareTag("Furnace"))
//        {
//            furnaceTextGO.SetActive(false);
//            furnaceFullTextGO.SetActive(false);
//        }

//        if (other.gameObject.tag == "Ship")
//        {
//            gameObject.transform.SetParent(null);

//        }
//    }
//    //private void OnTriggerStay(Collider other)
//    //{
//    //    if (gameObject.transform.parent == null && other.gameObject.tag == "Ship")
//    //    {
//    //        //gameObject.transform.parent = shipRig.transform;
//    //        gameObject.transform.SetParent(shipRig.transform);

//    //    }
//    //}
//}
