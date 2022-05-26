using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class EnemyBodyCollisionDetection : MonoBehaviour
{
    public bodyPartMasterScript bPM;
    public bodyPartMasterScript.TargetGeneralArea tGA;
    public bodyPartMasterScript.TargetHead tHA;
    public bodyPartMasterScript.TargetTorso tTA;
    public bodyPartMasterScript.TargetArm tAA;
    public bodyPartMasterScript.TargetLeg tLA;
     
    //public EnemyScript eS;

    public DynamicInspector dI;

    public bool isEnabled;

    public void TakeDamage()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        
    }

}

[CustomEditor(typeof(EnemyBodyCollisionDetection))]
public class DynamicInspector : Editor
{
    SerializedProperty _BPM, _TGA, _THA, _TTA, _TAA, _TLA, _ES, _isEnabled;
    override public void OnInspectorGUI()
    {
        var eBCD = target as EnemyBodyCollisionDetection;
        _BPM = serializedObject.FindProperty("bPM");
        _TGA = serializedObject.FindProperty("tGA");
        _THA = serializedObject.FindProperty("tHA");
        _TTA = serializedObject.FindProperty("tTA");
        _TAA = serializedObject.FindProperty("tAA");
        _TLA = serializedObject.FindProperty("tLA");
        _ES = serializedObject.FindProperty("eS");

        using (var group = new EditorGUILayout.FadeGroupScope(Convert.ToSingle(eBCD.isEnabled)))
        {
            eBCD.dI = this;
            EditorGUILayout.PropertyField(_isEnabled, new GUIContent("Enable Spawner"));
            if (group.visible)
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(_BPM, new GUIContent("Entity Bodypart Manager"));
                EditorGUILayout.PropertyField(_TGA, new GUIContent("General Area"));
                switch (eBCD.tGA)
                {
                    case bodyPartMasterScript.TargetGeneralArea.None:

                        break;
                    case bodyPartMasterScript.TargetGeneralArea.Head:
                        EditorGUILayout.PropertyField(_THA, new GUIContent("Head Area"));
                        //switch (eBCD.bPM.targetHeadPoint)
                        //{
                        //    case bodyPartMasterScript.TargetHead.Head:

                        //        break;
                        //    case bodyPartMasterScript.TargetHead.Neck:

                        //        break;
                        //}

                        break;
                    case bodyPartMasterScript.TargetGeneralArea.Torso:
                        EditorGUILayout.PropertyField(_TTA, new GUIContent("Torso Area"));
                        //switch (eBCD.bPM.targetTorsoPoint)
                        //{
                        //    case bodyPartMasterScript.TargetTorso.Chest:

                        //        break;
                        //    case bodyPartMasterScript.TargetTorso.Stomach:

                        //        break;
                        //    case bodyPartMasterScript.TargetTorso.Hip:

                        //        break;
                        //    case bodyPartMasterScript.TargetTorso.Groin:

                        //        break;

                        //}
                        break;
                    case bodyPartMasterScript.TargetGeneralArea.Arm:
                        EditorGUILayout.PropertyField(_TAA, new GUIContent("Arm Area"));
                        //switch (eBCD.bPM.targetArmPoint)
                        //{
                        //    case bodyPartMasterScript.TargetArm.LUpperArm:

                        //        break;
                        //    case bodyPartMasterScript.TargetArm.RUpperArm:

                        //        break;
                        //    case bodyPartMasterScript.TargetArm.LForearm:

                        //        break;
                        //    case bodyPartMasterScript.TargetArm.RForearm:

                        //        break;
                        //    case bodyPartMasterScript.TargetArm.LWrist:

                        //        break;
                        //    case bodyPartMasterScript.TargetArm.RWrist:

                        //        break;
                        //    case bodyPartMasterScript.TargetArm.LHand:

                        //        break;
                        //    case bodyPartMasterScript.TargetArm.RHand:

                        //        break;

                        //}

                        break;

                    case bodyPartMasterScript.TargetGeneralArea.Leg:
                        EditorGUILayout.PropertyField(_TLA, new GUIContent("Leg Area"));
                        //switch (eBCD.bPM.targetLegPoint)
                        //{
                        //    case bodyPartMasterScript.TargetLeg.ThighL:

                        //        break;
                        //    case bodyPartMasterScript.TargetLeg.ThighR:

                        //        break;
                        //    case bodyPartMasterScript.TargetLeg.KneeL:

                        //        break;
                        //    case bodyPartMasterScript.TargetLeg.KneeR:

                        //        break;
                        //    case bodyPartMasterScript.TargetLeg.ShinL:

                        //        break;
                        //    case bodyPartMasterScript.TargetLeg.ShinR:

                        //        break;
                        //    case bodyPartMasterScript.TargetLeg.AnkleL:

                        //        break;
                        //    case bodyPartMasterScript.TargetLeg.AnkleR:

                        //        break;


                        //}

                        break;
                }
            }
        }

    }

}
