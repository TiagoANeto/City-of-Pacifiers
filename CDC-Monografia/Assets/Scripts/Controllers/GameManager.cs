using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager gameManager;
    public InputRef inputRef;
    public GameObject panelPause;
    public Animator animator;

    void Awake()
    {
        inputRef.PauseEvent += Pause;

        InicializeGameManager();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        scene = SceneManager.GetActiveScene();

        Debug.Log($"Cena carregada: {scene.name}");
        Debug.Log($"panelPause: {panelPause != null}");
        Debug.Log($"animator: {animator != null}");

        if (scene.name != "Menu" && scene.name != "Vitoria")
        {
            panelPause = GameObject.Find("/Canvas/PauseMenu");

            if (panelPause == null)
            {
                panelPause = GameObject.FindObjectOfType<Canvas>()?.transform.Find("PauseMenu")?.gameObject;
            }

            if (panelPause != null)
            {
                panelPause.SetActive(false);
                DontDestroyOnLoad(panelPause);
            }
            else
            {
                Debug.LogWarning("PanelPause não foi encontrado! Verifique se o caminho está correto ou se ele existe na cena.");
            }

            animator = GameObject.Find("/Canvas/Transitions")?.GetComponent<Animator>();

            // Verificação para garantir que o animator não seja null
            if (animator == null)
            {
                Debug.LogError("Animator não foi encontrado! Verifique se o objeto /Canvas/Transitions existe e tem um Animator.");
                return;
            }
        }
    }

    private void InicializeGameManager()
    {
        if (gameManager == null)
        {
            gameManager = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    public GameManager getInstance()
    {
        return gameManager;
    }

    private void Pause()
    {
        if (panelPause != null)
        {
            panelPause.SetActive(true);
        }
        else
        {
            Debug.LogError("PanelPause está nulo! Certifique-se de que ele foi inicializado.");
        }
    }

    public void NextLevel()
    {
        StartCoroutine(LoadLevelScene());
    }

    IEnumerator LoadLevelScene()
    {
        if (animator == null)
        {
            Debug.LogError("Animator não foi encontrado! O carregamento da cena não pode continuar.");
            yield break;  // Interrompe a execução do método se o animator for nulo
        }

        animator.SetTrigger("EndLevel");
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

        animator.SetTrigger("StartLevel");
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
