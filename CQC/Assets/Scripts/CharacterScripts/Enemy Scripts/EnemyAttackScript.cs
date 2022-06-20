using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackScript : MonoBehaviour
{
    [SerializeField]
    private EnemyMasterScript eMS;
    public bool blocked = false, parried = false, hit = false;
    private PlayerScript playerScript;
    private PlayerBlockScript playerBlockScript;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("BlockBox"))
        {
            playerBlockScript = other.GetComponent<PlayerBlockScript>();
            if (playerBlockScript.ActiveParry)
            {
                EnemyAttackParried();
            }
            else
            {
                EnemyAttackBlocked();
            }
        }
        else if(other.CompareTag("Player") && !blocked)
        {
            //playerScript.health -= eMS.AttackDamage;
            //eMS.anim.SetBool("Hit", true);
            EnemyAttackHit();

        }

    }
    public void ResetEnemyAttackVariables()
    {
        blocked = false;
        parried = false;
        hit = false;
    }

    public void EnemyAttackBlocked()
    {
        Debug.Log("BLOCKED!");
        eMS.enemyAttackScript = GetComponent<EnemyAttackScript>();
        eMS.actionStatus = EnemyMasterScript.ActionStatus.Blocked;
        parried = false;
        blocked = true;
    }

    public void EnemyAttackParried()
    {
        Debug.Log("*Dark Souls Parry Noise!*");
        eMS.enemyAttackScript = GetComponent<EnemyAttackScript>();
        eMS.actionStatus = EnemyMasterScript.ActionStatus.Parried;
        parried = true;
        blocked = true;
        //eMS.actionStatus = EnemyMasterScript.ActionStatus.Parried;
    }

    public void EnemyAttackHit()
    {
        eMS.enemyAttackScript = GetComponent<EnemyAttackScript>();

    }
    public void EnemyAttackHit(PlayerScript _pS)
    {
        eMS.enemyAttackScript = GetComponent<EnemyAttackScript>();
        _pS.TakeDamage(eMS.AttackDamage);
    }
}
