using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TargetManager : MonoBehaviour
{
	public static TargetManager instance;

	private List<Target> targets;
	private List<Food> foods;
	private float timeSinceStart;

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

	// Use this for initialization
	void Start()
	{
		targets = new List<Target>(UnityEngine.Object.FindObjectsOfType<Target>());
		timeSinceStart = Time.time;

		foods = LoadFood();
	}

	private List<Food> LoadFood()
	{
		var list = new List<Food>
		{
			new Food()
			{
				foodName = "apple",
					isGood = true
			},
			new Food()
			{
				foodName = "banana",
					isGood = true
			},
			new Food()
			{
				foodName = "berries",
					isGood = true
			},
			new Food()
			{
				foodName = "bread",
					isGood = true
			},
			new Food()
			{
				foodName = "cheese",
					isGood = true
			},
			new Food()
			{
				foodName = "chocolate",
					isGood = false
			},
			new Food()
			{
				foodName = "donut",
					isGood = false
			},
			new Food()
			{
				foodName = "egg",
					isGood = true
			},
			new Food()
			{
				foodName = "fries",
					isGood = false
			},
			new Food()
			{
				foodName = "nuts",
					isGood = true
			},
			new Food()
			{
				foodName = "oats",
					isGood = true
			},
			new Food()
			{
				foodName = "orange",
					isGood = true
			},
			new Food()
			{
				foodName = "pie",
					isGood = false
			},
			new Food()
			{
				foodName = "pizza",
					isGood = false
			},
			new Food()
			{
				foodName = "rice",
					isGood = true
			},
			new Food()
			{
				foodName = "tomato",
					isGood = true
			},
		};
		foreach (Food food in list)
		{
			food.sprite = Resources.Load<Sprite>(food.foodName);
		}
		return list;
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		if (GameManager.instance.gameIsStarted)
		{
			if (Time.time - timeSinceStart > 2.5f)
			{
				timeSinceStart = Time.time;
				OpenNewTarget();
			}
		}
	}

	public void DropAllTargets()
	{
		List<Target> candidates = targets.Where(target => !target.isClosed).ToList();
		foreach (var target in candidates)
		{
			target.Close();
		}
	}

	private void OpenNewTarget()
	{
		List<Target> candidates = targets.Where(target => target.isClosed).ToList();
		if (candidates.Count > 0)
		{
			var currentTarget = candidates[Random.Range(0, candidates.Count)];
			var currentFood = foods[Random.Range(0, foods.Count)];
			currentTarget.Open(25.0f, 5.0f, currentFood);
		}
	}

	public void TargetDestroyed()
	{
		if (GameManager.instance.gameIsStarted)
		{
			OpenNewTarget();
		}
	}
}
