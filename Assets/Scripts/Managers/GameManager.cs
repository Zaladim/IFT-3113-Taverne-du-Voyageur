using UnityEngine;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        [Header("Managers")] [SerializeField] private ClientManager clientManager;
        [SerializeField] private WaiterManager waiterManager;
        [SerializeField] private PlacementManager placementManager;
        [SerializeField] private ResourcesManager resourcesManager;

        [Header("Game Elements")] [SerializeField]
        private GameObject startPanel;

        [SerializeField] private GameObject gamePanel;
        public bool MouseControl { get; set; }

        private void Start()
        {
            startPanel.SetActive(true);
            gamePanel.SetActive(false);

            clientManager.gameObject.SetActive(false);
            waiterManager.gameObject.SetActive(false);
            placementManager.gameObject.SetActive(false);
            resourcesManager.gameObject.SetActive(false);
        }

        public void StartGame()
        {
            startPanel.SetActive(false);
            gamePanel.SetActive(true);

            clientManager.gameObject.SetActive(true);
            waiterManager.gameObject.SetActive(true);
            placementManager.gameObject.SetActive(true);
            resourcesManager.gameObject.SetActive(true);
        }

        public void QuitGame()
        {
            if (!runInEditMode)
                Application.Quit();
        }
    }
}