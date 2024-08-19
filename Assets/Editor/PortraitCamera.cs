#if UNITY_EDITOR
using System.Collections.Generic;
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

		private enum ColorType {
			Levin, Macchiato,
			White, Gray, Black,
			Red, Green, Blue
		};

		private static Dictionary<ColorType, string> DictionaryColor = new Dictionary<ColorType, string>() {
			{ ColorType.Levin, "#26BFBB" },
			{ ColorType.Macchiato, "#BF0000" },
			{ ColorType.White, "#FFFFFF" },
			{ ColorType.Gray, "#808080" },
			{ ColorType.Black, "#000000" },
			{ ColorType.Red, "#FF0000" },
			{ ColorType.Green, "#00FF00" },
			{ ColorType.Blue, "#0000FF" }
		};

		/// <summary>마끼아또 아바타 프로필용 카메라를 생성합니다.</summary>
		[MenuItem("Tools/VRSuya/PortraitCamera/Macchiato", priority = 1100)]
		public static void AddMacchaitoCamera() {
			AddNewCamera(DictionaryColor[ColorType.Macchiato]);
			return;
		}

		/// <summary>레빈 아바타 프로필용 카메라를 생성합니다.</summary>
		[MenuItem("Tools/VRSuya/PortraitCamera/Levin", priority = 1101)]
		public static void AddLevinCamera() {
			AddNewCamera(DictionaryColor[ColorType.Levin]);
			return;
		}

		/// <summary>흰색 프로필용 카메라를 생성합니다.</summary>
		[MenuItem("Tools/VRSuya/PortraitCamera/White", priority = 1200)]
		public static void AddWhiteCamera() {
			AddNewCamera(DictionaryColor[ColorType.White]);
			return;
		}

		/// <summary>검은색 프로필용 카메라를 생성합니다.</summary>
		[MenuItem("Tools/VRSuya/PortraitCamera/Black", priority = 1201)]
		public static void AddBlackCamera() {
			AddNewCamera(DictionaryColor[ColorType.Black]);
			return;
		}

		/// <summary>회색 프로필용 카메라를 생성합니다.</summary>
		[MenuItem("Tools/VRSuya/PortraitCamera/Gray", priority = 1202)]
		public static void AddGrayCamera() {
			AddNewCamera(DictionaryColor[ColorType.Gray]);
			return;
		}

		/// <summary>빨간색 프로필용 카메라를 생성합니다.</summary>
		[MenuItem("Tools/VRSuya/PortraitCamera/Red", priority = 1300)]
		public static void AddRedCamera() {
			AddNewCamera(DictionaryColor[ColorType.Red]);
			return;
		}

		/// <summary>초록색 프로필용 카메라를 생성합니다.</summary>
		[MenuItem("Tools/VRSuya/PortraitCamera/Green", priority = 1301)]
		public static void AddGreenCamera() {
			AddNewCamera(DictionaryColor[ColorType.Green]);
			return;
		}

		/// <summary>파란색 프로필용 카메라를 생성합니다.</summary>
		[MenuItem("Tools/VRSuya/PortraitCamera/Blue", priority = 1302)]
		public static void AddBlueCamera() {
			AddNewCamera(DictionaryColor[ColorType.Blue]);
			return;
		}

		/// <summary>지정된 색상으로 아바타 프로필용 카메라를 생성합니다.</summary>
		private static Camera AddNewCamera(string HEXColorCode) {
			VRC_AvatarDescriptor TargetAvatarDescriptor = GetVRCAvatar();
			if (TargetAvatarDescriptor) {
				GameObject newGameObject = new GameObject("PortraitCamera");
				Camera newCameraComponent = newGameObject.AddComponent<Camera>();
				newCameraComponent.clearFlags = CameraClearFlags.SolidColor;
				newCameraComponent.backgroundColor = HexToColor(HEXColorCode);
				newCameraComponent.fieldOfView = 1.0f;
				newCameraComponent.nearClipPlane = 0.01f;
				newCameraComponent.renderingPath = RenderingPath.Forward;
				newGameObject.transform.position = GetCameraPosition(TargetAvatarDescriptor);
				newGameObject.transform.rotation = GetCameraRotation(TargetAvatarDescriptor);
				Undo.RegisterCreatedObjectUndo(newGameObject, "Add New PortraitCamera");
				EditorUtility.SetDirty(newCameraComponent);
				SceneView.RepaintAll();
				return newCameraComponent;
			} else {
				return null;
			}
		}

		/// <summary>아바타의 뷰 포트를 기준으로 카메라의 위치를 반환합니다.</summary>
		/// <returns>최종 카메라의 벡터 좌표</returns>
		private static Vector3 GetCameraPosition(VRC_AvatarDescriptor AvatarDescriptor) {
			Vector3 newVector3 = new Vector3(0.0f, 1.2f, 13.5f);
			Vector3 Offset = new Vector3(0.0f, -0.02f, 14.0f);
			Transform AvatarTransform = AvatarDescriptor.gameObject.transform;
			Vector3 AvatarViewPosition = AvatarTransform.position + (AvatarTransform.rotation * AvatarDescriptor.ViewPosition);
			newVector3 = AvatarViewPosition + (AvatarTransform.rotation * Offset);
			return newVector3;
		}

		/// <summary>아바타를 기준으로 카메라의 회전을 반환합니다.</summary>
		/// <returns>최종 카메라의 회전계</returns>
		private static Quaternion GetCameraRotation(VRC_AvatarDescriptor AvatarDescriptor) {
			Quaternion AvatarRotation = AvatarDescriptor.gameObject.transform.rotation;
			Quaternion ReferenceRotation = Quaternion.Euler(0, 180, 0);
			Quaternion RotationDifference = AvatarRotation * Quaternion.Inverse(ReferenceRotation);
			return RotationDifference;
		}

		/// <summary>Scene에서 조건에 맞는 VRC AvatarDescriptor 컴포넌트 아바타 1개를 반환합니다.</summary>
		/// <returns>조건에 맞는 VRC 아바타</returns>
		private static VRC_AvatarDescriptor GetVRCAvatar() {
			VRC_AvatarDescriptor TargetAvatarDescriptor = GetAvatarDescriptorFromVRCSDKBuilder();
			if (!TargetAvatarDescriptor) TargetAvatarDescriptor = GetAvatarDescriptorFromSelection();
			if (!TargetAvatarDescriptor) TargetAvatarDescriptor = GetAvatarDescriptorFromVRCTool();
			return TargetAvatarDescriptor;
		}

		/// <summary>VRCSDK Builder에서 활성화 상태인 VRC 아바타를 반환합니다.</summary>
		/// <returns>VRCSDK Builder에서 활성화 상태인 VRC 아바타</returns>
		private static VRC_AvatarDescriptor GetAvatarDescriptorFromVRCSDKBuilder() {
			return null;
		}

		/// <summary>Unity 하이어라키에서 선택한 GameObject 중에서 VRC AvatarDescriptor 컴포넌트가 존재하는 아바타를 1개를 반환합니다.</summary>
		/// <returns>선택 중인 VRC 아바타</returns>
		private static VRC_AvatarDescriptor GetAvatarDescriptorFromSelection() {
			GameObject[] SelectedGameObjects = Selection.gameObjects;
			if (SelectedGameObjects.Length == 1) {
				VRC_AvatarDescriptor SelectedVRCAvatarDescriptor = SelectedGameObjects[0].GetComponent<VRC_AvatarDescriptor>();
				if (SelectedVRCAvatarDescriptor) {
					return SelectedVRCAvatarDescriptor;
				} else {
					return null;
				}
			} else if (SelectedGameObjects.Length > 1) {
				VRC_AvatarDescriptor SelectedVRCAvatarDescriptor = SelectedGameObjects
					.Where(SelectedGameObject => SelectedGameObject.activeInHierarchy == true)
					.Select(SelectedGameObject => SelectedGameObject.GetComponent<VRC_AvatarDescriptor>()).ToArray()[0];
				if (SelectedVRCAvatarDescriptor) {
					return SelectedVRCAvatarDescriptor;
				} else {
					return null;
				}
			} else {
				return null;
			}
		}

		/// <summary>Scene에서 활성화 상태인 VRC AvatarDescriptor 컴포넌트가 존재하는 아바타를 1개를 반환합니다.</summary>
		/// <returns>Scene에서 활성화 상태인 VRC 아바타</returns>
		private static VRC_AvatarDescriptor GetAvatarDescriptorFromVRCTool() {
			VRC_AvatarDescriptor[] AllVRCAvatarDescriptor = VRC.Tools.FindSceneObjectsOfTypeAll<VRC_AvatarDescriptor>().ToArray();
			if (AllVRCAvatarDescriptor.Length > 0) {
				return AllVRCAvatarDescriptor.Where(Avatar => Avatar.gameObject.activeInHierarchy).ToArray()[0];
			} else {
				return null;
			}
		}

		/// <summary>HEX 문자열을 Color로 변환합니다.</summary>
		/// <param name="HEXColorCode">변환할 HEX 문자열 (예: #RRGGBB 또는 #RRGGBBAA)</param>
		/// <returns>변환된 Color 객체</returns>
		private static Color HexToColor(string HEXColorCode) {
			if (HEXColorCode.StartsWith("#")) HEXColorCode = HEXColorCode.Substring(1);
			if (HEXColorCode.Length != 6 && HEXColorCode.Length != 8) return Color.black;
			if (HEXColorCode.Length == 6) HEXColorCode += "FF";
			byte r = byte.Parse(HEXColorCode.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
			byte g = byte.Parse(HEXColorCode.Substring(2, 4).Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
			byte b = byte.Parse(HEXColorCode.Substring(4, 6).Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
			byte a = byte.Parse(HEXColorCode.Substring(6, 8).Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
			return new Color32(r, g, b, a);
		}
	}
}
#endif