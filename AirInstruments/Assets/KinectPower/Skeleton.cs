using System;
using UnityEngine;

namespace KinectHelpers
{

	public class Skeleton
	{

    public Skeleton(int skeleton_id)
    {
    }

    public enum Joint : int
    {
		HipCenter = 0,
		SpineMid = 1,
		Neck = 2,
		Head = 3,
		ShoulderLeft = 4,
		ElbowLeft = 5,
		WristLeft = 6,
		HandLeft = 7,
		ShoulderRight = 8,
		ElbowRight = 9,
		WristRight = 10,
		HandRight = 11,
		HipLeft = 12,
		KneeLeft = 13,
		AnkleLeft = 14,
		FootLeft = 15,
		HipRight = 16,
		KneeRight = 17,
		AnkleRight = 18,
		FootRight = 19,
		ShoulderCenter = 20,
		HandTipLeft = 21,
		ThumbLeft = 22,
		HandTipRight = 23,
		ThumbRight = 24,
		Count = (ThumbRight+1)
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

	public void ReloadSkeleton() {
		// Ne fait rien. Relique de la Kinect UN.
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
                             0);
      return joint_status[(int)joint];
    }

	public static Joint GetSkeletonJointParent(Joint joint)
	{
		switch(joint)
		{
			case Joint.HipCenter:
				return Joint.HipCenter;
			case Joint.SpineMid:
				return Joint.HipCenter;
			case Joint.Neck:
				return Joint.ShoulderCenter;
			case Joint.Head:
				return Joint.Neck;
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
			case Joint.ShoulderCenter:
				return Joint.SpineMid;
			case Joint.HandTipLeft:
				return Joint.HandLeft;
			case Joint.ThumbLeft:
				return Joint.HandLeft;
			case Joint.HandTipRight:
				return Joint.HandRight;
			case Joint.ThumbRight:
				return Joint.HandTipRight;
		}
		
		return Joint.HipCenter;
	}

	private void LoadSkeleton() {
		if (Time.time == timeLastReload)
			return;

		// Allouer la memoire.
		if (timeLastReload == 0) {
			joint_positions = new float[3 * (int)Joint.Count];
			joint_orientations = new float[4 * (int)Joint.Count];
			joint_status = new JointStatus[(int)Joint.Count];
		}
		
		// Demander les infos du squelette a la DLL.
		int[] is_new = new int[1];
		skeleton_exists = KinectPowerInterop.GetJoints(joint_positions, joint_orientations, joint_status, is_new);
		is_different = is_new[0] == 1;

		// Noter le temps du dernier chargement.
		timeLastReload = Time.time;
	}

	public bool IsDifferent() {
		return is_different;
	}

    public void LoadSkeletonDepth()
    {
      if (timeLastReloadDepth != Time.time)
      {
		if (timeLastReloadDepth == 0) {
			joint_positions_depth = new int[3 * (int)Joint.Count];
		}
        KinectPowerInterop.GetJointsPositionDepth(joint_positions_depth);
      }
    }

	public bool IsSkeletonReliable()
	{
		// TODO: Arranger cette fonction pour Kinect 2.
		return true;

			/*
		int test = Array.FindAll (joint_status, x => x == JointStatus.Inferred || x == JointStatus.NotTracked).Length;	
		if(Array.FindAll(joint_status, x => x == JointStatus.Inferred || x == JointStatus.NotTracked).Length >= 12)
			return false;
		else
			return true;
			*/
	}

	private static float timeLastReload = 0.0f;
	private static float timeLastReloadDepth = 0.0f;

    private static bool skeleton_exists;
    private static float[] joint_positions;
	private static float[] joint_orientations;
    private static int[] joint_positions_depth;
    private static JointStatus[] joint_status;
	private static bool is_different = false;
  }
}
