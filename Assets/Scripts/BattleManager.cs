using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BattleManager : MonoBehaviour
{
    public UnityEvent OnTurnBegin = new UnityEvent();
    [Header("Battle Control Settings")]
    public bool playerMadeTurn;
    [SerializeField] Battler[] battlers;
    public Battler currentBattler, targetBattler;

    
    [Header("Battle Placement Settings")]
    [SerializeField] float timeToGetToAttackPoints; //how fast characters will occupy their battle positions
    [SerializeField] Transform playerAttackPoint; //these are position where battlers will stand in attack animation 
    [SerializeField] Transform enemyAttackPoint;
    Vector3[] oldPositions = new Vector3[2]; //for correctly returning battlers to their original positions

    [Header("References")]
    [SerializeField] ConsoleManager console;
    [SerializeField] DynamicCamera cameraController;

    //for convenience
    public Battler[] EnemyTeam { get; private set; }
    public Battler[] PlayerTeam { get; private set; }
    public bool PlayerTurn => !currentBattler.isEnemy;

    bool _skippedTurn;
    int _currentBattlerIndex = 1;

    private void Start()
    {
        foreach(Battler b in battlers)
        {
            //deciding turn order
            b.turnOrder = Random.Range(0, 16);
        }
        //for UI/AI purposes
        EnemyTeam = battlers.Where(battler => battler.isEnemy).ToArray();
        PlayerTeam = battlers.Where(battler => !battler.isEnemy).ToArray();
        //for convenience, sort orders by their turn order
        battlers = battlers.OrderBy(battler => battler.turnOrder).ToArray();

        currentBattler = battlers[0];
        //let's go
        StartCoroutine(MakeTurn());
    }

    //turn process
    IEnumerator MakeTurn()
    {
        while (true)
        {
            OnTurnBegin.Invoke(); // UI event
            //notify whose turn is this
            console.TurnStart(currentBattler.battlerName);
            if (currentBattler.isEnemy)
            {
                //let the message sit there for a bit
                yield return new WaitForSeconds(1);
                //change it to a "thinking" one
                console.AIThinking(currentBattler.battlerName);
                yield return new WaitForSeconds(1 + Random.Range(0.5f,1.5f)); //simulate enemy thinking
                //pick a target, if null - skip 
                targetBattler = currentBattler.AI_MakeDecision(PlayerTeam);
                if (targetBattler == null)
                    SkipTurn();
            }
            else
            {
                playerMadeTurn = false;
                //wait until UI responses with picked target/skip
                yield return new WaitUntil(() => playerMadeTurn);
            }
            //purely visual, just so console has enough time to show that turn was skipped
            if (_skippedTurn)
            {
                console.SkipTurn(currentBattler.battlerName);
                yield return new WaitForSeconds(1.5f);
            }
            else
            {
                //if we fightin', we fightin'
                yield return StartCoroutine(Attack());
            }

            //increase index by 1 and in case we try to go over battlers count, round back to zero
            currentBattler = battlers[_currentBattlerIndex++ % battlers.Length];
            //reset
            _skippedTurn = false;
            yield return null;
        }
    }
    
    //stores picked target
    public void Player_PickTarget(int index)
    {
        targetBattler = EnemyTeam[index];
        playerMadeTurn = true;
    }

    //notifies with console that a turn is skipped and continues the battle
    public void SkipTurn()
    {
        console.SkipTurn(currentBattler.battlerName);
        _skippedTurn = true;
        if (PlayerTurn)
            playerMadeTurn = true;
    }

    //this could probably be made as well with Timeline
    IEnumerator Attack()
    {
        //console print out
        console.Attack(currentBattler.battlerName, targetBattler.battlerName);

        //switch to a better view
        cameraController.ChangeTarget(CameraFocalPoint.BATTLEFIELD);
        cameraController.ChangeOffset(CameraOffset.DEFAULT);

        //remember default positions
        oldPositions[0] = currentBattler.transform.position;
        oldPositions[1] = targetBattler.transform.position;

        //if this is a 2D animated character, show some animations
        //if this is a glorified cube, tough luck then
        if (currentBattler.usesSpine)
        {
            currentBattler.animationState.SetAnimation(0, currentBattler.walkAnim, true);
        }
        if(targetBattler.usesSpine)
        {
            targetBattler.animationState.SetAnimation(0, targetBattler.walkAnim, true);
        }

        //move both participants to a front stage in a certain time
        float progress = 0;
        while(progress < timeToGetToAttackPoints)
        {
            if (PlayerTurn)
            {
                currentBattler.transform.position = Vector3.Lerp(oldPositions[0], playerAttackPoint.position, progress / timeToGetToAttackPoints);
                targetBattler.transform.position = Vector3.Lerp(oldPositions[1], enemyAttackPoint.position, progress / timeToGetToAttackPoints);
            }
            else
            {
                currentBattler.transform.position = Vector3.Lerp(oldPositions[0], enemyAttackPoint.position, progress / timeToGetToAttackPoints);
                targetBattler.transform.position = Vector3.Lerp(oldPositions[1], playerAttackPoint.position, progress / timeToGetToAttackPoints);
            }
            progress += Time.deltaTime;
            yield return null;
        }

        if (currentBattler.usesSpine)
        {
            currentBattler.animationState.SetAnimation(0, currentBattler.idleAnim, true);
        }
        if (targetBattler.usesSpine)
        {
            targetBattler.animationState.SetAnimation(0, targetBattler.idleAnim, true);
        }

        //wait a bit
        yield return new WaitForSeconds(0.5f);

        //attack
        if (currentBattler.usesSpine)
        {
            currentBattler.animationState.SetAnimation(0, currentBattler.attackAnim, false);
            currentBattler.animationState.AddAnimation(0, currentBattler.idleAnim, true, 0);
        }

        //pokemon like delay
        //for added suspense
        yield return new WaitForSeconds(0.8f);
        //dummy for actual hit/miss/crit calculations
        if (Random.value < 0.8f)
        {
            console.Hit();
            if (targetBattler.usesSpine)
            {
                targetBattler.animationState.SetAnimation(0, targetBattler.hitAnim, false);
            }
        }
        else
        {
            if (Random.value < 0.5f)
                console.Miss();
            else
            {
                console.Crit();
                if (targetBattler.usesSpine)
                {
                    targetBattler.animationState.SetAnimation(0, targetBattler.hitAnim, false);
                }
            }
        }
        //wait a bit
        yield return new WaitForSeconds(0.8f);

        //get back with animations
        if (currentBattler.usesSpine)
        {
            currentBattler.animationState.SetAnimation(0, currentBattler.walkAnim, true);
        }
        if (targetBattler.usesSpine)
        {
            targetBattler.animationState.SetAnimation(0, targetBattler.walkAnim, true);
        }

        progress = 0;
        while (progress < timeToGetToAttackPoints)
        {
            if (PlayerTurn)
            {
                currentBattler.transform.position = Vector3.Lerp(playerAttackPoint.position, oldPositions[0], progress / timeToGetToAttackPoints);
                targetBattler.transform.position = Vector3.Lerp(enemyAttackPoint.position, oldPositions[1], progress / timeToGetToAttackPoints);
            }
            else
            {
                currentBattler.transform.position = Vector3.Lerp(enemyAttackPoint.position, oldPositions[0], progress / timeToGetToAttackPoints);
                targetBattler.transform.position = Vector3.Lerp(playerAttackPoint.position, oldPositions[1], progress / timeToGetToAttackPoints);
            }
            progress += Time.deltaTime*2;
            yield return null;
        }
        if (currentBattler.usesSpine)
        {
            currentBattler.animationState.SetAnimation(0, currentBattler.idleAnim, true);
        }
        if (targetBattler.usesSpine)
        {
            targetBattler.animationState.SetAnimation(0, targetBattler.idleAnim, true);
        }
    }
}
