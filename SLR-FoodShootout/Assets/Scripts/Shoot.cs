using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour
{
	public Animator anim;
	public GameObject bullet;
	public GameObject barrel;

	public AudioSource shoot;

	private float lerpTime;

	private void Start()
	{
		lerpTime = 0.10f;
	}

	private void Update()
	{
		if (GameManager.instance.gameIsStarted)
		{
			if (Input.GetMouseButtonDown(0)) //mouse click
			{
				ShootRaycast(Input.mousePosition);
			}
			else if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) //screen touch
			{
				Touch touch = Input.GetTouch(0);
				ShootRaycast(touch.position);
			}
		}
	}

	private void ShootRaycast(Vector3 hitPosition)
	{
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(hitPosition);
		if (Physics.Raycast(ray, out hit))
		{
			FaceHitPoint(hit.point);
			if (hit.rigidbody != null)
			{
				var target = hit.rigidbody.gameObject.transform.root.gameObject.GetComponent<Target>();
				if (target != null)
				{
					target.Shot();
				}
			}
		}
	}

	private void FaceHitPoint(Vector3 target)
	{
		anim.Play("IDLE"); //reset the animation in order to play it again
		anim.SetTrigger("Shoot"); //play the shoot animation

		shoot.pitch = Random.Range(0.85f, 1.15f);
		shoot.Play();

		Quaternion startRot = transform.rotation; //rotation before looking at hit point
		transform.LookAt(target);
		Quaternion endRot = transform.rotation; //rotation towards hit point

		GameObject newBullet = Instantiate(bullet, barrel.transform.position, Quaternion.identity) as GameObject;
		newBullet.GetComponent<Rigidbody>().AddForce(transform.forward * 1000.0f, ForceMode.Impulse);

		transform.rotation = startRot;

		StopAllCoroutines();
		StartCoroutine(FaceRoutine(startRot, endRot));
	}

	private IEnumerator FaceRoutine(Quaternion start, Quaternion end)
	{
		float startTime = Time.time;
		while (Time.time - startTime < lerpTime)
		{
			float time = (Time.time - startTime) / lerpTime;
			transform.rotation = Quaternion.Slerp(start, end, time);
			yield return null;
		}
	}
}
