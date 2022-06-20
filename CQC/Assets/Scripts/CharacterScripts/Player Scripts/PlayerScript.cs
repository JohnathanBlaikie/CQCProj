using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public bodyPartMasterScript.TargetGeneralArea tGA;
    public enum ActionState { Idle, Blocking, Attacking};
    public ActionState actionState;

    public Camera cam;

    public GameObject enemyGO;
    private Ray ray;
    private RaycastHit rayHit;

    public Animator animator;

    public GameObject player, cameraContainer;
    public CapsuleCollider playerHB;
    public Rigidbody rig, tempForeignRig;
    public float health;
    public float moveSpeed, mSpeedX, mSpeedY, firstPersonMaxYCamLock, firstPersonMinYCamLock, jumpForce, fallForce, maxRCDistance;
    public float lightHitDamage, heavyHitDamage, hitForce;
    [SerializeField]
    private float animatorSlowMoMin, animatorSlowMoResetRate;

    private float pitch, yaw;
    private int layerMask = 1 << 11;
    [SerializeField]
    private bool okToJump, okToWallJump, lockCursor, invertedCamera;
    
    [HideInInspector]
    public PlayerAttackScript pAS;
    public PlayerBlockScript pBS;

    /// <summary>
    /// Directional Attack Variables
    /// </summary>
    private enum LookDirection { Up, UpLeft, UpRight, Left, Right, DownLeft, DownRight, Down };
    [SerializeField]
    LookDirection lookDir, storedLookDir;

    private Vector2[] lookAxisRaw = new Vector2[2];

    //Todo: fix warping, rig ragdolls.
    // Start is called before the first frame update
    void Start()
    {
        //cam = GetComponent<Camera>();
        rig = GetComponent<Rigidbody>();
        animator.StartRecording(30);
    }

    // Update is called once per frame
    void Update()
    {
        if(lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
        ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        
        //rig.MoveRotation(rig.rotation * Quaternion.Euler(new Vector3(0, Input.GetAxis("Mouse X") * mSpeedX, 0)));

        //rig.MoveRotation(rig.rotation * Quaternion.Euler(new Vector3(Input.GetAxis("Mouse Y") * -mSpeedY, 0, 0)));
        rig.MovePosition(transform.position + (transform.forward * Input.GetAxis("Vertical") * moveSpeed) + (transform.right * Input.GetAxis("Horizontal") * moveSpeed));

        ///Get the player's most recent mouse movement.
        ///
        lookAxisRaw[0] = lookAxisRaw[1];
        lookAxisRaw[1] = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        Vector2 lAS = LookAxisSimple(lookAxisRaw[0], lookAxisRaw[1]);
        GetMouseMovementDirection(lAS);
        //Debug.Log(lAS);

        yaw += mSpeedX * Input.GetAxis("Mouse X");
        if (invertedCamera)
        {
            pitch = Mathf.Min(firstPersonMaxYCamLock, Mathf.Max(firstPersonMinYCamLock, pitch + (mSpeedY * Input.GetAxis("Mouse Y"))));
        }
        else
        {
            pitch = Mathf.Min(firstPersonMaxYCamLock, Mathf.Max(firstPersonMinYCamLock, pitch + (mSpeedY * -Input.GetAxis("Mouse Y"))));
        }
        //cameraContainer.transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
        rig.MoveRotation(Quaternion.Euler(new Vector3(0, yaw, 0)));

        if(animator.speed < 1)
        {
            animator.speed += animatorSlowMoResetRate * Time.deltaTime;
        }
        else
        {
            animator.speed = 1;
        }

        //Use this for weapons and debugging, otherwise all damage will be done with animations and colliders in the future.
        if (Input.GetKeyDown(KeyCode.Mouse0) && (animator.GetFloat("Right") == 0 && animator.GetFloat("Up") == 0))
        {
            //ResetPAS(pAS);
            storedLookDir = lookDir;
            Punch();

            //DebugPunch();
        }
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (actionState != ActionState.Attacking)
            {
                Block();
            }
        }
        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            if(actionState == ActionState.Blocking)
            {
                actionState = ActionState.Idle;
                animator.SetInteger("BlockInt", 0);
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) && (okToJump || okToWallJump))
        {
            rig.AddForce(transform.up * jumpForce);
        }
    }

    private void FixedUpdate()
    {

    }

    private void LateUpdate()
    {
        cameraContainer.transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);

    }

    //Start here, working on animator settings
    void Punch()
    {
        actionState = ActionState.Attacking;
        switch (lookDir)
        {
            case LookDirection.Up:
                animator.SetFloat("Up", 1);
                break;

            case LookDirection.UpLeft:
                //animator.SetFloat("Right", -1);
                //animator.SetFloat("Up", 1);
                break;

            case LookDirection.UpRight:
                //animator.SetFloat("Right", 1);
                //animator.SetFloat("Up", 1);
                break;

            case LookDirection.Left:
                animator.SetFloat("Right", -1);
                //Set to zero in animation
                break;

            case LookDirection.Right:
                animator.SetFloat("Right", 1);
                break;

            case LookDirection.DownLeft:
                //animator.SetFloat("Right", -1);
                //animator.SetFloat("Up", -1);
                break;

            case LookDirection.DownRight:
                //animator.SetFloat("Right", 1);
                //animator.SetFloat("Up", -1);
                break;

            case LookDirection.Down:
                //animator.SetFloat("Up", -1);
                break;
        }


    }

    void Block()
    {
        animator.SetInteger("BlockInt", 1);
        actionState = ActionState.Blocking;
    }

    void InitiateJump()
    {
        
    }

    ///Sets "Right" and "Up" variables in the animator to 0 
    public void ResetPunchAnimVars()
    {
        animator.SetFloat("Right", 0);
        animator.SetFloat("Up", 0);
        actionState = ActionState.Idle;
        ResetPAS(pAS);
    }

    public void ResetBlockAnimVars()
    {
        animator.SetFloat("Block", 0);
        actionState = ActionState.Idle;
        ResetPBS(pBS);
    }

    void DebugPunch()
    {
        //if (Physics.Raycast(ray, out rayHit))
        if (Physics.Raycast(ray, out rayHit, maxRCDistance, ~layerMask, QueryTriggerInteraction.Ignore))
        {
            if (rayHit.transform.CompareTag("Enemy"))
            {
                print("I'm looking at " + rayHit.transform.name + "'s " + rayHit.collider.transform.GetComponent<EnemyBodySimple>().tGA + ", it is an enemy!");
                //print(rayHit.collider.transform.GetComponent<EnemyBodySimple>().tGA.ToString());
                rayHit.collider.transform.GetComponent<EnemyBodySimple>().TakeDamage(lightHitDamage);
                tempForeignRig = rayHit.collider.transform.GetComponent<Rigidbody>();
                HitForceApplication(cam.transform, tempForeignRig);
                tempForeignRig = null;
            }
            else
                print("I'm looking at " + rayHit.transform.name);

        }
        else
        {
            print("I'm looking at nothing!");
            tGA = bodyPartMasterScript.TargetGeneralArea.None;
        }
    }

    void BasicAttackParse()
    {
        switch (lookDir)
        {
            case LookDirection.Up:
                //tempForeignRig.AddForce(cam.transform.rotation)
                animator.SetFloat("Punch", 1);
                    break;
            case LookDirection.UpLeft:
                //animator.Play
                break;
            case LookDirection.UpRight:
                //animator.Play
                break;
            case LookDirection.Left:
                //animator.Play

                break;
            case LookDirection.Right:

                break;
            case LookDirection.DownLeft:

                break;
            case LookDirection.DownRight:

                break;
            case LookDirection.Down:

                break;

        }



    }

    void BlockAttack()
    {
        //play block animation here;

    }

    void GetMouseMovementDirection(Vector2 lookRawV2)
    {
        switch (lookRawV2.x)
        {
            case 1:
                if (lookRawV2.y > 0)
                {
                    lookDir = LookDirection.UpRight;
                }
                else if(lookRawV2.y == 0)
                {
                    lookDir = LookDirection.Right;
                }
                else
                {
                    lookDir = LookDirection.DownRight;
                }
                break;
            case 0:
                if (lookRawV2.y > 0)
                {
                    lookDir = LookDirection.Up;
                }
                else if (lookRawV2.y == 0)
                {
                    ///This should be left neutral so it doesn't reset the more nuanced directions.
                }
                else
                {
                    lookDir = LookDirection.Down;
                }
                break;
            case -1:
                if (lookRawV2.y > 0)
                {
                    lookDir = LookDirection.UpLeft;
                }
                else if (lookRawV2.y == 0)
                {
                    lookDir = LookDirection.Left;
                }
                else
                {
                    lookDir = LookDirection.DownLeft;
                }
                break;
        }
    }

    Vector2 LookAxisSimple(Vector2 _LastLookAxisRaw, Vector2 _NewLookAxisRaw)
    {
        float x1, y1, x2, y2;
        Vector2[] tempV2 = new Vector2[2];
        if (_LastLookAxisRaw.x > 0)
        {
            x1 = 1;
        }
        else if (_LastLookAxisRaw.x < 0)
        {
            x1 = -1;
        }
        else
        {
            x1 = 0;
        }

        if (_LastLookAxisRaw.y > 0)
        {
            y1 = 1;
        }
        else if (_LastLookAxisRaw.y < 0)
        {
            y1 = -1;
        }
        else
        {
            y1 = 0;
        }

        if (_NewLookAxisRaw.x > 0)
        {
            x2 = 1;
        }
        else if (_NewLookAxisRaw.x < 0)
        {
            x2 = -1;
        }
        else
        {
            x2 = 0;
        }

        if (_NewLookAxisRaw.y > 0)
        {
            y2 = 1;
        }
        else if (_NewLookAxisRaw.y < 0)
        {
            y2 = -1;
        }
        else
        {
            y2 = 0;
        }
        tempV2[0] = new Vector2(x1, y1);
        tempV2[1] = new Vector2(x2, y2);

        float tempFloat = Vector2.Distance(tempV2[0], tempV2[1]);
        if (tempFloat < 1)
        {
            return tempV2[1];
        }
        else
        {
            return tempV2[0];
        }
    }

    public void HitForceApplication(Rigidbody _AffectedRig)
    {
        animator.speed = animatorSlowMoMin;
        switch (storedLookDir)
        {
            case LookDirection.Up:
                //tempForeignRig.AddForce(cam.transform.rotation)
                _AffectedRig.AddForce((cam.transform.up * hitForce));
                break;
            case LookDirection.UpLeft:
                _AffectedRig.AddForce((cam.transform.up * hitForce / 2) - (cam.transform.right * hitForce / 2));
                break;
            case LookDirection.UpRight:
                _AffectedRig.AddForce((cam.transform.up * hitForce / 2) + (cam.transform.right * hitForce / 2));
                break;
            case LookDirection.Left:
                _AffectedRig.AddForce((-cam.transform.right * hitForce));

                break;
            case LookDirection.Right:
                _AffectedRig.AddForce((cam.transform.right * hitForce));

                break;
            case LookDirection.DownLeft:
                _AffectedRig.AddForce((-cam.transform.up * hitForce / 2) - (cam.transform.right * hitForce / 2));
                break;
            case LookDirection.DownRight:
                _AffectedRig.AddForce((-cam.transform.up * hitForce / 2) + (cam.transform.right * hitForce / 2));

                break;
            case LookDirection.Down:
                _AffectedRig.AddForce((-cam.transform.up * hitForce));

                break;
        }
        //tempForeignRig.AddForce(cam.transform.forward * hitForce);

    }

    public void HitForceApplication(Transform _CamRotation, Rigidbody _AffectedRig)
    {
        switch (lookDir)
        {
            case LookDirection.Up:
                //tempForeignRig.AddForce(cam.transform.rotation)
                _AffectedRig.AddForce((_CamRotation.up * hitForce));
                break;
            case LookDirection.UpLeft:
                _AffectedRig.AddForce((_CamRotation.up * hitForce / 2) - (_CamRotation.right * hitForce / 2));
                break;
            case LookDirection.UpRight:
                _AffectedRig.AddForce((_CamRotation.up * hitForce / 2) + (_CamRotation.right * hitForce / 2));
                break;
            case LookDirection.Left:
                _AffectedRig.AddForce((-_CamRotation.right * hitForce));

                break;
            case LookDirection.Right:
                _AffectedRig.AddForce((_CamRotation.right * hitForce));

                break;
            case LookDirection.DownLeft:
                _AffectedRig.AddForce((-_CamRotation.up * hitForce / 2) - (_CamRotation.right * hitForce / 2));
                break;
            case LookDirection.DownRight:
                _AffectedRig.AddForce((-_CamRotation.up * hitForce / 2) + (_CamRotation.right * hitForce / 2));

                break;
            case LookDirection.Down:
                _AffectedRig.AddForce((-_CamRotation.up * hitForce));

                break;
        }
        //tempForeignRig.AddForce(cam.transform.forward * hitForce);
       
    }
    void ResetPAS(PlayerAttackScript _pAS)
    {
        if (_pAS != null)
        {
            _pAS.ResetEBSList();
        }

    }

    void ResetPBS(PlayerBlockScript _pBS)
    {
        if(_pBS != null)
        {
            _pBS.ResetPBSList();
        }
    }

    public void TakeDamage(float damageValue)
    {
        health -= damageValue;
    }

    public void TogglePerfectParryBool()
    {
        pBS.TogglePerfectParry();
    }
    //void ResetPAS(PlayerAttackScript[] pAS)
    //{
    //    for (int i = 0; i < pAS.Length; i++)
    //    {
    //        pAS[i].ResetEBSList();
    //    }
    //}
}
