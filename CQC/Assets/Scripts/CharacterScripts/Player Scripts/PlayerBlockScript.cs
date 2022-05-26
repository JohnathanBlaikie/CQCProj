using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBlockScript : MonoBehaviour
{
    [SerializeField]
    private PlayerScript pS;
    [HideInInspector]
    public int eBSIncrement = 0;
    //private EnemyBodySimple[] eBSArray;
    private List<EnemyAttackScript> eAList = new List<EnemyAttackScript>();
    private EnemyAttackScript tempEAS;
    public bool ActiveParry = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyAttack"))
        {
            tempEAS = other.gameObject.GetComponent<EnemyAttackScript>();
            if (tempEAS.hit == false)
            {
                eAList.Add(tempEAS);
                //if(ActiveParry)
                //{
                    //tempEAS.parried = true;
                //}
                //tempEAS.blocked = true;

                //tempEAS.TakeDamage(pS.lightHitDamage);
                //pS.HitForceApplication(other.GetComponent<Rigidbody>());
                //pS.pAS = this.GetComponent<PlayerAttackScript>();
            }

        }
    }

    public void TogglePerfectParry()
    {
        ActiveParry = !ActiveParry;
    }

    public void ResetPBSList()
    {
        
        try
        {

            if (eAList[0] != null)
            {
                for (int i = 0; i < eAList.Count; i++)
                {
                    eAList[i].blocked = false;
                }
                eAList.Clear();
            }
        }
        catch { }
    }
}
