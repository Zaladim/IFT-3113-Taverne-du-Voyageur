using System;
using Managers;
using UnityEngine;
using UnityEngine.UIElements;

namespace Interface
{
    public class Tutorial : MonoBehaviour
    {
        [Header("Tools")]
        [SerializeField] private GameManager gameManager;
        
        [Header("Game Elements")]
        [SerializeField] private GameObject closeAll;
        [SerializeField] private GameObject buildMenu;
        [SerializeField] private GameObject buyMenu;
        [SerializeField] private GameObject enrollMenu;
        //[SerializeField] private GameObject settingsMenu;
        
        [Header("Tutorial Parts")]
        [SerializeField] private GameObject part1;
        [SerializeField] private GameObject part2;

        public void StartTuto()
        {
            gameManager.ToggleGamePaused();
            closeAll.SetActive(false);
            part1.SetActive(true);
        }

        public void StartPart2()
        {
            part1.SetActive(false);
            part2.SetActive(true);
            
            buyMenu.SetActive(true);
        }

        private void FinishTuto()
        {
            gameManager.ToggleGamePaused();
            closeAll.SetActive(true);
        }
    }
}