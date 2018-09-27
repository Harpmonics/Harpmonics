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

    private SteamVR_LoadLevel loadLevelScript;

    public void Start()
    {
        InputManager.AddCallback(OnControllerCallback);

        SceneManager.sceneLoaded += OnSceneLoaded;

        loadLevelScript = GetComponent<SteamVR_LoadLevel>();
            
        loadLevelScript.levelName = menuScene;
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        // TODO: Check if this is still necessary
        VRTK_SDKManager.AttemptTryLoadSDKSetupFromList(false);
    }

    private void OnControllerCallback(InputManager.InputState inputState)
    {
        if (inputState.Buttons.HasFlag(InputManager.InputState.ActiveFunction.OpenMenu))
            Load(menuScene);
    }

    public void Load(string scene)
    {
        loadLevelScript.levelName = scene;

        loadLevelScript.Trigger();
    }
}
