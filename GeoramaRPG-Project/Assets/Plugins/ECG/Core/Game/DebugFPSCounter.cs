
using UnityEngine;

namespace Core
{
	public class DebugFPSCounter : MonoBehaviour
	{
#if !STABLE
	    // Attach this to any object to make a frames/second indicator.
	    //
	    // It calculates frames/second over each updateInterval,
	    // so the display does not keep changing wildly.
	    //
	    // It is also fairly accurate at very low FPS counts (<10).
	    // We do this not by simply counting frames per interval, but
	    // by accumulating FPS for each frame. This way we end up with
	    // corstartRect overall FPS even if the interval renders something like
	    // 5.5 frames.

		static readonly string DISPLAY_STRING = "f0";
	 
	    Rect m_Rect = new Rect(10.0f, 10.0f, 120.0f, 75.0f); // The rect the window is initially displayed at.
	    float m_Frequency = 10.0f; // The update frequency of the fps
		float m_MinMaxFrequency = 10.0f;
	     
	    float m_Time = 0.0f;
		float m_Time2 = 0.0f;
		float m_TimeLastFrame = 0.0f;
	    int m_Frames  = 0; // Frames drawn over the interval
	    GUIStyle m_Style; // The style the text will be displayed at, based en defaultSkin.label.
		
		protected float m_Low = 0.01f;
		protected float m_High = 0.1f;

		protected float m_TimeSinceStart = 0.0f;
		protected float m_TimeSpentAbove5FPS = 0.0f;
		protected float m_TimeSpentAbove10FPS = 0.0f;
		protected float m_TimeSpentAbove15FPS = 0.0f;
		protected float m_TimeSpentAbove30FPS = 0.0f;
	     
	    void Start()
	    {
			m_Rect = new Rect(10, 10, 120, 50);
	    }
	     
	    void Update()
	    {
			m_Rect.x = 0;
			m_Rect.y =  0.25f * Screen.height - m_Rect.height;

			float timeThisFrame = Time.realtimeSinceStartup - m_TimeLastFrame;

			if (timeThisFrame < m_High)
			{
				m_High = timeThisFrame;
			}
			if (timeThisFrame > m_Low)
			{
				//if (Time.deltaTime > 1.0f / GameUtil.FPS30)
				//{
				//	Debug.Log("HUDFPS.Update() NEW LOW " + (1.0f / Time.deltaTime) + "!!!! " + Time.time);
				//}
				m_Low = timeThisFrame;
			}
			
			m_Time += timeThisFrame;
			m_Frames++;

			if (m_Time > m_Frequency)
			{
				m_Time = timeThisFrame;
				m_Frames = 1;
			}

			m_Time2 += timeThisFrame;
			if (m_Time2 > m_MinMaxFrequency)
			{
				m_Time2 = timeThisFrame;
				m_Low = timeThisFrame;
				m_High = timeThisFrame;
			}

			m_TimeLastFrame = Time.realtimeSinceStartup;

			m_TimeSinceStart += timeThisFrame;
			if (1.0f / timeThisFrame >= 5.0f - Core.Util.SUPER_LOW_PRECISION_EPSILON)
			{
				m_TimeSpentAbove5FPS += timeThisFrame;
			}
			if (1.0f / timeThisFrame >= 10.0f - Core.Util.SUPER_LOW_PRECISION_EPSILON)
			{
				m_TimeSpentAbove10FPS += timeThisFrame;
			}
			if (1.0f / timeThisFrame >= 15.0f - Core.Util.SUPER_LOW_PRECISION_EPSILON)
			{
				m_TimeSpentAbove15FPS += timeThisFrame;
			}
			if (1.0f / timeThisFrame >= 30.0f - Core.Util.SUPER_LOW_PRECISION_EPSILON)
			{
				m_TimeSpentAbove30FPS += timeThisFrame;
			}
	    }
	    
	    void OnGUI()
	    {
	        // Copy the default label skin, change the color and the alignement
	        if (m_Style == null)
			{
	            m_Style = new GUIStyle(GUI.skin.label);
	            m_Style.normal.textColor = Color.white;
	            m_Style.alignment = TextAnchor.MiddleCenter;
	        }
	        
			float fps = m_Frames / m_Time;
			GUI.color = (fps >= 30.0f) ? Color.green : ((fps >= 10.0f) ? Color.yellow : Color.red);

			GUILayout.BeginArea(m_Rect, GUI.skin.box);
			//Debug.Log(mFrames + " " + mAccum + " " + mLow + " " + mHigh);
			float minFps = 1.0f / m_Low;
			float maxFps = 1.0f / m_High;

			float width = m_Rect.width;
			float height = m_Rect.height / 3.0f;

			GUI.Label(
				new Rect(0.0f, 0.0f, width, height), 
				fps.ToString(DISPLAY_STRING) + " FPS (" + minFps.ToString(DISPLAY_STRING) + "," + maxFps.ToString(DISPLAY_STRING) + ")", 
				m_Style);

			GUI.Label(
				new Rect(0.0f, 1.0f * height, width, height), 
				Screen.width + " x " + Screen.height,
				m_Style);

			if (Core.TimeScaleManager.Get() != null)
			{
				GUI.Label(
					new Rect(0.0f, 2.0f * height, width, height), 
					Core.TimeScaleManager.Get().GetEditorSlowMo() + "% ", 
					m_Style);
			}
			GUILayout.EndArea();
	    }
#endif
	}
}
