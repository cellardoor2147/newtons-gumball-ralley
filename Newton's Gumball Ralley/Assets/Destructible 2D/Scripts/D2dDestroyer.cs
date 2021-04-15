using UnityEngine;

namespace Destructible2D
{
	/// <summary>This component will automatically destroy the current GameObject after a certain amount of time.</summary>
	[HelpURL(D2dHelper.HelpUrlPrefix + "D2dDestroyer")]
	[AddComponentMenu(D2dHelper.ComponentMenuPrefix + "Destroyer")]
	public class D2dDestroyer : MonoBehaviour
	{
		/// <summary>This will decrease by 1 every second, and the current GameObject will be destroyed when it reaches 0.</summary>
		public float Life { set { life = value; } get { return life; } } [SerializeField] private float life = 1.0f;

		/// <summary>If you want a different GameObject to be destroyed then specify it here.</summary>
		public GameObject Target { set { target = value; } get { return target; } } [SerializeField] private GameObject target;

		/// <summary>If you enable this then the attached SpriteRenderer.color will be faded out before destruction.</summary>
		public bool Fade { set { fade = value; } get { return fade; } } [SerializeField] private bool fade;

		/// <summary>The amount of seconds the fade out effect spans.</summary>
		public float FadeDuration { set { fadeDuration = value; } get { return fadeDuration; } } [SerializeField] private float fadeDuration = 1.0f;

		/// <summary>If you enable this then the Transform.localScale value will shrink to 0 before destruction.</summary>
		public bool Shrink { set { shrink = value; } get { return shrink; } } [SerializeField] private bool shrink;

		/// <summary>The amount of seconds the shrink effect spans.</summary>
		public float ShrinkDuration { set { shrinkDuration = value; } get { return shrinkDuration; } } [SerializeField] private float shrinkDuration = 1.0f;

		/// <summary>Should these settings get randomized when this component is enabled?</summary>
		public bool RandomizeOnEnable { set { randomizeOnEnable = value; } get { return randomizeOnEnable; } } [SerializeField] private bool randomizeOnEnable;

		/// <summary>The minimum randomized Life value.</summary>
		public float LifeMin { set { lifeMin = value; } get { return lifeMin; } } [SerializeField] private float lifeMin = 0.5f;

		/// <summary>The minimum randomized Life value.</summary>
		public float LifeMax { set { lifeMax = value; } get { return lifeMax; } } [SerializeField] private float lifeMax = 1.0f;

		[SerializeField]
		private Color startColor;

		[SerializeField]
		private Vector3 startLocalScale;

		[System.NonSerialized]
		private SpriteRenderer spriteRenderer;

		protected virtual void OnEnable()
		{
			if (RandomizeOnEnable == true)
			{
				Life = Random.Range(LifeMin, LifeMax);
			}
		}

		protected virtual void Update()
		{
			Life -= Time.deltaTime;

			if (Life > 0.0f)
			{
				if (Fade == true)
				{
					UpdateFade();
				}

				if (Shrink == true)
				{
					UpdateShrink();
				}
			}
			else if (target != null)
			{
				D2dHelper.Destroy(target);
			}
			else
			{
				D2dHelper.Destroy(gameObject);
			}
		}

		private void UpdateFade()
		{
			if (FadeDuration > 0.0f)
			{
				if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();

				if (spriteRenderer != null)
				{
					if (FadeDuration > 0.0f && Life < FadeDuration)
					{
						if (startColor == default(Color))
						{
							startColor = spriteRenderer.color;
						}

						var finalColor = startColor;

						finalColor.a *= Life / FadeDuration;

						spriteRenderer.color = finalColor;
					}
				}
			}
		}

		private void UpdateShrink()
		{
			if (ShrinkDuration > 0.0f)
			{
				if (startLocalScale == default(Vector3))
				{
					startLocalScale = transform.localScale;
				}

				// Setting a zero scale might cause issues, so don't
				if (startLocalScale != Vector3.zero)
				{
					var finalScale = startLocalScale;

					finalScale *= Life / FadeDuration;

					transform.localScale = finalScale;
				}
			}
		}
	}
}

#if UNITY_EDITOR
namespace Destructible2D.Inspector
{
	using UnityEditor;
	using TARGET = D2dDestroyer;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class D2dDestroyer_Editor : D2dEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			BeginError(Any(tgts, t => t.Life < 0.0f));
				Draw("life", "This will decrease by 1 every second, and the current GameObject will be destroyed when it reaches 0.");
			EndError();
			Draw("target", "If you want a different GameObject to be destroyed then specify it here.");

			Separator();

			Draw("fade", "If you enable this then the attached SpriteRenderer.color will be faded out before destruction.");

			if (Any(tgts, t => t.Fade == true))
			{
				if (Any(tgts, t => t.GetComponent<SpriteRenderer>() == null))
				{
					Warning("There is no SpriteRenderer on this GameObject, so you cannot fade out.");
				}
				BeginIndent();
					BeginError(Any(tgts, t => t.FadeDuration <= 0.0f));
						Draw("fadeDuration", "The amount of seconds the fade out effect spans.");
					EndError();
				EndIndent();
			}

			Separator();

			Draw("shrink", "If you enable this then the Transform.localScale value will shrink to 0 before destruction.");

			if (Any(tgts, t => t.Shrink == true))
			{
				BeginIndent();
					BeginError(Any(tgts, t => t.ShrinkDuration <= 0.0f));
						Draw("shrinkDuration", "The amount of seconds the shrink effect spans.");
					EndError();
				EndIndent();
			}

			Separator();

			Draw("randomizeOnEnable", "Should these settings get randomized when this component is enabled?");

			if (Any(tgts, t => t.RandomizeOnEnable == true))
			{
				BeginIndent();
					BeginError(Any(tgts, t => t.LifeMin < 0.0f || t.LifeMin > t.LifeMax));
						Draw("lifeMin", "The minimum randomized Life value.");
					EndError();

					BeginError(Any(tgts, t => t.LifeMax < 0.0f || t.LifeMin > t.LifeMax));
						Draw("lifeMax", "The minimum randomized Life value.");
					EndError();
				EndIndent();
			}
		}
	}
}
#endif