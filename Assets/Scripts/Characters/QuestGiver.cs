using Managers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Characters
{
    public class QuestGiver : MonoBehaviour
    {
        [SerializeField] private ResourcesManager ressources;
        [SerializeField, Range(0, 100)] private int luckyQuest = 50;
        [SerializeField] private int goldAwardMin = 10;
        [SerializeField] private int goldAwardMax = 25;
        [SerializeField] private int reputationAwardMin = 5;
        [SerializeField] private int reputationAwardMax = 10;

        private void Awake()
        {
            ressources = GameObject.FindGameObjectWithTag("RessourcesManager").GetComponent<ResourcesManager>();
        }

        public void ReturnQuest()
        {
            if (ressources.Gold <= 0 )
                return;
        
            if (Random.Range(0, 100) >= luckyQuest)
            {
                ressources.Gold -= Random.Range(goldAwardMin, goldAwardMax);
                ressources.Reputation += Random.Range(reputationAwardMin, reputationAwardMax);
            }
        }
    }
}
