using Interface;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        [Header("Managers")] [SerializeField] private ClientManager clientManager;
        [SerializeField] private WaiterManager waiterManager;
        [SerializeField] private PlacementManager placementManager;
        [SerializeField] private ResourcesManager resourcesManager;
        [SerializeField] private TimeManager timeManager;

        [Header("Game Elements")] [SerializeField]
        private GameObject startPanel;

        [SerializeField] private GameObject gamePanel;
        [SerializeField] private GameObject settingsPanel;
        [SerializeField] private Tutorial tutorial;

        [Header("Time Debug")] [SerializeField]
        private bool isGamePaused = false;

        [SerializeField] private bool isGameForcedPaused = false;

        public bool MouseControl { get; set; }

        private void Start()
        {
            startPanel.SetActive(true);
            gamePanel.SetActive(false);

            clientManager.gameObject.SetActive(false);
            waiterManager.gameObject.SetActive(false);
            placementManager.gameObject.SetActive(false);
            resourcesManager.gameObject.SetActive(true);
            timeManager.gameObject.SetActive(true);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ToggleGameForcedPause();
                if (gamePanel.activeSelf) settingsPanel.SetActive(!settingsPanel.activeSelf);
            }
        }

        public void StartGame()
        {
            startPanel.SetActive(false);
            gamePanel.SetActive(true);

            clientManager.gameObject.SetActive(true);
            waiterManager.gameObject.SetActive(true);
            placementManager.gameObject.SetActive(true);
            resourcesManager.gameObject.SetActive(true);

            tutorial.StartTuto();
        }

        public void UseBasicLayout(bool isInUse)
        {
            const int basicLayoutSeats = 60;

            resourcesManager.Seats += isInUse ? basicLayoutSeats : -basicLayoutSeats;
        }

        public void RestartGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void QuitGame()
        {
            Application.Quit();
        }

        public void ToggleGameForcedPause()
        {
            if (isGameForcedPaused)
            {
                if (isGamePaused)
                {
                    timeManager.DefrostTime(0);
                }
                else
                {
                    timeManager.DefrostTime();
                }
            }
            else
            {
                timeManager.FreezeTime();
            }

            isGameForcedPaused = !isGameForcedPaused;
        }

        public void ToggleGamePaused()
        {
            if (isGamePaused)
            {
                timeManager.Apply();
            }
            else
            {
                timeManager.LockTime();
            }

            isGamePaused = !isGamePaused;
        }
    }
}