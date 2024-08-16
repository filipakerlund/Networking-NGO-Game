using UnityEngine;

public class GameResultUI : MonoBehaviour
{
    [SerializeField] Canvas winCanvas;
    [SerializeField] Canvas loseCanvas;

    private void Start()
    {
        if (winCanvas == null || loseCanvas == null)
        {
            Canvas[] canvases = GetComponents<Canvas>();
            foreach (Canvas canvas in canvases)
            {
                if (canvas.name == "WinCanvas")
                {
                    winCanvas = canvas;
                }
                else if (canvas.name == "LoseCanvas")
                {
                    loseCanvas = canvas;
                }
            }
        }
    }
    
    private void OnEnable() 
    {
        HealthSystem.GameResultEvent += GameResult;
    }

    private void OnDisable()
    {
        HealthSystem.GameResultEvent -= GameResult;
    }

    private void GameResult(bool isWinner)
    {
        if (isWinner)
        {
            winCanvas.enabled = true;
            Time.timeScale = 0f;
        }
        else
        {
            loseCanvas.enabled = true;
            Time.timeScale = 0f;
        }
    }

    public void QuitButton()
    {
        Application.Quit();
    }
}
