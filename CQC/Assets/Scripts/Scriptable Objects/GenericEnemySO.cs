using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemySO", menuName = "Enemy Scriptable Objects", order = 1)]
public class GenericEnemySO : ScriptableObject
{
    public float headHealth, neckHealth, chestHealth,
        torsoHealth, hipHealth, groinHealth;

    public float shoulderLHealth, shoulderRHealth, 
        upperArmLHealth, upperArmRHealth, elbowLHealth, elbowRHealth, 
        forearmLHealth, forearmRHealth, WristLHealth, WristRHealth,
        HandLHealth, HandRHealth;

    public float thighLHealth, thighRHealth, kneeLHealth,
        kneeRHealth, shinLHealth, shinRHealth, ankleLHealth,
        ankleRHealth, footLHealth, footRHealth;

    /// <summary>
    /// Copies and returns an "enemy"'s Scriptable Object.
    /// </summary>
    /// <param name="gESO"></param>
    /// <returns></returns>
    public static GenericEnemySO EnemySOCopy(GenericEnemySO gESO)
    {
        GenericEnemySO _gESO = CreateInstance<GenericEnemySO>();
        _gESO.headHealth = gESO.headHealth;
        _gESO.neckHealth = gESO.neckHealth;
        _gESO.chestHealth = gESO.chestHealth;
        _gESO.torsoHealth = gESO.torsoHealth;
        _gESO.hipHealth = gESO.hipHealth;
        _gESO.groinHealth = gESO.groinHealth;
        _gESO.shoulderLHealth = gESO.shoulderLHealth;
        _gESO.shoulderRHealth = gESO.shoulderRHealth;
        _gESO.upperArmLHealth = gESO.upperArmLHealth;
        _gESO.upperArmRHealth = gESO.upperArmRHealth;
        _gESO.elbowLHealth = gESO.elbowLHealth;
        _gESO.elbowRHealth = gESO.elbowRHealth;
        _gESO.forearmLHealth = gESO.forearmLHealth;
        _gESO.forearmRHealth = gESO.forearmRHealth;
        _gESO.WristLHealth = gESO.WristLHealth;
        _gESO.WristRHealth = gESO.WristRHealth;
        _gESO.HandLHealth = gESO.HandLHealth;
        _gESO.HandRHealth = gESO.HandRHealth;
        _gESO.thighLHealth = gESO.thighLHealth;
        _gESO.thighRHealth = gESO.thighRHealth;
        _gESO.kneeLHealth = gESO.kneeLHealth;
        _gESO.kneeRHealth = gESO.kneeRHealth;
        _gESO.shinLHealth = gESO.shinLHealth;
        _gESO.shinRHealth = gESO.shinRHealth;
        _gESO.ankleLHealth = gESO.ankleLHealth;
        _gESO.ankleRHealth = gESO.ankleRHealth;
        _gESO.footLHealth = gESO.footLHealth;
        _gESO.footRHealth = gESO.footRHealth;

        return _gESO;
    }

    /// <summary>
    /// Copies and returns an "enemy"'s Scriptable Object.
    /// </summary>
    /// <returns></returns>
    public GenericEnemySO EnemySOCopy()
    {
        GenericEnemySO _gESO = CreateInstance<GenericEnemySO>();
        _gESO.headHealth = headHealth;
        _gESO.neckHealth = neckHealth;
        _gESO.chestHealth = chestHealth;
        _gESO.torsoHealth = torsoHealth;
        _gESO.hipHealth = hipHealth;
        _gESO.groinHealth = groinHealth;
        _gESO.shoulderLHealth = shoulderLHealth;
        _gESO.shoulderRHealth = shoulderRHealth;
        _gESO.upperArmLHealth = upperArmLHealth;
        _gESO.upperArmRHealth = upperArmRHealth;
        _gESO.elbowLHealth = elbowLHealth;
        _gESO.elbowRHealth = elbowRHealth;
        _gESO.forearmLHealth = forearmLHealth;
        _gESO.forearmRHealth = forearmRHealth;
        _gESO.WristLHealth = WristLHealth;
        _gESO.WristRHealth = WristRHealth;
        _gESO.HandLHealth = HandLHealth;
        _gESO.HandRHealth = HandRHealth;
        _gESO.thighLHealth = thighLHealth;
        _gESO.thighRHealth = thighRHealth;
        _gESO.kneeLHealth = kneeLHealth;
        _gESO.kneeRHealth = kneeRHealth;
        _gESO.shinLHealth = shinLHealth;
        _gESO.shinRHealth = shinRHealth;
        _gESO.ankleLHealth = ankleLHealth;
        _gESO.ankleRHealth = ankleRHealth;
        _gESO.footLHealth = footLHealth;
        _gESO.footRHealth = footRHealth;

        return _gESO;
    }

    public GenericEnemySO EnemySOCopy(EnemyMasterScript eMS)
    {
        GenericEnemySO _gESO = CreateInstance<GenericEnemySO>();
        _gESO.headHealth = eMS.headHealth;
        _gESO.neckHealth = eMS.neckHealth;
        _gESO.chestHealth = eMS.chestHealth;
        _gESO.torsoHealth = eMS.torsoHealth;
        _gESO.hipHealth = eMS.hipHealth;
        _gESO.groinHealth = eMS.groinHealth;
        _gESO.shoulderLHealth = eMS.shoulderLHealth;
        _gESO.shoulderRHealth = eMS.shoulderRHealth;
        _gESO.upperArmLHealth = eMS.upperArmLHealth;
        _gESO.upperArmRHealth = eMS.upperArmRHealth;
        _gESO.elbowLHealth = eMS.elbowLHealth;
        _gESO.elbowRHealth = eMS.ElbowRHealth;
        _gESO.forearmLHealth = eMS.forearmLHealth;
        _gESO.forearmRHealth = eMS.forearmRHealth;
        _gESO.WristLHealth = eMS.wristLHealth;
        _gESO.WristRHealth = eMS.wristRHealth;
        _gESO.HandLHealth = eMS.handLHealth;
        _gESO.HandRHealth = eMS.handRHealth;
        _gESO.thighLHealth = eMS.thighLHealth;
        _gESO.thighRHealth = eMS.thighRHealth;
        _gESO.kneeLHealth = eMS.kneeLHealth;
        _gESO.kneeRHealth = eMS.kneeRHealth;
        _gESO.shinLHealth = eMS.shinLHealth;
        _gESO.shinRHealth = eMS.shinRHealth;
        _gESO.ankleLHealth = eMS.ankleLHealth;
        _gESO.ankleRHealth = eMS.ankleRHealth;
        _gESO.footLHealth = eMS.footLHealth;
        _gESO.footRHealth = eMS.footRHealth;

        return _gESO;
    }
}
