using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Destructible2D.Examples
{
	/// <summary>This component constantly stamps the current position, allowing you to make effects like melting.</summary>
	[HelpURL(D2dHelper.HelpUrlPrefix + "D2dRepeatStamp")]
	[AddComponentMenu(D2dHelper.ComponentMenuPrefix + "Repeat Stamp")]
	public class D2dRepeatStamp : MonoBehaviour
	{
		/// <summary>The layers the stamp works on.</summary>
		public LayerMask Layers = -1;

		/// <summary>This allows you to change the painting type.</summary>
		public D2dDestructible.PaintType Paint;

		/// <summary>The shape of the stamp.</summary>
		public Texture2D Shape;

		/// <summary>The stamp shape will be multiplied by this.
		/// Solid White = No Change</summary>
		public Color Color = Color.white;

		/// <summary>The size of the stamp in world space.</summary>
		public Vector2 Size = Vector2.one;

		/// <summary>The delay between each repeat stamp.</summary>
		public float Delay = 0.25f;

		private float cooldown;

		protected virtual void Update()
		{
			cooldown -= Time.deltaTime;

			if (cooldown <= 0.0f)
			{
				cooldown = Delay;

				var angle = Random.Range(0.0f, 360.0f);

				D2dStamp.All(Paint, transform.position, Size, angle, Shape, Color, Layers);
			}
		}
	}
}

#if UNITY_EDITOR
namespace Destructible2D.Examples
{
	using UnityEditor;
	using TARGET = D2dRepeatStamp;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class D2dRepeatStamp_Editor : D2dEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("Layers", "The layers the stamp works on.");

			Separator();

			Draw("Paint", "This allows you to change the painting type.");
			BeginError(Any(tgts, t => t.Shape == null));
				Draw("Shape", "The shape of the stamp.");
			EndError();
			Draw("Color", "The stamp shape will be multiplied by this.\nSolid White = No Change");
			Draw("Size", "The size of the stamp in world space.");
			Draw("Delay", "The delay between each repeat stamp.");
		}
	}
}
#endif