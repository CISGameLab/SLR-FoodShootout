using UnityEngine;

public class BulletDestroyer : MonoBehaviour
{
	private float startTime;
	public float killTime;
	private void Start()
	{
		startTime = Time.time;
	}

	private void FixedUpdate()
	{
		if (Time.time - startTime > killTime)
		{
			Destroy(gameObject);
		}
	}
}
