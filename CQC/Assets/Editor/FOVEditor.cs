using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnemyMasterScript))]
public class FOVEditor : Editor
{
    private void OnSceneGUI()
    {
        EnemyMasterScript eMS = (EnemyMasterScript)target;

        Handles.color = Color.white;
        Handles.DrawWireArc(eMS.transform.position, Vector3.up, Vector3.forward, 360, eMS.fovRadius);

        Vector3 viewAngle1 = DirectionFromAngle(eMS.transform.eulerAngles.y, -eMS.fovAngle / 2);
        Vector3 viewAngle2 = DirectionFromAngle(eMS.transform.eulerAngles.y, eMS.fovAngle / 2);

        Handles.color = Color.yellow;
        Handles.DrawLine(eMS.transform.position, eMS.transform.position + viewAngle1 * eMS.fovRadius);
        Handles.DrawLine(eMS.transform.position, eMS.transform.position + viewAngle2 * eMS.fovRadius);

        Handles.color = Color.gray;
        Handles.DrawWireArc(eMS.transform.position, Vector3.up, Vector3.forward, 360, eMS.maxAttackRange);


        if (eMS.canSeePlayer && eMS.playerInRange)
        {
            Handles.color = Color.red;
            Handles.DrawLine(eMS.transform.position, eMS.pS.transform.position);
        }
        else if(eMS.canSeePlayer)
        {
            Handles.color = Color.green;
            Handles.DrawLine(eMS.transform.position, eMS.pS.transform.position);

        }
    }

    private Vector3 DirectionFromAngle(float eulerY, float angleInDegrees)
    {
        angleInDegrees += eulerY;

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
