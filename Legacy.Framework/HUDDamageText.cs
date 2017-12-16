using System;
using System.Collections.Generic;
using Legacy;
using Legacy.Core.Combat;
using Legacy.Core.Configuration;
using Legacy.EffectEngine;
using Legacy.MMGUI;
using UnityEngine;
using Object = System.Object;

[AddComponentMenu("NGUI/Examples/HUD DamageText")]
public class HUDDamageText : MonoBehaviour
{
	private Transform m_targetAnchor;

	[SerializeField]
	private UIAtlas m_atlas;

	[SerializeField]
	public Single capSize;

	[SerializeField]
	public Single capSizeCrit;

	public UIFont font;

	public UIFont critFont;

	public UILabel.Effect effect;

	public UILabel.Effect critEffect;

	[SerializeField]
	private Boolean m_IsStatic;

	[SerializeField]
	private Boolean m_IsFixCritPosition;

	[SerializeField]
	public List<Vector3> FixCritPostions = new List<Vector3>(5);

	[SerializeField]
	private Int32 m_layer;

	[SerializeField]
	private Int32 m_depth;

	[SerializeField]
	private Single m_scaleCrit = 1.5f;

	[SerializeField]
	private Single m_staticDelay = 0.75f;

	public AnimationCurve alphaCurve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(1f, 1f),
		new Keyframe(3f, 0f)
	});

	public AnimationCurve staticPopupCurve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(1f, 1f),
		new Keyframe(3f, 0f)
	});

	private List<Entry> m_list = new List<Entry>();

	private List<Entry> m_unused = new List<Entry>();

	private Entry[] m_crits = new Entry[5];

	private Int32 m_counter;

	public Camera GameCamera;

	public Camera UICamera;

	public Boolean isVisible => m_list.Count != 0;

    public Transform TargetAnchor
	{
		get => m_targetAnchor;
        set => m_targetAnchor = value;
    }

	private void Start()
	{
		if (FXMainCamera.Instance != null)
		{
			GameCamera = FXMainCamera.Instance.DefaultCamera.camera;
			UICamera = GUIMainCamera.Instance.camera;
		}
	}

	public void Clear()
	{
		for (Int32 i = m_list.Count - 1; i >= 0; i--)
		{
			Delete(m_list[i]);
		}
	}

	private Entry Create(Boolean p_crit, EDamageType p_type, Boolean p_hasBackground)
	{
		Int32 num = Math.Max(NGUITools.CalculateNextDepth(gameObject), m_depth);
		Entry entry;
		if (m_unused.Count > 0)
		{
			entry = m_unused[m_unused.Count - 1];
			m_unused.RemoveAt(m_unused.Count - 1);
			entry.label.gameObject.SetActive(true);
		}
		else
		{
			entry = new Entry();
			entry.background = NGUITools.AddWidget<UISprite>(gameObject);
			entry.background.layer = m_layer;
			entry.background.atlas = m_atlas;
			entry.background.name = "Background";
			entry.label = NGUITools.AddWidget<UILabel>(gameObject);
			entry.label.layer = m_layer;
			entry.label.MakePixelPerfect();
			entry.label.effectDistance = Vector2.one;
			entry.label.symbolStyle = UIFont.SymbolStyle.Colored;
			entry.label.effectStyle = UILabel.Effect.Outline;
			entry.label.limbicBlankHeight = 100;
			entry.label.name = m_counter.ToString();
			m_counter++;
		}
		entry.time = Time.timeSinceLevelLoad;
		entry.background.depth = num;
		switch (p_type)
		{
		case EDamageType.PHYSICAL:
			if (ConfigManager.Instance.Game.ChineseVersion)
			{
				entry.background.spriteName = "PIC_damage_physical_chinese";
			}
			else
			{
				entry.background.spriteName = "PIC_damage_physical";
			}
			break;
		case EDamageType.AIR:
			entry.background.spriteName = "PIC_damage_air";
			break;
		case EDamageType.EARTH:
			entry.background.spriteName = "PIC_damage_earth";
			break;
		case EDamageType.FIRE:
			entry.background.spriteName = "PIC_damage_fire";
			break;
		case EDamageType.WATER:
			entry.background.spriteName = "PIC_damage_water";
			break;
		case EDamageType.DARK:
			entry.background.spriteName = "PIC_damage_dark";
			break;
		case EDamageType.LIGHT:
			entry.background.spriteName = "PIC_damage_light";
			break;
		case EDamageType.PRIMORDIAL:
			entry.background.spriteName = "PIC_damage_prime";
			break;
		}
		entry.backgroundSize = new Vector2(152f, 152f);
		NGUITools.SetActiveSelf(entry.background.gameObject, p_hasBackground);
		entry.IsCrit = p_crit;
		entry.label.depth = num + 1;
		entry.label.cachedTransform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
		entry.background.cachedTransform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
		m_list.Add(entry);
		if (m_targetAnchor != null)
		{
			entry.startPos = m_targetAnchor.position;
		}
		if (m_IsStatic)
		{
			entry.velocity = 80f;
			entry.active = false;
		}
		else
		{
			entry.velocity = 150f;
			entry.active = true;
		}
		entry.currentY = 0f;
		entry.offset = m_list.Count;
		entry.offsetX = 0f;
		if (m_IsFixCritPosition && entry.IsCrit)
		{
			AddToCritList(entry);
		}
		return entry;
	}

	private void AddToCritList(Entry ent)
	{
		Single num = Time.timeSinceLevelLoad;
		Int32 num2 = 0;
		for (Int32 i = 0; i < 5; i++)
		{
			if (m_crits[i] == null)
			{
				ent.FixCritPosInList = i;
				m_crits[i] = ent;
				return;
			}
			if (m_crits[i] is Entry)
			{
				if (m_crits[i].time < num)
				{
					num = m_crits[i].time;
					num2 = i;
				}
			}
		}
		ent.FixCritPosInList = num2;
		m_crits[num2] = ent;
	}

	private void Delete(Entry ent)
	{
		if (m_IsFixCritPosition && ent.IsCrit)
		{
			m_crits[ent.FixCritPosInList] = null;
		}
		m_list.Remove(ent);
		m_unused.Add(ent);
		ent.label.gameObject.SetActive(false);
		ent.background.gameObject.SetActive(false);
	}

	public void Add(Object obj, Boolean crit, Color c, Single stayDuration)
	{
		if (!enabled)
		{
			return;
		}
		if (obj is AttackResult)
		{
			AttackResult attackResult = (AttackResult)obj;
			Single angle = Legacy.Random.Range(-1f, 1f);
			Int32 num = 0;
			foreach (DamageResult damageResult in attackResult.DamageResults)
			{
				Int32 effectiveValue = damageResult.EffectiveValue;
				Entry entry = Create(crit, damageResult.Type, damageResult.Type != EDamageType.HEAL);
				Single value = 0.75f;
				entry.isHeal = (damageResult.Type == EDamageType.HEAL);
				entry.stay = stayDuration;
				entry.label.color = c;
				entry.val = effectiveValue;
				entry.label.effectStyle = ((!entry.IsCrit) ? effect : critEffect);
				entry.label.font = ((!entry.IsCrit) ? font : critFont);
				entry.DamagePercentage = Mathf.Clamp(value, 0.5f, 1f);
				entry.label.text = entry.val.ToString();
				entry.angle = angle;
				entry.offsetX = num * 50;
				entry.isText = false;
				if (entry.isHeal)
				{
					if (m_IsStatic)
					{
						entry.velocity = 0f;
					}
					else
					{
						entry.velocity = 80f;
					}
				}
				num++;
			}
		}
		else
		{
			Entry entry2 = Create(crit, EDamageType.NONE, false);
			entry2.stay = stayDuration;
			entry2.label.color = c;
			entry2.val = 0;
			entry2.label.effectStyle = ((!entry2.IsCrit) ? effect : critEffect);
			entry2.label.font = ((!entry2.IsCrit) ? font : critFont);
			entry2.DamagePercentage = 1f;
			entry2.label.text = obj.ToString();
			entry2.angle = 0f;
			entry2.isHeal = false;
			entry2.isText = true;
			if (m_IsStatic)
			{
				entry2.velocity = 80f;
			}
			else
			{
				entry2.velocity = 20f;
			}
		}
	}

	private void OnDisable()
	{
		Int32 i = m_list.Count;
		while (i > 0)
		{
			Entry entry = m_list[--i];
			if (entry.background != null)
			{
				entry.background.enabled = false;
			}
			if (entry.label != null)
			{
				entry.label.enabled = false;
			}
			else
			{
				m_list.RemoveAt(i);
			}
		}
	}

	private void Update()
	{
		Single timeSinceLevelLoad = Time.timeSinceLevelLoad;
		Single time = alphaCurve[alphaCurve.length - 1].time;
		Single num = time;
		for (Int32 i = m_list.Count - 1; i >= 0; i--)
		{
			Entry entry = m_list[i];
			if (i == 0 && !entry.active)
			{
				entry.active = true;
				entry.time = Time.timeSinceLevelLoad;
			}
			if (entry.active)
			{
				Single num2 = timeSinceLevelLoad - entry.movementStart;
				entry.label.alpha = alphaCurve.Evaluate(num2);
				entry.background.alpha = alphaCurve.Evaluate(num2);
				Vector2 vector = Vector2.zero;
				Single num3 = (!entry.IsCrit) ? 1f : m_scaleCrit;
				Single num4 = staticPopupCurve.Evaluate(num2) * entry.label.font.size * num3;
				vector = staticPopupCurve.Evaluate(num2) * entry.backgroundSize * num3;
				num4 = Mathf.Max(0.01f, num4);
				vector.x = Mathf.Max(0.01f, vector.x);
				vector.y = Mathf.Max(0.01f, vector.y);
				entry.background.cachedTransform.localScale = new Vector3(vector.x, vector.y, 1f);
				entry.label.cachedTransform.localScale = new Vector3(num4, num4, 1f);
				if (!entry.isHeal && num2 > m_staticDelay && i < m_list.Count - 1 && !m_list[i + 1].active)
				{
					m_list[i + 1].active = true;
					m_list[i + 1].time = Time.timeSinceLevelLoad;
				}
				if (num2 > num)
				{
					if (entry.isHeal && i < m_list.Count - 1)
					{
						m_list[i + 1].active = true;
						m_list[i + 1].time = Time.timeSinceLevelLoad;
					}
					Delete(entry);
				}
				else
				{
					entry.background.enabled = true;
					entry.label.enabled = true;
				}
			}
		}
		for (Int32 j = m_list.Count - 1; j >= 0; j--)
		{
			Entry entry2 = m_list[j];
			if (entry2.active)
			{
				if (m_IsStatic)
				{
					entry2.currentY += entry2.velocity * Time.deltaTime;
					Vector3 zero = Vector3.zero;
					zero.y += entry2.currentY;
					entry2.background.cachedTransform.localPosition = zero;
					zero.z = -1f;
					entry2.label.cachedTransform.localPosition = zero;
				}
				else
				{
					Vector3 vector2 = Vector3.zero;
					vector2 = GameCamera.WorldToViewportPoint(entry2.startPos);
					entry2.label.transform.position = UICamera.ViewportToWorldPoint(vector2);
					vector2 = entry2.label.transform.localPosition;
					vector2.x = Mathf.RoundToInt(vector2.x);
					vector2.y = Mathf.RoundToInt(vector2.y);
					vector2.z = 0f;
					if (entry2.IsCrit && m_IsFixCritPosition)
					{
						entry2.background.cachedTransform.localPosition = vector2 + gameObject.transform.localPosition + FixCritPostions[entry2.FixCritPosInList];
						vector2.z = -1f;
						entry2.label.cachedTransform.localPosition = vector2 + gameObject.transform.localPosition + FixCritPostions[entry2.FixCritPosInList];
					}
					else
					{
						Single num5 = 3f + entry2.offset;
						Single num6 = timeSinceLevelLoad - entry2.movementStart;
						entry2.currentY += entry2.velocity * Time.deltaTime;
						if (!entry2.isHeal && !entry2.isText)
						{
							entry2.velocity -= 200f * Time.deltaTime;
						}
						Single currentY = entry2.currentY;
						Single num7 = num6 * 30f * entry2.angle;
						entry2.label.cachedTransform.localPosition = new Vector3(vector2.x + num7 * num5 + entry2.offsetX, vector2.y + currentY * num5, -1f);
						entry2.background.cachedTransform.localPosition = new Vector3(vector2.x + num7 * num5 + entry2.offsetX, vector2.y + currentY * num5, 0f);
					}
				}
			}
		}
	}

	protected class Entry
	{
		public Single time;

		public Single stay;

		public Single offset;

		public Int32 val;

		public UILabel label;

		public Boolean IsCrit;

		public UISprite background;

		public Vector2 backgroundSize = Vector2.zero;

		public Single angle;

		public Single DamagePercentage = 1f;

		public Int32 FixCritPosInList;

		public Vector3 startPos;

		public Single velocity;

		public Single currentY;

		public Single offsetX;

		public Boolean active;

		public Boolean isHeal;

		public Boolean isText;

		public Single movementStart => time + stay;
	}
}
