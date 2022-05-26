using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBodySimple : MonoBehaviour
{
    public bodyPartMasterScript bPM;
    public bodyPartMasterScript.TargetGeneralArea tGA;
    public bodyPartMasterScript.TargetHead tHA;
    public bodyPartMasterScript.TargetTorso tTA;
    public bodyPartMasterScript.TargetArm tAA;
    public bodyPartMasterScript.TargetLeg tLA;

    public EnemyMasterScript eS;

    public Rigidbody rig;

    public bool hitCheck = false;
    

    private void Start()
    {
        try
        {
            rig = GetComponent<Rigidbody>();
            //rig.isKinematic = true;
            //rig.constraints = RigidbodyConstraints.FreezeAll;
            
        }
        catch { }
    }

    public void TakeDamage(float damage)
    {
        eS.anim.SetBool("HitRecoil", true);
        switch (tGA)
        {
            case bodyPartMasterScript.TargetGeneralArea.None:

                break;

            case bodyPartMasterScript.TargetGeneralArea.Head:
                eS.DamageHead(tHA, damage);
                break;

            case bodyPartMasterScript.TargetGeneralArea.Torso:
                eS.DamageTorso(tTA, damage);
                break;
                
            case bodyPartMasterScript.TargetGeneralArea.Arm:
                eS.DamageArm(tAA, damage);
                break;

            case bodyPartMasterScript.TargetGeneralArea.Leg:
                eS.DamageLeg(tLA, damage);
                break;
        }
        if(eS.health <= 0)
        {
            try
            {
                rig.isKinematic = false;
            }
            catch
            {
                if(GetComponentInParent<Rigidbody>() != null)
                {
                    GetComponentInParent<Rigidbody>().isKinematic = false;
                }
            }
            finally
            {

            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {

    }

}
