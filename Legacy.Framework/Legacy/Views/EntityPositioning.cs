using System;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.Map;
using UnityEngine;

namespace Legacy.Views
{
	public class EntityPositioning
	{
		private PositioningTemplate m_Template;

		private Entity[] m_GameObjects;

		private GridSlot m_gridSlot;

		internal GameObject m_SlotOrigin;

		private Position m_lastPartyPos;

		private Quaternion m_rotation = Quaternion.identity;

		private EDirection m_direction;

		private Boolean m_diagonal;

		private Int32 m_count;

		internal EntityPositioning(GridSlot p_slot, PositioningTemplate p_template)
		{
			m_gridSlot = p_slot;
			m_Template = p_template;
		}

		public EDirection Direction => m_direction;

	    public Position Position => m_gridSlot.Position;

	    public Boolean AddView(IMoveable go, ESize size, out Vector3 localPosition, Int32 p_comeFrom, EntityPositioning p_oldPositioning)
		{
			localPosition = Vector3.zero;
			if (go == null)
			{
				return false;
			}
			if (m_GameObjects == null)
			{
				m_GameObjects = new Entity[m_Template.Length];
			}
			if (p_comeFrom != -1)
			{
				CalculateRotation();
				Boolean flag = AddViewWithPreviousSlot(go, size, out localPosition, p_comeFrom, p_oldPositioning);
				if (flag)
				{
					return true;
				}
			}
			else if (go is LevelEntityView)
			{
				LevelEntityView levelEntityView = (LevelEntityView)go;
				EDirection direction = levelEntityView.MyController.Direction;
				SetDirection(direction);
			}
			Int32 num = -1;
			for (Int32 i = 0; i < m_GameObjects.Length; i++)
			{
				if (size != ESize.MEDIUM || i != 1)
				{
					if (size != ESize.BIG || i == 1)
					{
						if (num == -1 && m_GameObjects[i].View == null)
						{
							num = i;
						}
						if (m_GameObjects[i].View == go)
						{
							num = i;
							break;
						}
					}
				}
			}
			if (num > -1)
			{
				if (m_count == 0)
				{
					num = 1;
				}
				Single num2 = (Single)m_direction * 90f;
				if (m_diagonal)
				{
					num2 -= 45f;
				}
				localPosition = SetViewIntoSlot(go, num, num);
				return true;
			}
			localPosition = Vector3.zero;
			return false;
		}

		private Boolean AddViewWithPreviousSlot(IMoveable go, ESize size, out Vector3 localPosition, Int32 p_comeFrom, EntityPositioning p_oldPositioning)
		{
			Boolean searchNearestSlot = false;
			if (p_oldPositioning != null)
			{
				EDirection direction = EDirectionFunctions.GetDirection(p_oldPositioning.Position, m_gridSlot.Position);
				if (direction != m_direction && direction != EDirectionFunctions.Add(m_direction, 2))
				{
					searchNearestSlot = true;
				}
			}
			Boolean flag;
			if (size == ESize.MEDIUM)
			{
				flag = AddViewWithPreviousSlotMedium(go, out localPosition, p_comeFrom, p_oldPositioning, searchNearestSlot);
			}
			else if (size == ESize.BIG)
			{
				flag = AddViewWithPreviousSlotBig(go, out localPosition, p_comeFrom);
			}
			else
			{
				flag = AddViewWithPreviousSlotSmall(go, out localPosition, p_comeFrom, p_oldPositioning, searchNearestSlot);
			}
			return !flag;
		}

		private Boolean AddViewWithPreviousSlotMedium(IMoveable go, out Vector3 localPosition, Int32 p_comeFrom, EntityPositioning p_oldPositioning, Boolean searchNearestSlot)
		{
			Boolean result = false;
			localPosition = Vector3.zero;
			if (!searchNearestSlot && m_count == 0)
			{
				localPosition = SetViewIntoSlot(go, 0, p_comeFrom);
			}
			else if (m_count == 0)
			{
				Int32 p_comeFrom2 = 1;
				localPosition = SetViewIntoSlot(go, p_comeFrom2, p_comeFrom);
			}
			else if (m_count == 1)
			{
				if (m_GameObjects[1].View != null)
				{
					if (p_comeFrom == 2)
					{
						AlterPositionForView(1, 0);
					}
					else
					{
						AlterPositionForView(1, 2);
					}
					if (p_comeFrom != 1)
					{
						localPosition = SetViewIntoSlot(go, p_comeFrom, p_comeFrom);
					}
					else
					{
						localPosition = SetViewIntoSlot(go, p_comeFrom, 0);
					}
				}
				else
				{
					result = true;
				}
			}
			return result;
		}

		private Boolean AddViewWithPreviousSlotBig(IMoveable go, out Vector3 localPosition, Int32 p_comeFrom)
		{
			localPosition = SetViewIntoSlot(go, p_comeFrom, 1);
			return false;
		}

		private Boolean AddViewWithPreviousSlotSmall(IMoveable go, out Vector3 localPosition, Int32 p_comeFrom, EntityPositioning p_oldPositioning, Boolean searchNearestSlot)
		{
			if (m_count == 0)
			{
				AddViewWithPreviousSlotSmallSingle(go, out localPosition, p_comeFrom, p_oldPositioning, searchNearestSlot);
			}
			else if (m_count == 1)
			{
				AddViewWithPreviousSlotSmallDuo(go, out localPosition, p_comeFrom, p_oldPositioning, searchNearestSlot);
			}
			else if (searchNearestSlot)
			{
				AddViewWithPreviousSlotSmallTrioSearchNearest(go, out localPosition, p_comeFrom, p_oldPositioning);
			}
			else
			{
				AddViewWithPreviousSlotSmallTrio(go, out localPosition, p_comeFrom, p_oldPositioning);
			}
			return false;
		}

		private void AddViewWithPreviousSlotSmallSingle(IMoveable go, out Vector3 localPosition, Int32 p_comeFrom, EntityPositioning p_oldPositioning, Boolean searchNearestSlot)
		{
			if (!searchNearestSlot)
			{
				localPosition = SetViewIntoSlot(go, p_comeFrom, 1);
			}
			else if (m_diagonal)
			{
				localPosition = SetViewIntoSlot(go, p_comeFrom, p_comeFrom);
			}
			else
			{
				Int32 p_slot = 1;
				localPosition = SetViewIntoSlot(go, p_comeFrom, p_slot);
			}
		}

		private void AddViewWithPreviousSlotSmallDuo(IMoveable go, out Vector3 localPosition, Int32 p_comeFrom, EntityPositioning p_oldPositioning, Boolean searchNearestSlot)
		{
			if (!searchNearestSlot)
			{
				if (m_GameObjects[1].View != null)
				{
					if (m_GameObjects[1].TargetIndex == p_comeFrom)
					{
						Int32 p_slot = -1;
						for (Int32 i = 0; i < m_GameObjects.Length; i++)
						{
							if (m_GameObjects[i].View == null)
							{
								p_slot = i;
							}
						}
						localPosition = SetViewIntoSlot(go, p_comeFrom, p_slot);
					}
					else if (m_GameObjects[m_GameObjects[1].TargetIndex].View == null)
					{
						AlterPositionForView(1, m_GameObjects[1].TargetIndex);
						localPosition = SetViewIntoSlot(go, p_comeFrom, 1);
					}
					else
					{
						for (Int32 j = 0; j < m_GameObjects.Length; j++)
						{
							if (m_GameObjects[j].View == null)
							{
								AlterPositionForView(p_comeFrom, j);
							}
						}
						localPosition = SetViewIntoSlot(go, p_comeFrom, p_comeFrom);
					}
				}
				else
				{
					localPosition = SetViewIntoSlot(go, p_comeFrom, p_comeFrom);
				}
			}
			else if (m_diagonal)
			{
				if (m_GameObjects[p_comeFrom].View == null)
				{
					localPosition = SetViewIntoSlot(go, p_comeFrom, p_comeFrom);
				}
				else
				{
					Int32 p_newSlot = -1;
					for (Int32 k = 0; k < m_GameObjects.Length; k++)
					{
						if (m_GameObjects[k].View == null)
						{
							p_newSlot = k;
							break;
						}
					}
					AlterPositionForView(p_comeFrom, p_newSlot);
					localPosition = SetViewIntoSlot(go, p_comeFrom, p_comeFrom);
				}
			}
			else
			{
				Int32 num = GetNearestSlotByDirection(p_oldPositioning, p_comeFrom);
				if (m_GameObjects[num].View != null)
				{
					if (m_GameObjects[num].TargetIndex != num)
					{
						AlterPositionForView(num, 1);
					}
					else
					{
						num = 1;
					}
				}
				localPosition = SetViewIntoSlot(go, p_comeFrom, num);
			}
		}

		private void AddViewWithPreviousSlotSmallTrio(IMoveable go, out Vector3 localPosition, Int32 p_comeFrom, EntityPositioning p_oldPositioning)
		{
			if (m_GameObjects[1].TargetIndex == 1)
			{
				Int32 num = -1;
				if (p_comeFrom == 1)
				{
					num = ((m_GameObjects[1].View == null) ? 1 : -1);
					if (num == -1)
					{
						num = ((m_GameObjects[0].View == null) ? 0 : 2);
					}
				}
				else if (m_GameObjects[p_comeFrom].View == null)
				{
					num = p_comeFrom;
				}
				else
				{
					for (Int32 i = 0; i < m_GameObjects.Length; i++)
					{
						if (m_GameObjects[i].View == null)
						{
							num = i;
						}
					}
				}
				localPosition = SetViewIntoSlot(go, p_comeFrom, num);
			}
			else if (!m_diagonal)
			{
				if (m_GameObjects[p_comeFrom].View != null)
				{
					Int32 num2 = m_GameObjects[p_comeFrom].TargetIndex;
					if (m_GameObjects[num2].View != null)
					{
						for (Int32 j = 0; j < m_GameObjects.Length; j++)
						{
							if (m_GameObjects[j].View == null)
							{
								num2 = j;
							}
						}
					}
					else
					{
						num2 = p_comeFrom;
						AlterPositionForView(p_comeFrom, m_GameObjects[p_comeFrom].TargetIndex);
					}
					localPosition = SetViewIntoSlot(go, p_comeFrom, num2);
				}
				else
				{
					Int32 targetIndex = m_GameObjects[1].TargetIndex;
					AlterPositionForView(1, targetIndex);
					localPosition = SetViewIntoSlot(go, p_comeFrom, 1);
				}
			}
			else if (m_GameObjects[p_comeFrom].View == null)
			{
				localPosition = SetViewIntoSlot(go, p_comeFrom, p_comeFrom);
			}
			else
			{
				Int32 p_newSlot = -1;
				for (Int32 k = 0; k < m_GameObjects.Length; k++)
				{
					if (m_GameObjects[k].View == null)
					{
						p_newSlot = k;
						break;
					}
				}
				AlterPositionForView(p_comeFrom, p_newSlot);
				localPosition = SetViewIntoSlot(go, p_comeFrom, p_comeFrom);
			}
		}

		private void AddViewWithPreviousSlotSmallTrioSearchNearest(IMoveable go, out Vector3 localPosition, Int32 p_comeFrom, EntityPositioning p_oldPositioning)
		{
			if (m_diagonal)
			{
				localPosition = SetViewIntoSlot(go, p_comeFrom, p_comeFrom);
			}
			else
			{
				Int32 num = GetNearestSlotByDirection(p_oldPositioning, p_comeFrom);
				if (m_GameObjects[num].View != null)
				{
					if (num == 0)
					{
						if (m_GameObjects[1].View != null && m_GameObjects[1].TargetIndex != 1)
						{
							AlterPositionForView(1, 2);
						}
						if (m_GameObjects[1].View == null)
						{
							AlterPositionForView(0, 1);
						}
						else if (m_GameObjects[0].TargetIndex != 0)
						{
							AlterPositionForView(0, 2);
						}
						else
						{
							for (Int32 i = 0; i < m_GameObjects.Length; i++)
							{
								if (m_GameObjects[i].View == null)
								{
									num = i;
									break;
								}
							}
						}
					}
					else if (num == 2)
					{
						if (m_GameObjects[1].View != null && m_GameObjects[1].TargetIndex != 1)
						{
							AlterPositionForView(1, 0);
						}
						if (m_GameObjects[1].View == null)
						{
							AlterPositionForView(2, 1);
						}
						else if (m_GameObjects[2].TargetIndex != 2)
						{
							AlterPositionForView(2, 0);
						}
						else
						{
							for (Int32 j = 0; j < m_GameObjects.Length; j++)
							{
								if (m_GameObjects[j].View == null)
								{
									num = j;
									break;
								}
							}
						}
					}
				}
				localPosition = SetViewIntoSlot(go, p_comeFrom, num);
			}
		}

		public void RemoveView(IMoveable p_view)
		{
			Int32 slotIndex = GetSlotIndex(p_view);
			if (slotIndex != -1)
			{
				m_GameObjects[slotIndex] = default(Entity);
				UpdateCount();
			}
		}

		public void UpdateCount()
		{
			m_count = 0;
			for (Int32 i = 0; i < m_GameObjects.Length; i++)
			{
				if (m_GameObjects[i].View != null)
				{
					m_count++;
				}
			}
		}

		public Int32 GetSlotIndex(IMoveable p_view)
		{
			if (m_GameObjects != null && p_view != null)
			{
				for (Int32 i = 0; i < m_GameObjects.Length; i++)
				{
					if (m_GameObjects[i].View == p_view)
					{
						return i;
					}
				}
			}
			return -1;
		}

		private Vector3 SetViewIntoSlot(IMoveable p_view, Int32 p_comeFrom, Int32 p_slot)
		{
			m_GameObjects[p_slot] = new Entity(p_view, p_comeFrom);
			UpdateCount();
			return m_rotation * m_Template[p_slot];
		}

		private void AlterPositionForView(Int32 p_oldSlot, Int32 p_newSlot)
		{
			m_GameObjects[p_newSlot] = new Entity(m_GameObjects[p_oldSlot]);
			Vector3 vector = m_rotation * m_Template[p_newSlot];
			vector += Helper.SlotLocalPosition(m_gridSlot.Position, m_gridSlot.Height);
			if (m_GameObjects[p_newSlot].View != null)
			{
				m_GameObjects[p_newSlot].View.Move(vector);
			}
			m_GameObjects[p_oldSlot] = default(Entity);
		}

		private Int32 GetNearestSlotByDirection(EntityPositioning p_otherPositioning, Int32 p_fromSlot)
		{
			if (p_otherPositioning.Direction == m_direction)
			{
				Position position = p_otherPositioning.Position;
				Position position2 = Position;
				EDirection edirection = EDirectionFunctions.Add(m_direction, -(Int32)EDirectionFunctions.GetDirection(ref position2, ref position));
				if (edirection == EDirection.EAST)
				{
					return 2;
				}
				if (edirection == EDirection.WEST)
				{
					return 0;
				}
			}
			else if (p_otherPositioning.Direction == EDirectionFunctions.Add(m_direction, 2))
			{
				return p_fromSlot;
			}
			return 0;
		}

		private void SetDirection(EDirection p_dir)
		{
			m_rotation = Helper.GridDirectionToQuaternion(p_dir);
			m_diagonal = false;
		}

		private void CalculateRotation()
		{
			Position position = LegacyLogic.Instance.WorldManager.Party.Position;
			if (m_lastPartyPos == position)
			{
				return;
			}
			m_lastPartyPos = position;
			Position position2 = m_gridSlot.Position;
			EDirection edirection = EDirectionFunctions.GetDirection(ref position2, ref position);
			if (edirection == EDirection.COUNT)
			{
				edirection = EDirection.NORTH;
			}
			m_direction = edirection;
			Position position3 = position - m_gridSlot.Position;
			Quaternion quaternion = Helper.GridDirectionToQuaternion(edirection);
			if (Mathf.Abs(position3.X) == Mathf.Abs(position3.Y))
			{
				Single angle;
				if (position3.Y > 0)
				{
					angle = ((position3.X >= 0) ? 45f : -45f);
				}
				else
				{
					angle = ((position3.X >= 0) ? -45f : 45f);
				}
				quaternion *= Quaternion.AngleAxis(angle, Vector3.up);
				m_diagonal = true;
			}
			else
			{
				m_diagonal = false;
			}
			m_rotation = quaternion;
		}

		private struct Entity
		{
			public readonly IMoveable View;

			public readonly Int32 TargetIndex;

			public Entity(Entity other)
			{
				View = other.View;
				TargetIndex = other.TargetIndex;
			}

			public Entity(IMoveable view, Int32 targetIndex)
			{
				View = view;
				TargetIndex = targetIndex;
			}
		}
	}
}
