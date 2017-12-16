using System;
using Legacy;
using UnityEngine;

[Serializable]
public class AudioItem
{
	public String Name;

	public LoopMode Loop;

	public Int32 loopSequenceCount;

	public Single loopSequenceOverlap;

	public Single loopSequenceRandomDelay;

	public Boolean DestroyOnLoad = true;

	public Single Volume = 1f;

	public AudioPickSubItemMode SubItemPickMode = AudioPickSubItemMode.RandomNotSameTwice;

	public Single MinTimeBetweenPlayCalls = 0.1f;

	public Int32 MaxInstanceCount;

	public Single Delay;

	public Boolean overrideAudioSourceSettings;

	public Single audioSource_MinDistance = 1f;

	public Single audioSource_MaxDistance = 500f;

	public AudioSubItem[] subItems = Arrays<AudioSubItem>.Empty;

	internal Int32 _lastChosen = -1;

	internal Double _lastPlayedTime = -1.0;

	public AudioCategory category { get; private set; }

	private void Awake()
	{
		if (Loop == (LoopMode)3)
		{
			Loop = LoopMode.LoopSequence;
		}
		_lastChosen = -1;
	}

	internal void _Initialize(AudioCategory categ)
	{
		category = categ;
		_NormalizeSubItems();
	}

	private void _NormalizeSubItems()
	{
		Single num = 0f;
		Int32 num2 = 0;
		foreach (AudioSubItem audioSubItem in subItems)
		{
			audioSubItem.item = this;
			if (_IsValidSubItem(audioSubItem))
			{
				num += audioSubItem.Probability;
			}
			audioSubItem._subItemID = num2;
			num2++;
		}
		if (num <= 0f)
		{
			return;
		}
		Single num3 = 0f;
		foreach (AudioSubItem audioSubItem2 in subItems)
		{
			if (_IsValidSubItem(audioSubItem2))
			{
				num3 += audioSubItem2.Probability / num;
				audioSubItem2._SummedProbability = num3;
			}
		}
	}

	private static Boolean _IsValidSubItem(AudioSubItem item)
	{
		AudioSubItemType subItemType = item.SubItemType;
		if (subItemType != AudioSubItemType.Clip)
		{
			return subItemType == AudioSubItemType.Item && item.ItemModeAudioID != null && item.ItemModeAudioID.Length > 0;
		}
		return item.Clip != null;
	}

	[Serializable]
	public enum LoopMode
	{
		DoNotLoop,
		LoopSubitem,
		LoopSequence,
		PlaySequenceAndLoopLast = 4
	}
}
