using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager instance;

	private float currentTime;
	private float gameTime;

	public GameObject howToPlayMenu;
	public GameObject mainMenu;

	private int score;

	private int highScore;

	public TMP_Text gameScoreText;
	public TMP_Text currentTimeText;

	public TMP_Text mainHighScore;
	public TMP_Text mainScore;

	public AudioSource musicSource;
	public AudioSource highScoreSource;
	public AudioSource buttonSource;
	public AudioSource startSource;
	public AudioSource endSource;

	public bool gameIsStarted;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else if (instance != this)
		{
			Destroy(gameObject);
		}
	}

	private void FixedUpdate()
	{
		if (gameIsStarted)
		{
			if (Time.time - currentTime > gameTime)
			{
				currentTimeText.text = "Time Left: 0.00";
				EndGame();
				TargetManager.instance.DropAllTargets();
			}
			else
			{
				var time = gameTime - (Time.time - currentTime);
				currentTimeText.text = "Time Left: " + time.ToString("F2");
			}
		}
	}

	public void SetScore(int delta)
	{
		if (score + delta < 0)
		{
			score = 0;
		}
		else
		{
			score += delta;
		}
		gameScoreText.text = "Score: " + score.ToString();
	}

	private void Start()
	{
		gameTime = 45.0f;

		RenderSettings.ambientLight = Color.black;

		highScore = PlayerPrefs.GetInt("score");
		mainHighScore.text = "High Score: " + highScore.ToString();

		musicSource.Play();

		howToPlayMenu.SetActive(false);
		mainMenu.SetActive(true);

		gameIsStarted = false;
		Reset();
	}

	private void Reset()
	{
		currentTime = Time.time;
		score = 0;
		gameScoreText.text = "Score: 0";
		mainScore.text = "Score: 0";
	}

	public void ResetScore()
	{
		score = 0;
	}

	private IEnumerator EndGameDelay()
	{
		yield return new WaitForSeconds(1.5f);
		musicSource.Play();
		mainMenu.SetActive(true);
	}

	private void EndGame()
	{
		endSource.Play();
		gameIsStarted = false;
		mainScore.text = "Points Earned: " + score.ToString();
		SetScore();
		StartCoroutine(EndGameDelay());
	}

	private void SetScore()
	{
		//sounds[2].Play();//menu music
		StopAllCoroutines();
		if (PlayerPrefs.HasKey("score"))
		{
			if (score > PlayerPrefs.GetInt("score"))
			{
				SetHighScore();
				highScoreSource.Play();
				//display high score UI
			}
		}
		else
		{
			SetHighScore();
		}
		highScore = PlayerPrefs.GetInt("score");
		mainHighScore.text = "High Score: " + highScore.ToString();
	}

	private void SetHighScore()
	{
		PlayerPrefs.SetInt("score", score);
	}

	public void ButtonNoise()
	{
		buttonSource.Play();
	}

	public void Quit()
	{
		ButtonNoise();
		Application.Quit();
	}

	public void StartGame()
	{
		startSource.Play();
		ButtonNoise();
		mainMenu.SetActive(false);
		musicSource.Stop();
		Reset();
		gameIsStarted = true;
	}

	public void HowToPlay()
	{
		ButtonNoise();
		howToPlayMenu.SetActive(true);
	}

	public void Back()
	{
		ButtonNoise();
		howToPlayMenu.SetActive(false);
	}
}
