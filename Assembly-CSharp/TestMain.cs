using System;
using System.Collections;
using System.Collections.Generic;
using AssetBundles.Core;
using Legacy;
using Legacy.Animations;
using Legacy.Configuration;
using Legacy.Core.Spells;
using Legacy.EffectEngine;
using Legacy.EffectEngine.Effects;
using Legacy.Views;
using UnityEngine;
using Object = System.Object;

public class TestMain : MonoBehaviour
{
	private TestMonster m_ActiveMonsterData;

	private GameObject m_ActiveMonster;

	private LevelEntityView m_ActiveMonsterView;

	private LevelEntityCombatView m_ActiveMonsterCombatView;

	private LevelEntitySpellView m_ActiveMonsterSpellView;

	private AnimatorControl m_ActiveMonsterAnimationHandler;

	private List<BuffFX> m_PartyBuffEffects = new List<BuffFX>();

	private List<FXQueue> m_PartyBuffGameObjects = new List<FXQueue>();

	private List<BuffFX> m_MonsterBuffEffects = new List<BuffFX>();

	private List<FXQueue> m_MonsterBuffGameObjects = new List<FXQueue>();

	private Boolean m_LastSpellIsMonster;

	private TestSpell m_LastSpell;

	private ETarget m_TargetOrWizard;

	private Boolean m_MonsterSwitched;

	[SerializeField]
	private TestDatabase m_Database;

	[SerializeField]
	private GameObject m_Party;

	[SerializeField]
	private GameObject m_Member1;

	[SerializeField]
	private GameObject m_Member2;

	[SerializeField]
	private GameObject m_Member3;

	[SerializeField]
	private GameObject m_Member4;

	[SerializeField]
	private GameObject m_PartySlot;

	[SerializeField]
	private GameObject m_MonsterSlot;

	[SerializeField]
	private GameObject m_GuiRoot;

	public GameObject ActiveMonster => m_ActiveMonster;

    public TestMonster ActiveMonsterData => m_ActiveMonsterData;

    public LevelEntityView ActiveMonsterView => m_ActiveMonsterView;

    public LevelEntityCombatView ActiveMonsterCombatView => m_ActiveMonsterCombatView;

    public LevelEntitySpellView ActiveMonsterSpellView => m_ActiveMonsterSpellView;

    public AnimatorControl ActiveMonsterAnimationHandler => m_ActiveMonsterAnimationHandler;

    public List<BuffFX> PartyBuffEffects => m_PartyBuffEffects;

    public List<BuffFX> MonsterBuffEffects => m_MonsterBuffEffects;

    private void Start()
	{
		SoundConfigManager.Settings.SFXVolume = 1f;
		if (m_Database.Monsters.Length > 0)
		{
			LoadMonster(m_Database.Monsters[0]);
		}
	}

	private void Update()
	{
		if (Input.GetKeyUp(KeyCode.F12))
		{
			Application.CaptureScreenshot("Screenshot_" + DateTime.UtcNow.Ticks + ".png");
		}
		if (Input.GetKeyUp(KeyCode.F2))
		{
			m_GuiRoot.SetActive(!m_GuiRoot.activeSelf);
		}
		if (Input.GetKeyUp(KeyCode.Space) && m_LastSpell != null)
		{
			if (m_LastSpellIsMonster)
			{
				MonsterCastSpell(m_TargetOrWizard, m_LastSpell);
			}
			else
			{
				PartyCastSpell(m_TargetOrWizard, m_LastSpell);
			}
		}
	}

	public void LoadMonster(TestMonster data)
	{
		if (m_ActiveMonster != null)
		{
			Destroy(m_ActiveMonster);
			Resources.UnloadUnusedAssets();
		}
		m_ActiveMonsterData = data;
		StartCoroutine(LoadMonster(data.Prefab));
		m_LastSpell = null;
	}

	private IEnumerator LoadMonster(String p_PrefabName)
	{
		AssetBundleManager manager = GetComponent<AssetBundleManager>();
		AssetRequest request = manager.RequestAsset(p_PrefabName);
		if (request == null)
		{
			Debug.Log("unknow path " + p_PrefabName);
			yield break;
		}
		if (request == null || request.Status == ERequestStatus.Error)
		{
			Debug.Log(request.ErrorText);
			yield break;
		}
		yield return StartCoroutine(request);
		GameObject asset = (GameObject)request.Asset;
		m_ActiveMonster = (GameObject)Instantiate(asset);
		m_ActiveMonsterView = m_ActiveMonster.GetComponent<LevelEntityView>();
		m_ActiveMonsterCombatView = m_ActiveMonster.GetComponent<LevelEntityCombatView>();
		m_ActiveMonsterSpellView = m_ActiveMonster.GetComponent<LevelEntitySpellView>();
		m_ActiveMonsterAnimationHandler = m_ActiveMonster.GetComponentInChildren<AnimatorControl>();
		m_ActiveMonster.transform.position = default(Vector3);
		m_ActiveMonster.transform.rotation = Quaternion.identity;
		yield break;
	}

	public void PartyCastSpell(ETarget wizard, TestSpell spell)
	{
		m_LastSpellIsMonster = false;
		m_LastSpell = spell;
		m_TargetOrWizard = wizard;
		if (!String.IsNullOrEmpty(spell.EffectPath))
		{
			FXQueue fxqueue = Helper.ResourcesLoad<FXQueue>(spell.EffectPath, false);
			if (fxqueue != null && m_ActiveMonster != null)
			{
				if (wizard == ETarget.Party)
				{
					wizard = (ETarget)UnityEngine.Random.Range(1, 4);
				}
				GameObject gameObject;
				switch (wizard)
				{
				case ETarget.Member1:
					gameObject = m_Member1;
					break;
				case ETarget.Member2:
					gameObject = m_Member2;
					break;
				case ETarget.Member3:
					gameObject = m_Member3;
					break;
				case ETarget.Member4:
					gameObject = m_Member4;
					break;
				default:
					return;
				}
				Vector3 forward = m_Party.transform.forward;
				Vector3 p_slotLeft = -m_Party.transform.right;
				ETargetType targetType = spell.TargetType;
				FXArgs args;
				if ((targetType & ETargetType.PARTY) == ETargetType.PARTY)
				{
					args = new FXArgs(m_PartySlot, m_PartySlot, m_PartySlot, m_PartySlot, m_PartySlot.transform.position, forward, p_slotLeft, m_PartySlot.transform.position, new List<GameObject>
					{
						m_PartySlot
					});
				}
				else if ((targetType & ETargetType.LINE_OF_SIGHT) == ETargetType.LINE_OF_SIGHT)
				{
					args = new FXArgs(m_PartySlot, m_MonsterSlot, m_PartySlot, m_MonsterSlot, m_PartySlot.transform.position, forward, p_slotLeft, m_MonsterSlot.transform.position, new List<GameObject>
					{
						m_ActiveMonster
					});
				}
				else if ((targetType & ETargetType.MULTY) == ETargetType.MULTY)
				{
					args = new FXArgs(gameObject, m_MonsterSlot, gameObject, m_MonsterSlot, m_PartySlot.transform.position, forward, p_slotLeft, m_MonsterSlot.transform.position, new List<GameObject>
					{
						m_ActiveMonster
					});
				}
				else if ((targetType & ETargetType.SINGLE) == ETargetType.SINGLE)
				{
					FXTags component = m_ActiveMonster.GetComponent<FXTags>();
					GameObject p_endPoint = m_ActiveMonster;
					if (component != null)
					{
						p_endPoint = component.FindOne("HitSpot");
					}
					else
					{
						Debug.LogError("FXTags not found!!\nTarget=" + m_ActiveMonster, m_ActiveMonster);
					}
					args = new FXArgs(gameObject, m_MonsterSlot, gameObject, p_endPoint, m_PartySlot.transform.position, forward, p_slotLeft, m_MonsterSlot.transform.position, new List<GameObject>
					{
						m_ActiveMonster
					});
				}
				else if ((targetType & ETargetType.ADJACENT) == ETargetType.ADJACENT)
				{
					args = new FXArgs(gameObject, m_PartySlot, gameObject, m_PartySlot, m_PartySlot.transform.position, forward, p_slotLeft, m_MonsterSlot.transform.position, new List<GameObject>
					{
						m_ActiveMonster
					});
				}
				else
				{
					Debug.LogError("error !! spellType: " + targetType);
					args = new FXArgs(gameObject, m_ActiveMonster, gameObject, m_ActiveMonster, m_PartySlot.transform.position, forward, p_slotLeft, m_MonsterSlot.transform.position, new List<GameObject>
					{
						m_ActiveMonster
					});
				}
				FXQueue fxqueue2 = Helper.Instantiate<FXQueue>(fxqueue);
				fxqueue2.Finished += delegate(Object sender, EventArgs e)
				{
					m_ActiveMonsterAnimationHandler.Hit();
				};
				fxqueue2.Execute(args);
			}
			else
			{
				Debug.LogError("SpellEffect not found! " + spell.EffectPath);
			}
		}
		PartyCastBuff(spell);
		MonsterCastBuff(spell);
	}

	public void MonsterCastSpell(ETarget target, TestSpell spell)
	{
		m_LastSpellIsMonster = false;
		m_LastSpell = spell;
		m_TargetOrWizard = target;
		FXDescription fxdescription = Helper.ResourcesLoad<FXDescription>(spell.EffectPath, false);
		if (fxdescription == null)
		{
			Debug.LogError("FXDescription not found! at " + spell.EffectPath, this);
			return;
		}
		if (target == ETarget.Party)
		{
			target = (ETarget)UnityEngine.Random.Range(1, 4);
		}
		GameObject gameObject;
		switch (target)
		{
		case ETarget.Member1:
			gameObject = m_Member1;
			break;
		case ETarget.Member2:
			gameObject = m_Member2;
			break;
		case ETarget.Member3:
			gameObject = m_Member3;
			break;
		case ETarget.Member4:
			gameObject = m_Member4;
			break;
		default:
			return;
		}
		Vector3 forward = m_ActiveMonster.transform.forward;
		Vector3 p_slotLeft = -m_ActiveMonster.transform.right;
		m_ActiveMonsterAnimationHandler.AttackMagic(spell.AnimationNum);
		AnimatorEventHandler component = m_ActiveMonsterAnimationHandler.GetComponent<AnimatorEventHandler>();
		GameObject activeMonster = m_ActiveMonster;
		ETargetType targetType = spell.TargetType;
		FXArgs p_args;
		if ((targetType & ETargetType.MULTY) == ETargetType.MULTY)
		{
			p_args = new FXArgs(activeMonster, m_PartySlot, activeMonster, m_PartySlot, activeMonster.transform.position, forward, p_slotLeft, m_PartySlot.transform.position, new List<GameObject>
			{
				m_Member1,
				m_Member2,
				m_Member3,
				m_Member4
			});
		}
		else if ((targetType & ETargetType.SINGLE) == ETargetType.SINGLE)
		{
			p_args = new FXArgs(activeMonster, gameObject, activeMonster, gameObject, activeMonster.transform.position, forward, p_slotLeft, m_PartySlot.transform.position, new List<GameObject>
			{
				gameObject
			});
		}
		else if ((targetType & ETargetType.ADJACENT) == ETargetType.ADJACENT)
		{
			p_args = new FXArgs(activeMonster, activeMonster, activeMonster, activeMonster, activeMonster.transform.position, forward, p_slotLeft, m_PartySlot.transform.position, new List<GameObject>
			{
				gameObject
			});
		}
		else
		{
			p_args = new FXArgs(activeMonster, activeMonster, activeMonster, activeMonster, activeMonster.transform.position, forward, p_slotLeft, m_PartySlot.transform.position, new List<GameObject>
			{
				gameObject
			});
		}
		fxdescription = Helper.Instantiate<FXDescription>(fxdescription);
		fxdescription.Configurate(component, p_args);
	}

	private void PartyCastBuff(TestSpell spell)
	{
		foreach (BuffFX buffFX in m_PartyBuffEffects)
		{
			if (buffFX != null)
			{
				buffFX.Destroy();
			}
		}
		m_PartyBuffEffects.Clear();
		foreach (FXQueue fxqueue in m_PartyBuffGameObjects)
		{
			if (fxqueue != null)
			{
				Destroy(fxqueue.gameObject, 5f);
			}
		}
		m_PartyBuffGameObjects.Clear();
		foreach (String text in spell.SorcererBuffEffectPath)
		{
			if (!String.IsNullOrEmpty(text))
			{
				BuffFX buffFX2 = Helper.ResourcesLoad<BuffFX>(text, false);
				if (buffFX2 != null)
				{
					Vector3 forward = m_Party.transform.forward;
					Vector3 p_slotLeft = -m_Party.transform.right;
					buffFX2 = Helper.Instantiate<BuffFX>(buffFX2);
					FXQueue fxqueue2 = new GameObject("FXQueue " + buffFX2.name).AddComponent<FXQueue>();
					fxqueue2.SetData(new FXQueue.Entry[]
					{
						new FXQueue.Entry(buffFX2, 0f, 0f)
					}, 0);
					FXArgs args = new FXArgs(m_PartySlot, m_PartySlot, m_PartySlot, m_PartySlot, m_PartySlot.transform.position, forward, p_slotLeft, m_PartySlot.transform.position);
					fxqueue2.Execute(args);
					m_PartyBuffGameObjects.Add(fxqueue2);
					m_PartyBuffEffects.Add(buffFX2);
				}
				else
				{
					Debug.LogError("PartyCastBuff: BuffEffect not found! " + text);
				}
			}
		}
	}

	private void MonsterCastBuff(TestSpell spell)
	{
		if (m_ActiveMonster == null)
		{
			return;
		}
		foreach (BuffFX buffFX in m_MonsterBuffEffects)
		{
			if (buffFX != null)
			{
				buffFX.Destroy();
			}
		}
		m_MonsterBuffEffects.Clear();
		foreach (FXQueue fxqueue in m_MonsterBuffGameObjects)
		{
			if (fxqueue != null)
			{
				Destroy(fxqueue.gameObject, 5f);
			}
		}
		m_MonsterBuffGameObjects.Clear();
		foreach (String text in spell.TargetBuffEffectPath)
		{
			if (!String.IsNullOrEmpty(text))
			{
				BuffFX buffFX2 = Helper.ResourcesLoad<BuffFX>(text, false);
				if (buffFX2 != null)
				{
					Vector3 forward = m_Party.transform.forward;
					Vector3 p_slotLeft = -m_Party.transform.right;
					buffFX2 = Helper.Instantiate<BuffFX>(buffFX2);
					FXQueue fxqueue2 = new GameObject("FXQueue " + buffFX2.name).AddComponent<FXQueue>();
					fxqueue2.SetData(new FXQueue.Entry[]
					{
						new FXQueue.Entry(buffFX2, 0f, 0f)
					}, 0);
					FXArgs args = new FXArgs(m_ActiveMonster, m_ActiveMonster, m_ActiveMonster, m_ActiveMonster, m_ActiveMonster.transform.position, forward, p_slotLeft, m_ActiveMonster.transform.position);
					fxqueue2.Execute(args);
					m_MonsterBuffGameObjects.Add(fxqueue2);
					m_MonsterBuffEffects.Add(buffFX2);
					m_ActiveMonsterAnimationHandler.Hit();
				}
				else
				{
					Debug.LogError("MonsterCastBuff: BuffEffect not found! " + text);
				}
			}
		}
	}
}
