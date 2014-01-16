using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Kinect
{
    class Skeleton
    {
        public Skeleton(uint playerId) {
            this.playerId = playerId;
        }

        public Vector3 GetJointPosition(int joint) {
            return Manager.Instance.GetJointPosition(playerId, joint);
        }

        private uint playerId;
    }
}
