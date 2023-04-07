using UnityEngine;
using System.Collections;

namespace Core
{
	namespace UI
	{
		[DisallowMultipleComponent]
		[RequireComponent(typeof(RectTransform))]
		public class WorldUIBehaviour : MonoBehaviour, System.IComparable
		{
			public uint mZSortPriority = 0;
			public bool mUseRectTransformXY = false;
			public bool mUseFixedZ = false;
			public float mFixedZ = 10.0f;
			public Vector2 mWorldSize = Vector2.one;
			public Vector2 mScale = Vector2.one;

			float mDepth = 0.0f;
			public float GetSortPriority() { return mZSortPriority > 0 ? -(int)mZSortPriority : mDepth; }

			[SerializeField]
			bool mWorldUICheckIn = true;
			[SerializeField]
			Transform mParent = null;
			[SerializeField]
			Vector3 mLocalPosition = Vector3.zero;
			RectTransform mRectTransform = null;
			public RectTransform GetRect() { return mRectTransform; }

			Camera mCamera = null;
			Transform mCameraTransform = null;

			public int CompareTo(object obj)
			{
				if (obj == null || obj.GetType() != typeof(WorldUIBehaviour))
				{
					return 1;
				}
				return mDepth.CompareTo(((WorldUIBehaviour)obj).mDepth);
			}

			Camera GetCamera()
			{
				if (mCamera == null)
				{
					mCamera = Camera.main;
				}
				return mCamera;
			}

			Transform GetCameraTransform()
			{
				if (mCameraTransform == null && GetCamera() != null)
				{
					mCameraTransform = GetCamera().transform;
				}
				return mCameraTransform;
			}

			public static void SetScale(RectTransform rectTransform, Vector2 scale)
			{
				WorldUIBehaviour transform = rectTransform.GetComponentInParent<WorldUIBehaviour>();
				if (transform == null)
				{
					rectTransform.localScale = scale;
					return;
				}
				transform.mScale = scale;
			}

			public static void SetPosition(RectTransform rectTransform, Vector2 position)
			{
				WorldUIBehaviour transform = rectTransform.GetComponentInParent<WorldUIBehaviour>();
				if (transform == null)
				{
					rectTransform.position = position;
					return;
				}
				transform.SetScreenPosition(position);
			}

			public Transform GetParent()
			{
				return mParent;
			}

			public void SetParent(Transform parent)
			{
				mParent = parent;
			}

			public Vector3 GetLocalPosition()
			{
				return mLocalPosition;
			}

			public void SetLocalPosition(Vector3 position)
			{
				mLocalPosition = position;
			}

			public Vector3 GetWorldPosition()
			{
				return mParent == null ? mLocalPosition : mParent.TransformPoint(mLocalPosition);
			}

			public void SetWorldPosition(Vector3 position)
			{
				mLocalPosition = mParent == null ? position : mParent.InverseTransformPoint(position);
			}

			public Vector2 GetScreenPosition()
			{
				if (mUseRectTransformXY)
				{
					return mRectTransform.position;
				}
				return GetCamera().WorldToScreenPoint(GetWorldPosition());
			}

			public void SetScreenPosition(Vector2 position)
			{
				Vector3 positionWithDepth = new Vector3(position.x, position.y, mDepth);
				mRectTransform.position = positionWithDepth;
				SetWorldPosition(GetCamera().ScreenToWorldPoint(positionWithDepth));
			}

			float CalculateDepth()
			{
				if (mUseFixedZ)
				{
					return mFixedZ;
				}
				Vector3 viewDir = GetWorldPosition() - GetCameraTransform().position;
				return Vector3.Dot(viewDir, GetCameraTransform().forward);
			}

			void Awake()
			{
				mRectTransform = GetComponent<RectTransform>();
				if (mWorldUICheckIn)
				{
					WorldUIManager.CheckIn(this);
				}
				ScaleToCamera();
			}

			void OnDestroy()
			{
				if (mWorldUICheckIn)
				{
					WorldUIManager.CheckOut(this);
				}
			}

			public static Vector3 ScreenToCanvas(Vector3 screenPoint, Canvas canvas)
			{
				Vector3 canvasPoint = screenPoint;
				RectTransform rectTransform = canvas.transform as RectTransform;
				if (canvas.renderMode == RenderMode.ScreenSpaceCamera && canvas.worldCamera != null)
				{
					RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, screenPoint, canvas.worldCamera, out canvasPoint);
				}
				return canvasPoint;
			}

			public void ScaleToCamera()
			{
				if (Camera.main == null)
				{
					return;
				}

				mDepth = CalculateDepth();
				Vector2 screenPosition = GetScreenPosition();

				// update world position for fixed z depth and rect transform x y.
				Vector3 worldPosition = GetCamera().ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, mDepth));
				if (mUseRectTransformXY || mUseFixedZ)
				{
					SetWorldPosition(worldPosition);
				}

				Vector2 right = GetCamera().WorldToScreenPoint(worldPosition + (GetCameraTransform().right * mWorldSize.x));
				float width = right.x - screenPosition.x;
				float height = (mWorldSize.y / mWorldSize.x) * width;

				//Vector2 up = GetCamera().WorldToScreenPoint(worldPosition + (GetCameraTransform().up * mWorldSize.y));
				//Vector2 size = new Vector2(Vector2.Distance(screenPosition, right), Vector2.Distance(screenPosition, up));
				//size = size / Util.GetCanvasScale(mRectTransform);

				mRectTransform.localScale = new Vector3(mScale.x, mScale.y, mRectTransform.localScale.z);
				mRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width * mScale.x);
				mRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height * mScale.y);
				mRectTransform.position = ScreenToCanvas(screenPosition, GetComponentInParent<Canvas>());
			}
		} 
	}
}
