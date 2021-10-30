using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTimer : MonoBehaviour
{
	[SerializeField] private int timer;

	void Start()
	{
		Destroy(this.gameObject, timer);
	}
}
