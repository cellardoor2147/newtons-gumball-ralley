using UnityEngine;
using UnityEngine.EventSystems;

namespace Destructible2D.Examples
{
	/// <summary>This component turns the current UI element into a button that links to the specified action.</summary>
	[ExecuteInEditMode]
	[HelpURL(D2dHelper.HelpUrlPrefix + "D2dLinkTo")]
	[AddComponentMenu(D2dHelper.ComponentMenuPrefix + "Link To")]
	public class D2dLinkTo : MonoBehaviour, IPointerClickHandler
	{
		public enum LinkType
		{
			Publisher,
			PreviousScene,
			NextScene
		}

		/// <summary>The action that will be performed when this UI element is clicked.</summary>
		public LinkType Link { set { link = value; } get { return link; } } [SerializeField] private LinkType link;

		protected virtual void Update()
		{
			switch (link)
			{
				case LinkType.PreviousScene:
				case LinkType.NextScene:
				{
					var group = GetComponent<CanvasGroup>();

					if (group != null)
					{
						var show = GetCurrentLevel() >= 0 && GetLevelCount() > 1;

						group.alpha          = show == true ? 1.0f : 0.0f;
						group.blocksRaycasts = show;
						group.interactable   = show;
					}
				}
				break;
			}
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			switch (link)
			{
				case LinkType.Publisher:
				{
					Application.OpenURL("http://carloswilkes.com");
				}
				break;

				case LinkType.PreviousScene:
				{
					var index = GetCurrentLevel();

					if (index >= 0)
					{
						if (--index < 0)
						{
							index = GetLevelCount() - 1;
						}

						LoadLevel(index);
					}
				}
				break;

				case LinkType.NextScene:
				{
					var index = GetCurrentLevel();

					if (index >= 0)
					{
						if (++index >= GetLevelCount())
						{
							index = 0;
						}

						LoadLevel(index);
					}
				}
				break;
			}
		}

		private static int GetCurrentLevel()
		{
			var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
			var index = scene.buildIndex;

			if (index >= 0)
			{
				if (UnityEngine.SceneManagement.SceneManager.GetSceneByBuildIndex(index).path != scene.path)
				{
					return -1;
				}
			}

			return index;
		}

		private static int GetLevelCount()
		{
			return UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;
		}

		private static void LoadLevel(int index)
		{
			UnityEngine.SceneManagement.SceneManager.LoadScene(index);
		}
	}
}

#if UNITY_EDITOR
namespace Destructible2D.Examples
{
	using UnityEditor;
	using TARGET = D2dLinkTo;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class D2dLinkTo_Editor : D2dEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("link", "The action that will be performed when this UI element is clicked.");
		}
	}
}
#endif