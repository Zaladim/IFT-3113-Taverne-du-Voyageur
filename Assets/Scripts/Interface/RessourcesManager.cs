using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RessourcesManager : MonoBehaviour
{
    [SerializeField] private int gold = 500;
    [SerializeField] private Text display;

    public int Gold
    {
        get => gold;
        set
        {
            gold = value;
            display.text = "Gold: " + gold;
        }
    }

    private void Awake()
    {
        display.text = "Gold: " + gold;
    }
}
