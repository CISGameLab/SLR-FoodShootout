using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager instance;

	public List<Food> collectedFood;

	private float currentTime;
	private float gameTime;

	public GameObject pickup;

	public GameObject howToPlayMenu;
	public GameObject finishMenu;
	public GameObject mainMenu;
	public GameObject bag;

	private int score;
	private int items;

	private int highScore;

	public TMP_Text gameScoreText;
	public TMP_Text currentTimeText;
	public TMP_Text gameItemsText;

	public TMP_Text resultsText;

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
		finishMenu.SetActive(false);

		gameIsStarted = false;
		Reset();
	}

	private void Reset()
	{
		currentTime = Time.time;
		score = 0;
		items = 0;
		gameScoreText.text = "Score: 0";
		mainScore.text = "Score: 0";
		gameItemsText.text = "Items Collected: " + items.ToString();
		collectedFood = new List<Food>();
	}

	public void ResetScore()
	{
		score = 0;
	}

	private IEnumerator EndGameDelay()
	{
		yield return new WaitForSeconds(1.5f);
		musicSource.Play();
		finishMenu.SetActive(true);
	}

	private void EndGame()
	{
		endSource.Play();
		gameIsStarted = false;
		mainScore.text = "Points Earned: " + score.ToString();
		var goodItems = collectedFood.Where(food => food.isGood).Count();
		var badItems = collectedFood.Where(food => !food.isGood).Count();
		resultsText.text = "You collected " + goodItems + " good food items and " + badItems + " bad food items to end up with a total score of " + score;
		SetScore();
		StartCoroutine(EndGameDelay());
	}

	private void SetScore()
	{
		if (PlayerPrefs.HasKey("score"))
		{
			if (score > PlayerPrefs.GetInt("score"))
			{
				SetHighScore();
				highScoreSource.Play();
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

	public void Finish()
	{
		ButtonNoise();
		finishMenu.SetActive(false);
		mainMenu.SetActive(true);
	}

	public void CollectFood(Transform targetLoc, Food targetFood)
	{
		GameObject newPickup = Instantiate(pickup, targetLoc.position, Quaternion.identity) as GameObject;
		newPickup.transform.Rotate(0, 90, 0);
		newPickup.GetComponent<SpriteRenderer>().sprite = targetFood.sprite;
		collectedFood.Add(targetFood);
		items++;
		gameItemsText.text = "Items Collected: " + items.ToString();
		StartCoroutine(CollectFoodRoutine(newPickup.transform));
	}

	private IEnumerator CollectFoodRoutine(Transform pickup)
	{
		var startTime = Time.time;
		while (Time.time - startTime < 2.5f)
		{
			var time = (Time.time - startTime) / 2.5f;
			pickup.position = Vector3.Lerp(pickup.position, bag.transform.position, time);
			pickup.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, time);
			yield return null;
		}
	}
}
