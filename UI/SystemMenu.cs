using UnityEngine;
using UnityEngine.SceneManagement;

namespace SuspiciousGames.Saligia.UI
{
    public abstract class SystemMenu : MonoBehaviour
    {
        public void LoadScene(int buildIndex)
        {
            SceneManager.LoadScene(buildIndex, LoadSceneMode.Single);
        }
        public void CloseApplication()
        {
#if UNITY_EDITOR
            // Application.Quit() does not work in the editor so
            // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
            UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
        }
        public void OpenUrl(string URL = "https://suspicious.games")
        {
            Application.OpenURL(URL);
        }

        public void ChangeTimescale(float scale)
        {
            Time.timeScale = scale;
        }

    }

}


