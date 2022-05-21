using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BattleManagerUI : MonoBehaviour
{
    public bool mode; // true is targeting mode, false is default mode
    [Header("UI Elements")]
    [SerializeField] GameObject targetingMarker;
    [SerializeField] GameObject battleMenu;

    [Header("Targeting Options")]
    int _currentPickIndex;
    float MarkerOffset => battleManagerInstance.currentBattler.markerOffset;

    [Header("References")]
    [SerializeField] BattleManager battleManagerInstance;
    [SerializeField] DynamicCamera cameraController;
    [SerializeField] ConsoleManager console;

    void Awake()
    {
        battleManagerInstance.OnTurnBegin.AddListener(OnTurnStarted);
    }

    // Update is called once per frame
    void Update()
    {
        //we pickin' now
        if (mode)
        {
            //always keep marker on current target
            targetingMarker.transform.position = battleManagerInstance.EnemyTeam[_currentPickIndex].transform.position + Vector3.up * MarkerOffset;

            //new Input System would be preferable (and probably better because less code in Update), but this is a prototype, who cares
            //on A -> go left
            if (Input.GetKeyDown(KeyCode.A))
            {
                _currentPickIndex--;
                if (_currentPickIndex < 0) _currentPickIndex = 3;
                cameraController.ChangeTarget(battleManagerInstance.EnemyTeam[_currentPickIndex].transform);
            }
            //on D -> go right
            else if (Input.GetKeyDown(KeyCode.D))
            {
                _currentPickIndex = (_currentPickIndex + 1) % 4;
                cameraController.ChangeTarget(battleManagerInstance.EnemyTeam[_currentPickIndex].transform);
            }
            //enter -> pick up a target
            else if(Input.GetKeyDown(KeyCode.Return))
            {
                battleManagerInstance.Player_PickTarget(_currentPickIndex);
                mode = false;
            }
        }
    }

    void OnTurnStarted()
    {
        //if it's player's turn, show commands menu, else hide
        battleMenu.SetActive(!battleManagerInstance.currentBattler.isEnemy);
        //marker up on the current battler
        targetingMarker.transform.position = battleManagerInstance.currentBattler.transform.position + Vector3.up * MarkerOffset;
        //change camera to current battler
        cameraController.ChangeTarget(battleManagerInstance.currentBattler.transform);
        cameraController.ChangeOffset(CameraOffset.CLOSER);
    }

    //called on pressing "attack" button
    public void SwitchToTargetingMode()
    {
        //we pickin' now
        mode = true;
        //disable commands to prevent strange stuff from hapenning
        battleMenu.SetActive(false);
        //console notification
        console.PickTarget(battleManagerInstance.currentBattler.battlerName);

        //this is called for player team only, so we can be sure of that
        //would be cool to show enemy picking target similarly, but it's not that important
        cameraController.ChangeTarget(CameraFocalPoint.ENEMY_TEAM);

        _currentPickIndex = 0;
    }

    //called on pressing "skip" button
    //self-explanatory
    public void SkipTurn()
    {
        battleMenu.SetActive(false);
        battleManagerInstance.SkipTurn();
    }
}
