using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
#if ENABLE_INPUT_SYSTEM
using NewCode = Unity​Engine.InputSystem.Key;
#endif

namespace Destructible2D
{
	/// <summary>This component combines finger and mouse and keyboard inputs into a single interface.</summary>
	[HelpURL(D2dHelper.HelpUrlPrefix + "D2dInputManager")]
	[AddComponentMenu(D2dHelper.ComponentMenuPrefix + "Input Manager")]
	public class D2dInputManager : MonoBehaviour
	{
		public enum AxisGesture
		{
			HorizontalDrag,
			VerticalDrag,
			Twist,
			HorizontalPull,
			VerticalPull
		}

		[System.Serializable]
		public struct Axis
		{
			public int         FingerCount;
			public AxisGesture FingerGesture;
			public float       FingerSensitivity;

			public KeyCode KeyNegative;
			public KeyCode KeyPositive;
			public KeyCode KeyNegativeAlt;
			public KeyCode KeyPositiveAlt;
			public float   KeySensitivity;

			public Axis(int fCount, AxisGesture fGesture, float fSensitivty, KeyCode kNegative, KeyCode kPositive, KeyCode kNegativeAlt, KeyCode kPositiveAlt, float kSensitivity)
			{
				FingerCount       = fCount;
				FingerGesture     = fGesture;
				FingerSensitivity = fSensitivty;
				KeyNegative       = kNegative;
				KeyPositive       = kPositive;
				KeyNegativeAlt    = kNegativeAlt;
				KeyPositiveAlt    = kPositiveAlt;
				KeySensitivity    = kSensitivity;
			}

			public float GetValue()
			{
				var value   = 0.0f;
				var fingers = GetFingers(true, true);

				value -= IsDown(KeyNegative) == true ? KeySensitivity : 0.0f;
				value += IsDown(KeyPositive) == true ? KeySensitivity : 0.0f;

				value -= IsDown(KeyNegativeAlt) == true ? KeySensitivity : 0.0f;
				value += IsDown(KeyPositiveAlt) == true ? KeySensitivity : 0.0f;

				if (FingerCount > 0 && fingers.Count == FingerCount)
				{
					switch (FingerGesture)
					{
						case AxisGesture.HorizontalDrag: value += GetAverageDeltaScaled(fingers).x * FingerSensitivity; break;
						case AxisGesture.VerticalDrag: value += GetAverageDeltaScaled(fingers).y * FingerSensitivity; break;
						case AxisGesture.Twist: value += GetAverageTwistRadians(fingers) * FingerSensitivity; break;
						case AxisGesture.HorizontalPull: value += GetAveragePullScaled(fingers).x * FingerSensitivity; break;
						case AxisGesture.VerticalPull: value += GetAveragePullScaled(fingers).y * FingerSensitivity; break;
					}
				}

				return value;
			}
		}

		[System.Serializable]
		public struct Trigger
		{
			public bool    UseFinger;
			public bool    UseMouse;
			public KeyCode UseKey;

			public Trigger(bool uFinger, bool uMouse, KeyCode uKey)
			{
				UseFinger = uFinger;
				UseMouse  = uMouse;
				UseKey    = uKey;
			}

			public bool WentDown(Finger finger)
			{
				if (UseFinger == true && finger.Index >= 0 && finger.Down == true)
				{
					return true;
				}

				if (UseMouse == true && finger.Index == MOUSE_FINGER_INDEX && finger.Down == true)
				{
					return true;
				}

				if (UseKey != KeyCode.None && finger.Index == HOVER_FINGER_INDEX && D2dInputManager.WentDown(UseKey) == true)
				{
					return true;
				}

				return false;
			}

			public bool IsDown(Finger finger)
			{
				if (UseFinger == true && finger.Index >= 0 && finger.Up == false)
				{
					return true;
				}

				if (UseMouse == true && finger.Index == MOUSE_FINGER_INDEX && finger.Up == false)
				{
					return true;
				}

				if (UseKey != KeyCode.None && finger.Index == HOVER_FINGER_INDEX && D2dInputManager.IsDown(UseKey) == true)
				{
					return true;
				}

				return false;
			}

			public bool WentUp(Finger finger, bool useAnyFinger = false)
			{
				if (useAnyFinger == true && finger.Up == true)
				{
					return true;
				}

				if (UseFinger == true && finger.Index >= 0 && finger.Up == true)
				{
					return true;
				}

				if (UseMouse == true && finger.Index == MOUSE_FINGER_INDEX && finger.Up == true)
				{
					return true;
				}

				if (UseKey != KeyCode.None && finger.Index == HOVER_FINGER_INDEX && D2dInputManager.WentUp(UseKey) == true)
				{
					return true;
				}

				return false;
			}
		}

		public abstract class Link
		{
			public Finger Finger;

			public static T Find<T>(List<T> links, Finger finger)
				where T : Link, new()
			{
				if (links != null)
				{
					foreach (var link in links)
					{
						if (link.Finger == finger)
						{
							return link;
						}
					}
				}

				return null;
			}

			public static T Create<T>(ref List<T> links, Finger finger)
				where T : Link, new()
			{
				var link = Find(links, finger);

				if (link == null)
				{
					if (links == null)
					{
						links = new List<T>();
					}

					link = new T();

					link.Finger = finger;

					links.Add(link);
				}
				else
				{
					Debug.LogError("Link already exists!");
				}

				return link;
			}

			public static void ClearAll<T>(List<T> links)
				where T : Link
			{
				if (links != null)
				{
					foreach (var link in links)
					{
						link.Clear();
					}

					links.Clear();
				}
			}

			public static void ClearAndRemove<T>(List<T> links, T link)
				where T : Link
			{
				if (link != null)
				{
					link.Clear();

					if (links != null)
					{
						links.Remove(link);
					}
				}
			}

			public virtual void Clear()
			{
			}
		}

		public class Finger
		{
			public int     Index;
			public float   Pressure;
			public bool    Down;
			public bool    Up;
			public float   Age;
			public bool    StartedOverGui;
			public Vector2 StartScreenPosition;
			public Vector2 ScreenPosition;
			public Vector2 ScreenPositionOld;
			public Vector2 ScreenPositionOldOld;
			public Vector2 ScreenPositionOldOldOld;

			public float SmoothScreenPositionDelta
			{
				get
				{
					if (Up == false)
					{
						return Vector2.Distance(ScreenPositionOldOld, ScreenPositionOld);
					}

					return Vector2.Distance(ScreenPositionOldOld, ScreenPosition);
				}
			}

			public Vector2 GetSmoothScreenPosition(float t)
			{
				if (Up == false)
				{
					return Hermite(ScreenPositionOldOldOld, ScreenPositionOldOld, ScreenPositionOld, ScreenPosition, t);
				}

				return Vector2.LerpUnclamped(ScreenPositionOldOld, ScreenPosition, t);
			}
		}

		/// <summary>Fingers that began touching the screen on top of these UI layers will be ignored.</summary>
		public LayerMask GuiLayers { set { guiLayers = value; } get { return guiLayers; } } [SerializeField] private LayerMask guiLayers = 1 << 5;

		/// <summary>This event will tell you when a finger begins touching the screen.</summary>
		public static event System.Action<Finger> OnFingerDown;

		/// <summary>This event will tell you when a finger has begun, is, or has just stopped touching the screen.</summary>
		public static event System.Action<Finger> OnFingerUpdate;

		/// <summary>This event will tell you when a finger stops touching the screen.</summary>
		public static event System.Action<Finger> OnFingerUp;

		public const int MOUSE_FINGER_INDEX = -1;

		public const int HOVER_FINGER_INDEX = -1337;

		private static List<RaycastResult> tempRaycastResults = new List<RaycastResult>(10);

		private static PointerEventData tempPointerEventData;

		private static EventSystem tempEventSystem;

		private static List<Finger> fingers = new List<Finger>();

		private static List<Finger> filleredFingers = new List<Finger>();

		private static Stack<Finger> pool = new Stack<Finger>();

		private static List<D2dInputManager> instances = new List<D2dInputManager>();

		public static float ScaleFactor
		{
			get
			{
				var dpi = Screen.dpi;

				if (dpi <= 0)
				{
					dpi = 200.0f;
				}

				return 200.0f / dpi;
			}
		}

		public static List<Finger> GetFingers(bool ignoreStartedOverGui = false, bool ignoreHover = false)
		{
			filleredFingers.Clear();

			foreach (var finger in fingers)
			{
				if (ignoreStartedOverGui == true && finger.StartedOverGui == true)
				{
					continue;
				}

				if (ignoreHover == true && finger.Index == HOVER_FINGER_INDEX)
				{
					continue;
				}

				filleredFingers.Add(finger);
			}

			return filleredFingers;
		}

		public static bool PointOverGui(Vector2 screenPosition, int guiLayers = 1 << 5)
		{
			return RaycastGui(screenPosition, guiLayers).Count > 0;
		}

		/// <summary>This method gives you all UI elements under the specified screen position, where element 0 is the first/top one.</summary>
		public static List<RaycastResult> RaycastGui(Vector2 screenPosition, int guiLayers = 1 << 5)
		{
			tempRaycastResults.Clear();

			var currentEventSystem = EventSystem.current;

			if (currentEventSystem != null)
			{
				// Create point event data for this event system?
				if (currentEventSystem != tempEventSystem)
				{
					tempEventSystem = currentEventSystem;

					if (tempPointerEventData == null)
					{
						tempPointerEventData = new PointerEventData(tempEventSystem);
					}
					else
					{
						tempPointerEventData.Reset();
					}
				}

				// Raycast event system at the specified point
				tempPointerEventData.position = screenPosition;

				currentEventSystem.RaycastAll(tempPointerEventData, tempRaycastResults);

				// Loop through all results and remove any that don't match the layer mask
				if (tempRaycastResults.Count > 0)
				{
					for (var i = tempRaycastResults.Count - 1; i >= 0; i--)
					{
						var raycastResult = tempRaycastResults[i];
						var raycastLayer  = 1 << raycastResult.gameObject.layer;

						if ((raycastLayer & guiLayers) == 0)
						{
							tempRaycastResults.RemoveAt(i);
						}
					}
				}
			}

			return tempRaycastResults;
		}

		public static Vector2 GetAveragePosition(List<Finger> fingers)
		{
			var total = Vector2.zero;

			foreach (var finger in fingers)
			{
				total += finger.ScreenPosition;
			}

			return fingers.Count == 0 ? total : total / fingers.Count;
		}

		public static Vector2 GetAverageOldPosition(List<Finger> fingers)
		{
			var total = Vector2.zero;

			foreach (var finger in fingers)
			{
				total += finger.ScreenPositionOld;
			}

			return fingers.Count == 0 ? total : total / fingers.Count;
		}

		public static Vector2 GetAveragePullScaled(List<Finger> fingers)
		{
			var total = Vector2.zero;

			foreach (var finger in fingers)
			{
				total += finger.ScreenPosition - finger.StartScreenPosition;
			}

			return fingers.Count == 0 ? total : total * ScaleFactor / fingers.Count;
		}

		public static Vector2 GetAverageDeltaScaled(List<Finger> fingers)
		{
			var total = Vector2.zero;

			foreach (var finger in fingers)
			{
				total += finger.ScreenPosition - finger.ScreenPositionOld;
			}

			return fingers.Count == 0 ? total : total * ScaleFactor / fingers.Count;
		}

		public static float GetAverageTwistRadians(List<Finger> fingers)
		{
			var total     = 0.0f;
			var center    = GetAveragePosition(fingers);
			var oldCenter = GetAverageOldPosition(fingers);

			foreach (var finger in fingers)
			{
				total += GetDeltaRadians(finger, center, oldCenter);
			}

			return fingers.Count == 0 ? total : total / fingers.Count;
		}

		/// <summary>If your component uses this component, then make sure you call this method at least once before you use it (e.g. from <b>Awake</b>).</summary>
		public static void EnsureThisComponentExists()
		{
			if (instances.Count == 0 && Application.isPlaying == true)
			{
				new GameObject(typeof(D2dInputManager).Name).AddComponent<D2dInputManager>();
			}
		}

		protected virtual void OnEnable()
		{
			instances.Add(this);
		}

		protected virtual void OnDisable()
		{
			instances.Add(this);
		}

		protected virtual void Update()
		{
			// Remove previously up fingers, or mark them as up in case the up event isn't read correctly
			for (var i = fingers.Count - 1; i >= 0; i--)
			{
				var finger = fingers[i];

				if (finger.Up == true)
				{
					fingers.RemoveAt(i); pool.Push(finger);
				}
				else
				{
					finger.Up = true;
				}
			}

			// Update real fingers
			if (TouchCount > 0)
			{
				for (var i = 0; i < TouchCount; i++)
				{
					int id; Vector2 position; float pressure; bool up;

					GetTouch(i, out id, out position, out pressure, out up);

					AddFinger(id, position, pressure, up);
				}
			}
			// If there are no real touches, simulate some from the mouse?
			else
			{
				var set = false;
				var up  = false;

				GetMouse(ref set, ref up);

				AddFinger(HOVER_FINGER_INDEX, MousePosition, 0.0f, false);

				if (set == true || up == true)
				{
					AddFinger(MOUSE_FINGER_INDEX, MousePosition, 1.0f, up);
				}
			}

			// Events
			foreach (var finger in fingers)
			{
				if (finger.Down == true && OnFingerDown   != null) OnFingerDown  .Invoke(finger);
				if (                       OnFingerUpdate != null) OnFingerUpdate.Invoke(finger);
				if (finger.Up   == true && OnFingerUp     != null) OnFingerUp    .Invoke(finger);
			}
		}

		private Finger FindFinger(int index)
		{
			foreach (var finger in fingers)
			{
				if (finger.Index == index)
				{
					return finger;
				}
			}

			return null;
		}

		private void AddFinger(int index, Vector2 screenPosition, float pressure, bool up)
		{
			var finger = FindFinger(index);

			if (finger == null)
			{
				finger = pool.Count > 0 ? pool.Pop() : new Finger();

				finger.Index = index;
				finger.Down  = true;
				finger.Age   = 0.0f;

				finger.StartedOverGui          = PointOverGui(screenPosition, guiLayers);
				finger.StartScreenPosition     = screenPosition;
				finger.ScreenPositionOld       = screenPosition;
				finger.ScreenPositionOldOld    = screenPosition;
				finger.ScreenPositionOldOldOld = screenPosition;

				fingers.Add(finger);
			}
			else
			{
				finger.Down = false;
				finger.Age += Time.deltaTime;

				finger.ScreenPositionOldOldOld = finger.ScreenPositionOldOld;
				finger.ScreenPositionOldOld    = finger.ScreenPositionOld;
				finger.ScreenPositionOld       = finger.ScreenPosition;
			}

			finger.Pressure       = pressure;
			finger.ScreenPosition = screenPosition;
			finger.Up             = up;
		}

		private static Vector2 Hermite(Vector2 a, Vector2 b, Vector2 c, Vector2 d, float t)
		{
			var mu2 = t * t;
			var mu3 = mu2 * t;
			var x   = HermiteInterpolate(a.x, b.x, c.x, d.x, t, mu2, mu3);
			var y   = HermiteInterpolate(a.y, b.y, c.y, d.y, t, mu2, mu3);

			return new Vector2(x, y);
		}

		private static float HermiteInterpolate(float y0,float y1, float y2,float y3, float mu, float mu2, float mu3)
		{
			var m0 = (y1 - y0) * 0.5f + (y2 - y1) * 0.5f;
			var m1 = (y2 - y1) * 0.5f + (y3 - y2) * 0.5f;
			var a0 =  2.0f * mu3 - 3.0f * mu2 + 1.0f;
			var a1 =         mu3 - 2.0f * mu2 + mu;
			var a2 =         mu3 -        mu2;
			var a3 = -2.0f * mu3 + 3.0f * mu2;

			return(a0*y1+a1*m0+a2*m1+a3*y2);
		}

		private static float GetRadians(Vector2 screenPosition, Vector2 referencePoint)
		{
			return Mathf.Atan2(screenPosition.x - referencePoint.x, screenPosition.y - referencePoint.y);
		}

		private static float GetDeltaRadians(Finger finger, Vector2 referencePoint, Vector2 lastReferencePoint)
		{
			var a = GetRadians(finger.ScreenPositionOld, lastReferencePoint);
			var b = GetRadians(finger.ScreenPosition, referencePoint);
			var d = Mathf.Repeat(a - b, Mathf.PI * 2.0f);

			if (d > Mathf.PI)
			{
				d -= Mathf.PI * 2.0f;
			}

			return d;
		}

#if ENABLE_INPUT_SYSTEM
		private static System.Collections.Generic.Dictionary<KeyCode, NewCode> keyMapping = new System.Collections.Generic.Dictionary<KeyCode, NewCode>()
		{
			{ KeyCode.None, NewCode.None },
			{ KeyCode.Backspace, NewCode.Backspace },
			{ KeyCode.Tab, NewCode.Tab },
			{ KeyCode.Clear, NewCode.None },
			{ KeyCode.Return, NewCode.Enter },
			{ KeyCode.Pause, NewCode.Pause },
			{ KeyCode.Escape, NewCode.Escape },
			{ KeyCode.Space, NewCode.Space },
			{ KeyCode.Exclaim, NewCode.None },
			{ KeyCode.DoubleQuote, NewCode.None },
			{ KeyCode.Hash, NewCode.None },
			{ KeyCode.Dollar, NewCode.None },
			{ KeyCode.Percent, NewCode.None },
			{ KeyCode.Ampersand, NewCode.None },
			{ KeyCode.Quote, NewCode.Quote },
			{ KeyCode.LeftParen, NewCode.None },
			{ KeyCode.RightParen, NewCode.None },
			{ KeyCode.Asterisk, NewCode.None },
			{ KeyCode.Plus, NewCode.None },
			{ KeyCode.Comma, NewCode.Comma },
			{ KeyCode.Minus, NewCode.Minus },
			{ KeyCode.Period, NewCode.Period },
			{ KeyCode.Slash, NewCode.Slash },
			{ KeyCode.Alpha1, NewCode.Digit1 },
			{ KeyCode.Alpha2, NewCode.Digit2 },
			{ KeyCode.Alpha3, NewCode.Digit3 },
			{ KeyCode.Alpha4, NewCode.Digit4 },
			{ KeyCode.Alpha5, NewCode.Digit5 },
			{ KeyCode.Alpha6, NewCode.Digit6 },
			{ KeyCode.Alpha7, NewCode.Digit7 },
			{ KeyCode.Alpha8, NewCode.Digit8 },
			{ KeyCode.Alpha9, NewCode.Digit9 },
			{ KeyCode.Alpha0, NewCode.Digit0 },
			{ KeyCode.Colon, NewCode.None },
			{ KeyCode.Semicolon, NewCode.Semicolon },
			{ KeyCode.Less, NewCode.None },
			{ KeyCode.Equals, NewCode.Equals },
			{ KeyCode.Greater, NewCode.None },
			{ KeyCode.Question, NewCode.None },
			{ KeyCode.At, NewCode.None },
			{ KeyCode.LeftBracket, NewCode.LeftBracket },
			{ KeyCode.Backslash, NewCode.Backslash },
			{ KeyCode.RightBracket, NewCode.RightBracket },
			{ KeyCode.Caret, NewCode.None },
			{ KeyCode.Underscore, NewCode.None },
			{ KeyCode.BackQuote, NewCode.Backquote },
			{ KeyCode.A, NewCode.A },
			{ KeyCode.B, NewCode.B },
			{ KeyCode.C, NewCode.C },
			{ KeyCode.D, NewCode.D },
			{ KeyCode.E, NewCode.E },
			{ KeyCode.F, NewCode.F },
			{ KeyCode.G, NewCode.G },
			{ KeyCode.H, NewCode.H },
			{ KeyCode.I, NewCode.I },
			{ KeyCode.J, NewCode.J },
			{ KeyCode.K, NewCode.K },
			{ KeyCode.L, NewCode.L },
			{ KeyCode.M, NewCode.M },
			{ KeyCode.N, NewCode.N },
			{ KeyCode.O, NewCode.O },
			{ KeyCode.P, NewCode.P },
			{ KeyCode.Q, NewCode.Q },
			{ KeyCode.R, NewCode.R },
			{ KeyCode.S, NewCode.S },
			{ KeyCode.T, NewCode.T },
			{ KeyCode.U, NewCode.U },
			{ KeyCode.V, NewCode.V },
			{ KeyCode.W, NewCode.W },
			{ KeyCode.X, NewCode.X },
			{ KeyCode.Y, NewCode.Y },
			{ KeyCode.Z, NewCode.Z },
			{ KeyCode.LeftCurlyBracket, NewCode.None },
			{ KeyCode.Pipe, NewCode.None },
			{ KeyCode.RightCurlyBracket, NewCode.None },
			{ KeyCode.Tilde, NewCode.None },
			{ KeyCode.Delete, NewCode.Delete },
			{ KeyCode.Keypad0, NewCode.Numpad0 },
			{ KeyCode.Keypad1, NewCode.Numpad1 },
			{ KeyCode.Keypad2, NewCode.Numpad2 },
			{ KeyCode.Keypad3, NewCode.Numpad3 },
			{ KeyCode.Keypad4, NewCode.Numpad4 },
			{ KeyCode.Keypad5, NewCode.Numpad5 },
			{ KeyCode.Keypad6, NewCode.Numpad6 },
			{ KeyCode.Keypad7, NewCode.Numpad7 },
			{ KeyCode.Keypad8, NewCode.Numpad8 },
			{ KeyCode.Keypad9, NewCode.Numpad9 },
			{ KeyCode.KeypadPeriod, NewCode.NumpadPeriod },
			{ KeyCode.KeypadDivide, NewCode.NumpadDivide },
			{ KeyCode.KeypadMultiply, NewCode.NumpadMultiply },
			{ KeyCode.KeypadMinus, NewCode.NumpadMinus },
			{ KeyCode.KeypadPlus, NewCode.NumpadPlus },
			{ KeyCode.KeypadEnter, NewCode.NumpadEnter },
			{ KeyCode.KeypadEquals, NewCode.NumpadEquals },
			{ KeyCode.UpArrow, NewCode.UpArrow },
			{ KeyCode.DownArrow, NewCode.DownArrow },
			{ KeyCode.RightArrow, NewCode.RightArrow },
			{ KeyCode.LeftArrow, NewCode.LeftArrow },
			{ KeyCode.Insert, NewCode.Insert },
			{ KeyCode.Home, NewCode.Home },
			{ KeyCode.End, NewCode.End },
			{ KeyCode.PageUp, NewCode.PageUp },
			{ KeyCode.PageDown, NewCode.PageDown },
			{ KeyCode.F1, NewCode.F1 },
			{ KeyCode.F2, NewCode.F2 },
			{ KeyCode.F3, NewCode.F3 },
			{ KeyCode.F4, NewCode.F4 },
			{ KeyCode.F5, NewCode.F5 },
			{ KeyCode.F6, NewCode.F6 },
			{ KeyCode.F7, NewCode.F7 },
			{ KeyCode.F8, NewCode.F8 },
			{ KeyCode.F9, NewCode.F9 },
			{ KeyCode.F10, NewCode.F10 },
			{ KeyCode.F11, NewCode.F11 },
			{ KeyCode.F12, NewCode.F12 },
			{ KeyCode.F13, NewCode.None },
			{ KeyCode.F14, NewCode.None },
			{ KeyCode.F15, NewCode.None },
			{ KeyCode.Numlock, NewCode.NumLock },
			{ KeyCode.CapsLock, NewCode.CapsLock },
			{ KeyCode.ScrollLock, NewCode.ScrollLock },
			{ KeyCode.RightShift, NewCode.RightShift },
			{ KeyCode.LeftShift, NewCode.LeftShift },
			{ KeyCode.RightControl, NewCode.RightCtrl },
			{ KeyCode.LeftControl, NewCode.LeftCtrl },
			{ KeyCode.RightAlt, NewCode.RightAlt },
			{ KeyCode.LeftAlt, NewCode.LeftAlt },
			{ KeyCode.RightCommand, NewCode.RightCommand },
			//{ KeyCode.RightApple, NewCode.RightApple },
			{ KeyCode.LeftCommand, NewCode.LeftCommand },
			//{ KeyCode.LeftApple, NewCode.LeftApple },
			{ KeyCode.LeftWindows, NewCode.LeftWindows },
			{ KeyCode.RightWindows, NewCode.RightWindows },
			{ KeyCode.AltGr, NewCode.AltGr },
			{ KeyCode.Help, NewCode.None },
			{ KeyCode.Print, NewCode.PrintScreen },
			{ KeyCode.SysReq, NewCode.None },
			{ KeyCode.Break, NewCode.None },
			{ KeyCode.Menu, NewCode.ContextMenu },
		};

		[UnityEngine.RuntimeInitializeOnLoadMethod]
		static void Enable()
		{
			UnityEngine.InputSystem.EnhancedTouch.EnhancedTouchSupport.Enable();
		}

		private static UnityEngine.InputSystem.Controls.ButtonControl GetButtonControl(KeyCode oldKey)
		{
			if (UnityEngine.InputSystem.Mouse.current != null)
			{
				if (oldKey == KeyCode.Mouse0) return UnityEngine.InputSystem.Mouse.current.leftButton;
				if (oldKey == KeyCode.Mouse1) return UnityEngine.InputSystem.Mouse.current.rightButton;
				if (oldKey == KeyCode.Mouse2) return UnityEngine.InputSystem.Mouse.current.middleButton;
			}

			NewCode newKey;

			if (keyMapping.TryGetValue(oldKey, out newKey) == true)
			{
				return UnityEngine.InputSystem.Keyboard.current[newKey];
			}

			return null;
		}

		public static int TouchCount
		{
			get
			{
				return UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches.Count;
			}
		}

		public static void GetTouch(int index, out int id, out UnityEngine.Vector2 position, out float pressure, out bool set)
		{
			var touch = UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches[index];

			id = touch.finger.index;

			position = touch.screenPosition;

			pressure = touch.pressure;

			set =
				touch.phase == UnityEngine.InputSystem.TouchPhase.Canceled ||
				touch.phase == UnityEngine.InputSystem.TouchPhase.Ended;
		}

		public static void GetMouse(ref bool set, ref bool up)
		{
			if (UnityEngine.InputSystem.Mouse.current == null)
			{
				return;
			}

			var controls = UnityEngine.InputSystem.Mouse.current.allControls;

			for (var i = 0; i < controls.Count; i++)
			{
				var button = controls[i] as UnityEngine.InputSystem.Controls.ButtonControl;

				if (button != null)
				{
					set |= button.isPressed;
					up  |= button.wasReleasedThisFrame;
				}
			}
		}

		public static UnityEngine.Vector2 MousePosition
		{
			get
			{
				return UnityEngine.InputSystem.Mouse.current.position.ReadValue();
			}
		}

		public static bool MouseExists
		{
			get
			{
				return UnityEngine.InputSystem.Mouse.current != null;
			}
		}

		public static bool KeyboardExists
		{
			get
			{
				return UnityEngine.InputSystem.Keyboard.current != null;
			}
		}

		public static bool WentDown(KeyCode key)
		{
			var control = GetButtonControl(key); return control != null && control.wasPressedThisFrame;
		}

		public static bool IsDown(KeyCode key)
		{
			var control = GetButtonControl(key); return control != null && control.isPressed;
		}

		public static bool WentUp(KeyCode key)
		{
			var control = GetButtonControl(key); return control != null && control.wasReleasedThisFrame;
		}
#else

		public static int TouchCount
		{
			get
			{
				return UnityEngine.Input.touchCount;
			}
		}

		public static void GetTouch(int index, out int id, out UnityEngine.Vector2 position, out float pressure, out bool up)
		{
			var touch = UnityEngine.Input.GetTouch(index);

			id = touch.fingerId;

			position = touch.position;

			pressure = touch.pressure;

			up =
				touch.phase == UnityEngine.TouchPhase.Canceled ||
				touch.phase == UnityEngine.TouchPhase.Ended;
		}

		public static void GetMouse(ref bool set, ref bool up)
		{
			for (var i = 0; i < 4; i++)
			{
				set |= UnityEngine.Input.GetMouseButton(i);
				up  |= UnityEngine.Input.GetMouseButtonUp(i);
			}
		}

		public static UnityEngine.Vector2 MousePosition
		{
			get
			{
				return UnityEngine.Input.mousePosition;
			}
		}

		public static bool MouseExists
		{
			get
			{
				return UnityEngine.Input.mousePresent;
			}
		}

		public static bool KeyboardExists
		{
			get
			{
				return true;
			}
		}

		public static bool WentDown(KeyCode key)
		{
			return UnityEngine.Input.GetKeyDown(key);
		}

		public static bool IsDown(KeyCode key)
		{
			return UnityEngine.Input.GetKey(key);
		}

		public static bool WentUp(KeyCode key)
		{
			return UnityEngine.Input.GetKeyUp(key);
		}
#endif
	}
}