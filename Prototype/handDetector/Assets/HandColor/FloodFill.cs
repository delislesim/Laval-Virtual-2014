using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.HandColor
{
    class FloodFill
    {
        public static void Fill(int width, int height, int start, int tolerance, int[] bitmap) {
            FloodFill floodFill = new FloodFill(width, height, start, tolerance, bitmap);
        }

        private FloodFill(int width, int height, int start, int tolerance, int[] bitmap) {
            this.width = width;
            this.height = height;
            this.start = start;
            this.tolerance = tolerance;
            this.bitmap = bitmap;
        }

        private void FillInternal() { 
            
        }



        private struct Range {
            public int start;
            public int end;
        }

        private int width;
        private int height;
        private int start;
        private int tolerance;
        private int[] bitmap;    
    }
}
