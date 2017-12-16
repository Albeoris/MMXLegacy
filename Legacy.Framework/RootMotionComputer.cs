using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;

[AddComponentMenu("Mixamo/Root Motion Computer")]
public class RootMotionComputer : MonoBehaviour
{
	public Transform rootNode;

	public Animation anim;

	public Transform pelvis;

	public Vector3 pelvisRightAxis = Vector3.right;

	public Transform[] rootBones;

	public Boolean isManagedExternally;

	public RootMotionComputationMode computationMode = RootMotionComputationMode.TranslationAndRotation;

	public Boolean applyMotion = true;

	private Quaternion dRotation = Quaternion.identity;

	private AnimInfo[] m_AnimInfos;

	private Dictionary<AnimationState, AnimInfo> m_animInfoTable = new Dictionary<AnimationState, AnimInfo>();

	public Boolean isDebugMode = true;

	public Single debugGizmoSize = 0.25f;

	private Boolean isFirstFrame = true;

	public Quaternion deltaRotation => dRotation;

    public Vector3 deltaEulerAngles => dRotation.eulerAngles;

    private void Awake()
	{
		if (!isManagedExternally)
		{
			Initialize();
		}
	}

	public void Initialize()
	{
		if (anim == null)
		{
			anim = gameObject.GetComponentInChildren<Animation>();
			if (anim == null)
			{
				Debug.LogError("No animation component has been specified.", this);
			}
			else if (isDebugMode)
			{
				Debug.LogWarning(String.Format("No animation component has been specified. Using the animation component on {0}.", gameObject.name), this);
			}
		}
		if (rootNode == null)
		{
			rootNode = transform;
			if (isDebugMode)
			{
				Debug.LogWarning(String.Format("No root object has been manually specified. Assuming that {0} is the root object to be moved.", gameObject.name), this);
			}
		}
		if (pelvis == null)
		{
			Transform[] componentsInChildren = GetComponentsInChildren<Transform>();
			foreach (Transform transform in componentsInChildren)
			{
				if (pelvis == null && (transform.name.ToLower() == "hips" || transform.name.ToLower().Contains("pelvis")))
				{
					pelvis = transform;
				}
			}
			if (pelvis == null)
			{
				foreach (Transform transform2 in componentsInChildren)
				{
					if (!(transform2.GetComponent<SkinnedMeshRenderer>() == null))
					{
						Transform[] componentsInChildren2 = transform2.GetComponentsInChildren<Transform>();
						if (componentsInChildren2.Length > 1)
						{
							pelvis = transform2;
						}
					}
				}
			}
			if (pelvis == null)
			{
				Debug.LogError("No pelvis transform has been specified.", this);
			}
			else if (isDebugMode)
			{
				Debug.LogWarning(String.Format("No pelvis object as been manually specified. Assuming that {0} is the pelvis object to track.", pelvis.name));
			}
		}
		Boolean isPlaying = anim.isPlaying;
		m_animInfoTable.Clear();
		IEnumerator enumerator = anim.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Object obj = enumerator.Current;
				AnimationState aState = (AnimationState)obj;
				AddAnimInfoToTable(aState);
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
		anim.Sample();
		anim.Stop();
		anim.enabled = true;
		IEnumerator enumerator2 = anim.GetEnumerator();
		try
		{
			while (enumerator2.MoveNext())
			{
				Object obj2 = enumerator2.Current;
				AnimationState aState2 = (AnimationState)obj2;
				SetupNewAnimInfo(aState2);
			}
		}
		finally
		{
			IDisposable disposable2;
			if ((disposable2 = (enumerator2 as IDisposable)) != null)
			{
				disposable2.Dispose();
			}
		}
		IEnumerator enumerator3 = anim.GetEnumerator();
		try
		{
			while (enumerator3.MoveNext())
			{
				Object obj3 = enumerator3.Current;
				AnimationState animationState = (AnimationState)obj3;
				AnimInfo animInfo = m_animInfoTable[animationState];
				animationState.weight = animInfo.currentWeight;
				animationState.normalizedTime = animInfo.currentNormalizedTime;
			}
		}
		finally
		{
			IDisposable disposable3;
			if ((disposable3 = (enumerator3 as IDisposable)) != null)
			{
				disposable3.Dispose();
			}
		}
		if (isPlaying)
		{
			anim.Play();
		}
		else
		{
			anim.Stop();
		}
	}

	public void AddAnimInfoToTable(AnimationState aState)
	{
		if (aState == null)
		{
			throw new ArgumentNullException("aState");
		}
		CreateAnimInfo(aState);
	}

	private AnimInfo CreateAnimInfo(AnimationState aState)
	{
		AnimInfo animInfo = new AnimInfo(aState);
		animInfo.currentNormalizedTime = aState.normalizedTime;
		animInfo.currentWeight = aState.weight;
		m_animInfoTable.Add(aState, animInfo);
		return animInfo;
	}

	public void SetupNewAnimInfo(AnimationState aState)
	{
		AnimInfo animInfo = m_animInfoTable[aState];
		Boolean enabled = aState.enabled;
		WrapMode wrapMode = aState.wrapMode;
		aState.weight = 1f;
		aState.enabled = true;
		aState.wrapMode = WrapMode.Once;
		aState.normalizedTime = 0f;
		anim.Sample();
		animInfo.startAxis = GetProjectedAxis(pelvis, pelvisRightAxis);
		animInfo.previousAxis = GetProjectedAxis(pelvis, pelvisRightAxis);
		aState.normalizedTime = 1f;
		anim.Sample();
		animInfo.endAxis = GetProjectedAxis(pelvis, pelvisRightAxis);
		animInfo.totalRotation = Quaternion.FromToRotation(animInfo.startAxis, animInfo.endAxis);
		aState.normalizedTime = 0f;
		aState.weight = 0f;
		aState.enabled = enabled;
		aState.wrapMode = wrapMode;
		anim.Sample();
	}

	private void LateUpdate()
	{
		if (!isManagedExternally && rootNode != null)
		{
			ComputeRootMotion();
		}
	}

	public void ComputeRootMotion()
	{
		if (!anim.isPlaying)
		{
			return;
		}
		Boolean flag = computationMode == RootMotionComputationMode.TranslationAndRotation;
		Int32 num = LoadAnimInfos(anim, ref m_AnimInfos);
		AnimInfo[] animInfos = m_AnimInfos;
		Int32 num2 = 0;
		while (num2 < animInfos.Length && num2 < num)
		{
			AnimInfo animInfo = animInfos[num2];
			animInfo.currentNormalizedTime = animInfo.AnimState.normalizedTime;
			animInfo.currentWeight = animInfo.AnimState.weight;
			animInfo.AnimState.weight = 0f;
			num2++;
		}
		Array.Sort<AnimInfo>(animInfos, 0, num, StateLayerSort.Default);
		Single num3 = 1f;
		Int32 num4 = 0;
		while (num4 < animInfos.Length && num4 < num)
		{
			Int32 layer = animInfos[num4].layer;
			Single num5 = 0f;
			Int32 num6 = 0;
			while (num6 < animInfos.Length && num6 < num)
			{
				AnimInfo animInfo = animInfos[num6];
				if (animInfo.layer == layer)
				{
					if (!animInfo.AnimState.enabled || num3 <= 0f)
					{
						animInfo.contributingWeight = 0f;
					}
					else
					{
						animInfo.contributingWeight = num3 * animInfo.currentWeight;
					}
					num5 += animInfo.contributingWeight;
				}
				num6++;
			}
			if (num5 > 1f)
			{
				Single num7 = 1f / num5;
				Int32 num8 = 0;
				while (num8 < animInfos.Length && num8 < num)
				{
					AnimInfo animInfo = animInfos[num8];
					if (animInfo.layer == layer)
					{
						animInfo.contributingWeight *= num7;
					}
					num8++;
				}
				num5 = 1f;
			}
			num3 -= num5;
			num4++;
		}
		dRotation = Quaternion.identity;
		Int32 num9 = 0;
		while (num9 < animInfos.Length && num9 < num)
		{
			AnimInfo animInfo = animInfos[num9];
			if (animInfo.contributingWeight != 0f)
			{
				if (animInfo.AnimState.blendMode != AnimationBlendMode.Additive)
				{
					animInfo.AnimState.weight = 1f;
					animInfo.AnimState.time -= Time.deltaTime * animInfo.AnimState.speed;
					animInfo.previousNormalizedTime = animInfo.AnimState.normalizedTime;
					anim.Sample();
					animInfo.previousAxis = GetProjectedAxis(pelvis, pelvisRightAxis);
					animInfo.AnimState.normalizedTime = animInfo.currentNormalizedTime;
					anim.Sample();
					animInfo.currentAxis = GetProjectedAxis(pelvis, pelvisRightAxis);
					Single num10 = animInfo.previousNormalizedTime - (Int32)animInfo.previousNormalizedTime;
					Single num11 = animInfo.currentNormalizedTime - (Int32)animInfo.currentNormalizedTime;
					animInfo.previousNormalizedTime = 1f + num10;
					animInfo.currentNormalizedTime = 1f + num11;
					if (num10 > num11)
					{
						if (flag)
						{
							dRotation *= Quaternion.Slerp(Quaternion.identity, Quaternion.FromToRotation(animInfo.previousAxis, animInfo.endAxis) * Quaternion.FromToRotation(animInfo.startAxis, animInfo.currentAxis), animInfo.contributingWeight);
						}
					}
					else if (flag)
					{
						dRotation *= Quaternion.Slerp(Quaternion.identity, Quaternion.FromToRotation(animInfo.previousAxis, animInfo.currentAxis), animInfo.contributingWeight);
					}
					animInfo.AnimState.weight = 0f;
				}
			}
			num9++;
		}
		Int32 num12 = 0;
		while (num12 < animInfos.Length && num12 < num)
		{
			AnimInfo animInfo = animInfos[num12];
			animInfo.AnimState.weight = animInfo.currentWeight;
			num12++;
		}
		anim.Sample();
		if (isFirstFrame)
		{
			dRotation = Quaternion.FromToRotation(Vector3.right, GetProjectedAxis(pelvis, pelvisRightAxis));
			isFirstFrame = false;
		}
		if (flag)
		{
			Quaternion lhs = Quaternion.FromToRotation(GetProjectedAxis(pelvis, pelvisRightAxis), Vector3.right);
			for (Int32 i = 0; i < rootBones.Length; i++)
			{
				rootBones[i].localRotation = lhs * rootBones[i].localRotation;
			}
		}
		if (!applyMotion)
		{
			return;
		}
		if (flag)
		{
			rootNode.localRotation *= dRotation;
		}
	}

	private Vector3 GetProjectedPosition(Transform t)
	{
		Vector3 result = rootNode.InverseTransformPoint(t.position);
		result.y = 0f;
		return result;
	}

	private Vector3 GetProjectedAxis(Transform t, Vector3 axis)
	{
		Vector3 result = rootNode.InverseTransformDirection(t.TransformDirection(axis));
		result.y = 0f;
		return result;
	}

	private Int32 LoadAnimInfos(Animation animation, ref AnimInfo[] array)
	{
		Int32 num = 0;
		IEnumerator enumerator = animation.GetEnumerator();
		while (enumerator.MoveNext())
		{
			num++;
		}
		enumerator.Reset();
		if (array == null || num > array.Length || array.Length - num > array.Length * 0.5f)
		{
			Array.Resize<AnimInfo>(ref array, num);
		}
		for (Int32 i = 0; i < array.Length; i++)
		{
			if (i < num && enumerator.MoveNext())
			{
				AnimationState animationState = (AnimationState)enumerator.Current;
				if (!m_animInfoTable.TryGetValue(animationState, out array[i]))
				{
					array[i] = CreateAnimInfo(animationState);
				}
				array[i].ResetLayer();
			}
			else
			{
				array[i] = null;
			}
		}
		if (num != m_animInfoTable.Count)
		{
			m_animInfoTable.Clear();
			Int32 num2 = 0;
			while (num2 < array.Length && num2 < num)
			{
				m_animInfoTable.Add(array[num2].AnimState, array[num2]);
				num2++;
			}
		}
		return num;
	}

	private class StateLayerSort : IComparer<AnimInfo>
	{
		public static readonly StateLayerSort Default = new StateLayerSort();

		public Int32 Compare(AnimInfo x, AnimInfo y)
		{
			return y.AnimState.layer.CompareTo(x.AnimState.layer);
		}
	}

	private class AnimInfo
	{
		public AnimationState AnimState;

		public Single currentNormalizedTime;

		public Single previousNormalizedTime;

		public Single currentWeight;

		public Single contributingWeight;

		public Vector3 currentAxis;

		public Vector3 previousAxis;

		public Vector3 startAxis;

		public Vector3 endAxis;

		public Quaternion totalRotation;

		public Int32 layer;

		public AnimInfo(AnimationState animState)
		{
			AnimState = animState;
		}

		public void ResetLayer()
		{
			layer = AnimState.layer;
		}
	}

	public enum RootMotionComputationMode
	{
		ZTranslation,
		XZTranslation,
		TranslationAndRotation
	}
}
