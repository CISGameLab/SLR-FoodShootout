using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
	private Animator anim;
	public BoxCollider boxCollider;
	public SpriteRenderer spriteRenderer;

	public AudioClip goodHit;
	public AudioClip badHit;

	public AudioSource hitType;
	public AudioSource rollSource;
	public AudioSource hitSource;

	private Food foodRef;

	public bool isClosed;
	public bool canMove;

	public Vector3 startLoc;
	public Vector3 endLoc;

	public float speed;
	private bool moveToEnd;

	private float minSpeedMultiplier;
	private float maxSpeedMultiplier;

	private float openTime;
	private float timeSinceOpen;

	private bool isGood;

	private bool closeInvoked;

	private void Start()
	{
		anim = GetComponent<Animator>();
		moveToEnd = true;

		minSpeedMultiplier = 0.25f;
		maxSpeedMultiplier = 1.0f;

		isClosed = true;
		closeInvoked = false;
		spriteRenderer.sprite = null;
	}

	private void LateUpdate()
	{
		if (!isClosed && canMove && !closeInvoked)
		{
			if (moveToEnd)
			{
				if (Vector3.Distance(transform.position, endLoc) < 0.25f)
				{
					moveToEnd = false;
				}
				else
				{
					transform.Translate((endLoc - transform.position).normalized * speed * GetSpeedMultiplier() * Time.deltaTime);
				}
			}
			else
			{
				if (Vector3.Distance(transform.position, startLoc) < 0.25f)
				{
					moveToEnd = true;
				}
				else
				{
					transform.Translate((startLoc - transform.position).normalized * speed * GetSpeedMultiplier() * Time.deltaTime);
				}
			}
		}
		if (!isClosed && !closeInvoked)
		{
			if (Time.time - timeSinceOpen > openTime)
			{
				Close();
			}
		}
	}

	private float GetSpeedMultiplier()
	{
		float result;
		if (Vector3.Distance(transform.position, endLoc) < maxSpeedMultiplier)
		{
			result = Vector3.Distance(transform.position, endLoc);
			if (result < minSpeedMultiplier)
			{
				result = minSpeedMultiplier;
			}
		}
		else if (Vector3.Distance(transform.position, startLoc) < maxSpeedMultiplier)
		{
			result = Vector3.Distance(transform.position, startLoc);
			if (result < minSpeedMultiplier)
			{
				result = minSpeedMultiplier;
			}
		}
		else
		{
			result = maxSpeedMultiplier;
		}
		return result;
	}

	public void Shot()
	{
		hitType.pitch = Random.Range(0.85f, 1.15f);
		hitType.Play();
		if (isGood)
		{
			int bonus = Mathf.RoundToInt(10 * (1 - (Time.time - timeSinceOpen) / openTime));
			int points = 5 + bonus;
			GameManager.instance.SetScore(points);
			hitSource.clip = goodHit;
		}
		else
		{
			GameManager.instance.SetScore(-10);
			hitSource.clip = badHit;
		}
		hitSource.pitch = Random.Range(0.85f, 1.15f);
		hitSource.Play();
		GameManager.instance.CollectFood(boxCollider.gameObject.transform, foodRef);
		Close();
	}

	public void Open(float speed, float openTime, Food food)
	{
		anim.SetBool("isClosed", false);

		rollSource.pitch = Random.Range(0.85f, 1.15f);
		rollSource.Play();

		spriteRenderer.sprite = food.sprite;
		timeSinceOpen = Time.time;
		foodRef = food;
		this.openTime = openTime;
		this.speed = speed;
		this.isGood = food.isGood;

		boxCollider.enabled = true;
		isClosed = false;
	}

	public void Close()
	{
		anim.SetBool("isClosed", true);

		rollSource.pitch = Random.Range(0.85f, 1.15f);
		rollSource.Play();

		spriteRenderer.sprite = null;

		boxCollider.enabled = false;
		closeInvoked = true;
		StartCoroutine(CloseDelay());

		TargetManager.instance.TargetDestroyed();
	}

	private IEnumerator CloseDelay()
	{
		yield return new WaitForSeconds(0.5f);
		closeInvoked = false;
		isClosed = true;
	}
}
