using System;
using System.Collections.Generic;
using Legacy;
using UnityEngine;

[Serializable]
public class AudioCategory
{
	public String Name;

	private AudioCategory _parentCategory;

	[SerializeField]
	private String _parentCategoryName;

	public GameObject AudioObjectPrefab;

	public AudioItem[] AudioItems = Arrays<AudioItem>.Empty;

	[SerializeField]
	private Single _volume = 1f;

	public AudioCategory(AudioController audioController)
	{
		this.audioController = audioController;
	}

	public Single Volume
	{
		get => _volume;
	    set
		{
			_volume = value;
			_ApplyVolumeChange();
		}
	}

	public Single VolumeTotal
	{
		get
		{
			AudioCategory parentCategory = this.parentCategory;
			if (parentCategory != null)
			{
				return parentCategory.VolumeTotal * _volume;
			}
			return _volume;
		}
	}

	public AudioCategory parentCategory
	{
		get
		{
			if (String.IsNullOrEmpty(_parentCategoryName))
			{
				return null;
			}
			if (_parentCategory == null)
			{
				if (audioController != null)
				{
					_parentCategory = audioController._GetCategory(_parentCategoryName);
				}
				else
				{
					Debug.LogWarning("_audioController == null");
				}
			}
			return _parentCategory;
		}
		set
		{
			if (value == this)
			{
				throw new ArgumentException("AudioCategory: value == this, self parenting");
			}
			_parentCategory = value;
			if (value != null)
			{
				_parentCategoryName = _parentCategory.Name;
			}
			else
			{
				_parentCategoryName = null;
			}
		}
	}

	public AudioController audioController { get; set; }

	internal String _AnalyseAudioItems(Dictionary<String, AudioItem> audioItemsDict)
	{
		if (AudioItems == null)
		{
			return null;
		}
		String text = null;
		foreach (AudioItem audioItem in AudioItems)
		{
			if (audioItem != null)
			{
				audioItem._Initialize(this);
				if (audioItemsDict != null)
				{
					if (!audioItemsDict.ContainsKey(audioItem.Name))
					{
						audioItemsDict.Add(audioItem.Name, audioItem);
					}
					else
					{
						text = text + "Multiple audio items with name '" + audioItem.Name + "'\n";
					}
				}
			}
		}
		return text;
	}

	internal Int32 _GetIndexOf(AudioItem audioItem)
	{
		if (AudioItems == null)
		{
			return -1;
		}
		for (Int32 i = 0; i < AudioItems.Length; i++)
		{
			if (audioItem == AudioItems[i])
			{
				return i;
			}
		}
		return -1;
	}

	private void _ApplyVolumeChange()
	{
		AudioObject[] playingAudioObjects = AudioController.GetPlayingAudioObjects(false);
		foreach (AudioObject audioObject in playingAudioObjects)
		{
			if (_IsCategoryParentOf(audioObject.category, this))
			{
				audioObject._ApplyVolumeBoth();
			}
		}
	}

	private Boolean _IsCategoryParentOf(AudioCategory toTest, AudioCategory parent)
	{
		for (AudioCategory audioCategory = toTest; audioCategory != null; audioCategory = audioCategory.parentCategory)
		{
			if (audioCategory == parent)
			{
				return true;
			}
		}
		return false;
	}
}
