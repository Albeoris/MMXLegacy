using System;
using UnityEngine;

namespace Legacy.Configuration.Quality
{
	public abstract class QualityConfigurationBase : MonoBehaviour
	{
		public abstract void OnQualityConfigutationChanged();

		protected virtual void Awake()
		{
			OnQualityConfigutationChanged();
		}

		[Serializable]
		public class QualityFloatValue
		{
			public Single LowQualityValue;

			public Single[] IntermediateValues;

			public Single MaxQualityValue;

			private Single CurrentValue;

			public void SetQualityValue(Int32 p_qualityStep)
			{
				if (p_qualityStep == 0)
				{
					CurrentValue = LowQualityValue;
				}
				else if (IntermediateValues != null && p_qualityStep <= IntermediateValues.Length)
				{
					CurrentValue = IntermediateValues[p_qualityStep - 1];
				}
				else
				{
					CurrentValue = MaxQualityValue;
				}
			}

			public Single GetQualityValue()
			{
				return CurrentValue;
			}
		}
	}
}
