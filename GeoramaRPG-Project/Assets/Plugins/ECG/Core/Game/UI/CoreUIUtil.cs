
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
	public static class UIUtil
	{
		public static string VertsToString(int verts)
		{
			return verts < 1000 ? verts.ToString() :
				((float)verts / 1000).ToString("F1") + "k";
		}

		public static string BytesToMB(ulong bytes)
		{
			double mb = (double)bytes / 1024.0 / 1024.0;
			return mb.ToString("F2");
		}

		// Same formating Unity uses for sizes in the editor
		public static string BytesToString(ulong bytes)
		{
			double kb = (double)bytes / 1024.0;
			double mb = kb / 1024.0;
			if (mb < 0.5)
			{
				return Str.Build(Mathf.RoundToInt((float)kb).ToString(), "KB");
			}
			else
			{
				return Str.Build(mb.ToString("F1"), "MB");
			}
		}

		public static long MbToBytes(float mb)
		{
			return (long)(mb * 1024 * 1024);
		}

		public static string SecondsToString(float time)
		{
			if (time > 86400.0f)
			{
				return SecondsToStringDays(time);
			}
			if (time > 3600.0f)
			{
				return SecondsToStringHours(time);
			}
			if (time > 60.0f)
			{
				return SecondsToStringMinutes(time);
			}
			return SecondsToStringSeconds(time);
		}

		public static string SecondsToStringSeconds(float time)
		{
			int t = Mathf.FloorToInt(time);
			return string.Format("{0}s", t);
		}

		public static string SecondsToStringMinutes(float time)
		{
			int t = Mathf.FloorToInt(time);
			int seconds = t % 60;
			int minutes = t / 60;
			return string.Format("{0}m {1}s", minutes, seconds);
		}

		public static string SecondsToStringHours(float time)
		{
			int t = Mathf.FloorToInt(time);
			int seconds = t % 60;
			int minutes = t / 60;
			int hours = minutes / 60;
			minutes -= hours * 60;
			return string.Format("{0}h {1}m", hours, minutes);
		}

		public static string SecondsToStringDays(float time)
		{
			int t = Mathf.FloorToInt(time);
			int seconds = t % 60;
			int minutes = t / 60;
			int hours = minutes / 60;
			minutes -= hours * 60;
			int days = hours / 24;
			hours -= days * 24;
			return string.Format("{0}d {1}h", days, hours);
		}

		private static Material s_GreyscaleMat = null;
		private static Material GetGreyscaleMat()
		{
			if (s_GreyscaleMat != null)
			{
				return s_GreyscaleMat;
			}
			const string shaderName = "UI/Greyscale";
			Shader shader = Shader.Find(shaderName);
			if (shader == null)
			{
				Debug.LogErrorFormat("Core.UIUtil.GetGreyscaleMat() Unable to find Shader! {0}", shaderName);
				return null;
			}
			s_GreyscaleMat = new Material(shader);
			return s_GreyscaleMat;
		}

		public static void SetGreyscale(MaskableGraphic image, bool grey, float brightness = 1.0f, Color tint = new Color())
		{
			SetGreyscale(image, grey ? 1.0f : 0.0f, brightness, tint);
		}

		public static void SetGreyscale(MaskableGraphic image, float amount = 1.0f, float brightness = 1.0f, Color tint = new Color())
		{
			bool apply = amount > Core.Util.EPSILON;
			image.material = apply ? GetGreyscaleMat() : null;
			if (apply && image.material != null)
			{
				if (!Core.Util.Approximately(amount, 1.0f))
				{
					image.material.SetFloat("_GreyscaleAmount", amount);
				}
				if (!Core.Util.Approximately(brightness, 1.0f))
				{
					image.material.SetFloat("_Brightness", brightness);
				}
				if (!Color.Equals(tint, new Color()))
				{
					image.material.SetColor("_Tint", tint);
				}
			}
		}

		public static void SetGreyscale(GameObject gameObject, bool grey, float brightness = 1.0f, Color tint = new Color())
		{
			foreach (MaskableGraphic graphic in gameObject.GetComponentsInChildren<MaskableGraphic>())
			{
				SetGreyscale(graphic, grey, brightness, tint);
			}
		}
	}
}
