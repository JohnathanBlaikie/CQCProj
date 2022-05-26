using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackScript : MonoBehaviour
{
    [SerializeField]
    private PlayerScript pS;
    [HideInInspector]
    public int eBSIncrement = 0;
    //private EnemyBodySimple[] eBSArray;
    private List<EnemyBodySimple> eBSList = new List<EnemyBodySimple>();
    private EnemyBodySimple tempEBS;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            //try
            //{
            //    if (eBSArray[eBSIncrement] == null)
            //    {
            //        eBSArray[eBSIncrement] = other.GetComponent<EnemyBodySimple>();
            //        eBSIncrement++;
            //    }
            //    else
            //    {
            //        while (eBSArray[eBSIncrement] != null)
            //        {
            //            eBSIncrement++;
            //        }
            //    }
            //    eBSArray[eBSIncrement] = other.GetComponent<EnemyBodySimple>();
            //    if (other.GetComponent<Rigidbody>() != null && eBSArray[eBSIncrement].hitCheck == false)
            //    {
            //        pS.HitForceApplication(other.GetComponent<Rigidbody>());
            //        eBSArray[eBSIncrement].hitCheck = true;
            //    }
            //    int tempInt = 0;
            //    while (pS.pAS[tempInt] != null)
            //    {
            //        tempInt++;
            //    }
            //    pS.pAS[tempInt] = GetComponent<PlayerAttackScript>();

            //}
            //catch { Debug.LogWarning("Collided with " + other.name + ", but there was a problem!"); }
            //finally { }
            tempEBS = other.gameObject.GetComponent<EnemyBodySimple>();
            if (tempEBS.hitCheck == false)
            {
                eBSList.Add(tempEBS);
                tempEBS.hitCheck = true;
                tempEBS.TakeDamage(pS.lightHitDamage);
                Debug.Log(tempEBS.transform.name);
                try
                {
                    pS.HitForceApplication(other.GetComponent<Rigidbody>());
                }
                catch
                {
                    pS.HitForceApplication(other.GetComponentInParent<Rigidbody>());

                }
                pS.pAS = this.GetComponent<PlayerAttackScript>();
            }

        }
    }

    public void ResetEBSList()
    {
        //for(int i = eBSIncrement; i >= 0; i--)
        //{
        //    eBSArray[i].hitCheck = false;
        //    eBSArray[i] = null;
        //}
        //eBSIncrement = 0;
        try
        {

            if (eBSList[0] != null)
            {
                for (int i = 0; i < eBSList.Count; i++)
                {
                    eBSList[i].hitCheck = false;
                }
                eBSList.Clear();
            }
        }
        catch { }
    }
}
