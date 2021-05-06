using System.Collections.Generic;
using Interface;
using Pathfinding;
using UnityEngine;

namespace Managers
{
    public class PlacementManager : MonoBehaviour
    {
        [Header("Managers")] [SerializeField] private ResourcesManager rm;
        [SerializeField] private GameManager gameManager;

        [Header("Debug")] [SerializeField] private List<Node> nodes;

        public int Price { get; set; }
        public int Reputation { get; set; }

        public void CreateBlueprint(GameObject blueprint)
        {
            if (rm.Gold < Price)
            {
                gameManager.GamePause = false;
                gameManager.NotificationSystem.CreateNotification(
                    $"Not enough money... \n {Price - rm.Gold} coins missing!", 4f, NotificationType.Warning
                );
                return;
            }

            Instantiate(blueprint, new Vector3(-200, 0, -200), Quaternion.identity);
            rm.Gold -= Price;
            rm.Reputation += Reputation;
        }

        public void InitAllNodes()
        {
            foreach (var node in nodes) node.initialize();
        }
    }
}