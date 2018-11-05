using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ActiveSceneSetter : MonoBehaviour {
    [SerializeField]
    string nowSceneName;

    void Start() {
        Scene scene = SceneManager.GetSceneByName(nowSceneName);
        SceneManager.SetActiveScene(scene);
    }
}
