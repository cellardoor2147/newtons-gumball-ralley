using UnityEngine;
using System.Collections.Generic;
using FSA = UnityEngine.Serialization.FormerlySerializedAsAttribute;

namespace Destructible2D.Examples
{
	/// <summary>This component allows you to add a camera flash effect using a full screen UI element that fades in and out.</summary>
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(CanvasGroup))]
	[HelpURL(D2dHelper.HelpUrlPrefix + "D2dCameraFlash")]
	[AddComponentMenu(D2dHelper.ComponentMenuPrefix + "Camera Flash")]
	public class D2dCameraFlash : MonoBehaviour
	{
		/// <summary>All active and enabled D2dCameraFlash instances in the scene.</summary>
		public static List<D2dCameraFlash> Instances = new List<D2dCameraFlash>();

		/// <summary>The current flash strength. This gets reduced automatically.</summary>
		public float Flash;

		/// <summary>The speed at which the Flash value gets reduced.</summary>
		[FSA("FlashDampening")] public float FlashDamping = 10.0f;

		[System.NonSerialized]
		private CanvasGroup cachedCanvasGroup;

		protected virtual void OnEnable()
		{
			Instances.Add(this);

			if (cachedCanvasGroup == null) cachedCanvasGroup = GetComponent<CanvasGroup>();
		}

		protected virtual void OnDisable()
		{
			Instances.Remove(this);
		}

		protected virtual void LateUpdate()
		{
			if (Application.isPlaying == true)
			{
				var factor = D2dHelper.DampenFactor(FlashDamping, Time.deltaTime, 0.1f);

				Flash = Mathf.Lerp(Flash, 0.0f, factor);
			}

			cachedCanvasGroup.alpha = Flash > 0.005f ? Flash : 0.0f;
		}
	}
}

#if UNITY_EDITOR
namespace Destructible2D.Examples
{
	using UnityEditor;
	using TARGET = D2dCameraFlash;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class D2dCameraFlash_Editor : D2dEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("Flash", "The current flash strength. This gets reduced automatically.");
			Draw("FlashDamping", "The speed at which the Flash value gets reduced.");
		}
	}
}
#endif