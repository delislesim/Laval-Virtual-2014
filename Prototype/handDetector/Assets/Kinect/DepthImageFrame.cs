using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Kinect
{
    public class DepthImageFrame
    {
        public const int PlayerIndexBitmask = 7;
        public const int PlayerIndexBitmaskWidth = 3;

        private short[] depthImage;
        private int width;
        private int height;

        public DepthImageFrame(int width, int height, short[] depthImage) {
            Debug.Assert(depthImage != null);
            this.width = width;
            this.height = height;
            this.depthImage = depthImage;
        }

        public int GetPlayerAtPosition(int x, int y) {
            return depthImage[GetIndexOfPosition(x, y)] & PlayerIndexBitmask;
        }

        public int GetDepthAtPosition(int x, int y) {
            return depthImage[GetIndexOfPosition(x, y)] >> PlayerIndexBitmaskWidth;
        }

        public int GetWidth() {
            return width;
        }

        public int GetHeight() {
            return height;
        }

        private int GetIndexOfPosition(int x, int y) {
            return y * width + x;
        }

    }
}
