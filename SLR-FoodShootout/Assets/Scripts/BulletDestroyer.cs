using UnityEngine;

public class BulletDestroyer : MonoBehaviour
{
	private float startTime;
	private void Start()
	{
		startTime = Time.time;
	}

	private void FixedUpdate()
	{
		if (Time.time - startTime > 1.0f)
		{
			Destroy(gameObject);
		}
	}
}
