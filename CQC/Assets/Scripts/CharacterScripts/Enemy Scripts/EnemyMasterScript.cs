using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMasterScript : MonoBehaviour
{
    public EnemyBodySimple[] enemyBodySimpleArray;
    public EnemyBodySimple enemyBodySimple;
    public EnemyAttackScript enemyAttackScript;
    public PlayerScript pS;
    public GenericEnemySO gESO;
    public GameObject head;
    public enum ActionStatus { Idle, Roaming, Patrolling, Seeking, Attacking, Stunned, Parried}
    public ActionStatus actionStatus;
    public enum LastAttackDirection { PunchRight, PunchLeft, KickRight, KickLeft, Misc }
    public LastAttackDirection lastAttack;
    public Animator anim;
    [SerializeField]
    private float seekDistance, minDistance, moveSpeed, accelSpeed, parryStun, parryStunMaxLength;
    public float AttackDamage;
    
    public float health;
    public float headHealth, neckHealth, chestHealth, torsoHealth, hipHealth, groinHealth;
    public float shoulderLHealth, shoulderRHealth, upperArmLHealth, upperArmRHealth, elbowLHealth, ElbowRHealth, forearmLHealth, forearmRHealth, wristLHealth, wristRHealth, handLHealth, handRHealth;
    public float thighLHealth, thighRHealth, kneeLHealth, kneeRHealth, shinLHealth, shinRHealth, ankleLHealth, ankleRHealth, footLHealth, footRHealth;

    private int tempAnimRightInt, tempAnimUpInt, tempAnimTypeInt;
    private bool canAttack;

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
                    if (Vector3.Distance(gameObject.transform.position, pS.gameObject.transform.position) < minDistance && canAttack)
                    {
                        //Debug.Log("Close enough to attack");

                        ///Use following line once other animations are done to randomize attack patterns.
                        //tempAnimInt = anim.SetInteger("Right", Random.Range(-1,1)); ///Do something like this for the variables when anims are done.

                        GenerateAction();
                        anim.SetBool("CanAttack", true);
                    }
                   
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

    public void ApplyDamage(bodyPartMasterScript.TargetGeneralArea tGA)
    {
        //switch (tGA)
        //{
        //    case bodyPartMasterScript.TargetGeneralArea.None:

        //        break;
        //    case bodyPartMasterScript.TargetGeneralArea.Head:
        //        switch (tHA)
        //        {
        //            case bodyPartMasterScript.TargetHead.Head:

        //                break;

        //            case bodyPartMasterScript.TargetHead.Neck:

        //                break;
        //        }

        //        break;
        //    case bodyPartMasterScript.TargetGeneralArea.Torso:
        //        switch (tTA)
        //        {
        //            case bodyPartMasterScript.TargetTorso.Chest:

        //                break;

        //            case bodyPartMasterScript.TargetTorso.Stomach:

        //                break;

        //            case bodyPartMasterScript.TargetTorso.Hip:

        //                break;

        //            case bodyPartMasterScript.TargetTorso.Groin:

        //                break;

        //        }
        //        break;
        //    case bodyPartMasterScript.TargetGeneralArea.Arm:
        //        switch (tAA)
        //        {
        //            case bodyPartMasterScript.TargetArm.LUpperArm:

        //                break;

        //            case bodyPartMasterScript.TargetArm.RUpperArm:

        //                break;

        //            case bodyPartMasterScript.TargetArm.LForearm:

        //                break;

        //            case bodyPartMasterScript.TargetArm.RForearm:

        //                break;

        //            case bodyPartMasterScript.TargetArm.LWrist:

        //                break;

        //            case bodyPartMasterScript.TargetArm.RWrist:

        //                break;

        //            case bodyPartMasterScript.TargetArm.LHand:

        //                break;

        //            case bodyPartMasterScript.TargetArm.RHand:

        //                break;

        //        }

        //        break;

        //    case bodyPartMasterScript.TargetGeneralArea.Leg:
        //        switch (tLA)
        //        {
        //            case bodyPartMasterScript.TargetLeg.ThighL:

        //                break;

        //            case bodyPartMasterScript.TargetLeg.ThighR:

        //                break;

        //            case bodyPartMasterScript.TargetLeg.KneeL:

        //                break;

        //            case bodyPartMasterScript.TargetLeg.KneeR:

        //                break;
        //            case bodyPartMasterScript.TargetLeg.ShinL:

        //                break;

        //            case bodyPartMasterScript.TargetLeg.ShinR:

        //                break;

        //            case bodyPartMasterScript.TargetLeg.AnkleL:

        //                break;

        //            case bodyPartMasterScript.TargetLeg.AnkleR:

        //                break;


        //        }

        //        break;
    }

    private void StageParry()
    {
        gameObject.transform.LookAt(pS.gameObject.transform);
        anim.SetBool("Blocked", true);
        anim.SetBool("Parried", true);
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
                    lastAttack = LastAttackDirection.KickLeft;
                    Debug.Log("Placeholder for KickLeft");
                    tempAnimUpInt = 1;
                }
                break;
            case 0:
                //Misc. anims go here, maybe taunts, blocks, etc., for now it just defaults to a right punch.
                tempAnimRightInt = 1;
                tempAnimUpInt = 1;
                break;
            case 1:
                if (tempAnimUpInt == 1)
                {
                    lastAttack = LastAttackDirection.PunchRight;
                }
                else
                {
                    lastAttack = LastAttackDirection.KickRight;
                    Debug.Log("Placeholder for KickRight");
                    tempAnimUpInt = 1;

                }
                break;


        }
        ///Since the only animations I have are left jabs, I'm going to force left-only anims till I get the rights.
        //anim.SetInteger("Right", tempAnimRightInt);
        anim.SetInteger("Right", -1);
        anim.SetInteger("Up", tempAnimUpInt);
        anim.SetFloat("AttackType", (float)tempAnimTypeInt);
        canAttack = false;
    }
}
