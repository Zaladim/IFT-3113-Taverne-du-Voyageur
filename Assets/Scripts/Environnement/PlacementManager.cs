using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementManager : MonoBehaviour
{
    [SerializeField] private GameObject roomUI;
    [SerializeField] private RessourcesManager ressources;

    private int price;

    public int Price
    {
        get => price;
        set => price = value;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (roomUI.gameObject.activeSelf)
            {
                roomUI.gameObject.SetActive(false);
            } else
            {
                roomUI.gameObject.SetActive(true);
            }
        }
    }

    public void CreateBlueprint(GameObject blueprint)
    {
        if (ressources.Gold >= price)
        {
            Instantiate(blueprint);
            ressources.Gold -= price;
            if (roomUI.gameObject.activeSelf)
            {
                roomUI.gameObject.SetActive(false);
            } else
            {
                roomUI.gameObject.SetActive(true);
            }
        }
    }
}
