using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DroneManager))]
public class FieldOfViewEditor : Editor
{
    private void OnSceneGUI()
    {
        DroneManager fov = (DroneManager)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(fov.transform.position, Vector3.forward, Vector3.right, 360, fov.radius);

        Vector3 viewAngle01 = DirectionFromAngle(-fov.transform.eulerAngles.z + 180, -fov.angle / 2);
        Vector3 viewAngle02 = DirectionFromAngle(-fov.transform.eulerAngles.z + 180, fov.angle / 2);

        Handles.color = Color.yellow;
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngle01 * fov.radius);
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngle02 * fov.radius);

        if (fov.canSeePlayer)
        {
            Handles.color = Color.green;
            Handles.DrawLine(fov.transform.position, fov.playerRef.transform.position);
        }
    }

    private Vector3 DirectionFromAngle(float eulerZ, float angleInDegrees)
    {
        angleInDegrees += eulerZ;

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), Mathf.Cos(angleInDegrees * Mathf.Deg2Rad), 0);
    }


}
