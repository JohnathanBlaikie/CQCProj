using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMasterScript : MonoBehaviour
{
    public EnemyBodySimple[] enemyBodySimpleArray;
    public EnemyBodySimple enemyBodySimple;
    public EnemyAttackScript enemyAttackScript;
    public PlayerScript pS;
    public LayerMask targetMask;
    public LayerMask[] obstructionMasks;
    public GenericEnemySO gESO;
    public GameObject head;
    public GameObject[] attackCollider;
    
    public enum ActionStatus { Idle, Roaming, Patrolling, Seeking, Attacking, Blocking, Evading, Stunned, Parried, Blocked }
    public ActionStatus actionStatus;
    public enum LastAttackDirection { PunchRight, PunchLeft, KickRight, KickLeft, Misc }
    public LastAttackDirection lastAttack;
    public Animator anim;
    [Header("Misc. Attributes")]
    [Range (0,360)]
    public float fovAngle;
    public float fovRadius;
    public float maxAttackRange;
    [SerializeField]
    private float moveSpeed, rotationSpeed, accelSpeed, parryStun, parryStunMaxLength, seekTimerMax, seekTimer;
    public float AttackDamage;
    public bool canSeePlayer, playerInRange;

    [Header("Health Values")]
    public float health;
    public float headHealth, neckHealth, chestHealth, torsoHealth, hipHealth, groinHealth;
    public float shoulderLHealth, shoulderRHealth, upperArmLHealth, upperArmRHealth, elbowLHealth, ElbowRHealth, forearmLHealth, forearmRHealth, wristLHealth, wristRHealth, handLHealth, handRHealth;
    public float thighLHealth, thighRHealth, kneeLHealth, kneeRHealth, shinLHealth, shinRHealth, ankleLHealth, ankleRHealth, footLHealth, footRHealth;

    private int tempAnimRightInt, tempAnimUpInt, tempAnimTypeInt;
    private bool canAttack;

    private Vector3 investigationPoint;

    //Figure out how to efficiently manage each body part's health
    // Start is called before the first frame update
    void Start()
    {
        gESO.EnemySOCopy();
        if(health > 0)
        {
            canAttack = true;
            for(int i = 0; i < enemyBodySimpleArray.Length; i++)
            {
                try
                {
                    enemyBodySimpleArray[i].eS = this;
                    enemyBodySimpleArray[i].rig.isKinematic = true;
                    enemyBodySimpleArray[i].rig.useGravity = false;
                    enemyBodySimpleArray[i].rig.interpolation = RigidbodyInterpolation.None;
                }
                catch
                {
                    //Debug.LogWarning("Something went wrong when setting " + enemyBodySimpleArray[i].gameObject.name + " to Kinematic!");
                    
                }
                
            }
            if (head == null)
            {
                for (int i = 0; i < enemyBodySimpleArray.Length; i++)
                {
                    if (enemyBodySimpleArray[i].tGA == bodyPartMasterScript.TargetGeneralArea.Head)
                    {
                        if (enemyBodySimpleArray[i].tHA == bodyPartMasterScript.TargetHead.Head)
                        {
                            head = enemyBodySimpleArray[i].gameObject;
                        }
                    }
                }
            }
            StartCoroutine(FOVRoutine());
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        if (health <= 0)
        {
            anim.enabled = false;
            for (int i = 0; i < enemyBodySimpleArray.Length; i++)
            {
                try
                {
                    enemyBodySimpleArray[i].rig.isKinematic = false;
                    enemyBodySimpleArray[i].rig.useGravity = true;
                    enemyBodySimpleArray[i].rig.interpolation = RigidbodyInterpolation.Interpolate;
                    
                    
                }
                catch
                {
                    //Debug.LogWarning("Something went wrong when setting " + enemyBodySimpleArray[i].gameObject.name + " to Non-Kinematic!");
                }
            }
        }
        else
        {
            switch (actionStatus)
            {
                case ActionStatus.Idle:
                    anim.SetInteger("PlayerDetectionState", 0);
                    break;

                case ActionStatus.Attacking:
                    anim.SetInteger("PlayerDetectionState", 3);
                    //Debug.Log(Vector3.Distance(gameObject.transform.position, pS.gameObject.transform.position));
                    if (playerInRange && canSeePlayer && canAttack)
                    {
                        GenerateAction();
                        //anim.SetBool("CanAttack", true);
                    }
                    else if(!playerInRange && !canSeePlayer)
                    {
                        investigationPoint = LogLastKnownPosition(pS.transform);
                        actionStatus = ActionStatus.Seeking;
                        seekTimer = seekTimerMax;
                    }

                    break;

                case ActionStatus.Seeking:
                    //Add a countdown timer that sets the actionstatus to Patrolling if player is not found.
                    seekTimer -= Time.deltaTime * Time.timeScale;
                    if (canSeePlayer)
                    {
                        actionStatus = ActionStatus.Attacking;
                    }
                    else if(seekTimer <= 0)
                    {
                        actionStatus = ActionStatus.Patrolling;
                    }
                    ///TODO: ROTATE TOWARDS LAST KNOWN POSITION
                    ///
                    
                    Vector3 tempTargetDirection = (pS.transform.position - transform.position).normalized;
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(tempTargetDirection), rotationSpeed * Time.deltaTime * Time.timeScale);

                    break; 

                case ActionStatus.Patrolling:
                    if (canSeePlayer)
                    {
                        actionStatus = ActionStatus.Attacking;
                        
                    }
                    seekTimer = seekTimerMax;
                    break;

                case ActionStatus.Blocking:
                    anim.SetBool("Blocking", true);
                    ///Maybe combine blocking and evading into one "Defending" State.
                    break;

                case ActionStatus.Evading:
                    anim.SetBool("Evading", true);
                    break;

                case ActionStatus.Blocked:
                    AttackBlocked();
                    break;

                case ActionStatus.Parried:
                    StageParry();
                    break;

                case ActionStatus.Stunned:
                    if(parryStun > 0)
                    {
                        parryStun -= Time.deltaTime * Time.timeScale;
                    }
                    else
                    {
                        actionStatus = ActionStatus.Attacking;
                    }
                    break;
            }
        }
        //if(actionStatus == ActionStatus.Attacking)
        //{
        //    head.transform.LookAt(pS.gameObject.transform);
        //    //Once animation is done, use that to move rather than this below
        //    anim.SetInteger("PlayerDetectionState", 3);
        //}
    }

    private IEnumerator FOVRoutine()
    {
        float delay = 0.2f;
        WaitForSeconds wait = new WaitForSeconds(delay);

        while (true)
        {
            yield return wait;
            FOVCheck();
        }
    }

    private void FOVCheck()
    {
        Collider[] rangeCheck = Physics.OverlapSphere(transform.position, fovRadius, targetMask);

        if (rangeCheck.Length != 0)
        {
            Transform target = rangeCheck[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < fovAngle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);
                for (int i = 0; i < obstructionMasks.Length; i++)
                {
                    if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMasks[i]))
                    {
                        canSeePlayer = true;
                        if (distanceToTarget < maxAttackRange)
                        {
                            playerInRange = true;
                        }
                        else
                        {
                            playerInRange = false;
                        }
                    }
                    else
                    {
                        canSeePlayer = false;
                        playerInRange = false;
                    }
                }
            }
            else
            {
                canSeePlayer = false;
                playerInRange = false;
            }
        }
        else if (canSeePlayer || playerInRange)
        {
            canSeePlayer = false;
            playerInRange = false;
        }
    }

    private Vector3 LogLastKnownPosition(Transform _transform)
    {
        Vector3 tempV3 = _transform.transform.position;

        return tempV3;
    }

    public void ResetNPCAnimValues()
    {
        anim.SetFloat("PlayerDetectionState", 0);
        anim.SetBool("CanAttack", false);
        anim.SetBool("Blocked", false);
        anim.SetBool("Parried", false);
        anim.SetBool("HitRecoil", false);
        canAttack = true;
        ResetEASVariables(enemyAttackScript);
    }

    public void EnableAttackBox()
    {
        switch (lastAttack)
        {
            case LastAttackDirection.Misc:

                break;
            case LastAttackDirection.PunchLeft:
                attackCollider[0].SetActive(true);
                break;
            case LastAttackDirection.PunchRight:
                attackCollider[1].SetActive(true);
                break;
            case LastAttackDirection.KickLeft:
                attackCollider[2].SetActive(true);
                break;
            case LastAttackDirection.KickRight:
                attackCollider[3].SetActive(true);
                break;


        }
        Debug.Log("Enabled Attack Box");
    }
    public void DisableAttackBox()
    {
        for(int i = 0; i < attackCollider.Length; i++)
        {
            attackCollider[i].SetActive(false);
        }
        Debug.Log("Disabled Attack Box");

    }


    public void DamageHead(bodyPartMasterScript.TargetHead tHA, float damage)
    {
        switch (tHA)
        {
            case bodyPartMasterScript.TargetHead.Head:
                headHealth -= damage;
                health -= damage * 2;
                anim.SetFloat("HitReactionLocation", 4);
                break;

            case bodyPartMasterScript.TargetHead.Neck:
                neckHealth -= damage;
                health -= damage * 4;
                anim.SetFloat("HitReactionLocation", 4);
                break;
        }
    }

    public void DamageTorso(bodyPartMasterScript.TargetTorso tTA, float damage)
    {
        switch (tTA)
        {
            case bodyPartMasterScript.TargetTorso.Chest:
                chestHealth -= damage;
                health -= damage;
                anim.SetFloat("HitReactionLocation", 3);

                break;

            case bodyPartMasterScript.TargetTorso.Stomach:
                torsoHealth -= damage;
                health -= damage * 1.2f;
                anim.SetFloat("HitReactionLocation", 1);
                break;

            case bodyPartMasterScript.TargetTorso.Hip:
                hipHealth -= damage;
                health -= damage;
                anim.SetFloat("HitReactionLocation", 1);
                break;

            case bodyPartMasterScript.TargetTorso.Groin:
                groinHealth -= damage;
                health -= damage * 10;

                break;

        }

    }

    public void DamageArm(bodyPartMasterScript.TargetArm tAA, float damage)
    {
        switch (tAA)
        {
            case bodyPartMasterScript.TargetArm.LShoulder:
                shoulderLHealth -= damage;
                break;

            case bodyPartMasterScript.TargetArm.RShoulder:
                shoulderRHealth -= damage;
                break;

            case bodyPartMasterScript.TargetArm.LUpperArm:
                upperArmLHealth -= damage;
                break;

            case bodyPartMasterScript.TargetArm.RUpperArm:
                upperArmRHealth -= damage;
                break;

            case bodyPartMasterScript.TargetArm.LElbow:
                elbowLHealth -= damage;
                break;

            case bodyPartMasterScript.TargetArm.RElbow:
                ElbowRHealth -= damage;
                break;

            case bodyPartMasterScript.TargetArm.LForearm:
                forearmLHealth -= damage;
                break;

            case bodyPartMasterScript.TargetArm.RForearm:
                forearmRHealth -= damage;
                break;

            case bodyPartMasterScript.TargetArm.LWrist:
                wristLHealth -= damage;
                break;

            case bodyPartMasterScript.TargetArm.RWrist:
                wristRHealth -= damage;
                break;

            case bodyPartMasterScript.TargetArm.LHand:
                handLHealth -= damage;
                break;

            case bodyPartMasterScript.TargetArm.RHand:
                handRHealth -= damage;
                break;

        }

    }

    public void DamageLeg(bodyPartMasterScript.TargetLeg tLA, float damage)
    {
        switch(tLA)
        {
            case bodyPartMasterScript.TargetLeg.ThighL:
                thighLHealth -= damage;
                break;

            case bodyPartMasterScript.TargetLeg.ThighR:
                thighRHealth -= damage;
                break;

            case bodyPartMasterScript.TargetLeg.KneeL:
                kneeLHealth -= damage;
                break;

            case bodyPartMasterScript.TargetLeg.KneeR:
                kneeRHealth -= damage;
                break;

            case bodyPartMasterScript.TargetLeg.ShinL:
                shinLHealth -= damage;
                break;

            case bodyPartMasterScript.TargetLeg.ShinR:
                shinRHealth -= damage;
                break;

            case bodyPartMasterScript.TargetLeg.AnkleL:
                ankleLHealth -= damage;
                break;

            case bodyPartMasterScript.TargetLeg.AnkleR:
                ankleRHealth -= damage;
                break;

            case bodyPartMasterScript.TargetLeg.FootL:
                footLHealth -= damage;
                break;

            case bodyPartMasterScript.TargetLeg.FootR:
                footRHealth -= damage;
                break;
        }


    }

    void ResetEASVariables(EnemyAttackScript _eAS)
    {
        if(_eAS != null)
        _eAS.ResetEnemyAttackVariables();
    }

    private void AttackBlocked()
    {
        anim.SetBool("Blocked", true);
        anim.SetBool("Parried", false);
        anim.SetFloat("AttackType", 0);
        //Only a temporarily cut corner, but I'm re-using the attack-type
        //float for blocking/parrying.
        anim.speed = 0;
        Vector3 tempTargetPosition = new Vector3
            (pS.transform.position.x, transform.position.y, pS.transform.position.z);
        //transform.LookAt(tempTargetPosition);
        anim.speed = 1;
        actionStatus = ActionStatus.Attacking;
    }

    private void StageParry()
    {
        anim.SetBool("Blocked", true);
        anim.SetBool("Parried", true);
        anim.SetFloat("AttackType", 1);

        anim.speed = 0;
        Vector3 tempRotation = new Vector3
            (pS.transform.position.x, transform.position.y, pS.transform.position.z);
        transform.LookAt(tempRotation);
        anim.speed = 1;
        switch (lastAttack)
        {
            case LastAttackDirection.PunchRight:
                gameObject.transform.position = pS.gameObject.transform.position + (pS.gameObject.transform.forward * 1.5f) + (pS.gameObject.transform.right * -0.35f);

                break;
            case LastAttackDirection.PunchLeft:
                gameObject.transform.position = pS.gameObject.transform.position + (pS.gameObject.transform.forward * 1.5f) + (pS.gameObject.transform.right * 0.35f);

                break;

            case LastAttackDirection.KickRight:
                gameObject.transform.position = pS.gameObject.transform.position + (pS.gameObject.transform.forward * 1.5f) + (pS.gameObject.transform.right * -0.35f);

                break;

            case LastAttackDirection.KickLeft:
                gameObject.transform.position = pS.gameObject.transform.position + (pS.gameObject.transform.forward * 1.5f) + (pS.gameObject.transform.right * 0.35f);

                break;

            case LastAttackDirection.Misc:

                break;
        }
        //gameObject.transform.rotation = new Quaternion(-pS.gameObject.transform.rotation.x, gameObject.transform.rotation.y, -pS.gameObject.transform.rotation.z, -pS.gameObject.transform.rotation.w);
        actionStatus = ActionStatus.Stunned;
        parryStun = parryStunMaxLength;
    }

    private void GenerateAction()
    {
        tempAnimRightInt = Random.Range(-1, 2);
        tempAnimUpInt = Random.Range(-1, 2);
        tempAnimTypeInt = Random.Range(0, 4);
        Debug.Log(tempAnimRightInt + " " + tempAnimUpInt);
        
        switch (tempAnimRightInt)
        {
            case -1:
                if (tempAnimUpInt == 1)
                {
                    lastAttack = LastAttackDirection.PunchLeft;
                }
                else
                {
                    //lastAttack = LastAttackDirection.KickLeft;
                    Debug.Log("Placeholder for KickLeft");
                    lastAttack = LastAttackDirection.PunchLeft;
                }
                anim.SetBool("RightBool", false);

                break;
            case 0:
                //Misc. anims go here, maybe taunts, blocks, etc., for now it just defaults to a right punch.
                tempAnimRightInt = 1;
                tempAnimUpInt = 1;
                anim.SetBool("RightBool", true);
                break;
            case 1:
                if (tempAnimUpInt == 1)
                {
                    lastAttack = LastAttackDirection.PunchRight;
                }
                else
                {
                    //lastAttack = LastAttackDirection.KickRight;
                    Debug.Log("Placeholder for KickRight");
                    lastAttack = LastAttackDirection.PunchRight;
                }
                anim.SetBool("RightBool", true);

                break;


        }
        anim.SetInteger("Right", tempAnimRightInt);
        anim.SetInteger("Up", tempAnimUpInt);
        anim.SetFloat("AttackType", tempAnimTypeInt);
        anim.SetBool("CanAttack", true);
        canAttack = false;
    }
}
