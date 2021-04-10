using UnityEngine;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        public bool MouseControl { get; set; }

        public void QuitGame()
        {
            if (!runInEditMode)
                Application.Quit();
        }
    }
}