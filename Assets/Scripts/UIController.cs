using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using SgLib;

public class UIController : MonoBehaviour
{
    public PlayerController playerController;
    public GameObject title;
    public Text score;
    public Text bestScore;
    public Text gold;
    // The rebButton and yellowButton are there for illustration purpose only
    public GameObject coloredButtons;
    public GameObject buttons;
    public Button muteBtn;
    public Button unMuteBtn;
    // public Button watchForCoinsBtn;
    public GameObject toastMsg;
    public GameObject blackPanel;

    Animator scoreAnimator;
    bool hasCheckedGameOver = false;

    void OnEnable()
    {
        ScoreManager.ScoreUpdated += OnScoreUpdated;
    }

    void OnDisable()
    {
        ScoreManager.ScoreUpdated -= OnScoreUpdated;
    }

    // Use this for initialization
    void Start()
    {
        scoreAnimator = score.GetComponent<Animator>();
        // watchForCoinsBtn.gameObject.SetActive(false);
        toastMsg.gameObject.SetActive(false);
        HideUIButtons();
        HideColoredButtons();

        if (PlayerController.isFirstLoad)
        {
            title.SetActive(true);
            score.gameObject.SetActive(false);
            Invoke("ShowUIButtons", 0.5f);
        }
        else
        {
            title.SetActive(false);
            score.gameObject.SetActive(true);
            ShowColoredButtons();
        }
    }

    // Update is called once per frame
    void Update()
    {
        score.text = ScoreManager.Instance.Score.ToString();
        bestScore.text = ScoreManager.Instance.HighScore.ToString();
        gold.text = CoinManager.Instance.Coins.ToString();

        if (playerController.isGameOver && !hasCheckedGameOver)
        {
            hasCheckedGameOver = true;
            Invoke("ShowUIButtons", 1f);
        }
    }

    void OnScoreUpdated(int newScore)
    {
        scoreAnimator.Play("NewScore");
    }

    public void HandlePlayButton()
    {
        if (!PlayerController.isFirstLoad)
        {
            playerController.RestartGame(0.5f);
        }
        else
        {
            title.SetActive(false);
            HideUIButtons();
            ShowColoredButtons();
            playerController.StartGame();
        }
    }

    public void HandleSoundButton()
    {
        SoundManager.Instance.ToggleMute();
        // UpdateMuteButtons();
    }

    public void HandleShopButton()
    {
        SceneManager.LoadScene("CharacterSelection");
    }

    public void ShowUIButtons()
    {
        buttons.SetActive(true);
        blackPanel.SetActive(true);
        // UpdateMuteButtons();
        HideColoredButtons();
    }

    public void HideUIButtons()
    {
        buttons.SetActive(false);
        blackPanel.SetActive(false);
        score.gameObject.SetActive(true);
    }

    public void ShowColoredButtons()
    {
        coloredButtons.SetActive(true);
    }

    public void HideColoredButtons()
    {
        coloredButtons.SetActive(false);
    }

    public void ShowWatchForCoinsBtn()
    {
        // watchForCoinsBtn.gameObject.SetActive(true);
    }

    public void HideWatchForCoinsBtn()
    {
        // watchForCoinsBtn.gameObject.SetActive(false);
    }

    public void ShowToastMsg(string msg, float hideAfter = 4f)
    {
        toastMsg.transform.Find("Text").GetComponent<Text>().text = msg;
        toastMsg.SetActive(true);
        Invoke("HideToastMsg", hideAfter);
    }

    public void HideToastMsg()
    {
        toastMsg.SetActive(false);
    }
}
