using System;

namespace Legacy.Core.Entities
{
	public abstract class BaseObject
	{
		private EObjectType m_type;

		private Int32 m_staticId;

		private Int32 m_spawnerID;

		public BaseObject()
		{
		}

		public BaseObject(Int32 p_staticID, EObjectType p_type, Int32 p_spawnerID)
		{
			m_type = p_type;
			InitBaseObject(p_staticID, p_spawnerID);
		}

		public EObjectType Type => m_type;

	    public Int32 StaticID => m_staticId;

	    public Int32 SpawnerID => m_spawnerID;

	    public Boolean IsDestroyed { get; private set; }

		public virtual void Destroy()
		{
			m_spawnerID = 0;
			m_staticId = 0;
			IsDestroyed = true;
		}

		protected void InitBaseObject(Int32 p_staticID, Int32 p_spawnerID)
		{
			m_staticId = p_staticID;
			m_spawnerID = p_spawnerID;
			LoadStaticData();
		}

		protected virtual void LoadStaticData()
		{
		}
	}
}
