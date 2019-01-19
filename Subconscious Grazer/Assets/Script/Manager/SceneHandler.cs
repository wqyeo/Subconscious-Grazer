using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : Singleton<SceneHandler> {

    public void QuitGame() {
        PlayerPrefs.Save();
        Application.Quit();
    }

    public void SwitchScene(int sceneBuildIndex) {
        SceneManager.LoadScene(sceneBuildIndex);
    }

}
