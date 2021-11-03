﻿using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private Controller leftController;
    private Controller rightController;
    public GameObject gameOverUI;
    public bool isGameOver;
    public GameObject pauseMenuUI;
    private bool isMenuButtonReady = true;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one GameManager in scene!");
            return;
        }
        instance = this;
    }

    private void Start()
    {
        Time.timeScale = 1f;
        isGameOver = false;
        leftController = GameObject.Find("Left Controller").GetComponent<Controller>();
        rightController = GameObject.Find("Right Controller").GetComponent<Controller>();
    }

    private void Update()
    {
        if (isGameOver) return;

        if (leftController.isMenuButton && isMenuButtonReady)
        {
            TogglePauseUI();
            isMenuButtonReady = false;
        }
        if (!isMenuButtonReady && !leftController.isMenuButton) isMenuButtonReady = true;
    }

    private void TogglePauseUI()
    {
        pauseMenuUI.SetActive(!pauseMenuUI.activeSelf);

        if (pauseMenuUI.activeSelf) Time.timeScale = 0f;
        else Time.timeScale = 1f;
    }

    public void EndGame()
    {
        isGameOver = true;
        gameOverUI.SetActive(true);
    }
}
