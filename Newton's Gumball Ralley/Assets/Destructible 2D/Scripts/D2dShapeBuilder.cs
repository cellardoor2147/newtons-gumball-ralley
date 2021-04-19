using UnityEngine;

namespace Destructible2D
{
	/// <summary>This tool allows you to create shape textures that can be used to destroy sprites with scorch marks. These textures are normally difficult to create, so this tool allows you to combine two separate textures to make one.</summary>
	[CreateAssetMenu(fileName = "New Shape", menuName = "Rendering/Shape Builder (Destructible 2D)")]
	public class D2dShapeBuilder : ScriptableObject
	{
		/// <summary>This allows you to specify the texture used to build the RGB information, which is used to make scorch marks, where a color of white means there is no scorch mark.</summary>
		public Texture2D ColorTexture { set { colorTexture = value; } get { return colorTexture; } } [SerializeField] private Texture2D colorTexture;

		/// <summary>This allows you to specify the texture used to build the Alpha information, which is used to make holes, where a color of white means there is maximum destruction/damage there.</summary>
		public Texture2D AlphaTexture { set { alphaTexture = value; } get { return alphaTexture; } } [SerializeField] private Texture2D alphaTexture;

		[SerializeField] private Texture2D generatedTexture;

		/// <summary>This static method allows you to build a shape texture based on the specified color and alpha textures.</summary>
		public static bool Generate(ref Texture2D output, Texture2D scorch, Texture2D shape)
		{
			var created = false;

			if (scorch != null && shape != null)
			{
				var sizeX  = Mathf.Max(scorch.width , shape.width );
				var sizeY  = Mathf.Max(scorch.height, shape.height);
				var stepAX = 1.0f / (scorch.width  - 1);
				var stepAY = 1.0f / (scorch.height - 1);
				var stepBX = 1.0f / ( shape.width  - 1);
				var stepBY = 1.0f / ( shape.height - 1);

				if (output == null)
				{
					output  = new Texture2D(sizeX, sizeY);
					created = true;
				}
				else if (output.width != sizeX || output.height != sizeY)
				{
					output.Resize(sizeX, sizeY);
				}

				for (var y = 0; y < sizeY; y++)
				{
					for (var x = 0; x < sizeX; x++)
					{
						var color = scorch.GetPixelBilinear(x * stepAX, y * stepAY);

						color.a = shape.GetPixelBilinear(x * stepBX, y * stepBY).a;

						output.SetPixel(x, y, color);
					}
				}

				output.Apply();
			}

			return created;
		}

		public void Generate()
		{
			if (Generate(ref generatedTexture, colorTexture, alphaTexture) == true)
			{
#if UNITY_EDITOR
				UnityEditor.AssetDatabase.AddObjectToAsset(generatedTexture, this);
#endif
			}

			generatedTexture.name = name;
#if UNITY_EDITOR
			UnityEditor.AssetDatabase.SaveAssets();
#endif
		}
	}
}

#if UNITY_EDITOR
namespace Destructible2D
{
	using UnityEditor;
	using TARGET = D2dShapeBuilder;

	[CustomEditor(typeof(TARGET))]
	public class D2dStampBuilder_Editor : D2dEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			BeginError(Any(tgts, t => t.ColorTexture == null));
				Draw("colorTexture", "This allows you to specify the texture used to build the RGB information, which is used to make scorch marks, where a color of white means there is no scorch mark.");
			EndError();
			BeginError(Any(tgts, t => t.AlphaTexture == null));
				Draw("alphaTexture", "This allows you to specify the texture used to build the Alpha information, which is used to make holes, where a color of white means there is maximum destruction/damage there.");
			EndError();

			if (Button("Generate") == true)
			{
				Each(tgts, t => t.Generate(), true);
			}
		}
	}
}
#endif