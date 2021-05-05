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

        [Header("Time Debug"), Tooltip("Does nothing if changed in editor!"), SerializeField]
        private bool isGamePaused;

        [SerializeField, Tooltip("Does nothing if changed in editor!")]
        private bool isGameForcedPaused;

        public bool GamePause
        {
            get => isGamePaused;
            set
            {
                if (value)
                {
                    timeManager.LockTime();
                    isGamePaused = true;
                }
                else
                {
                    timeManager.Apply();
                    isGamePaused = false;
                }
            }
        }

        public bool GameForcePause
        {
            get => isGameForcedPaused;
            set
            {
                if (value)
                {
                    timeManager.FreezeTime();
                    isGameForcedPaused = true;
                }
                else
                {
                    if (isGamePaused)
                    {
                        timeManager.DefrostTime(0);
                    }
                    else
                    {
                        timeManager.DefrostTime();
                    }

                    isGameForcedPaused = false;
                }
            }
        }

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

            //tutorial.StartTuto();
        }

        public void RestartGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void QuitGame()
        {
            Application.Quit();
        }

        public void ToggleGameForcedPause() => GameForcePause = !GameForcePause;
        public void ToggleGamePaused() => GamePause = !GamePause;

        public bool IsGameRunnig() => !GamePause && !GameForcePause;
        public bool IsGameStopped() => GamePause || GameForcePause;

        public void IncreaseGameSpeed(int n)
        {
            timeManager.ScaleTime(n);

            if (IsGameRunnig())
                timeManager.Apply();
        }

        public void GrowthGameSpeed(int k)
        {
            timeManager.ScaleTimeBy(k);

            if (IsGameRunnig())
                timeManager.Apply();
        }

        public void SetGameSpeed(int n)
        {
            timeManager.SetTimeScale(n);

            if (IsGameRunnig())
                timeManager.Apply();
        }
    }
}