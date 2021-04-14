using UnityEngine;
using System.Collections.Generic;

namespace Destructible2D.Examples
{
	/// <summary>This component spawns and throws a prefab when you click and drag across the screen.</summary>
	[HelpURL(D2dHelper.HelpUrlPrefix + "D2dDragToThrow")]
	[AddComponentMenu(D2dHelper.ComponentMenuPrefix + "Drag To Throw")]
	public class D2dDragToThrow : MonoBehaviour
	{
		class Link : D2dInputManager.Link
		{
			public Vector3 Start;

			public GameObject Visual;

			public override void Clear()
			{
				Destroy(Visual);
			}
		}

		/// <summary>The controls used to trigger the slice.</summary>
		public D2dInputManager.Trigger Controls = new D2dInputManager.Trigger { UseFinger = true, UseMouse = true };

		/// <summary>The Z position in world space this component will use. For normal 2D scenes this should be 0.</summary>
		public float Intercept;

		/// <summary>The prefab used to show what the slice will look like.</summary>
		public GameObject IndicatorPrefab;

		/// <summary>The scale of the throw indicator.</summary>
		public float Scale = 1.0f;

		/// <summary>The minimum distance the throw is calculated using in world space.
		/// 0 = Unlimited.</summary>
		public float DistanceMin;

		/// <summary>The maximum distance the throw is calculated using in world space.
		/// 0 = Unlimited.</summary>
		public float DistanceMax;

		/// <summary>The prefab that gets thrown.</summary>
		public GameObject ProjectilePrefab;

		/// <summary>How fast the projectile will be launched.</summary>
		public float ProjectileSpeed = 10.0f;

		/// <summary>How much spread is added to the project when fired.</summary>
		public float ProjectileSpread;

		/// <summary>The projectile will be rotated by this angle in degrees.</summary>
		public float ProjectileAngle;

		[SerializeField]
		private List<Link> links = new List<Link>();

		protected virtual void OnEnable()
		{
			D2dInputManager.EnsureThisComponentExists();
		}

		private float GetAngleAndClampCurrentPos(Vector3 startPos, ref Vector3 currentPos)
		{
			if (startPos != currentPos)
			{
				var distance = Vector3.Distance(currentPos, startPos);

				if (DistanceMin > 0.0f && distance < DistanceMin)
				{
					distance = DistanceMin;
				}

				if (DistanceMax > 0.0f && distance > DistanceMax)
				{
					distance = DistanceMax;
				}

				currentPos = startPos + (currentPos - startPos).normalized * distance;
			}

			return D2dHelper.Atan2(currentPos - startPos) * Mathf.Rad2Deg;
		}

		protected virtual void Update()
		{
			// Loop through all fingers + mouse + mouse hover
			foreach (var finger in D2dInputManager.GetFingers(true))
			{
				// Did this finger go down based on the current control settings?
				if (Controls.WentDown(finger) == true)
				{
					// Create a link with this finger and additional data
					var link = D2dInputManager.Link.Create(ref links, finger);

					// Create an indicator for this link?
					if (IndicatorPrefab != null)
					{
						link.Visual = Instantiate(IndicatorPrefab);

						link.Visual.SetActive(true);
					}

					link.Start = GetPosition(finger.ScreenPosition);
				}
			}

			// Loop through all links in reverse so they can be removed
			for (var i = links.Count - 1; i >= 0; i--)
			{
				var link     = links[i];
				var position = GetPosition(link.Finger.ScreenPosition);

				// Update indicator?
				if (link.Visual != null)
				{
					var angle = GetAngleAndClampCurrentPos(link.Start, ref position);
					var scale = Vector3.Distance(position, link.Start) * Scale;

					link.Visual.transform.position   = link.Start;
					link.Visual.transform.rotation   = Quaternion.Euler(0.0f, 0.0f, -angle);
					link.Visual.transform.localScale = new Vector3(scale, scale, scale);
				}

				// Did this finger go up based on the current control settings?
				if (Controls.WentUp(link.Finger, true) == true)
				{
					// Spawn
					var angle      = GetAngleAndClampCurrentPos(link.Start, ref position) + ProjectileAngle + Random.Range(-ProjectileSpread, ProjectileSpread);
					var projectile = Instantiate(ProjectilePrefab, link.Start, Quaternion.Euler(0.0f, 0.0f, -angle));

					projectile.SetActive(true);

					// Apply velocity?
					var rigidbody2D = projectile.GetComponent<Rigidbody2D>();

					if (rigidbody2D != null)
					{
						rigidbody2D.velocity = (position - link.Start) * ProjectileSpeed;
					}

					// Destroy indicator
					D2dInputManager.Link.ClearAndRemove(links, link);
				}
			}
		}

		private Vector3 GetPosition(Vector2 screenPosition)
		{
			// Make sure the camera exists
			var camera = D2dHelper.GetCamera(null);

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
	using TARGET = D2dDragToThrow;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class D2dDragToThrow_Editor : D2dEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("Controls", "The controls used to trigger the stamp.");
			Draw("Intercept", "The Z position in world space this component will use. For normal 2D scenes this should be 0.");
			BeginError(Any(tgts, t => t.IndicatorPrefab == null));
				Draw("IndicatorPrefab", "The prefab used to show what the slice will look like.");
			EndError();
			Draw("Scale", "The scale of the throw indicator.");
			Draw("DistanceMin", "The minimum distance the throw is calculated using in world space.\n\n0 = Unlimited.");
			Draw("DistanceMax", "The maximum distance the throw is calculated using in world space.\n\n0 = Unlimited.");

			Separator();

			BeginError(Any(tgts, t => t.ProjectilePrefab == null));
				Draw("ProjectilePrefab", "The prefab that gets thrown.");
			EndError();
			Draw("ProjectileSpeed", "How fast the projectile will be launched.");
			Draw("ProjectileSpread", "How much spread is added to the project when fired.");
			Draw("ProjectileAngle", "The projectile will be rotated by this angle in degrees.");
		}
	}
}
#endif