using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public string sceneName;       // Assign the scene name in Inspector
    public AudioSource buttonSound; // Drag & drop an AudioSource in Inspector

    public void LoadScene()
    {
        if (buttonSound != null)
        {
            buttonSound.Play(); // Play the sound effect
            Invoke(nameof(LoadNextScene), buttonSound.clip.length); // Wait for sound to finish
        }
        else
        {
            LoadNextScene(); // Load immediately if no sound
        }
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}
