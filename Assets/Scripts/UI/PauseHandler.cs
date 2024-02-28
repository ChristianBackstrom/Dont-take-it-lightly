using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseHandler : MonoBehaviour
{
    [SerializeField] 
    private LoadingScreen fadeoutPrefab;
    [SerializeField] private Canvas fadeoutCanvas;

    private InputHandler[] inputs;

    private Animator animator;

    private bool paused = false;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>(true);

        inputs = FindObjectsOfType<InputHandler>();

        for (int i = 0; i < inputs.Length; i++)
        {
            inputs[i].OnSetup += PauseHandler_OnSetup;
        }

        Continue();
    }

    private void OnDisable()
    {
        for (int i = 0; i < inputs.Length; i++)
        {
            inputs[i].OnSetup -= PauseHandler_OnSetup;
            inputs[i].Pause.performed -= Pause_performed;
        }
    }

    private void PauseHandler_OnSetup(InputHandler input)
    {
        input.Pause.performed += Pause_performed;
    }

    private void Pause_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (paused) { Continue(); return; }

        Pause();

    }

    public void Pause()
    {
        paused = true;
        Time.timeScale = 0;

        transform.GetChild(0).gameObject.SetActive(true);
        animator.SetTrigger("FadeIn");
    }

    public void Continue()
    {
        paused = false;
        Time.timeScale = 1;
        
        transform.GetChild(0).gameObject.SetActive(false);
    }

    public void Settings()
    {
        animator.SetTrigger("Open");
    }

    public async void Load()
    {
        paused = false;
        Time.timeScale = 1;

        LoadingScreen loadScreen = Instantiate(fadeoutPrefab, fadeoutCanvas.transform);
        
        await loadScreen.FadeOut();

        SaveDataHandler.Load(SaveDataHandler.currentFilename);
        transform.GetChild(0).gameObject.SetActive(false);

        await loadScreen.FadeIn();

        Destroy(loadScreen.gameObject);
    }

    public async void ExitToMenu()
    {
        paused = false;
        Time.timeScale = 1;

        SaveDataHandler.Save(SaveDataHandler.currentFilename);

        LoadingScreen loadScreen = Instantiate(fadeoutPrefab, fadeoutCanvas.transform);

        await loadScreen.FadeOut();

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(0);
        loadScreen.UpdateLoadingScreen("Please Wait");
    }
}
