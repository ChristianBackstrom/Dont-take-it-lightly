using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fog : MonoBehaviour
{
	[SerializeField]
	private PooledMonoBehaviour[] cloudPrefabs;

	[SerializeField]
	private int amount = 200;

	[SerializeField]
	private float growthSpeed = 1;

	[SerializeField]
	private float scale = 1;

	private bool shouldSpawn = true;
	private float extraspeeed = 1;

	public void StartCloudSpawn()
	{
		StartCoroutine(InstantiateClouds());
	}

	private IEnumerator InstantiateClouds()
	{
		for (int i = 0; i < amount; i++)
		{
			SpawnCloud();

			yield return new WaitForSeconds((2.0f / growthSpeed) / amount);
		}
	}

	private void SpawnCloud()
	{
		if (!shouldSpawn)
		{
			return;
		}

		Vector3 pos = GetRandomPos();
		Quaternion rot = Quaternion.AngleAxis(UnityEngine.Random.value * 360, Vector3.up);
		PooledMonoBehaviour cloud = cloudPrefabs[UnityEngine.Random.Range(0, cloudPrefabs.Length)];
		var gm = cloud.GetAtPosAndRot<PooledMonoBehaviour>(pos, rot);
		StartCoroutine(GowAndDie(gm.transform));
	}

	private Vector3 GetRandomPos()
	{
		float x = UnityEngine.Random.Range(-transform.localScale.x / 2.0f, transform.localScale.x / 2.0f);
		float y = -UnityEngine.Random.value;
		float z = UnityEngine.Random.Range(-transform.localScale.z / 2.0f, transform.localScale.z / 2.0f);
		return transform.position + new Vector3(x, y, z);
	}

	private IEnumerator GowAndDie(Transform cloud)
	{
		float t = 0;

		Vector3 startScale = Vector3.zero;
		Vector3 targetScale = Vector3.one * scale;

		float speed = UnityEngine.Random.Range(0.75f, 1.0f) * growthSpeed;

		while (t <= 1.0f)
		{
			t += Time.deltaTime * speed * extraspeeed;

			cloud.transform.localScale = Vector3.Lerp(startScale, targetScale, EasingFunctions.EaseOut(t));

			yield return null;
		}

		while (t >= 0.0f)
		{
			t -= Time.deltaTime * speed * extraspeeed;

			cloud.transform.localScale = Vector3.Lerp(startScale, targetScale, EasingFunctions.EaseOut(t));

			yield return null;
		}

		cloud.gameObject.SetActive(false);
		SpawnCloud();
	}

	public void StopSpawning()
	{
		extraspeeed = 2f;
		shouldSpawn = false;
	}
}
