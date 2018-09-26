using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using VRTK;

public class LoadScene : MonoBehaviour
{
    [Tooltip("Menu scene to be activated when user wants to exit to menu.")]
    public string menuScene = "Menu";

    public void Start()
    {
        InputManager.AddCallback(OnControllerCallback);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        VRTK_SDKManager.AttemptTryLoadSDKSetupFromList(false);
    }

    private void OnControllerCallback(InputManager.InputState inputState)
    {
        if (inputState.Buttons.HasFlag(InputManager.InputState.ActiveFunction.OpenMenu))
            Load(menuScene);
    }

    public void Load(string scene)
    {
        // SceneManager.LoadScene(scene, LoadSceneMode.Single);

        // SteamVR has asynchronous loading, but we could use the above
        SteamVR_LoadLevel.Begin(scene, true, 0.5f, 0, 0, 0, 1);
    }
}
