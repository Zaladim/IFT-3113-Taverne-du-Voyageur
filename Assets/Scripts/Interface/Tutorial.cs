using System;
using Managers;
using UnityEngine;
using UnityEngine.UIElements;

namespace Interface
{
    public class Tutorial : MonoBehaviour
    {
        [Header("Tools")] [SerializeField] private GameManager gameManager;

        [Header("Game Elements")] [SerializeField]
        private GameObject closeAll;
        [SerializeField] private GameObject buildMenu;
        [SerializeField] private GameObject buyMenu;
        [SerializeField] private GameObject enrollMenu;
        [SerializeField] private GameObject clientIcon;
        [SerializeField] private GameObject reputationIcon;
        [SerializeField] private GameObject coinIcon;
        [SerializeField] private GameObject beerIcon;

        [Header("Tutorial Parts")] [SerializeField]
        private GameObject part1;
        [SerializeField] private GameObject part2;
        [SerializeField] private GameObject part3;
        [SerializeField] private GameObject part4;
        [SerializeField] private GameObject part5;
        [SerializeField] private GameObject lastPart;

        public void StartTuto()
        {
            gameManager.ToggleGamePaused();
            closeAll.SetActive(false);

            clientIcon.SetActive(false);
            reputationIcon.SetActive(false);
            coinIcon.SetActive(false);
            beerIcon.SetActive(false);

            part1.SetActive(true);
            part2.SetActive(false);
            part3.SetActive(false);
            part4.SetActive(false);
            part5.SetActive(false);
            lastPart.SetActive(false);
        }

        public void StartPart2()
        {
            part1.SetActive(false);
            part2.SetActive(true);

            buyMenu.SetActive(true);
            coinIcon.SetActive(true);
            beerIcon.SetActive(true);
        }

        public void StartPart3()
        {
            part2.SetActive(false);
            part3.SetActive(true);

            buildMenu.SetActive(true);
            clientIcon.SetActive(true);
            reputationIcon.SetActive(true);
        }

        public void StartPart4()
        {
            part3.SetActive(false);
            part4.SetActive(true);

            enrollMenu.SetActive(true);
        }

        public void StartPart5()
        {
            part4.SetActive(false);
            part5.SetActive(true);
        }

        public void LastPart()
        {
            part5.SetActive(false);
            lastPart.SetActive(true);
        }

        public void FinishTuto()
        {
            buildMenu.SetActive(true);
            buyMenu.SetActive(true);
            enrollMenu.SetActive(true);
            coinIcon.SetActive(true);
            beerIcon.SetActive(true);
            clientIcon.SetActive(true);
            reputationIcon.SetActive(true);

            gameManager.ToggleGamePaused();
            closeAll.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}