
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Random = UnityEngine.Random;

public static class TransformExtensions
{
	public static void ApplyRecursively(this Transform t, Action<Transform> action)
	{
		action(t);
		foreach (Transform child in t)
			ApplyRecursively(child, action);
	}
}

namespace Core
{
	public static class Util
	{
		public static readonly float FPS30 = 30.0f;
		public static readonly float SPF15 = 1.0f / 15.0f;
		public static readonly float SPF30 = 1.0f / 30.0f;
		public static readonly float SPF60 = 1.0f / 60.0f;
		public static readonly float EPSILON = 0.00001f; // When Mathf.Epsilon is too small
		public static readonly float LOW_PRECISION_EPSILON = 0.0001f; // When Epsilon is too small
		public static readonly float SUPER_LOW_PRECISION_EPSILON = 0.001f; // When Low Precision Epsilon is too small
		public static readonly float LOWEST_PRECISION_EPSILON = 0.01f; // When Low Precision Epsilon is too small
		public static readonly float DECIMAL_PLACES = 1000.0f;
		public static readonly float ASPECT_16_9 = 16.0f / 9.0f;
		public static readonly float ASPECT_9_16 = 9.0f / 16.0f;

		public static readonly float VECTOR_EPSILON = 0.01f;
		public static readonly float VECTOR_EPSILON_SQR = VECTOR_EPSILON * VECTOR_EPSILON;

		public static readonly Color ECGColor = new Color(0.0f, 0.68627451f, 0.23137255f, 1.0f);

		public static bool IsRelease()
		{
#if RELEASE
			return true;
#else
			return false;
#endif
		}
		
		public static float Cos(float degrees) { return Mathf.Cos(Mathf.Deg2Rad * degrees); }
		public static float Sin(float degrees) { return Mathf.Sin(Mathf.Deg2Rad * degrees); }
		public static float Tan(float degrees) { return Mathf.Tan(Mathf.Deg2Rad * degrees); }

		// Make sure to clamp input otherwise Acos and Asin will return NaN
		// On project Oko a floating point error was causing ACos to return NaN which when applied to a transform crashed the game
		public static float ACos(float cos) { return Mathf.Rad2Deg * Mathf.Acos(Mathf.Clamp(cos, -1.0f, 1.0f)); }
		public static float ASin(float sin) { return Mathf.Rad2Deg * Mathf.Asin(Mathf.Clamp(sin, -1.0f, 1.0f)); }
		public static float ATan(float tan) { return Mathf.Rad2Deg * Mathf.Atan(tan); }

		public static string NumberToRomanNumerals(int number)
		{
			string thousand = "m";
			string[] hundreds = {string.Empty, "c", "cc", "ccc", "cd", "d", "dc", "dcc", "dccc", "cm"};
			string[] tens = {string.Empty, "x", "xx", "xxx", "xl", "l", "lx", "lxx", "lxxx", "xc"};
			string[] ones = {string.Empty, "i", "ii", "iii", "iv", "v", "vi", "vii", "viii", "ix"};

			string romanNumerals = string.Empty;

			for (int i = 0; i < number / 1000; i++)
			{
				romanNumerals += thousand;
			}
			number %= 1000;
			if (number > 100)
			{
				romanNumerals += hundreds[number / 100];
			}
			number %= 100;
			if (number > 10)
			{
				romanNumerals += tens[number / 10];
			}
			number %= 10;
			if (number > 0)
			{
				romanNumerals += ones[number / 1];
			}

			return romanNumerals;
		}
		
		public static bool CoinFlip()
		{
			return Random.Range(0, 2) > 0;
		}

		public static string NumberToWords(int number)
		{
			if (number == 0)
				return "zero";

			if (number < 0)
				return "minus " + NumberToWords(Mathf.Abs(number));

			string words = "";

			if ((number / 1000000) > 0)
			{
				words += NumberToWords(number / 1000000) + " million ";
				number %= 1000000;
			}

			if ((number / 1000) > 0)
			{
				words += NumberToWords(number / 1000) + " thousand ";
				number %= 1000;
			}

			if ((number / 100) > 0)
			{
				words += NumberToWords(number / 100) + " hundred ";
				number %= 100;
			}

			if (number > 0)
			{
				if (words != "")
					words += "and ";

				var unitsMap = new[] { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
				var tensMap = new[] { "zero", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };

				if (number < 20)
					words += unitsMap[number];
				else
				{
					words += tensMap[number / 10];
					if ((number % 10) > 0)
						words += "-" + unitsMap[number % 10];
				}
			}

			return words;
		}

		public static System.Globalization.NumberFormatInfo SPACE_SEPARATED_NUMBER_FORMAT
		{
			get
			{
				System.Globalization.NumberFormatInfo format = new System.Globalization.NumberFormatInfo();
				format.NumberGroupSeparator = " ";
				return format;
			}
		}

		public static string FormatNumber(long number)
		{
			return number.ToString("N0", SPACE_SEPARATED_NUMBER_FORMAT);
		}

		public static string FormatNumber(int number)
		{
			return number.ToString("N0", SPACE_SEPARATED_NUMBER_FORMAT);
		}

		// default 2 cause $.
		public static string FormatNumber(float number, int places = 2)
		{
			return number.ToString("N" + places.ToString(), SPACE_SEPARATED_NUMBER_FORMAT);
		}

		public static Transform GetTopParent(Transform child)
		{
			Transform topParent = child;
			while (topParent.parent != null)
			{
				topParent = topParent.parent;
			}
			return topParent;
		}

		public static T GetComponentInHighestParent<T>(Transform transform) where T : Component
		{
			T highestComponent = transform.GetComponent<T>();
			while (transform.parent != null)
			{
				transform = transform.parent;
				T currentComponent = transform.GetComponent<T>();
				if (currentComponent != null)
				{
					highestComponent = currentComponent;
				}
			}
			return highestComponent;
		}

		public static T GetComponentInFamily<T>(GameObject obj) where T : Component
		{
			// Check this object first
			T component = obj.GetComponent<T>();
			if (component != null)
			{
				return component;
			}
			// Check our immediate children
			Transform transform = obj.transform;
			int childCount = transform.childCount;
			for (int i = 0; i < childCount; i++)
			{
				Transform child = transform.GetChild(i);
				component = child.GetComponent<T>();
				if (component != null)
				{
					return component;
				}
			}
			Transform parent = transform.parent;
			if (parent == null)
			{
				return null;
			}
			// Check our parent
			component = parent.GetComponent<T>();
			if (component != null)
			{
				return component;
			}
			// Check our siblings
			childCount = parent.childCount;
			for (int i = 0; i < childCount; i++)
			{
				Transform child = parent.GetChild(i);
				component = child.GetComponent<T>();
				if (component != null)
				{
					return component;
				}
			}
			return null;
		}

		public static void AssignParent(Transform child, Transform parent)
		{
			Vector3 scale = child.localScale;
			child.parent = parent;
			child.localPosition = Vector3.zero;
			child.localRotation = Quaternion.identity;
			//safeguard from unitys weird scale change on reparenting
			child.localScale = scale;
		}

		public static Transform FindChildByTag(Transform parent, string tag)
		{
			if (parent.CompareTag(tag))
			{
				return parent;
			}
			foreach (Transform child in parent)
			{
				Transform search = FindChildByTag(child, tag);
				if (search != null)
				{
					return search;
				}
			}
			return null;
		}

		public static Transform FindChildIgnoreCase(Transform parent, string name)
		{
			if (parent.name.Length == name.Length && parent.name.ToLower() == name.ToLower())
			{
				return parent;
			}
			foreach (Transform child in parent)
			{
				Transform search = FindChildIgnoreCase(child, name);
				if (search != null)
				{
					return search;
				}
			}
			return null;
		}

		public static Transform FindHighestChild(Transform parent, Transform highest, Transform[] ignoredTransforms = null)
		{
			bool ignoreTransform = false;
			if (ignoredTransforms != null)
			{
				foreach (Transform ignoredBone in ignoredTransforms)
				{
					if (ignoredBone.GetInstanceID() == parent.GetInstanceID())
					{
						ignoreTransform = true;
						break;
					}
				}
			}

			if (!ignoreTransform && (highest == null || parent.position.y > highest.position.y))
			{
				highest = parent;
			}
			foreach (Transform child in parent)
			{
				highest = FindHighestChild(child, highest, ignoredTransforms);
			}
			return highest;
		}

		public static Transform FindLowestChild(Transform parent, Transform lowest, Transform[] ignoredTransforms = null)
		{
			bool ignoreTransform = false;
			if (ignoredTransforms != null)
			{
				foreach (Transform ignoredBone in ignoredTransforms)
				{
					if (ignoredBone.GetInstanceID() == parent.GetInstanceID())
					{
						ignoreTransform = true;
						break;
					}
				}
			}

			if (!ignoreTransform && (lowest == null || parent.position.y < lowest.position.y))
			{
				lowest = parent;
			}
			foreach (Transform child in parent)
			{
				lowest = FindHighestChild(child, lowest, ignoredTransforms);
			}
			return lowest;
		}
		
		public static Transform FindFurthestChild(Transform parent, Transform furthest, Transform[] ignoredTransforms = null)
		{
			bool ignoreTransform = false;
			if (ignoredTransforms != null)
			{
				foreach (Transform ignoredBone in ignoredTransforms)
				{
					if (ignoredBone.GetInstanceID() == parent.GetInstanceID())
					{
						ignoreTransform = true;
						break;
					}
				}
			}

			if (!ignoreTransform && (furthest == null || parent.position.y < furthest.position.y))
			{
				furthest = parent;
			}
			foreach (Transform child in parent)
			{
				furthest = FindHighestChild(child, furthest, ignoredTransforms);
			}
			return furthest;
		}

		public static T[] ReSizeArray<T>(T[] array, int size)
		{
			if (array.Length == size)
			{
				return array;
			}
			T[] newArray = new T[size];
			System.Array.Copy(array, newArray, Mathf.Min(size, array.Length));
			return newArray;
		}

		public static Vector3 FindGround(Vector3 position, Vector3 up, int layerMask)
		{
			Ray ray = new Ray(position + up, -up);
			RaycastHit groundHit;
			if (Physics.Raycast(ray, out groundHit, Mathf.Infinity, layerMask))
			{
				return groundHit.point;
			}
			return position;
		}

		public static float PixelsPerMeter(Vector3 worldPosition)
		{
			Vector3 screenP1 = Camera.main.WorldToScreenPoint(worldPosition);
			Vector3 screenP2 = Camera.main.WorldToScreenPoint(worldPosition + Camera.main.transform.right);
			return Vector2.Distance(screenP1, screenP2);
		}

		public static float MetersPerPixel(Vector3 screenPosition)
		{
			screenPosition.z = Mathf.Max(screenPosition.z, Camera.main.nearClipPlane);
			Vector3 worldP1 = Camera.main.ScreenToWorldPoint(screenPosition);
			Vector3 worldP2 = Camera.main.ScreenToWorldPoint(screenPosition + Vector3.up);
			return Vector2.Distance(worldP1, worldP2);
		}

		public static Vector3 ProjectOntoCanvas(Vector3 worldPosition, Canvas canvas, Camera camera, float zDepthMultiplier = 0.0f)
		{
			Vector3 screenPosition = worldPosition;
			if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
			{
				screenPosition = camera.WorldToScreenPoint(worldPosition);
				screenPosition.z = 0.0f;
			}
			else if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
			{
				Vector3 fromCamera = worldPosition - camera.transform.position;
                Ray cameraRay = new Ray(camera.transform.position, fromCamera.normalized);
				float distance = canvas.planeDistance / Vector3.Dot(cameraRay.direction, camera.transform.forward);
				distance = Mathf.Lerp(distance, distance + fromCamera.magnitude, zDepthMultiplier);
                screenPosition = cameraRay.GetPoint(distance);
			}
			return screenPosition;
		}

		public static void ScaleWorldCanvasToViewport(Canvas canvas, Camera camera, bool resizeToMatchAspect, bool resizeHeight)
		{
			if (canvas == null)
			{
				Debug.LogError("Core.Util.SizeWorldCanvasToViewport: Null canvas.");
				return;
			}
			if (canvas.renderMode != RenderMode.WorldSpace)
			{
				Debug.LogError("Core.Util.SizeWorldCanvasToViewport: Canvas " + DebugUtil.GetScenePath(canvas.gameObject) + " is not a world space canvas.", canvas);
			}
			if (camera == null)
			{
				Debug.LogError("Core.Util.SizeWorldCanvasToViewport: Null camera.");
				return;
			}

			RectTransform rectTransform = canvas.GetComponent<RectTransform>();
			Vector3 toCanvas = rectTransform.position - camera.transform.position;
			float projectionDistanceToCamera = Vector3.Dot(toCanvas.normalized, camera.transform.forward) * toCanvas.magnitude;
			Vector3 screenBottomLeft = camera.ScreenToWorldPoint(new Vector3(0.0f, 0.0f, projectionDistanceToCamera));
			Vector3 screenTopRight = camera.ScreenToWorldPoint(new Vector3(camera.pixelWidth, camera.pixelHeight, projectionDistanceToCamera));
			screenBottomLeft = camera.transform.InverseTransformPoint(screenBottomLeft);
			screenTopRight = camera.transform.InverseTransformPoint(screenTopRight);
			float viewportWidth = Mathf.Abs(screenBottomLeft.x - screenTopRight.x);
			float viewportHeight = Mathf.Abs(screenBottomLeft.y - screenTopRight.y);

			if (resizeToMatchAspect)
			{
				if (resizeHeight)
				{
					if (Approximately(viewportWidth, 0.0f))
					{
						Debug.LogError("CoreGameUtil.ScaleWorldCanvasToViewport: Trying to resize to a viewport with width 0.");
					}
					else
					{
						rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, rectTransform.sizeDelta.x * viewportHeight / viewportWidth);
					}
				}
				else
				{
					if (Approximately(viewportHeight, 0.0f))
					{
						Debug.LogError("CoreGameUtil.ScaleWorldCanvasToViewport: Trying to resize to a viewport with height 0.");
					}
					else
					{
						rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.y * viewportWidth / viewportHeight, rectTransform.sizeDelta.y);
					}
				}
			}

			if (Approximately(rectTransform.sizeDelta.x, 0.0f))
			{
				Debug.LogError("CoreGameUtil.ScaleWorldCanvasToViewport: Trying to resize a rect transform with width 0.");
			}
			else if (Approximately(rectTransform.sizeDelta.y, 0.0f))
			{
				Debug.LogError("CoreGameUtil.ScaleWorldCanvasToViewport: Trying to resize a rect transform with height 0.");
			}
			else
			{
				float scaleToViewport = Mathf.Min(viewportWidth / rectTransform.sizeDelta.x, viewportHeight / rectTransform.sizeDelta.y);
				rectTransform.localScale = Vector3.one * scaleToViewport;
			}
		}

		public static Vector2 GetCanvasResolution(RectTransform rectTransform)
		{
			CanvasScaler parentCanvasScaler = GetComponentInHighestParent<CanvasScaler>(rectTransform.transform);
			if (parentCanvasScaler == null)
			{
				return Vector2.one;
			}
			Canvas parentCanvas = GetComponentInHighestParent<Canvas>(rectTransform.transform);
			if (parentCanvas == null)
			{
				return Vector2.one;
			}
			RectTransform parentCanvasRectTransform = parentCanvas.transform as RectTransform;
			if (parentCanvasRectTransform == null)
			{
				return Vector2.one;
			}
			return parentCanvasRectTransform.sizeDelta;
		}

		public static float GetCanvasScale(RectTransform rectTransform)
		{
			Canvas parentCanvas = GetComponentInHighestParent<Canvas>(rectTransform.transform);
			if (parentCanvas == null)
			{
				return 1.0f;
			}
			return parentCanvas.scaleFactor;
		}

		public static Vector3 GetWorldScale(Transform transform)
		{
			Vector3 scale = transform.localScale;
			Transform parent = transform.parent;
			while (parent != null)
			{
				scale = Vector3Mul(scale, parent.localScale);
				parent = parent.parent;
			}
			return scale;
		}

		public static Rect ScreenRectForRectTransform(RectTransform rectTransform, Canvas canvas = null)
        {
			if (canvas == null)
			{
				canvas = GetComponentInHighestParent<Canvas>(rectTransform.transform);
			}
			if (canvas == null)
			{
				Debug.LogError(string.Format("CoreGameUtil.ScreenRectForRectTransform: RectTransform on {0} is not a child of a canvas.", DebugUtil.GetScenePath(rectTransform.gameObject)), rectTransform);
				return default(Rect);
			}
			if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
			{
				Vector2 size = rectTransform.rect.size * canvas.scaleFactor;
				Vector2 bottomLeftCorner = (Vector2)rectTransform.position - Core.Util.Vector2Mul(rectTransform.pivot, size);
				Rect rect = new Rect(bottomLeftCorner, size);
				return rect;
			}
			else
			{
				Vector3[] corners = new Vector3[4];
				rectTransform.GetWorldCorners(corners);
				for (int i = 0; i < corners.Length; ++i)
				{
					if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
					{
						corners[i] = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, corners[i]);
					}
					else
					{
						corners[i] = RectTransformUtility.WorldToScreenPoint(Camera.main, corners[i]);
					}
				}
	            return new Rect(corners[0], corners[2] - corners[0]);
			}
        }

		public static IEnumerator Fade(Transform toFade, float alpha, float fadeTime, float startAlpha = -1f, Action<float> didSetAlpha = null)
		{
			var start = Time.realtimeSinceStartup;
			var end = start + fadeTime;
			if (startAlpha < 0f)
				toFade.ApplyRecursively(t => {
					var img = t.GetComponent<Image>();
					if (img != null)
						startAlpha = img.color.a;
					var cr = t.GetComponent<CanvasRenderer>();
					if (cr != null)
						startAlpha = cr.GetAlpha();
				});
			var deltaAlpha = alpha - startAlpha;
			while (Time.realtimeSinceStartup < end)
			{
				float progress = Mathf.Min(1f, (Time.realtimeSinceStartup - start) / fadeTime);
				float a = startAlpha + progress * deltaAlpha;
				SetAlphaRecursively(toFade, a);
				if (didSetAlpha != null)
					didSetAlpha(a);
				yield return null;
			}
			SetAlphaRecursively(toFade, alpha);
			if (didSetAlpha != null)
				didSetAlpha(alpha);
		}

		public static void SetAlphaRecursively(Transform transform, float alpha)
		{
			transform.ApplyRecursively(t =>
			{
				var image = t.GetComponent<Image>();
				var canvasRenderer = t.GetComponent<CanvasRenderer>();
				if (image != null)
				{
					var color = image.color;
					color.a = alpha;
					image.color = color;
				}
				else if (canvasRenderer != null)
				{
					canvasRenderer.SetAlpha(alpha);
				}
			});
		}

		public static IEnumerator DoForDuration(float duration, Action<float> onProgress, Action onComplete = null)
		{
			var start = Time.time;
			while (Time.time - start < duration)
			{
				var progress = Mathf.Clamp01((Time.time - start) / duration);
				if (onProgress != null)
					onProgress(progress);
				yield return null;
			}
			if (onProgress != null)
				onProgress(1f);
			if (onComplete != null)
				onComplete();
		}

		public static Color DivideColors(Color c1, Color c2)
		{
			return new Color(
				c2.r > Core.Util.EPSILON ? Mathf.Clamp01(c1.r / c2.r) : 0.0f, 
				c2.g > Core.Util.EPSILON ? Mathf.Clamp01(c1.g / c2.g) : 0.0f, 
				c2.b > Core.Util.EPSILON ? Mathf.Clamp01(c1.b / c2.b) : 0.0f, 
				c2.a > Core.Util.EPSILON ? Mathf.Clamp01(c1.a / c2.a) : 0.0f);
		}

		public static Color NewColor(Color color, float alpha)
		{
			return new Color(color.r, color.g, color.b, alpha);
		}

		public static float FramesToSeconds(int frames)
		{
			return (float)frames / FPS30;
		}
		
		public static int SecondsToFrames(float seconds)
		{
			return Mathf.RoundToInt(seconds * FPS30);
		}

		public static int FloatToInt(float value)
		{
			return Mathf.RoundToInt(value * DECIMAL_PLACES);
		}
		
		public static float IntToFloat(int value)
		{
			return (float)value / DECIMAL_PLACES;
		}

		public static float PctToFloat(int percent)
		{
			return (float)percent / 100.0f;
		}
		
		public static bool IntToBool(int value)
		{
			return value > 0;
		}
		
		public static int BoolToInt(bool value)
		{
			return value ? 1 : 0;
		}

		public static float Mod(float a, float n)
		{
			return a - (n * Mathf.FloorToInt((float)a / (float)n));
		}

		public static int Mod(int a, int n)
		{
			return a - (n * Mathf.FloorToInt((float)a / (float)n));
		}

		/// <summary> Compare lists for one element that overlaps </summary>
		public static bool Contains<T>(List<T> listA, List<T> listB)
		{
			foreach (T item in listA)
			{
				if (listB.Contains(item))
				{
					return true;
				}
			}
			return false;
		}

		public static Transform FindInTransformChildren(Transform parent, string name)
		{
			if (Core.Str.IsEmpty(name))
			{
				return null;
			}

			Transform found = parent.Find(name);
			if (found != null)
			{
				return found;
			}

			int childCount = parent.childCount;
			for (int i = 0; i < childCount; i++)
			{
				Transform child = parent.GetChild(i);
				found = FindInTransformChildren(child, name);
				if (found != null)
				{
					return found;
				}
			}
			
			return null;
		}
		
		public static bool IsTransformMyChild(Transform parent, Transform child)
		{
			while (child.parent != null)
			{
				if (child.parent.GetInstanceID() == parent.GetInstanceID())
				{
					return true;
				}
				child = child.parent;
			}
			return false;
		}

		public static readonly float DEFAULT_DPI = 160.0f;
		public static float GetDPI()
		{
			if (Application.isEditor)
			{
				return GetEditorDPI();
			}
			float dpi = Screen.dpi;
			if (Application.platform == RuntimePlatform.Android)
			{
				// https://forum.unity.com/threads/screen-dpi-on-android.414014/?_ga=2.101004169.963581864.1590768313-2134033358.1590418269
				// Get AndroidDevice DPI is the only way to get accurate dpi, Screen.dpi is only approximate on Android
				// Only problem is GetAndroidDPI() is set incorrectly on some devices and will be completely wrong
				// If GetAndroidDPI is too different from Screen.dpi then we shouldn't trust it
				// Examples	Actual	Screen.dpi
				// S7		577		560
				// Note9	516		560		
				float deviceDPI = GetAndroidDPI();
				float diff = Mathf.Abs(dpi - deviceDPI);
				dpi = diff > 100.0f ? dpi : deviceDPI;
			}
			// DPI is measured in native resolution but all of our games typically down rez.
			// We should scale DPI accordingly as input and screen positions are all relative to current resolution not native.
			float ratio = (float)Screen.height / Display.main.systemHeight;
			dpi *= ratio;
			return Approximately(dpi, 0.0f) ? DEFAULT_DPI : dpi;
		}

		public static float s_EditorDPI = 0.0f;
		public static float GetEditorDPI()
		{
#if UNITY_EDITOR
			if (Core.Util.Approximately(s_EditorDPI, 0.0f))
			{
				float dpi = Screen.dpi;
				UnityEngine.Object[] objects = Resources.FindObjectsOfTypeAll(System.Type.GetType("UnityEditor.GameView,UnityEditor"));
				if (objects.Length > 0 && objects[0] is EditorWindow gameView)
				{
					// The game view window is larger/different aspect than the actual render area
					Rect gameRect = gameView.position;
					gameRect.height -= 2.0f * UnityEditor.EditorGUIUtility.singleLineHeight; // To aprox account for window header
					float simPerReal = Screen.width > Screen.height ?
						Screen.width / gameRect.width :
						Screen.height / gameRect.height;
					s_EditorDPI = simPerReal * dpi;
				}
				else
				{
					s_EditorDPI = dpi;
				}
			}
			return s_EditorDPI;
#else
			return Screen.dpi;
#endif
		}

		private static float s_AndroidDPI = 0.0f;
		public static float GetAndroidDPI()
		{
			if (s_AndroidDPI > 0.0f)
			{
				return s_AndroidDPI;
			}
			if (Application.platform != RuntimePlatform.Android)
			{
				s_AndroidDPI = Screen.dpi;
				return s_AndroidDPI;
			}
			AndroidJavaClass activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject activity = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
			AndroidJavaObject metrics = new AndroidJavaObject("android.util.DisplayMetrics");
			activity.Call<AndroidJavaObject>("getWindowManager").Call<AndroidJavaObject>("getDefaultDisplay").Call("getMetrics", metrics);
			s_AndroidDPI = (metrics.Get<float>("xdpi") + metrics.Get<float>("ydpi")) * 0.5f;
			return s_AndroidDPI;
		}

		public static float GetDPMM()
		{
			return GetDPI() / 25.4f;
		}

		public static bool Approximately(float a, float b)
		{
			return Mathf.Abs(a - b) < EPSILON;
		}

        public static bool LessThanEquals(float a, float b)
        {
            float delta = a - b;
            bool result = delta < EPSILON;
            return result;
        }

        public static bool GreaterThanEquals(float a, float b)
        {
            float delta = a - b;
            bool result = delta > -EPSILON;
            return result;
        }

        public static bool Approximately2(Vector2 a, Vector2 b)
		{
			float sqrDist = SqrDistance2(a, b);
			return sqrDist < VECTOR_EPSILON_SQR;
		}

		public static bool Approximately(Vector3 a, Vector3 b)
		{
			float sqrDist = SqrDistance(a, b);
			return sqrDist < VECTOR_EPSILON_SQR;
		}

		public static bool ApproximatelyXZ(Vector3 a, Vector3 b)
		{
			float sqrDist = SqrDistanceXZ(a, b);
			return sqrDist < VECTOR_EPSILON_SQR;
		}

		public static bool ApproximatelyLowPrecision(float a, float b)
		{
			return Mathf.Abs(a - b) < LOW_PRECISION_EPSILON;
		}
		
		public static bool IsQuaternionIdentity(Quaternion q)
		{
			return Quaternion.Dot(q, Quaternion.identity) > 0.999f;
		}

		public static bool IsVectorZero(Vector3 v)
		{
			return Vector3.SqrMagnitude(v) < VECTOR_EPSILON_SQR;
		}

		public static bool IsVector2Zero(Vector2 v)
		{
			return Vector2.SqrMagnitude(v) < SUPER_LOW_PRECISION_EPSILON;
		}
		
		public static Quaternion XZRotation(Quaternion rotation)
		{
			return Quaternion.Euler(new Vector3(0.0f, rotation.eulerAngles.y, 0.0f));
		}

		public static Quaternion XZLookRotation(Vector3 forward)
		{
			return Quaternion.LookRotation(VectorXZ(forward), Vector3.up);
		}

		public static void ColorRenderer(GameObject obj, Color color)
		{
			if (obj == null)
			{
				return;
			}
			MeshRenderer[] renderers = obj.GetComponentsInChildren<MeshRenderer>();
			foreach (MeshRenderer renderer in renderers)
			{
				renderer.material.color = color;
			}
		}

		private static List<SkinnedMeshRenderer> s_SkinnedRenderers = new List<SkinnedMeshRenderer>();
		private static List<MeshRenderer> s_MeshRenderers = new List<MeshRenderer>();

		public static Material[] GetMaterials(GameObject obj, bool unskinned = true, bool skinned = true)
		{
			if (obj == null)
			{
				return new Material[] {};
			}
			if (skinned)
			{
				obj.GetComponentsInChildren(s_SkinnedRenderers);
			}
			if (unskinned)
			{
				obj.GetComponentsInChildren(s_MeshRenderers);
			}
			int meshCount = s_MeshRenderers.Count;
			int skinnedCount = s_SkinnedRenderers.Count;
			List<Material> materials = new List<Material>(meshCount + skinnedCount);
			for (int i = 0; i < meshCount; i++)
			{
				materials.AddRange(s_MeshRenderers[i].materials);
			}
			for (int i = 0; i < skinnedCount; i++)
			{
				materials.AddRange(s_SkinnedRenderers[i].materials);
			}
			s_MeshRenderers.Clear();
			s_SkinnedRenderers.Clear();
			return materials.ToArray();
		}

		public static void GetMaterials(List<Material> materials, GameObject obj, bool unskinned = true, bool skinned = true)
		{
			if (obj == null)
			{
				return;
			}
			if (skinned)
			{
				obj.GetComponentsInChildren(s_SkinnedRenderers);
			}
			if (unskinned)
			{
				obj.GetComponentsInChildren(s_MeshRenderers);
			}
			int meshCount = s_MeshRenderers.Count;
			int skinnedCount = s_SkinnedRenderers.Count;
			for (int i = 0; i < meshCount; i++)
			{
				materials.AddRange(s_MeshRenderers[i].materials);
			}
			for (int i = 0; i < skinnedCount; i++)
			{
				materials.AddRange(s_SkinnedRenderers[i].materials);
			}
			s_MeshRenderers.Clear();
			s_SkinnedRenderers.Clear();
		}

		public static List<Renderer> GetMeshRenderers(GameObject obj, bool unskinned = true, bool skinned = true)
		{
			if (obj == null)
			{
				return new List<Renderer>();
			}

			MeshRenderer[] meshRenderers = unskinned ?
				obj.GetComponentsInChildren<MeshRenderer>() :
				new MeshRenderer[] {};
			SkinnedMeshRenderer[] skinnedRenderers = skinned ?
				obj.GetComponentsInChildren<SkinnedMeshRenderer>() :
				new SkinnedMeshRenderer[] {};
			List<Renderer> renderers = new List<Renderer>(meshRenderers.Length + skinnedRenderers.Length);
			renderers.AddRange(meshRenderers);
			renderers.AddRange(skinnedRenderers);
			return renderers;
		}
		
		public static void SetVisible(GameObject gameObject, bool visible)
		{
			if (gameObject == null)
			{
				return;
			}
			
			//Debug.Log(visible + " " + DebugUtil.GetScenePath(gameObject) + " " + Time.time);
			
			Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>(true);
			foreach (Renderer render in renderers)
			{
				render.enabled = visible;
			}
			//		foreach (SkinnedMeshRenderer render in gameObject.GetComponentsInChildren<SkinnedMeshRenderer>())
			//		{
			//			render.enabled = visible;
			//		}
			//		foreach (MeshRenderer render in gameObject.GetComponentsInChildren<MeshRenderer>())
			//		{
			//			render.enabled = visible;
			//		}
			//		foreach (ParticleSystem particles in gameObject.GetComponentsInChildren<ParticleSystem>())
			//		{
			//			if (!visible && particles.isPlaying)
			//			{
			//				particles.Stop();
			//			}
			//			else if (visible && particles.isStopped)
			//			{
			//				particles.Play();
			//			}
			//		}
		}

		public static void SetMeshesVisible(GameObject gameObject, bool visible)
		{
			foreach (SkinnedMeshRenderer render in gameObject.GetComponentsInChildren<SkinnedMeshRenderer>())
			{
				render.enabled = visible;
			}
			foreach (MeshRenderer render in gameObject.GetComponentsInChildren<MeshRenderer>())
			{
				render.enabled = visible;
			}
		}

		public static float Normalize(ref Vector3 v, bool safe = false)
		{
			float mag = Mathf.Sqrt(v.x * v.x + v.y * v.y + v.z * v.z);
			if (mag < EPSILON)
			{
				Debug.Assert(safe, "GameUtil.Normalize() Passed zero vector, this is probably bad...");
				return EPSILON;
			}
			v /= mag; // Normalize vector
			return mag;
		}
		
		public static float Normalize2(ref Vector2 v, bool safe = false)
		{
			float mag = Mathf.Sqrt(v.x * v.x + v.y * v.y);
			if (mag < EPSILON)
			{
				Debug.Assert(safe, "GameUtil.Normalize2() Passed zero vector, this is probably bad...");
				return EPSILON;
			}
			v /= mag; // Normalize vector
			return mag;
		}

		public static float NormalizeXZ(ref Vector3 v, bool safe = false)
		{
			v.y = 0.0f;
			float mag = Mathf.Sqrt(v.x * v.x + v.z * v.z);
			if (mag < EPSILON)
			{
				Debug.Assert(safe, "GameUtil.NormalizeXZ() Passed zero vector, this is probably bad...");
				return EPSILON;
			}
			v /= mag; // Normalize vector
			return mag;
		}

		public static bool IsEnumInMask(int test, int mask)
		{
			return (mask & 1 << test) > 0;
		}
		
		public static float SqrDistance(Vector3 v1, Vector3 v2)
		{
			Vector3 v = v1 - v2;
			return v.x * v.x + v.y * v.y + v.z * v.z;
		}

		public static float SqrDistanceXZ(Vector3 v1, Vector3 v2)
		{
			Vector3 v = v1 - v2;
			return v.x * v.x + v.z * v.z;
		}

		public static float DistanceXZ(Vector3 v1, Vector3 v2)
		{
			Vector3 v = v1 - v2;
			return Mathf.Sqrt(v.x * v.x + v.z * v.z);
		}

		public static float SqrDistance2(Vector2 v1, Vector2 v2)
		{
			Vector2 v = v1 - v2;
			return v.x * v.x + v.y * v.y;
		}

		public static float MagnitudeXZ(Vector3 v)
		{
			return Mathf.Sqrt(v.x * v.x + v.z * v.z);
		}

		public static float SqrMagnitudeXZ(Vector3 v)
		{
			return v.x * v.x + v.z * v.z;
		}

		public static bool QuaternionEquals(Quaternion q1, Quaternion q2)
		{
			return Core.Util.ApproximatelyLowPrecision(q1.w, q2.w)
				&& Core.Util.ApproximatelyLowPrecision(q1.x, q2.x)
				&& Core.Util.ApproximatelyLowPrecision(q1.y, q2.y)
				&& Core.Util.ApproximatelyLowPrecision(q1.z, q2.z);
		}

		public static bool ColorEquals(Color c1, Color c2)
		{
			Color c = c2 - c1;
			return c.r < LOW_PRECISION_EPSILON && c.r > -LOW_PRECISION_EPSILON
				&& c.g < LOW_PRECISION_EPSILON && c.g > -LOW_PRECISION_EPSILON
				&& c.b < LOW_PRECISION_EPSILON && c.b > -LOW_PRECISION_EPSILON
				&& c.a < LOW_PRECISION_EPSILON && c.a > -LOW_PRECISION_EPSILON;
		}

		public static bool ColorRGBEquals(Color c1, Color c2)
		{
			Color c = c2 - c1;
			return c.r < LOW_PRECISION_EPSILON && c.r > -LOW_PRECISION_EPSILON
				&& c.g < LOW_PRECISION_EPSILON && c.g > -LOW_PRECISION_EPSILON
				&& c.b < LOW_PRECISION_EPSILON && c.b > -LOW_PRECISION_EPSILON;
		}

		public static bool VectorEquals(Vector2 v1, Vector2 v2)
		{
			Vector2 v = v1 - v2;
			return v.x < LOW_PRECISION_EPSILON && v.x > -LOW_PRECISION_EPSILON
				&& v.y < LOW_PRECISION_EPSILON && v.y > -LOW_PRECISION_EPSILON;
		}

		public static bool VectorEquals(Vector3 v1, Vector3 v2)
		{
			Vector3 v = v1 - v2;
			return v.x < LOW_PRECISION_EPSILON && v.x > -LOW_PRECISION_EPSILON
				&& v.y < LOW_PRECISION_EPSILON && v.y > -LOW_PRECISION_EPSILON
				&& v.z < LOW_PRECISION_EPSILON && v.z > -LOW_PRECISION_EPSILON;
		}

		public static bool VectorEquals(Vector4 v1, Vector4 v2)
		{
			Vector4 v = v1 - v2;
			return v.x < LOW_PRECISION_EPSILON && v.x > -LOW_PRECISION_EPSILON
				&& v.y < LOW_PRECISION_EPSILON && v.y > -LOW_PRECISION_EPSILON
				&& v.z < LOW_PRECISION_EPSILON && v.z > -LOW_PRECISION_EPSILON
				&& v.w < LOW_PRECISION_EPSILON && v.w > -LOW_PRECISION_EPSILON;
		}

		public static bool VectorEqualsXZ(Vector3 v1, Vector3 v2)
		{
			Vector3 v = v1 - v2;
			return v.x < LOW_PRECISION_EPSILON && v.x > -LOW_PRECISION_EPSILON
				&& v.z < LOW_PRECISION_EPSILON && v.z > -LOW_PRECISION_EPSILON;
		}

		public static Vector3 RotateXZ(Vector3 v, float radians)
		{
			float sin = Mathf.Sin(radians);
			float cos = Mathf.Cos(radians);
			v.x = cos * v.x - sin * v.z;
			v.z = sin * v.x + cos * v.z;
			return v;
		}
		
		public static Vector2 Rotate2D(Vector2 v, float radians)
		{
			float sin = Mathf.Sin(radians);
			float cos = Mathf.Cos(radians);
			return new Vector2(
				cos * v.x - sin * v.y,
				sin * v.x + cos * v.y);
		}

		public static Vector3 Vector3Div(Vector3 a, Vector3 div)
		{
			return new Vector3(a.x / div.x, a.y / div.y, a.z / div.z);
		}

		public static Vector3 Vector3Mul(Vector3 a, Vector3 b)
		{
			return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
		}

		public static Vector2 Vector2Mul(Vector2 a, Vector2 b)
		{
			return new Vector2(a.x * b.x, a.y * b.y);
		}

		public static Vector2 Vector3To2(Vector3 v)
		{
			return new Vector2(v.x, v.z);
		}

		public static Vector3 Vector2To3(Vector2 v, float height = 0.0f)
		{
			return new Vector3(v.x, height, v.y);
		}

		public static Vector3 VectorXZ(Vector3 v, float height = 0.0f)
		{
			v.y = height;
			return v;
		}
		
		public static Vector3 Saturate(Vector3 v)
		{
			return Vector3.Max(Vector3.Min(v, Vector3.one), Vector3.zero);
		}
		
		public static Vector3 Clamp(Vector3 v, float min, float max)
		{
			return Vector3.Max(Vector3.Min(v, max * Vector3.one), min * Vector3.one);
		}

		public static int Loop(int value, int min, int max, int maxIterations = 1)
		{
			if (maxIterations <= 0)
			{
				return value;
			}

			if (value < min)
			{
				return Loop(max - (min - value - 1), min, max, --maxIterations);
			}
			else if (value > max)
			{
				return Loop(min + (value - max - 1), min, max, --maxIterations);
			}
			return value;
		}

		public static Vector3 Vector3Abs(Vector3 v)
		{
			return new Vector3(
				v.x < 0.0f ? -v.x : v.x,
				v.y < 0.0f ? -v.y : v.y,
				v.z < 0.0f ? -v.z : v.z);
		}
		
		public static Vector3 ScreenToGround(Vector3 screenPos, Camera camera = null)
		{
			if (camera == null)
			{
				camera = Camera.main;
			}
			Ray r = camera.ScreenPointToRay(screenPos);
			return RayToGround(r);
		}

		public static Vector3 ViewportToGround(Vector3 viewportPos, Camera camera = null)
		{
			if (camera == null)
			{
				camera = Camera.main;
			}
			Ray r = camera.ViewportPointToRay(viewportPos);
			return RayToGround(r);
		}

		public static float RayDistanceToGround(Ray r)
		{
			float nl = Vector3.Dot(r.direction, Vector3.up);
			if (Approximately(nl, 0.0f))
			{
				return 0.0f;
			}
			float d = Vector3.Dot((Vector3.zero - r.origin), Vector3.up) / nl;
			return d;
		}

		public static Vector3 RayToGround(Ray r)
		{
			float nl = Vector3.Dot(r.direction, Vector3.up);
			if (Approximately(nl, 0.0f))
			{
				return Vector3.zero;
			}
			float d = Vector3.Dot((Vector3.zero - r.origin), Vector3.up) / nl;
			return r.GetPoint(d);
		}

		public enum DuplicateBehaviourMode
		{
			Duplicate = 0,
			UseMine,
			UseShared,
		}
		public static Component DuplicateBehaviour(
			GameObject owner, 
			MonoBehaviour behaviour, 
			DuplicateBehaviourMode allowDuplicates = DuplicateBehaviourMode.Duplicate)
		{
			if (owner == null)
			{
				Debug.LogError("Core.Util.DuplicateBehaviour() Owner is null");
				return null;
			}
			if (behaviour == null)
			{
				Debug.LogError("Core.Util.DuplicateBehaviour() Behaviour is null");
				return null;
			}
			System.Type type = behaviour.GetType();
			Component destination = null;
			Component baseComponent = owner.GetComponent(type);
			if (baseComponent != null)
			{
				switch (allowDuplicates)
				{
					case DuplicateBehaviourMode.Duplicate:
					{
						destination = owner.AddComponent(type);
						break;
					}
					case DuplicateBehaviourMode.UseShared:
					{
						destination = baseComponent;
						break;
					}
					case DuplicateBehaviourMode.UseMine:
					{
						return baseComponent;
					}
				}
			}
			else
			{
				destination = owner.AddComponent(type);
			}
			if (destination == null)
			{
				return null;
			}
			FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
			foreach (FieldInfo field in fields)
			{
				field.SetValue(destination, field.GetValue(behaviour));
			}
			return destination;
		}
		public static void DuplicateBehaviours(
			GameObject copyTo, 
			GameObject copyFrom,
			DuplicateBehaviourMode allowDuplicates = DuplicateBehaviourMode.Duplicate)
		{
			MonoBehaviour[] behaviours = copyFrom.GetComponents<MonoBehaviour>();
			foreach (MonoBehaviour behaviour in behaviours)
			{
				if (behaviour != null) // I don't know how this is possible but it is?
				{
					Core.Util.DuplicateBehaviour(copyTo, behaviour, allowDuplicates);
				}
			}
		}

		public static T CopyBehaviour<T>(T from, GameObject to, bool allowDuplicates = true) where T : MonoBehaviour
		{
			System.Type type = from.GetType();
			if (!allowDuplicates && to.GetComponent(type) != null)
			{
				return null;
			}

			T destination = (T)to.AddComponent(type);
			FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);

			foreach (FieldInfo field in fields)
			{
				field.SetValue(destination, field.GetValue(from));
			}

			return destination;
		}

		public static Vector3 LocalPointToGlobal(Vector3 localPoint, Transform parentTransform)
		{
			return (parentTransform.rotation * localPoint) + parentTransform.position;
		}
		
		public static Quaternion LocalRotationToGlobal(Quaternion localRotation, Transform parentTransform)
		{
			return parentTransform.rotation * localRotation;
		}
		
		public static Vector3 GlobalPointToLocal(Vector3 globalPoint, Transform parentTransform)
		{
			return Quaternion.Inverse(parentTransform.rotation) * (globalPoint - parentTransform.position);
		}
		
		public static Quaternion GlobalRotationToLocal(Quaternion globalRotation, Transform parentTransform)
		{
			return Quaternion.Inverse(parentTransform.rotation) * globalRotation;
		}

		public static float Cross2(Vector2 v1, Vector2 v2)
		{
			return (v1.y * v2.x) - (v1.x * v2.y);
		}

		public static Vector3 LerpArc1(Vector3 start, Vector3 end, float angle, float delta)
		{
			Vector3 toTarget = (end - start);
			Vector3 binormal = Vector3.Cross(toTarget.normalized, Vector3.up).normalized;
			Vector3 tangent = (Quaternion.AngleAxis(angle, binormal) * toTarget).normalized;
			toTarget.y = 0;
			float hypotenuse = toTarget.magnitude / 2.0f / Vector3.Dot(toTarget.normalized, tangent);
			Vector3 controlPoint = start + (tangent * hypotenuse);

			Vector3 intermediatePoint0 = Vector3.Lerp(start, controlPoint, delta);
			Vector3 intermediatePoint1 = Vector3.Lerp(controlPoint, end, delta);
			return Vector3.Lerp(intermediatePoint0, intermediatePoint1, delta);
		}

		public static Vector3 LerpArc2(Vector3 start, Vector3 end, float heightPercent, float delta)
		{
			return LerpArc3(start, end, heightPercent, 0.5f, delta);
		}

		public static Vector3 LerpArc3(Vector3 start, Vector3 end, float heightPercent, float apexPercent, float delta)
		{
			return LerpArc3(start, end, Vector3.up, heightPercent, apexPercent, delta);
		}

		public static Vector3 LerpArc3(Vector3 start, Vector3 end, Vector3 normal, float heightPercent, float apexPercent, float delta)
		{
			if (Approximately(heightPercent, 0.0f))
			{
				return Vector3.Lerp(start, end, delta);
			}

			float height = heightPercent * Vector3.Distance(start, end);
			Vector3 controlPoint = Vector3.LerpUnclamped(start, end, apexPercent) + (normal * height);

			Vector3 intermediatePoint0 = Vector3.Lerp(start, controlPoint, delta);
			Vector3 intermediatePoint1 = Vector3.Lerp(controlPoint, end, delta);
			return Vector3.Lerp(intermediatePoint0, intermediatePoint1, delta);
		}

		public static Vector2 Lerp2DArc4(Vector2 start, Vector2 end, float heightPercent, float apexPercent, float delta)
		{
			if (Approximately(Vector2.Distance(start, end), 0.0f))
			{
				return end;
			}
			if (Approximately(heightPercent, 0.0f))
			{
				return Vector2.Lerp(start, end, delta);
			}

			Vector2 forward = (end - start).normalized;
			Vector2 arcNormal = new Vector2(forward.y, -forward.x);
			if (forward.y < 0.0f || Core.Util.Approximately(Mathf.Sign(forward.x), Mathf.Sign(forward.y)))
			{
				arcNormal *= -1.0f;
			}
			
			float height = heightPercent * Vector2.Distance(start, end);
			Vector2 controlPoint = Vector2.LerpUnclamped(start, end, apexPercent) + (arcNormal * height);

			Vector2 intermediatePoint0 = Vector2.Lerp(start, controlPoint, delta);
			Vector2 intermediatePoint1 = Vector2.Lerp(controlPoint, end, delta);
			return Vector2.Lerp(intermediatePoint0, intermediatePoint1, delta);
		}

		public static Vector2 ProjectOntoRay2D(Vector2 point, Ray2D ray)
		{
			Vector2 hypotenuse = point - ray.origin;
			float distance = Vector2.Dot(ray.direction, hypotenuse);
			return ray.GetPoint(distance);
		}

		public static bool RayRayIntersection2D(out Vector2 intersection, Ray2D ray1, Ray2D ray2)
		{
			float dot = Vector2.Dot(ray1.direction.normalized, ray2.direction.normalized);
			if (Approximately(dot, 1.0f) || Approximately(dot, -1.0f))
			{
				intersection = Vector2.zero;
				return false;
			}

			float distance = (ray1.origin.y * ray2.direction.x + ray2.direction.y * ray2.origin.x - ray2.origin.y * ray2.direction.x - ray2.direction.y * ray1.origin.x)
				/ (ray1.direction.x * ray2.direction.y - ray1.direction.y * ray2.direction.x);
			intersection = ray1.origin + ray1.direction.normalized * distance;
			return distance > 0.0f;
		}

		public static bool RayRayIntersection(out Vector3 intersection, Ray ray1, Ray ray2)
		{

			intersection = Vector3.zero;

			Vector3 triangleHypotenuse = ray2.origin - ray1.origin;
			Vector3 planeNormal = Vector3.Cross(ray1.direction, ray2.direction);
			Vector3 triangleNormal = Vector3.Cross(triangleHypotenuse, ray2.direction);

			float planarFactor = Vector3.Dot(triangleHypotenuse, planeNormal);

			//Lines are not coplanar if plane normal and hypotenuse does not make a right angle.
			if ((planarFactor >= 0.00001f) || (planarFactor <= -0.00001f))
			{
				return false;
			}

			float adjacentSideLength = Vector3.Dot(triangleNormal, planeNormal) / planeNormal.sqrMagnitude;
			intersection = ray1.GetPoint(adjacentSideLength);

			return true;
		}

		public static bool InternalHomotheticCenter(Circle2D circle0, Circle2D circle1, out Vector2 ihCenter)
		{
			Circle2D biggerCircle, smallerCircle;
			if (circle0.radius > circle1.radius)
			{
				biggerCircle = circle0;
				smallerCircle = circle1;
			}
			else
			{
				biggerCircle = circle1;
				smallerCircle = circle0;
			}

			float radiiSum = biggerCircle.radius + smallerCircle.radius;
			ihCenter = biggerCircle.center * (smallerCircle.radius / radiiSum) + smallerCircle.center * (biggerCircle.radius / radiiSum);

			return true;
		}

		public static bool ExternalHomotheticCenter(Circle2D circle0, Circle2D circle1, out Vector2 ehCenter)
		{
			Circle2D biggerCircle, smallerCircle;
			if (circle0.radius > circle1.radius)
			{
				biggerCircle = circle0;
				smallerCircle = circle1;
			}
			else
			{
				biggerCircle = circle1;
				smallerCircle = circle0;
			}

			float radiiDifference = biggerCircle.radius - smallerCircle.radius;

			if (Approximately(radiiDifference, 0.0f))
			{
				ehCenter = Vector2.zero;
				return false;
			}

			ehCenter = biggerCircle.center * (-smallerCircle.radius / radiiDifference) + smallerCircle.center * (biggerCircle.radius / radiiDifference);

			return true;
		}

		public static Vector2[] TangetPoints(Circle2D circle, Vector2 collinearPoint)
		{
			Circle2D intersectingCircle = new Circle2D(Vector2.Lerp(collinearPoint, circle.center, 0.5f), Vector2.Distance(collinearPoint, circle.center) / 2.0f);

			Vector2[] tangentPoints = circle.Intersection(intersectingCircle);

			return tangentPoints;
		}

		public static bool IsEven(int i)
		{
			return i % 2 == 0;
		}
		public static bool IsOdd(int i)
		{
			return !IsEven(i);
		}

		public static void SetLayer(GameObject obj, int layer, bool recursive = true)
		{
			obj.layer = layer;
			Transform t = obj.transform;
			int length = t.childCount;
			for (int i = 0; i < length; i++)
			{
				SetLayer(t.GetChild(i).gameObject, layer, recursive);
			}
		}

		public static void SetLayer(GameObject obj, int layer, bool recursive, int[] untouchableLayers)
		{
			bool layerCanBeSet = true;
			for (int i = 0; i < untouchableLayers.Length; i++)
			{
				if (obj.layer == untouchableLayers[i])
				{
					layerCanBeSet = false;
					break;
				}
			}
			if (layerCanBeSet)
			{
				obj.layer = layer;
			}
			foreach (Transform child in obj.transform)
			{
				SetLayer(child.gameObject, layer, recursive);
			}
		}

		public static void SetLayer(GameObject obj, string layer, bool recursive = true)
		{
			SetLayer(obj,LayerMask.NameToLayer(layer), recursive);
		}

		public static long MaxLong(long a, long b)
		{
			return a > b ? a : b;
		}

		public static long MinLong(long a, long b)
		{
			return a < b ? a : b;
		}

		public static long ClampLong(long value, long min, long max)
		{
			return 
				value < min ? min : 
				value > max ? max : 
				value;
		}

		public static void ColorParticles(GameObject obj, Color color)
		{
			ParticleSystem[] systems = obj.GetComponentsInChildren<ParticleSystem>();
			foreach (ParticleSystem system in systems)
			{
				ParticleSystem.ColorOverLifetimeModule lifecolor = system.colorOverLifetime;
				lifecolor.color = new ParticleSystem.MinMaxGradient(color);
				lifecolor.enabled = true;
			}
		}

		// TODO: I don't think this function works quite right yet
		public static void AngleAxis(
			Vector3 v1, 
			Vector3 v2,
			out float angle, 
			out Vector3 axis)
		{
			float dot = Vector3.Dot(v1, v2);
			if (dot > 1.0f - EPSILON)
			{
				angle = 0.0f;
				axis = Vector3.up;
				return;
			}
			if (dot < -1.0f + EPSILON)
			{
				angle = 180.0f;
				axis = Vector3.up;
				return;
			}
			axis = Vector3.Cross(v1, v2);
			Normalize(ref axis);
			//Vector3 right = Vector3.Cross(v1, axis);
			float sign = 1.0f; // Mathf.Sign(Vector3.Dot(v2, right));
			angle = sign * ACos(dot);
		}

		public static T GetOrAddComponent<T>(GameObject obj) where T : Component
		{
			T component = obj.GetComponent<T>();
			if (component == null)
			{
				component = obj.AddComponent<T>();
			}
			return component;
		}

		public static T GetOrAddComponentInChildren<T>(GameObject obj) where T : Component
		{
			if (obj.TryGetComponentInChildren(out T component))
			{
				return component;
			}
			return obj.AddComponent<T>();
		}

		public static float TruncateDecimalPlaces(float value, int places)
		{
			// not sure if you care to handle negative numbers...       
			float f = 1;
			for (int i = 0; i < places; i++)
			{
				f *= 10;
			}
			return Mathf.Floor(value * f) / f;
		}

		public static string BytesToString(byte[] bytes)
		{
			string s = null;
			foreach (byte b in bytes)
			{
				s += b.ToString("x2");
			}
			return s;
		}
	}

	public struct Circle2D
	{
		public Vector2 center;
		public float radius;
		public float sqrRadius { get { return radius * radius; } }

		public Circle2D(Vector2 center, float radius)
		{
			this.center = center;
			this.radius = radius;
		}

		public Vector2[] GetArc(Vector2 startPoint, Vector2 endPoint, float intervalLength, bool clockwise = true)
		{
			// ensure all points ar on the circle.
			if (!Util.Approximately(Vector2.Distance(startPoint, center), radius))
			{
				Vector2 invNormal = (startPoint - center).normalized;
				startPoint = center + invNormal * radius;
			}
			if (!Util.Approximately(Vector2.Distance(endPoint, center), radius))
			{
				Vector2 invNormal = (endPoint - center).normalized;
				endPoint = center + invNormal * radius;
			}

			List<Vector2> points = new List<Vector2>();

			points.Add(startPoint);
			while (Vector2.Distance(points[points.Count - 1], endPoint) > intervalLength && points.Count < 9999)
			{
				Vector2 normal = (center - points[points.Count - 1]).normalized;
				Vector2 tangent = new Vector2(-normal.y, normal.x) * (clockwise ? 1.0f : -1.0f);
				Vector2 newNormal = (center - (points[points.Count - 1] + (tangent * intervalLength))).normalized;
				Vector2 newPoint = center - (newNormal * radius);
				points.Add(newPoint);
			}
			points.Add(endPoint);

			return points.ToArray();
		}

		public Vector2[] GetCircle(float intervalLength)
		{
			List<Vector2> points = new List<Vector2>();
			Vector2 right = center + new Vector2(radius, 0.0f);
			Vector2 left = center + new Vector2(-radius, 0.0f);
			points.AddRange(GetArc(right, left, intervalLength));
			points.AddRange(GetArc(left, right, intervalLength));
			return points.ToArray();
		}

		public Vector2[] Intersection(Ray2D intersectingRay)
		{
			Vector2 toCenter = intersectingRay.origin - center;

			float a = (intersectingRay.direction.x * intersectingRay.direction.x) + (intersectingRay.direction.y * intersectingRay.direction.y);
			float b = 2 * ((intersectingRay.direction.x * toCenter.x) + (intersectingRay.direction.y * toCenter.y));
			float c = (toCenter.x * toCenter.x) + (toCenter.y * toCenter.y) - (radius * radius);
			float delta = b * b - (4 * a * c);
			float sqrtDelta = Mathf.Sqrt(delta);

			// no intersection.
			if (delta < 0)
			{
				return new Vector2[0];
			}

			float distance0 = (-b + sqrtDelta) / (2 * a);
			float distance1 = (-b - sqrtDelta) / (2 * a);

			// ray is inside the circle or tangent to it => one intersection.
			if (Util.Approximately(delta, 0.0f) || toCenter.magnitude < radius)
			{
				return new Vector2[] { intersectingRay.GetPoint(distance0) };
			}

			return new Vector2[] { intersectingRay.GetPoint(distance0), intersectingRay.GetPoint(distance1) };
		}

		public Vector2[] Intersection(Circle2D intersectingCircle)
		{
			float distance = Vector2.Distance(center, intersectingCircle.center);

			if (Util.Approximately(distance, radius + intersectingCircle.radius))
			{
				// tangent circles.
				return new Vector2[] { Vector2.Lerp(center, intersectingCircle.center, radius / (radius + intersectingCircle.radius)) };
			}
			if (distance + Mathf.Min(radius, intersectingCircle.radius) < Mathf.Max(radius, intersectingCircle.radius) || distance > (radius + intersectingCircle.radius))
			{
				// no points of intersection.
				return new Vector2[0];
			}

			Vector2 centerToMidpoint = (intersectingCircle.center - center).normalized;
			float distanceToMidpoint = (sqrRadius - intersectingCircle.sqrRadius + (distance * distance)) / (2 * distance);
			Vector2 midpoint = center + centerToMidpoint * distanceToMidpoint;
			float distanceToIntersectionPoint = Mathf.Sqrt(sqrRadius - (distanceToMidpoint * distanceToMidpoint));

			Vector2[] intersectionPoints = new Vector2[2];
			intersectionPoints[0] = midpoint + new Vector2(centerToMidpoint.y, -centerToMidpoint.x) * distanceToIntersectionPoint;
			intersectionPoints[1] = midpoint + new Vector2(-centerToMidpoint.y, centerToMidpoint.x) * distanceToIntersectionPoint;

			return intersectionPoints;
		}
	}
}
