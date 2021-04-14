using UnityEngine;
using System.Collections.Generic;

namespace Destructible2D.Examples
{
	/// <summary>This component stamp all destructible sprites under the finger/mouse when you tap/click or press a key.</summary>
	[HelpURL(D2dHelper.HelpUrlPrefix + "D2dTapToStamp")]
	[AddComponentMenu(D2dHelper.ComponentMenuPrefix + "Tap To Stamp")]
	public class D2dTapToStamp : MonoBehaviour
	{
		public enum HitType
		{
			All,
			First
		}

		class Link : D2dInputManager.Link
		{
			public GameObject Visual;
			public Vector3    VisualScale;
			public float      Scale;
			public float      Twist;
			public float      Cooldown;

			public override void Clear()
			{
				Destroy(Visual);

				Cooldown = 0.0f;
				Visual   = null;
			}
		}

		/// <summary>The controls used to trigger the stamp.</summary>
		public D2dInputManager.Trigger Controls = new D2dInputManager.Trigger { UseFinger = true, UseMouse = true };

		/// <summary>The prefab used to show what the stamp will look like.</summary>
		public GameObject IndicatorPrefab;

		/// <summary>The time in seconds between each spawn.
		/// 0 = Once per click.</summary>
		public float Interval;

		/// <summary>This allows you to change the painting type.</summary>
		public D2dDestructible.PaintType Paint;

		/// <summary>The shape of the stamp.</summary>
		public Texture2D Shape;

		/// <summary>The stamp shape will be multiplied by this.
		/// nWhite = No Change.</summary>
		public Color Color = Color.white;

		/// <summary>The size of the stamp in world space.</summary>
		public Vector2 Size = Vector2.one;

		/// <summary>The scale will multiplied by this minimum random value.</summary>
		public float ScaleMin = 0.75f;

		/// <summary>The scale will multiplied by this maximum random value.</summary>
		public float ScaleMax = 1.25f;

		/// <summary>The angle of the stamp in degrees.</summary>
		public float Angle;

		/// <summary>The angle will offset by this minimum random value.</summary>
		public float TwistMin = 0.0f;

		/// <summary>The angle will offset by this maximum random value.</summary>
		public float TwistMax = 360.0f;

		/// <summary>How many destructibles should be hit?</summary>
		public HitType Hit;

		/// <summary>The destructible sprite layers we want to stamp.</summary>
		public LayerMask Layers = -1;

		/// <summary>The camera used to calculate the spawn point.
		/// None/null = Main Camera.</summary>
		public Camera Camera;

		/// <summary>The Z position in world space this component will use. For normal 2D scenes this should be 0.</summary>
		public float Intercept;

		private List<Link> links = new List<Link>();

		protected virtual void OnEnable()
		{
			D2dInputManager.EnsureThisComponentExists();
		}

		protected virtual void Update()
		{
			// Loop through all fingers + mouse + mouse hover
			foreach (var finger in D2dInputManager.GetFingers(true))
			{
				// Did this finger go down based on the current control settings?
				if (Controls.WentDown(finger) == true)
				{
					var scale = Random.Range(ScaleMin, ScaleMax);
					var twist = Random.Range(TwistMin, TwistMax);

					// Show a stamp indicator, and stamp later?
					if (IndicatorPrefab != null)
					{
						var link = D2dInputManager.Link.Create(ref links, finger);

						link.Visual      = Instantiate(IndicatorPrefab);
						link.VisualScale = link.Visual.transform.localScale;
						link.Scale       = scale;
						link.Twist       = twist;

						link.Visual.SetActive(true);
					}
					// Stamp immediately?
					else
					{
						DoStamp(finger, scale, twist);
					}
				}
			}

			// Loop through all links in reverse so they can be removed
			for (var i = links.Count - 1; i >= 0; i--)
			{
				var link = links[i];

				// Update indicator?
				if (link.Visual != null)
				{
					link.Visual.transform.position      = GetPosition(link.Finger.ScreenPosition);
					link.Visual.transform.localScale    = Vector3.Scale(link.VisualScale, new Vector3(Size.x, Size.y, 1.0f) * link.Scale);
					link.Visual.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, Angle + link.Twist);
				}

				if (Interval > 0.0f)
				{
					link.Cooldown -= Time.deltaTime;

					if (link.Cooldown <= 0.0f)
					{
						link.Cooldown = Interval;

						DoStamp(link.Finger, link.Scale, link.Twist);

						link.Scale = Random.Range(ScaleMin, ScaleMax);
						link.Twist = Random.Range(TwistMin, TwistMax);
					}
				}

				// Did this finger go up based on the current control settings?
				if (Controls.WentUp(link.Finger, true) == true)
				{
					if (Interval <= 0.0f)
					{
						DoStamp(link.Finger, link.Scale, link.Twist);
					}

					// Destroy indicator
					D2dInputManager.Link.ClearAndRemove(links, link);
				}
			}
		}

		private void DoStamp(D2dInputManager.Finger finger, float scale, float twist)
		{
			var position = GetPosition(finger.ScreenPosition);

			// Stamp everything at this point?
			if (Hit == HitType.All)
			{
				D2dStamp.All(Paint, position, Size * scale, Angle + twist, Shape, Color, Layers);
			}

			// Stamp the first thing at this point?
			if (Hit == HitType.First)
			{
				var destructible = default(D2dDestructible);

				if (D2dDestructible.TrySampleThrough(position, ref destructible) == true)
				{
					destructible.Paint(Paint, D2dStamp.CalculateMatrix(position, Size * scale, Angle + twist), Shape, Color);
				}
			}
		}

		private Vector3 GetPosition(Vector2 screenPosition)
		{
			// Make sure the camera exists
			var camera = D2dHelper.GetCamera(Camera);

			if (camera != null)
			{
				return D2dHelper.ScreenToWorldPosition(screenPosition, Intercept, camera);
			}

			return default(Vector3);
		}

		protected virtual void OnDestroy()
		{
			D2dInputManager.Link.ClearAll(links);
		}
	}
}

#if UNITY_EDITOR
namespace Destructible2D.Examples
{
	using UnityEditor;
	using TARGET = D2dTapToStamp;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class D2dTapToStamp_Editor : D2dEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("Controls", "The controls used to trigger the stamp.");
			Draw("IndicatorPrefab", "The prefab used to show what the stamp will look like.");
			Draw("Interval", "The time in seconds between each spawn.\n\n0 = Once per click.");

			Separator();

			Draw("Paint", "This allows you to change the painting type.");
			BeginError(Any(tgts, t => t.Shape == null));
				Draw("Shape", "The shape of the stamp.");
			EndError();
			Draw("Color", "The stamp shape will be multiplied by this.\n\nWhite = No Change.");
			BeginError(Any(tgts, t => t.Size.x == 0.0f || t.Size.y == 0.0f));
				Draw("Size", "The size of the stamp in world space.");
				BeginIndent();
					Draw("ScaleMin", "The scale will multiplied by this minimum random value.");
					Draw("ScaleMax", "The scale will multiplied by this maximum random value.");
				EndIndent();
			EndError();
			Draw("Angle", "The angle of the stamp in degrees.");
			BeginIndent();
				Draw("TwistMin", "The angle will offset by this minimum random value.");
				Draw("TwistMax", "The angle will offset by this maximum random value.");
			EndIndent();

			Separator();

			Draw("Hit", "How many destructibles should be hit?");
			BeginError(Any(tgts, t => t.Layers == 0));
				Draw("Layers", "The destructible sprite layers we want to stamp.");
			EndError();
			Draw("Camera", "The camera used to calculate the spawn point.\n\nNone/null = Main Camera.");
			Draw("Intercept", "The Z position in world space this component will use. For normal 2D scenes this should be 0.");
		}
	}
}
#endif