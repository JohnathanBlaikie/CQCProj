//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class BodyPartManagerOLD : MonoBehaviour
//{
//    //public EnemyBodySimple[] enemyBodySimples;
//    public PlayerScript pS;
//    public EnemyBodySimple enemyBodySimple;
//    public float health;
//    public float headHealth, neckHealth, chestHealth, torsoHealth, hipHealth, groinHealth;
//    public float shoulderLHealth, shoulderRHealth, upperArmLHealth, upperArmRHealth, ElbowLHealth, ElbowRHealth, forearmLHealth, forearmRHealth, wristLHealth, wristRHealth, handLHealth, handRHealth;
//    public float thighLHealth, thighRHealth, kneeLHealth, kneeRHealth, shinLHealth, shinRHealth, ankleLHealth, ankleRHealth, footLHealth, footRHealth;

//    //Figure out how to efficiently manage each body part's health
//    // Start is called before the first frame update
//    void Start()
//    {

//    }

//    // Update is called once per frame
//    void Update()
//    {

//    }

//    public void DamageHead(bodyPartMasterScript.TargetHead tHA, float damage)
//    {
//        switch (tHA)
//        {
//            case bodyPartMasterScript.TargetHead.Head:
//                headHealth -= damage;
//                break;

//            case bodyPartMasterScript.TargetHead.Neck:
//                neckHealth -= damage;
//                break;
//        }
//    }
//    public void DamageTorso(bodyPartMasterScript.TargetTorso tTA, float damage)
//    {
//        switch (tTA)
//        {
//            case bodyPartMasterScript.TargetTorso.Chest:
//                chestHealth -= damage;
//                break;

//            case bodyPartMasterScript.TargetTorso.Stomach:
//                torsoHealth -= damage;
//                break;

//            case bodyPartMasterScript.TargetTorso.Hip:
//                hipHealth -= damage;
//                break;

//            case bodyPartMasterScript.TargetTorso.Groin:
//                groinHealth -= damage;
//                break;

//        }

//    }

//    public void DamageArm(bodyPartMasterScript.TargetArm tAA, float damage)
//    {
//        switch (tAA)
//        {
//            case bodyPartMasterScript.TargetArm.LShoulder:
//                shoulderLHealth -= damage;
//                break;

//            case bodyPartMasterScript.TargetArm.RShoulder:
//                shoulderRHealth -= damage;
//                break;

//            case bodyPartMasterScript.TargetArm.LUpperArm:
//                upperArmLHealth -= damage;
//                break;

//            case bodyPartMasterScript.TargetArm.RUpperArm:
//                upperArmRHealth -= damage;
//                break;

//            case bodyPartMasterScript.TargetArm.LElbow:
//                ElbowLHealth -= damage;
//                break;

//            case bodyPartMasterScript.TargetArm.RElbow:
//                ElbowRHealth -= damage;
//                break;

//            case bodyPartMasterScript.TargetArm.LForearm:
//                forearmLHealth -= damage;
//                break;

//            case bodyPartMasterScript.TargetArm.RForearm:
//                forearmRHealth -= damage;
//                break;

//            case bodyPartMasterScript.TargetArm.LWrist:
//                wristLHealth -= damage;
//                break;

//            case bodyPartMasterScript.TargetArm.RWrist:
//                wristRHealth -= damage;
//                break;

//            case bodyPartMasterScript.TargetArm.LHand:
//                handLHealth -= damage;
//                break;

//            case bodyPartMasterScript.TargetArm.RHand:
//                handRHealth -= damage;
//                break;

//        }

//    }

//    public void DamageLeg(bodyPartMasterScript.TargetLeg tLA, float damage)
//    {
//        switch (tLA)
//        {
//            case bodyPartMasterScript.TargetLeg.ThighL:
//                thighLHealth -= damage;
//                break;

//            case bodyPartMasterScript.TargetLeg.ThighR:
//                thighRHealth -= damage;
//                break;

//            case bodyPartMasterScript.TargetLeg.KneeL:
//                kneeLHealth -= damage;
//                break;

//            case bodyPartMasterScript.TargetLeg.KneeR:
//                kneeRHealth -= damage;
//                break;

//            case bodyPartMasterScript.TargetLeg.ShinL:
//                shinLHealth -= damage;
//                break;

//            case bodyPartMasterScript.TargetLeg.ShinR:
//                shinRHealth -= damage;
//                break;

//            case bodyPartMasterScript.TargetLeg.AnkleL:
//                ankleLHealth -= damage;
//                break;

//            case bodyPartMasterScript.TargetLeg.AnkleR:
//                ankleRHealth -= damage;
//                break;

//            case bodyPartMasterScript.TargetLeg.FootL:
//                footLHealth -= damage;
//                break;

//            case bodyPartMasterScript.TargetLeg.FootR:
//                footRHealth -= damage;
//                break;
//        }


//    }

//    public void ApplyDamage(bodyPartMasterScript.TargetGeneralArea tGA)
//    {
//        //switch (tGA)
//        //{
//        //    case bodyPartMasterScript.TargetGeneralArea.None:

//        //        break;
//        //    case bodyPartMasterScript.TargetGeneralArea.Head:
//        //        switch (tHA)
//        //        {
//        //            case bodyPartMasterScript.TargetHead.Head:

//        //                break;

//        //            case bodyPartMasterScript.TargetHead.Neck:

//        //                break;
//        //        }

//        //        break;
//        //    case bodyPartMasterScript.TargetGeneralArea.Torso:
//        //        switch (tTA)
//        //        {
//        //            case bodyPartMasterScript.TargetTorso.Chest:

//        //                break;

//        //            case bodyPartMasterScript.TargetTorso.Stomach:

//        //                break;

//        //            case bodyPartMasterScript.TargetTorso.Hip:

//        //                break;

//        //            case bodyPartMasterScript.TargetTorso.Groin:

//        //                break;

//        //        }
//        //        break;
//        //    case bodyPartMasterScript.TargetGeneralArea.Arm:
//        //        switch (tAA)
//        //        {
//        //            case bodyPartMasterScript.TargetArm.LUpperArm:

//        //                break;

//        //            case bodyPartMasterScript.TargetArm.RUpperArm:

//        //                break;

//        //            case bodyPartMasterScript.TargetArm.LForearm:

//        //                break;

//        //            case bodyPartMasterScript.TargetArm.RForearm:

//        //                break;

//        //            case bodyPartMasterScript.TargetArm.LWrist:

//        //                break;

//        //            case bodyPartMasterScript.TargetArm.RWrist:

//        //                break;

//        //            case bodyPartMasterScript.TargetArm.LHand:

//        //                break;

//        //            case bodyPartMasterScript.TargetArm.RHand:

//        //                break;

//        //        }

//        //        break;

//        //    case bodyPartMasterScript.TargetGeneralArea.Leg:
//        //        switch (tLA)
//        //        {
//        //            case bodyPartMasterScript.TargetLeg.ThighL:

//        //                break;

//        //            case bodyPartMasterScript.TargetLeg.ThighR:

//        //                break;

//        //            case bodyPartMasterScript.TargetLeg.KneeL:

//        //                break;

//        //            case bodyPartMasterScript.TargetLeg.KneeR:

//        //                break;
//        //            case bodyPartMasterScript.TargetLeg.ShinL:

//        //                break;

//        //            case bodyPartMasterScript.TargetLeg.ShinR:

//        //                break;

//        //            case bodyPartMasterScript.TargetLeg.AnkleL:

//        //                break;

//        //            case bodyPartMasterScript.TargetLeg.AnkleR:

//        //                break;


//        //        }

//        //        break;
//    }

//}