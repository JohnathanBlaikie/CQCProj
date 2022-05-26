using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct bodyPartMasterScript
{
    public enum TargetGeneralArea { None, Head, Torso, Arm, Leg }
    public enum TargetHead { Head, Neck }
    public enum TargetTorso { Chest, Stomach, Hip, Groin }
    public enum TargetArm { LShoulder, RShoulder, LUpperArm, RUpperArm, LElbow, RElbow, LForearm, RForearm, LWrist, RWrist, LHand, RHand }
    public enum TargetLeg { ThighL, ThighR, KneeL, KneeR, ShinL, ShinR, AnkleL, AnkleR, FootL, FootR }
    public TargetGeneralArea targetPoint;
    public TargetHead targetHeadPoint;
    public TargetTorso targetTorsoPoint;
    public TargetArm targetArmPoint;
    public TargetLeg targetLegPoint;

}

