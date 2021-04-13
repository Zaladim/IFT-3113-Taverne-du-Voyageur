using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;
using Random = UnityEngine.Random;

public class QuestGiver : MonoBehaviour
{
    [SerializeField] private ResourcesManager ressources;


    private void Awake()
    {
        ressources = GameObject.FindGameObjectWithTag("RessourcesManager").GetComponent<ResourcesManager>();
    }

    public void ReturnQuest()
    {
        if (Random.Range(0, 100) >= 50)
        {
            ressources.Gold -= Random.Range(0, 100);
            ressources.Reputation += Random.Range(1, 10);
        }
    }
}
