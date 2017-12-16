using System;
using UnityEngine;

[Serializable]
public class AudioSubItem
{
	public AudioSubItemType SubItemType;

	public Single Probability = 1f;

	public String ItemModeAudioID;

	public AudioClip Clip;

	public Single Volume = 1f;

	public Single PitchShift;

	public Single Pan2D;

	public Single Delay;

	public Single RandomPitch;

	public Single RandomVolume;

	public Single RandomDelay;

	public Single ClipStopTime;

	public Single ClipStartTime;

	public Single FadeIn;

	public Single FadeOut;

	public Boolean RandomStartPosition;

	private Single _summedProbability = -1f;

	internal Int32 _subItemID;

	internal Single _SummedProbability
	{
		get => _summedProbability;
	    set => _summedProbability = value;
	}

	public AudioItem item { get; internal set; }

	public override String ToString()
	{
		if (SubItemType == AudioSubItemType.Clip)
		{
			return "CLIP: " + Clip.name;
		}
		return "ITEM: " + ItemModeAudioID;
	}
}
