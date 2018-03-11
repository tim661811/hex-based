using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Resource : MonoBehaviour 
{
    public int Amount;
    public int StartingAmount;

    public UnityEvent OnValueChanged;


    public void Awake()
    {
        Amount = StartingAmount;
        UpdateUI();
    }

    public void AddAmount(int value)
    {
        if (value < 0)
        {
            Debug.LogError("Can't add negative values!");
        }
        else
        {
            Amount += value;
            UpdateUI();
        }
    }

    public void SubtractAmount(int value)
    {
        if (value < 0 || Amount - value < 0)
        {
            Debug.LogError("Can't subtract negative values or fall below 0 wood/gold!");
        }
        else
        {
            Amount -= value;
            UpdateUI();
        }
    }

    public void UpdateUI()
    {
        OnValueChanged.Invoke();
    }

    public bool CanAfford(int Cost)
    {
        return Cost <= Amount;
    }
}
