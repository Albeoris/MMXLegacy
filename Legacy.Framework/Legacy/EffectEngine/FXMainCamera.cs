using System;
using Legacy.Core;
using Legacy.Core.Api;
using Legacy.Core.Combat;
using Legacy.Core.Entities;
using Legacy.Core.EventManagement;
using Legacy.Core.Map;
using Legacy.Core.PartyManagement;
using Legacy.Core.StaticData;
using Legacy.Utilities;
using UnityEngine;
using Object = System.Object;

namespace Legacy.EffectEngine
{
	public class FXMainCamera : MonoBehaviour
	{
		private const Single CRITICAL_HIT_SHAKE_INTENSITY = 0.15f;

		private const Single NORMAL_HIT_SHAKE_INTENSITY = 0.05f;

		private EModus m_cameraModus;

		private GameObject m_CurrentCamera;

		[SerializeField]
		private WalkCameraFX m_walkCameraFX;

		[SerializeField]
		private ShakeCameraFX m_shakeCameraFX;

		[SerializeField]
		private ReboundCameraFX m_reboundCameraFX;

		[SerializeField]
		private InteractiveObjectCamera m_chestCamera;

		[SerializeField]
		private FreeRotationCamera m_freeRotationCamera;

		[SerializeField]
		private CameraObliqueFrustum m_cameraOblique;

		[SerializeField]
		private GameObject m_DefaultCamera;

		[SerializeField]
		private Transform m_CameraOrigin;

		public static FXMainCamera Instance { get; private set; }

		public EModus CameraModus
		{
			get => m_cameraModus;
		    set
			{
				if (m_cameraModus != value)
				{
					m_cameraModus = value;
					OnCameraModusChanged();
				}
			}
		}

		public Transform CameraOriginPoint => m_CameraOrigin;

	    public GameObject DefaultCamera => m_DefaultCamera;

	    public GameObject CurrentCamera => m_CurrentCamera;

	    public FreeRotationCamera CameraRotator => m_freeRotationCamera;

	    public void SwitchCamera(GameObject camera)
		{
			if (m_CurrentCamera != null)
			{
				m_CurrentCamera.SetActive(false);
			}
			if (camera == null)
			{
				m_CurrentCamera = m_DefaultCamera;
			}
			else
			{
				m_CurrentCamera = camera;
			}
			m_CurrentCamera.camera.enabled = true;
			m_CurrentCamera.SetActive(true);
		}

		public void PlayWalkFX(Single moveTime)
		{
			if (m_cameraModus == EModus.Normal)
			{
				m_walkCameraFX.Play(moveTime);
			}
		}

		public void StopWalkFX()
		{
			if (m_cameraModus == EModus.Normal)
			{
				m_walkCameraFX.Stop();
			}
		}

		public void PlayShakeFX(Single shakeIntensity, Vector3 impactDirection)
		{
			if (m_cameraModus == EModus.Normal)
			{
				m_shakeCameraFX.Play(shakeIntensity, impactDirection);
			}
		}

		public void PlayReboundFX(Vector3 reboundDirection)
		{
			if (m_cameraModus == EModus.Normal)
			{
				m_reboundCameraFX.Play(reboundDirection);
			}
		}

		public void ResetCameraTransformation()
		{
			Transform transform = m_CurrentCamera.transform;
			transform.parent = this.transform;
			transform.position = m_CameraOrigin.position;
			transform.rotation = m_CameraOrigin.rotation;
		}

		private void Awake()
		{
			Instance = this;
			Shader.SetGlobalFloat("_DarkVisionRimScale", 1f);
			Shader.SetGlobalColor("_DarkVisionColor", new Color(0f, 0f, 0f));
			if (m_DefaultCamera != null)
			{
				AudioListener component = m_DefaultCamera.GetComponent<AudioListener>();
				if (component != null)
				{
					component.velocityUpdateMode = AudioVelocityUpdateMode.Dynamic;
				}
			}
			SwitchCamera(m_DefaultCamera);
			if (m_cameraOblique != null)
			{
				m_cameraOblique.VerticalOblique = -0.15f;
			}
			ResetCameraTransformation();
			DelayedEventManager.RegisterEvent(EDelayType.ON_FX_HIT, EEventType.MONSTER_ATTACKS, new EventHandler(OnMonsterAttacks));
			DelayedEventManager.RegisterEvent(EDelayType.ON_FX_HIT, EEventType.MONSTER_ATTACKS_RANGED, new EventHandler(OnMonsterAttacksRanged));
			DelayedEventManager.RegisterEvent(EDelayType.ON_FX_HIT, EEventType.MONSTER_CAST_SPELL, new EventHandler(OnMonsterCastSpell));
		}

		private void OnDestroy()
		{
			DelayedEventManager.UnregisterEvent(EDelayType.ON_FX_HIT, EEventType.MONSTER_ATTACKS, new EventHandler(OnMonsterAttacks));
			DelayedEventManager.UnregisterEvent(EDelayType.ON_FX_HIT, EEventType.MONSTER_ATTACKS_RANGED, new EventHandler(OnMonsterAttacksRanged));
			DelayedEventManager.UnregisterEvent(EDelayType.ON_FX_HIT, EEventType.MONSTER_CAST_SPELL, new EventHandler(OnMonsterCastSpell));
		}

		private void OnCameraModusChanged()
		{
			if (m_cameraModus == EModus.Cutscene)
			{
				foreach (BaseCameraFX baseCameraFX in GetComponentsInChildren<BaseCameraFX>())
				{
					baseCameraFX.CancelEffect();
				}
				m_freeRotationCamera.enabled = false;
			}
			else
			{
				m_freeRotationCamera.enabled = true;
			}
		}

		private void OnMonsterAttacks(Object p_sender, EventArgs p_args)
		{
			OnMonsterAttacksGeneric(p_sender, p_args, false);
		}

		private void OnMonsterAttacksRanged(Object p_sender, EventArgs p_args)
		{
			OnMonsterAttacksGeneric(p_sender, p_args, true);
		}

		private void OnMonsterAttacksGeneric(Object p_sender, EventArgs p_args, Boolean p_isRanged)
		{
			AttacksEventArgs attacksEventArgs = (AttacksEventArgs)p_args;
			Position position = LegacyLogic.Instance.WorldManager.Party.Position;
			Position position2 = ((Monster)p_sender).Position;
			EDirection lineOfSightDirection = EDirectionFunctions.GetLineOfSightDirection(position2, position);
			foreach (AttacksEventArgs.AttackedTarget attackedTarget in attacksEventArgs.Attacks)
			{
				if (attackedTarget.AttackTarget is Character)
				{
					OnAttackResult(attackedTarget.AttackResult.Result, EDirectionFunctions.GetVector3Dir(lineOfSightDirection));
				}
			}
		}

		private void OnMonsterCastSpell(Object p_sender, EventArgs p_args)
		{
			SpellEventArgs spellEventArgs = (SpellEventArgs)p_args;
			Position position = LegacyLogic.Instance.WorldManager.Party.Position;
			Position position2 = ((Monster)p_sender).Position;
			EDirection lineOfSightDirection = EDirectionFunctions.GetLineOfSightDirection(position2, position);
			foreach (AttackedTarget attackedTarget in spellEventArgs.SpellTargetsOfType<AttackedTarget>())
			{
				if (attackedTarget.Target is Character && attackedTarget.Result != null && attackedTarget.Result.DamageDone > 0)
				{
					OnAttackResult(attackedTarget.Result.Result, EDirectionFunctions.GetVector3Dir(lineOfSightDirection));
					return;
				}
			}
			if (StaticDataHandler.GetStaticData<MonsterSpellStaticData>(EDataType.MONSTER_SPELLS, spellEventArgs.Spell.StaticID).InflictedConditions.Length > 0)
			{
				PlayShakeFX(0.05f, Vector3.zero);
			}
		}

		private void OnAttackResult(EResultType attackResult, Vector3D impactDirection)
		{
			Vector3 impactDirection2 = new Vector3(impactDirection.X, impactDirection.Y, impactDirection.Z);
			if (attackResult == EResultType.CRITICAL_HIT)
			{
				PlayShakeFX(0.15f, impactDirection2);
			}
			else if (attackResult == EResultType.HIT)
			{
				PlayShakeFX(0.05f, Vector3.zero);
			}
		}

		public enum EModus
		{
			Normal,
			Cutscene
		}
	}
}
