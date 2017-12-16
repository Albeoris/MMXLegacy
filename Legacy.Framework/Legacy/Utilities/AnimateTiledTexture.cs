using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Legacy.Utilities
{
	[AddComponentMenu("MM Legacy/Utility/AnimateTiledTexture")]
	public class AnimateTiledTexture : MonoBehaviour
	{
		public Int32 _columns = 2;

		public Int32 _rows = 2;

		public Vector2 _scale = new Vector3(1f, 1f);

		public Vector2 _offset = Vector2.zero;

		public Vector2 _buffer = Vector2.zero;

		public Single _framesPerSecond = 10f;

		public Boolean _playOnce;

		public Boolean _disableUponCompletion;

		public Boolean _enableEvents;

		public Boolean _playOnEnable = true;

		public Boolean _newMaterialInstance;

		private Int32 _index;

		private Vector2 _textureSize = Vector2.zero;

		private Material _materialInstance;

		private Boolean _hasMaterialInstance;

		private Boolean _isPlaying;

		private List<VoidEvent> _voidEventCallbackList;

		public void RegisterCallback(VoidEvent cbFunction)
		{
			if (_enableEvents)
			{
				_voidEventCallbackList.Add(cbFunction);
			}
			else
			{
				Debug.LogWarning("AnimateTiledTexture: You are attempting to register a callback but the events of this object are not enabled!");
			}
		}

		public void UnRegisterCallback(VoidEvent cbFunction)
		{
			if (_enableEvents)
			{
				_voidEventCallbackList.Remove(cbFunction);
			}
			else
			{
				Debug.LogWarning("AnimateTiledTexture: You are attempting to un-register a callback but the events of this object are not enabled!");
			}
		}

		public void Play()
		{
			if (_isPlaying)
			{
				StopCoroutine("updateTiling");
				_isPlaying = false;
			}
			renderer.enabled = true;
			_index = _columns;
			StartCoroutine(updateTiling());
		}

		public void ChangeMaterial(Material newMaterial, Boolean newInstance = false)
		{
			if (newInstance)
			{
				if (_hasMaterialInstance)
				{
					Destroy(renderer.sharedMaterial);
				}
				_materialInstance = new Material(newMaterial);
				renderer.sharedMaterial = _materialInstance;
				_hasMaterialInstance = true;
			}
			else
			{
				renderer.sharedMaterial = newMaterial;
			}
			CalcTextureSize();
			renderer.sharedMaterial.SetTextureScale("_MainTex", _textureSize);
		}

		private void Awake()
		{
			if (_enableEvents)
			{
				_voidEventCallbackList = new List<VoidEvent>();
			}
			ChangeMaterial(renderer.sharedMaterial, _newMaterialInstance);
		}

		private void OnDestroy()
		{
			if (_hasMaterialInstance)
			{
				Destroy(renderer.sharedMaterial);
				_hasMaterialInstance = false;
			}
		}

		private void HandleCallbacks(List<VoidEvent> cbList)
		{
			for (Int32 i = 0; i < cbList.Count; i++)
			{
				cbList[i]();
			}
		}

		private void OnEnable()
		{
			CalcTextureSize();
			if (_playOnEnable)
			{
				Play();
			}
		}

		private void CalcTextureSize()
		{
			_textureSize = new Vector2(1f / _columns, 1f / _rows);
			_textureSize.x = _textureSize.x / _scale.x;
			_textureSize.y = _textureSize.y / _scale.y;
			_textureSize -= _buffer;
		}

		private IEnumerator updateTiling()
		{
			_isPlaying = true;
			Int32 checkAgainst = _rows * _columns;
			for (;;)
			{
				if (_index >= checkAgainst)
				{
					_index = 0;
					if (_playOnce)
					{
						if (checkAgainst == _columns)
						{
							break;
						}
						checkAgainst = _columns;
					}
				}
				ApplyOffset();
				_index++;
				yield return new WaitForSeconds(1f / _framesPerSecond);
			}
			if (_enableEvents)
			{
				HandleCallbacks(_voidEventCallbackList);
			}
			if (_disableUponCompletion)
			{
				gameObject.renderer.enabled = false;
			}
			_isPlaying = false;
		}

		private void ApplyOffset()
		{
			Vector2 offset = new Vector2(_index / (Single)_columns - _index / _columns, 1f - _index / _columns / (Single)_rows);
			if (offset.y == 1f)
			{
				offset.y = 0f;
			}
			offset.x += (1f / _columns - _textureSize.x) / 2f;
			offset.y += (1f / _rows - _textureSize.y) / 2f;
			offset.x += _offset.x;
			offset.y += _offset.y;
			renderer.sharedMaterial.SetTextureOffset("_MainTex", offset);
		}

		public delegate void VoidEvent();
	}
}
