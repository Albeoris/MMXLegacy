using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Legacy.EffectEngine.Effects
{
	public static class SkinnedMeshRendererGenerator
	{
		private const Single MAX_Y_FOR_BONES = 1f;

		private const Single WEIGHT_FACTOR = 0.7f;

		private static WaitForEndOfFrame WAIT_FOR_END_OF_FRAME = new WaitForEndOfFrame();

		public static IEnumerator GenerateSkin(Mesh p_mesh, SkinnedMeshRenderer p_parent, GameObject p_targetGO, Material p_material, OnFinished p_onFinished, Single BONE_OFFSET, Single MAX_Y_FOR_BONES, Single WEIGHT_FACTOR, Single p_maxBoneAxisOffset)
		{
			Transform[] bones = p_parent.bones;
			Single maxY = p_targetGO.transform.position.y + MAX_Y_FOR_BONES;
			List<Transform> possibleBonesList = new List<Transform>();
			List<Int32> possibleBonesIndeciesList = new List<Int32>();
			for (Int32 i = 0; i < bones.Length; i++)
			{
				if (maxY >= bones[i].transform.position.y)
				{
					possibleBonesList.Add(bones[i]);
					possibleBonesIndeciesList.Add(i);
				}
			}
			Transform[] possibleBones = possibleBonesList.ToArray();
			Int32[] possibleBonesIndecies = possibleBonesIndeciesList.ToArray();
			Int32 rootBoneIndex = Array.IndexOf<Transform>(bones, p_parent.rootBone);
			if (rootBoneIndex < 0)
			{
				rootBoneIndex = 0;
			}
			Int32[] tris = p_mesh.triangles;
			Vector3[] vertices = p_mesh.vertices;
			Int32 waitCounter = 0;
			for (Int32 j = 0; j < tris.Length; j += 6)
			{
				waitCounter++;
				if (waitCounter > 50)
				{
					waitCounter = 0;
					yield return WAIT_FOR_END_OF_FRAME;
				}
				Vector3 quadStart = vertices[tris[j]];
				Int32 nearestBoneIndexStart = 0;
				Single minBoneDistStart = -1f;
				for (Int32 k = 0; k < possibleBones.Length; k++)
				{
					Vector3 bonePos = possibleBones[k].position;
					Single distStart = (quadStart - bonePos).magnitude;
					distStart += Mathf.Abs(quadStart.y - bonePos.y);
					if (minBoneDistStart == -1f || distStart < minBoneDistStart)
					{
						nearestBoneIndexStart = possibleBonesIndecies[k];
						minBoneDistStart = distStart;
					}
				}
				Vector3 bonePosStart = bones[nearestBoneIndexStart].position;
				quadStart = bonePosStart * BONE_OFFSET + quadStart * (1f - BONE_OFFSET);
				Vector3 distBone = bonePosStart - quadStart;
				Vector3 move = Vector3.zero;
				if (Mathf.Abs(distBone.x) > p_maxBoneAxisOffset)
				{
					move.x = Mathf.Sign(distBone.x) * (Mathf.Abs(distBone.x) - p_maxBoneAxisOffset);
					quadStart.x += move.x;
					distBone = bonePosStart - quadStart;
				}
				if (Mathf.Abs(distBone.z) > p_maxBoneAxisOffset)
				{
					move.z = Mathf.Sign(distBone.z) * (Mathf.Abs(distBone.z) - p_maxBoneAxisOffset);
					quadStart.z += move.z;
				}
				vertices[tris[j + 1]] += quadStart - vertices[tris[j]];
				vertices[tris[j]] = quadStart;
			}
			p_mesh.vertices = vertices;
			yield break;
		}

		public delegate void OnFinished();
	}
}
