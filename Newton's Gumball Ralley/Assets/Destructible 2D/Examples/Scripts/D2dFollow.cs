using UnityEngine;

namespace Destructible2D.Examples
{
	/// <summary>This component causes the current GameObject to follow the target Transform.</summary>
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	[HelpURL(D2dHelper.HelpUrlPrefix + "D2dFollow")]
	[AddComponentMenu(D2dHelper.ComponentMenuPrefix + "Follow")]
	public class D2dFollow : MonoBehaviour
	{
		/// <summary>The target object you want this GameObject to follow.</summary>
		public Transform Target;

		public void UpdatePosition()
		{
			if (Target != null)
			{
				var position = transform.position;

				position.x = Target.position.x;
				position.y = Target.position.y;

				transform.position = position;
			}
		}

		protected virtual void Update()
		{
			UpdatePosition();
		}
	}
}

#if UNITY_EDITOR
namespace Destructible2D.Examples
{
	using UnityEditor;
	using TARGET = D2dFollow;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class D2dFollow_Editor : D2dEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			BeginError(Any(tgts, t => t.Target == null));
				Draw("Target", "The target object you want this GameObject to follow.");
			EndError();
		}
	}
}
#endif