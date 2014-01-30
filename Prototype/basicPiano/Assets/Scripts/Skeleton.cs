using UnityEngine;
using System;

namespace KinectHelpers {
	public class Skeleton {
		
		public Skeleton (int skeleton_id) {
			this.skeleton_id = skeleton_id;	
		}

		public enum Joint : int
		{
			HipCenter = 0,
			Spine,
			ShoulderCenter,
			Head,
			ShoulderLeft,
			ElbowLeft,
			WristLeft,
			HandLeft,
			ShoulderRight,
			ElbowRight,
			WristRight,
			HandRight,
			HipLeft,
			KneeLeft,
			AnkleLeft,
			FootLeft,
			HipRight,
			KneeRight,
			AnkleRight,
			FootRight,
			Count
		}

		public enum JointStatus : byte
		{
			Tracked = 0,
			Inferred = 1,
			NotTracked = 2
		}

		public bool Exists() {
			return skeleton_exists;
		}

		public JointStatus GetJointPosition(Joint joint, out Vector3 position) {
			LoadSkeleton ();
			position = new Vector3(joint_positions[3 * (int)joint + 0],
				                   joint_positions[3 * (int)joint + 1],
				                   joint_positions[3 * (int)joint + 2]);
			return joint_status[(int)joint];
		}

		public Quaternion GetNeckOrientation()
		{
			LoadSkeleton ();
			Quaternion rot = bone_orientations[(int)Joint.Head].absoluteRotation.rotationQuaternion;
			Vector3 eulerAngles = rot.eulerAngles;
			Quaternion fixedRot = Quaternion.Euler(eulerAngles.x, -eulerAngles.y+180, -eulerAngles.z);
			return fixedRot;
		}

		private void LoadSkeleton() {
			if (skeleton_loaded)
				return;
			
			joint_positions = new float[3 * (int)Joint.Count];
			joint_status = new JointStatus[(int)Joint.Count];
			bone_orientations = new KinectPowerInterop.NuiSkeletonBoneOrientation[(int)Joint.Count];

			skeleton_exists = KinectPowerInterop.GetJointsPosition(
			    skeleton_id,
				joint_positions,
				joint_status);

			if(skeleton_exists)
				KinectPowerInterop.GetBonesOrientation(skeleton_id, bone_orientations);

			skeleton_loaded = true;
		}

		private int skeleton_id;
		private bool skeleton_loaded;
		private bool skeleton_exists;
		private float[] joint_positions;
		private KinectPowerInterop.NuiSkeletonBoneOrientation[] bone_orientations;
		private JointStatus[] joint_status;
		private bool[] joints_tracked;
	}
}
