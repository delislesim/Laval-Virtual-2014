using System;
using UnityEngine;

namespace KinectHelpers
{

	public class Skeleton
	{

    public Skeleton(int skeleton_id)
    {
      this.skeleton_id = skeleton_id;
			is_face_tracking = false;
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

    public bool Exists()
    {
      LoadSkeleton();
      return skeleton_exists;
    }

    public JointStatus GetJointPosition(Joint joint, out Vector3 position)
    {
      LoadSkeleton();
      position = new Vector3(joint_positions[3 * (int)joint + 0],
                             joint_positions[3 * (int)joint + 1],
                             joint_positions[3 * (int)joint + 2]);
      return joint_status[(int)joint];
    }

    public JointStatus GetJointPositionDepth(Joint joint, out Vector3 position)
    {
      LoadSkeletonDepth();
      position = new Vector3(joint_positions_depth[3 * (int)joint + 0],
                             joint_positions_depth[3 * (int)joint + 1],
                             joint_positions_depth[3 * (int)joint + 2]);
      return joint_status[(int)joint];
    }

	public static Joint GetSkeletonJointParent(Joint joint)
	{
		switch(joint)
		{
		case Joint.HipCenter:
			return Joint.HipCenter;
		case Joint.Spine:
			return Joint.HipCenter;
		case Joint.ShoulderCenter:
			return Joint.Spine;
		case Joint.Head:
			return Joint.ShoulderCenter;
		case Joint.ShoulderLeft:
			return Joint.ShoulderCenter;
		case Joint.ElbowLeft:
			return Joint.ShoulderLeft;
		case Joint.WristLeft:
			return Joint.ElbowLeft;
		case Joint.HandLeft:
			return Joint.WristLeft;
		case Joint.ShoulderRight:
			return Joint.ShoulderCenter;
		case Joint.ElbowRight:
			return Joint.ShoulderRight;
		case Joint.WristRight:
			return Joint.ElbowRight;
		case Joint.HandRight:
			return Joint.WristRight;
		case Joint.HipLeft:
			return Joint.HipCenter;
		case Joint.KneeLeft:
			return Joint.HipLeft;
		case Joint.AnkleLeft:
			return Joint.KneeLeft;
		case Joint.FootLeft:
			return Joint.AnkleLeft;
		case Joint.HipRight:
			return Joint.HipCenter;
		case Joint.KneeRight:
			return Joint.HipRight;
		case Joint.AnkleRight:
			return Joint.KneeRight;
		case Joint.FootRight:
			return Joint.AnkleRight;
		}
		
		return Joint.HipCenter;
	}

	public Quaternion GetFaceRotation()
	{
			float[] faceRotationXYZ = new float[3];
			is_face_tracking = KinectPowerInterop.GetFaceRotation (faceRotationXYZ);
			Quaternion rotation = Quaternion.Euler (faceRotationXYZ [0], faceRotationXYZ [1], faceRotationXYZ [2]);
			return rotation;
	}

	public bool GetFaceTrackingStatus()
	{
		return is_face_tracking;
	}

	public Quaternion GetBoneOrientation(Joint joint)
	{
		LoadSkeleton ();
		Quaternion rotation = bone_orientations [(int)joint].absoluteRotation.rotationQuaternion;
		Vector3 eulerAngles = rotation.eulerAngles;
		Quaternion fixedRot = Quaternion.Euler (eulerAngles.x, -eulerAngles.y + 180, -eulerAngles.z);
		return fixedRot;
	}
	
	public void ReloadSkeleton() {
		skeleton_loaded = false;
		joint_positions_depth = null;

		// Se rappeler des dernieres positions pour voir s'il y a un changement.
		previous_joint_positions = joint_positions;

		LoadSkeleton ();
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

		// Determiner si le squelette est different de la derniere fois.
		is_different = false;
		if (previous_joint_positions == null) {
			is_different = true;
		} else {
			for (int i = 0; i < joint_positions.Length; ++i) {
				if (joint_positions[i] != previous_joint_positions[i]) {
					is_different = true;
					break;
				}
			}
		}
	}

	public bool IsDifferent() {
		return is_different;
	}

    public void LoadSkeletonDepth()
    {
      LoadSkeleton();
      if (joint_positions_depth == null)
      {
        joint_positions_depth = new int[3 * (int)Joint.Count];
        KinectPowerInterop.GetJointsPositionDepth(skeleton_id, joint_positions_depth);
      }
    }

	public int GetID() {
		return skeleton_id;
	}

	public bool IsSkeletonReliable()
	{
		int test = Array.FindAll (joint_status, x => x == JointStatus.Inferred || x == JointStatus.NotTracked).Length;
		Debug.Log (test + "\n");	
		if(Array.FindAll(joint_status, x => x == JointStatus.Inferred || x == JointStatus.NotTracked).Length >= 5)
			return false;
		else
			return true;
	}

    private int skeleton_id;

    private bool skeleton_loaded;
    private bool skeleton_exists;
    private float[] joint_positions;
	private KinectPowerInterop.NuiSkeletonBoneOrientation[] bone_orientations;
    private int[] joint_positions_depth;
    private JointStatus[] joint_status;
	private bool is_face_tracking;

	private float[] previous_joint_positions;

	private bool is_different = false;
  }
}
