using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace OliverLoescher
{
	public class PauseMenu : MonoBehaviour
	{
		[SerializeField]
		private GameObject menuObject = null;

		#region Initialize
		private void Start()
		{
			Application.focusChanged += OnFocusLost;
			InputSystem.Instance.Menu.Pause.performed += OnInput;

			menuObject.SetActive(false);
			Cursor.lockState = CursorLockMode.Locked;
		}

		private void OnDestroy()
		{
			Application.focusChanged -= OnFocusLost;
			InputSystem.Instance.FPS.CameraMove.performed -= OnInput;
		}

		private void OnEnable()
		{
			InputSystem.Instance.Menu.Enable();
		}

		private void OnDisable()
		{
			InputSystem.Instance.Menu.Disable();
		}

		public void OnInput(InputAction.CallbackContext ctx) => TogglePause();
		#endregion

		public void OnFocusLost(bool pFocused)
		{
			if (pFocused == false && PauseSystem.isPaused == false)
			{
				OnPause();
			}
		}

		public void TogglePause()
		{
			if (PauseSystem.isPaused)
			{
				OnUnpause();
			}
			else
			{
				OnPause();
			}
		}

		public void OnPause()
		{
			PauseSystem.Pause(true);
			menuObject.SetActive(true);
			Cursor.lockState = CursorLockMode.None;
		}

		public void OnUnpause()
		{
			PauseSystem.Pause(false);
			menuObject.SetActive(false);
			Cursor.lockState = CursorLockMode.Locked;
		}
	}
}