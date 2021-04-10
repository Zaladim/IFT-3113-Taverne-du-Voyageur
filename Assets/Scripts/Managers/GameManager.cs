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

        [Header("Game Elements")] [SerializeField]
        private GameObject startPanel;
        [SerializeField] private GameObject gamePanel;
        [SerializeField] private GameObject settingsPanel;

        [Header("Debug")] [SerializeField] [Tooltip("! Does nothing if changed in editor !")]
        private bool gameIsPaused;

        public bool MouseControl { get; set; }

        private void Start()
        {
            startPanel.SetActive(true);
            gamePanel.SetActive(false);

            clientManager.gameObject.SetActive(false);
            waiterManager.gameObject.SetActive(false);
            placementManager.gameObject.SetActive(false);
            resourcesManager.gameObject.SetActive(true);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ToggleGamePaused();
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
            if (!runInEditMode)
                Application.Quit();
        }

        private void PauseGame()
        {
            if (gameIsPaused)
                Time.timeScale = 0f;
            else
                Time.timeScale = 1;
        }

        public void ToggleGamePaused()
        {
            gameIsPaused = !gameIsPaused;
            PauseGame();
        }
    }
}