#if UNITY_EDITOR
using System.Linq;

using UnityEditor;
using UnityEngine;

using VRC.SDKBase;

/*
 * VRSuya PortraitCamera
 * Contact : vrsuya@gmail.com // Twitter : https://twitter.com/VRSuya
 */

namespace com.vrsuya.portraitcamera {

	public class PortraitCamera : MonoBehaviour {

		/// <summary>마끼아또 아바타 프로필용 카메라를 생성합니다.</summary>
		[MenuItem("Tools/VRSuya/PortraitCamera/Macchiato", priority = 1100)]
		public static void AddMacchaitoCamera() {
			AddNewCamera(new Color(0.749f, 0.0f, 0.0f));
			return;
		}

		/// <summary>레빈 아바타 프로필용 카메라를 생성합니다.</summary>
		[MenuItem("Tools/VRSuya/PortraitCamera/Levin", priority = 1101)]
		public static void AddLevinCamera() {
			AddNewCamera(new Color(0.149f, 0.749f, 0.733f));
			return;
		}

		/// <summary>흰색 프로필용 카메라를 생성합니다.</summary>
		[MenuItem("Tools/VRSuya/PortraitCamera/White", priority = 1200)]
		public static void AddWhiteCamera() {
			AddNewCamera(new Color(1.0f, 1.0f, 1.0f));
			return;
		}

		/// <summary>검은색 프로필용 카메라를 생성합니다.</summary>
		[MenuItem("Tools/VRSuya/PortraitCamera/Black", priority = 1201)]
		public static void AddBlackCamera() {
			AddNewCamera(new Color(0.0f, 0.0f, 0.0f));
			return;
		}

		/// <summary>회색 프로필용 카메라를 생성합니다.</summary>
		[MenuItem("Tools/VRSuya/PortraitCamera/Gray", priority = 1202)]
		public static void AddGrayCamera() {
			AddNewCamera(new Color(0.5f, 0.5f, 0.5f));
			return;
		}

		/// <summary>빨간색 프로필용 카메라를 생성합니다.</summary>
		[MenuItem("Tools/VRSuya/PortraitCamera/Red", priority = 1300)]
		public static void AddRedCamera() {
			AddNewCamera(new Color(1.0f, 0.0f, 0.0f));
			return;
		}

		/// <summary>초록색 프로필용 카메라를 생성합니다.</summary>
		[MenuItem("Tools/VRSuya/PortraitCamera/Green", priority = 1301)]
		public static void AddGreenCamera() {
			AddNewCamera(new Color(0.0f, 1.0f, 0.0f));
			return;
		}

		/// <summary>파란색 프로필용 카메라를 생성합니다.</summary>
		[MenuItem("Tools/VRSuya/PortraitCamera/Blue", priority = 1302)]
		public static void AddBlueCamera() {
			AddNewCamera(new Color(0.0f, 0.0f, 1.0f));
			return;
		}

		/// <summary>지정된 색상으로 아바타 프로필용 카메라를 생성합니다.</summary>
		private static Camera AddNewCamera(Color TargetColor) {
			GameObject newGameObject = new GameObject("PortraitCamera");
			Camera newCameraComponent = newGameObject.AddComponent<Camera>();
			newCameraComponent.clearFlags = CameraClearFlags.SolidColor;
			newCameraComponent.backgroundColor = TargetColor;
			newCameraComponent.fieldOfView = 1.0f;
			newCameraComponent.nearClipPlane = 0.01f;
			newCameraComponent.renderingPath = RenderingPath.Forward;
			newGameObject.transform.position = GetCameraPosition();
			newGameObject.transform.rotation = GetCameraRotation(newGameObject.transform.position);
			Undo.RegisterCreatedObjectUndo(newGameObject, "Add New PortraitCamera");
			EditorUtility.SetDirty(newCameraComponent);
			SceneView.RepaintAll();
			return newCameraComponent;
		}

		/// <summary>아바타의 뷰 포트를 기준으로 카메라의 위치를 반환합니다.</summary>
		/// <returns>최종 카메라의 벡터 좌표</returns>
		private static Vector3 GetCameraPosition() {
			Vector3 newVector3 = new Vector3(0.0f, 1.2f, 13.5f);
			Vector3 Offset = new Vector3(0.0f, -0.02f, 14.0f);
			if (GetVRCAvatar()) {
				VRC_AvatarDescriptor AvatarDescriptor = GetVRCAvatar();
				Transform AvatarTransform = AvatarDescriptor.gameObject.transform;
				Vector3 AvatarViewPosition = AvatarTransform.position + (AvatarTransform.rotation * AvatarDescriptor.ViewPosition);
				newVector3 = AvatarViewPosition + (AvatarTransform.rotation * Offset);
				return newVector3;
			} else {
				return newVector3;
			}
		}

		/// <summary>아바타를 기준으로 카메라의 회전을 반환합니다.</summary>
		/// <returns>최종 카메라의 회전계</returns>
		private static Quaternion GetCameraRotation(Vector3 CameraPosition) {
			if (GetVRCAvatar()) {
				VRC_AvatarDescriptor AvatarDescriptor = GetVRCAvatar();
				Transform AvatarTransform = AvatarDescriptor.gameObject.transform;
				Vector3 AvatarViewPosition = AvatarTransform.position + (AvatarTransform.rotation * AvatarDescriptor.ViewPosition);
				Vector3 DirectionToAvatar = AvatarViewPosition - CameraPosition;
				DirectionToAvatar.y = 0;
				Vector3 FlattenedDirection = new Vector3(DirectionToAvatar.x, 0, DirectionToAvatar.z);
				return Quaternion.LookRotation(FlattenedDirection);
			} else {
				return Quaternion.Euler(0, 180, 0);
			}
		}

		/// <summary>Scene에서 활성화 상태인 VRC AvatarDescriptor 컴포넌트를 가지고 있는 아바타 1개를 반환합니다.</summary>
		/// <returns>활성화 상태인 VRC 아바타</returns>
		private static VRC_AvatarDescriptor GetVRCAvatar() {
			VRC_AvatarDescriptor[] AllVRCAvatarDescriptor = VRC.Tools.FindSceneObjectsOfTypeAll<VRC_AvatarDescriptor>().ToArray();
			if (AllVRCAvatarDescriptor.Length > 0) {
				return AllVRCAvatarDescriptor.Where(Avatar => Avatar.gameObject.activeInHierarchy).ToArray()[0];
			} else {
				return null;
			}
		}
	}
}
#endif