using UnityEngine;

namespace Destructible2D
{
	// This class handles the various ways to modify the destruction state of a D2dDestructible.
	public partial class D2dDestructible
	{
		public enum PaintType
		{
			Cut,
			Heal,
			Subtract,
			SubtractInvColor,
			CutMinRGB
		}

		/// <summary>This paints the current destructible with the specified paint type, at the specified matrix, with the specified shape.</summary>
		public void Paint(PaintType paint, Matrix4x4 matrix, Texture2D shape, Color color)
		{
			switch (paint)
			{
				case PaintType.Cut: Cut(matrix, shape, color); break;
				case PaintType.Heal: Heal(matrix, shape, color); break;
				case PaintType.Subtract: Subtract(matrix, shape, color); break;
				case PaintType.SubtractInvColor: Subtract(matrix, shape, new Color(1.0f - color.r, 1.0f - color.g, 1.0f - color.b, color.a)); break;
				//case PaintType.SubtractInvColor: SubtractInvRGB(matrix, shape, color); break;
				case PaintType.CutMinRGB: CutMinRGB(matrix, shape, color); break;
			}
		}

		private static Matrix4x4 matrix;
		private static Matrix4x4 inverse;

		private static D2dRect   rect;
		private static Texture2D shape;
		private static int       shapeW;
		private static int       shapeH;
		private static Color     shapeColor;
		private static Vector3   shapeCoordA;
		private static Vector3   shapeCoordB;
		private static float     alphaPixelX;
		private static float     alphaPixelY;
		private static float     alphaHalfPixelX;
		private static float     alphaHalfPixelY;

		private float GetSolidStrength(byte alpha)
		{
			return System.Math.Min(1.0f, (255.0f - alpha) / solidRange);
		}

		private bool BeginCommon(Matrix4x4 newMatrix, Texture2D newShape, Color newColor)
		{
			if (ready == true && newShape != null)
			{
				rect       = default(D2dRect);
				matrix     = WorldToAlphaMatrix * newMatrix;
				alphaCount = -1;

				if (D2dHelper.CalculateRect(matrix, ref rect, alphaWidth, alphaHeight) == true)
				{
					inverse    = matrix.inverse;
					shape      = newShape;
					shapeW     = newShape.width;
					shapeH     = newShape.height;
					shapeColor = newColor * paintMultiplier;

					rect.MinX = Mathf.Clamp(rect.MinX, 0, alphaWidth );
					rect.MaxX = Mathf.Clamp(rect.MaxX, 0, alphaWidth );
					rect.MinY = Mathf.Clamp(rect.MinY, 0, alphaHeight);
					rect.MaxY = Mathf.Clamp(rect.MaxY, 0, alphaHeight);

					alphaPixelX     = D2dHelper.Reciprocal(alphaWidth );
					alphaPixelY     = D2dHelper.Reciprocal(alphaHeight);
					alphaHalfPixelX = alphaPixelX * 0.5f;
					alphaHalfPixelY = alphaPixelY * 0.5f;

					modifiedPixels.Clear();

					return true;
				}
			}

			return false;
		}

		public void Cut(Matrix4x4 newMatrix, Texture2D newShape, Color newColor)
		{
			if (indestructible == false && BeginCommon(newMatrix, newShape, newColor) == true)
			{
				for (var y = rect.MinY; y < rect.MaxY; y++)
				{
					var v      = y * alphaPixelY + alphaHalfPixelY;
					var offset = y * alphaWidth;

					for (var x = rect.MinX; x < rect.MaxX; x++)
					{
						var u = x * alphaPixelX + alphaHalfPixelX;

						if (CalculateShapeCoord(u, v) == true)
						{
							var index      = offset + x;
							var alphaPixel = alphaData[index];
							var alphaOld   = alphaPixel.a;
							var shapePixel = SampleShapeA();

							if (solidRange > 0)
							{
								shapePixel = (byte)(shapePixel * GetSolidStrength(alphaPixel.a));
							}

							alphaPixel.a = (byte)System.Math.Max(alphaPixel.a - shapePixel, 0);

							WriteCommon(index, alphaOld, alphaPixel);
						}
					}
				}

				EndCommon();
			}
		}

		public void Heal(Matrix4x4 newMatrix, Texture2D newShape, Color newColor)
		{
			if (indestructible == false && CanHeal == true && BeginCommon(newMatrix, newShape, newColor) == true)
			{
				var healData = healSnapshot.Data;
				
				for (var y = rect.MinY; y < rect.MaxY; y++)
				{
					var v      = y * alphaPixelY + alphaHalfPixelY;
					var offset = y * alphaWidth;

					for (var x = rect.MinX; x < rect.MaxX; x++)
					{
						var u = x * alphaPixelX + alphaHalfPixelX;

						if (CalculateShapeCoord(u, v) == true)
						{
							var index      = offset + x;
							var alphaPixel = alphaData[index];
							var alphaOld   = alphaPixel.a;
							var shapePixel = SampleShapeA();

							alphaPixel.a = (byte)System.Math.Min(alphaPixel.a + shapePixel, healData.AlphaData[index].a);

							WriteCommon(index, alphaOld, alphaPixel);
						}
					}
				}

				EndCommon();
			}
		}
		
		public void Subtract(Matrix4x4 newMatrix, Texture2D newShape, Color newColor)
		{
			if (indestructible == false && BeginCommon(newMatrix, newShape, newColor) == true)
			{
				for (var y = rect.MinY; y < rect.MaxY; y++)
				{
					var v      = y * alphaPixelY + alphaHalfPixelY;
					var offset = y * alphaWidth;

					for (var x = rect.MinX; x < rect.MaxX; x++)
					{
						var u = x * alphaPixelX + alphaHalfPixelX;

						if (CalculateShapeCoord(u, v) == true)
						{
							var index      = offset + x;
							var alphaPixel = alphaData[index];
							var alphaOld   = alphaPixel.a;
							var shapePixel = SampleShape();

							if (solidRange > 0)
							{
								shapePixel.a = (byte)(shapePixel.a * GetSolidStrength(alphaPixel.a));
							}

							alphaPixel.r = (byte)System.Math.Max(alphaPixel.r - shapePixel.r, 0);
							alphaPixel.g = (byte)System.Math.Max(alphaPixel.g - shapePixel.g, 0);
							alphaPixel.b = (byte)System.Math.Max(alphaPixel.b - shapePixel.b, 0);
							alphaPixel.a = (byte)System.Math.Max(alphaPixel.a - shapePixel.a, 0);

							WriteCommon(index, alphaOld, alphaPixel);
						}
					}
				}

				EndCommon();
			}
		}

		public void SubtractInvRGB(Matrix4x4 newMatrix, Texture2D newShape, Color newColor)
		{
			if (indestructible == false && BeginCommon(newMatrix, newShape, newColor) == true)
			{
				for (var y = rect.MinY; y < rect.MaxY; y++)
				{
					var v      = y * alphaPixelY + alphaHalfPixelY;
					var offset = y * alphaWidth;

					for (var x = rect.MinX; x < rect.MaxX; x++)
					{
						var u = x * alphaPixelX + alphaHalfPixelX;

						if (CalculateShapeCoord(u, v) == true)
						{
							var index      = offset + x;
							var alphaPixel = alphaData[index];
							var alphaOld   = alphaPixel.a;
							var shapePixel = SampleShape();

							if (solidRange > 0)
							{
								shapePixel.a = (byte)(shapePixel.a * GetSolidStrength(alphaPixel.a));
							}

							alphaPixel.r = (byte)System.Math.Max(alphaPixel.r - (255 - shapePixel.r), 0);
							alphaPixel.g = (byte)System.Math.Max(alphaPixel.g - (255 - shapePixel.g), 0);
							alphaPixel.b = (byte)System.Math.Max(alphaPixel.b - (255 - shapePixel.b), 0);
							alphaPixel.a = (byte)System.Math.Max(alphaPixel.a - shapePixel.a, 0);

							WriteCommon(index, alphaOld, alphaPixel);
						}
					}
				}

				EndCommon();
			}
		}

		public void CutMinRGB(Matrix4x4 newMatrix, Texture2D newShape, Color newColor)
		{
			if (indestructible == false && BeginCommon(newMatrix, newShape, newColor) == true)
			{
				for (var y = rect.MinY; y < rect.MaxY; y++)
				{
					var v      = y * alphaPixelY + alphaHalfPixelY;
					var offset = y * alphaWidth;

					for (var x = rect.MinX; x < rect.MaxX; x++)
					{
						var u = x * alphaPixelX + alphaHalfPixelX;

						if (CalculateShapeCoord(u, v) == true)
						{
							var index      = offset + x;
							var alphaPixel = alphaData[index];
							var alphaOld   = alphaPixel.a;
							var shapePixel = SampleShape();

							if (solidRange > 0)
							{
								shapePixel.a = (byte)(shapePixel.a * GetSolidStrength(alphaPixel.a));
							}

							alphaPixel.r = System.Math.Min(alphaPixel.r, shapePixel.r);
							alphaPixel.g = System.Math.Min(alphaPixel.g, shapePixel.g);
							alphaPixel.b = System.Math.Min(alphaPixel.b, shapePixel.b);
							alphaPixel.a = (byte)System.Math.Max(alphaPixel.a - shapePixel.a, 0);

							WriteCommon(index, alphaOld, alphaPixel);
						}
					}
				}

				EndCommon();
			}
		}

		private void WriteCommon(int index, byte alphaOld, Color32 alphaNew)
		{
			if (pixels == PixelsType.PixelatedBinary)
			{
				alphaNew.a = alphaNew.a > 127 ? (byte)255 : (byte)0;
			}

			if (monitorPixels == true)
			{
				var binaryA = alphaOld   > 127;
				var binaryB = alphaNew.a > 127;

				if (binaryA != binaryB)
				{
					modifiedPixels.Add(index);
				}
			}

			alphaData[index] = alphaNew;
		}

		private void EndCommon()
		{
			if (monitorPixels == true && modifiedPixels.Count > 0)
			{
				if (OnModifiedPixels != null)
				{
					OnModifiedPixels.Invoke(modifiedPixels);
				}

				if (OnGlobalModifiedPixels != null)
				{
					OnGlobalModifiedPixels.Invoke(this, modifiedPixels);
				}
			}

			AlphaModified.Add(rect);
		}

		private void CheckPixelChange(int i, byte a, byte b)
		{
			
		}

		private Color32 SampleShape()
		{
			var x = (int)(shapeCoordB.x * shapeW);
			var y = (int)(shapeCoordB.y * shapeH);

			return shape.GetPixel(x, y) * shapeColor;
		}

		private byte SampleShapeA()
		{
			var x = (int)(shapeCoordB.x * shapeW);
			var y = (int)(shapeCoordB.y * shapeH);

			return (byte)(shape.GetPixel(x, y).a * shapeColor.a * 255.0f);
		}

		private static bool CalculateShapeCoord(float u, float v)
		{
			shapeCoordA.x = u;
			shapeCoordA.y = v;

			shapeCoordB = inverse.MultiplyPoint(shapeCoordA);

			return shapeCoordB.x >= 0.0f && shapeCoordB.x < 1.0f && shapeCoordB.y >= 0.0f && shapeCoordB.y < 1.0f;
		}
	}
}