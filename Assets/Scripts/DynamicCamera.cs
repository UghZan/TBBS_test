using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CameraOffset
{
    DEFAULT,
    FAR,
    CLOSER
}
//TWO_TEAMS is right between two teams
//BATTLEFIELD is slightly lower, where two current battlers are playing out animations
public enum CameraFocalPoint
{
    PLAYER_TEAM,
    ENEMY_TEAM,
    TWO_TEAMS,
    BATTLEFIELD
}
public class DynamicCamera : MonoBehaviour
{
    [Header("Offset Presets")]
    [SerializeField] Vector3 defaultOffset;
    [SerializeField] Vector3 farOffset;
    [SerializeField] Vector3 closerOffset;

    [Header("Camera Focal Points")]
    [SerializeField] Transform playerTeam;
    [SerializeField] Transform enemyTeam;
    [SerializeField] Transform twoTeams;
    [SerializeField] Transform battleField;

    [Header("Follow Settings")]
    [SerializeField] Transform currentTarget;
    Vector3 _offset;
    public float followSpeed;

    public void ChangeTarget(Transform newTarget)
    {
        currentTarget = newTarget;
    }

    public void ChangeOffset(CameraOffset newOffset)
    {
        switch(newOffset)
        {
            case CameraOffset.DEFAULT:
                _offset = defaultOffset;
                break;
            case CameraOffset.FAR:
                _offset = farOffset;
                break;
            case CameraOffset.CLOSER:
                _offset = closerOffset;
                break;
        }
    }

    public void ChangeTarget(CameraFocalPoint newPoint)
    {
        switch (newPoint)
        {
            case CameraFocalPoint.PLAYER_TEAM:
                ChangeTarget(playerTeam);
                break;
            case CameraFocalPoint.ENEMY_TEAM:
                ChangeTarget(enemyTeam);
                break;
            case CameraFocalPoint.TWO_TEAMS:
                ChangeTarget(twoTeams);
                break;
            case CameraFocalPoint.BATTLEFIELD:
                ChangeTarget(battleField);
                break;
        }
    }

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, currentTarget.transform.position + _offset, Time.deltaTime * followSpeed);
    }
}
