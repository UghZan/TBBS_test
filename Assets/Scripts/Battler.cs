using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class Battler : MonoBehaviour
{
    [Header("Battler Stats")]
    public string battlerName;
    public bool isEnemy;
    public int turnOrder;
    public float markerOffset; //used to correctly place selection marker
    [Header("Animation Settings")]
    public bool usesSpine;

    public Spine.AnimationState animationState;
    [SpineAnimation]
    public string idleAnim;
    [SpineAnimation]
    public string walkAnim;
    [SpineAnimation]
    public string attackAnim;
    [SpineAnimation]
    public string hitAnim;

    private void Start()
    {
        if(usesSpine) animationState = GetComponent<SkeletonAnimation>().AnimationState;
    }

    //not really an "intelligence", but it works
    public Battler AI_MakeDecision(Battler[] enemies)
    {
        if(Random.value < 0.1)
        {
            return null; //we skippin'
        }
        else
        {
            //pick random enemy
            return enemies[Random.Range(0, enemies.Length)];
        }
    }
}
