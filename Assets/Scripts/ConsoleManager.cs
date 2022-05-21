using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class ConsoleManager : MonoBehaviour
{
    public TextMeshProUGUI text;

    public void AIThinking(string name)
    {
        text.text = $"{name} is thinking...";
    }

    public void SkipTurn(string name)
    {
        text.text = $"{name} skips turn...";
    }

    public void Attack(string attackerName, string targetName)
    {
        text.text = $"{attackerName} attacks {targetName}!";
    }

    public void PickTarget(string name)
    {
        text.text = $"{name} picks a target...";
    }

    public void TurnStart(string name)
    {
        text.text = $"It's {name} turn!";
    }
    public void Miss()
    {
        text.text = "They missed...";
    }

    public void Hit()
    {
        text.text = "Successful hit!";
    }

    public void Crit()
    {
        text.text = "Critical hit!";
    }
}
