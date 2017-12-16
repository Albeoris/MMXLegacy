using System;
using UnityEngine;

public class LigthningAtlasChanger : MonoBehaviour
{
	private Single time;

	public Single ChangeTime = 1f;

	private Single materialoffset;

	private Int32 State;

	private void Start()
	{
		time = Time.time;
	}

	private void Update()
	{
		if (time + ChangeTime < Time.time)
		{
			time = Time.time;
			switch (State)
			{
			case 0:
				materialoffset = 0.16666f;
				State = 1;
				break;
			case 1:
				materialoffset = 0.66666f;
				State = 2;
				break;
			case 2:
				materialoffset = 0.33333f;
				State = 3;
				break;
			case 3:
				materialoffset = 0.5f;
				State = 4;
				break;
			case 4:
				materialoffset = 0.83333f;
				State = 5;
				break;
			case 5:
				materialoffset = 0f;
				State = 0;
				break;
			default:
				materialoffset = 0f;
				State = 1;
				break;
			}
			renderer.material.mainTextureOffset = new Vector2(0f, materialoffset);
		}
	}
}
