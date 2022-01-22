using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private Controller leftController;
    private Controller rightController;
    public GameObject gameOverUI;
    public bool isGameOver;
    public GameObject pauseMenuUI;
    public AudioSource gameOverSound;
    public AudioSource uiClick;
    private bool isMenuButtonReady = true;
    public AudioSource playerDamageSound;
    public ParticleSystem orbDestroyEffect;

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
            uiClick.Play();
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
        gameOverSound.Play();
        isGameOver = true;
        gameOverUI.SetActive(true);
    }

    public void PlayPlayerDamageSound()
    {
        playerDamageSound.Play();
    }

    public void PlayDestroyOrbParticleEffect(Vector3 position)
    {
        orbDestroyEffect.gameObject.transform.position = position;
        orbDestroyEffect.Play();
    }
}
