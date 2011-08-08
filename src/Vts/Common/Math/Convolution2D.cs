using System;

namespace Vts.Common.Math
{
    /// <summary>
    /// Class containing the 'Roll' Algorithm, Custom Integration based off Tom, et al, curtesy of Owen Yang, David Cuccia, and Bernard Choi, 
    /// "Real-time blood flow imaging using the graphics processing unit," (paper in preparation)
    /// </summary>
    public class Convolution2D
    {
        ////////////////////////////////////////////////////////////////////////////////
        // 'Roll' Algorithm, Custom Integration based off Tom, et al, curtesy of Owen Yang
        // Owen Yang, David Cuccia, and Bernard Choi, "Real-time blood flow imaging using the
        // graphics processing unit," (paper in preparation)
        ////////////////////////////////////////////////////////////////////////////////
        // width and height are the width and height of the raw image, respecively
        // Raw is the raw speckle image
        // SpeckleContrast and SpeckleFlowIndex are preallocated 1D matricies
        // rollRow, rollColumn, rollRowSquares, rollColumnSquared are preallocated
        // buffers for holding the strip sums in the row and column and strip sums
        // of squared values in the row and column, respectively
        // the size of these 4 buffers are width*1
        // wR is the size of the sliding window radius
        // t is the exposure time of the camera used to obtain the raw speckle images
        /// <summary>
        /// 'Roll' Algorithm, Custom Integration based off Tom, et al, curtesy of Owen Yang
        /// Owen Yang, David Cuccia, and Bernard Choi, "Real-time blood flow imaging using the
        /// graphics processing unit," (paper in preparation)
        /// </summary>
        /// <param name="raw">Raw speckle image</param>
        /// <param name="speckleContrast">Speckle contrast, preallocated 1D matrix</param>
        /// <param name="speckleFlowIndex">Speckle flow index, preallocated 1D matrix</param>
        /// <param name="rollRow">Preallocated buffer for holding the strip sum in the row</param>
        /// <param name="rollColumn">Preallocated buffer for holding the strip sum in the column</param>
        /// <param name="rollRowSquared">Preallocated buffer for holding the strip sums of squared values in the row</param>
        /// <param name="rollColumnSquared">Preallocated buffer for holding the strip sums of squared values in the row</param>
        /// <param name="width">Width of the raw speckle image. Also used for calculating the size of the preallocated buffers, the 4 buffers are width*1</param>
        /// <param name="height">Height of the raw speckle image</param>
        /// <param name="wR">Size of the sliding window radius</param>
        /// <param name="t">Exposure time of the camera used to obtain the raw speckle images</param>
        public static void LsiRoll(
            int[] raw,
            float[] speckleContrast,
            float[] speckleFlowIndex,
            int[] rollRow,
            int[] rollColumn,
            int[] rollRowSquared,
            int[] rollColumnSquared,
            int width,
            int height,
            int wR,
            float t)
        {
            // Full window size
            int w = wR * 2 + 1;

            // Number of elements within sliding window
            int els = w * w;

            // Inverse degrees of freedom
            float iDoF = 1.0f / (els * (els - 1));

            // Step 1) Calculate first accumulated sum to start
            // Perform analysis on first accumulated row
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < w; j++)
                {
                    rollRow[i] += raw[j * width + i];
                    rollRowSquared[i] += (raw[j * width + i] * raw[j * width + i]);
                }

                if (i >= w)
                {
                    rollColumn[i] = rollColumn[i - 1] - rollRow[i - w] + rollRow[i];
                    rollColumnSquared[i] = rollColumnSquared[i - 1] - rollRowSquared[i - w] + rollRowSquared[i];

                    speckleContrast[wR * width + i - wR] =
                        (float)(els * System.Math.Sqrt(iDoF * (els * rollColumnSquared[i] - (rollColumn[i] * rollColumn[i]))) / rollColumn[i]);

                    speckleFlowIndex[wR * width + i - wR] =
                        (1f / (2f * speckleContrast[wR * width + i - wR] * speckleContrast[wR * width + i - wR] * t));
                }
                else
                {
                    rollColumn[w - 1] += rollRow[i];
                    rollColumnSquared[w - 1] += rollRowSquared[i];

                    if (i == (w - 1))
                    {
                        speckleContrast[wR * width + i - wR] =
                            (float)(els * System.Math.Sqrt(iDoF * (els * rollColumnSquared[w - 1] - (rollColumn[w - 1] * rollColumn[w - 1]))) / rollColumn[w - 1]);

                        //speckleFlowIndex[wR * width + i - wR] = (1f / (2f * speckleContrast[wR * width + iwR] * speckleContrast[wR * width + i - wR] * t));
                        speckleFlowIndex[wR * width + i - wR] = (1f / (2f * speckleContrast[wR * width + i - wR] * speckleContrast[wR * width + i - wR] * t));
                    }
                }
            }

            // Step 2) Perform optimized roll algorithm
            // Cumulate rows and columns simultaneously
            // Calculate SC and SFI immediately
            for (int j = w; j < height; j++)
            {
                rollColumn[w - 1] = 0;
                rollColumnSquared[w - 1] = 0;

                for (int i = 0; i < width; i++)
                {
                    rollRow[i] = rollRow[i] - raw[(j - w) * width + i] + raw[j * width + i];
                    rollRowSquared[i] = rollRowSquared[i] - (raw[(j - w) * width + i] * raw[(j - w) * width + i]) + (raw[j * width + i] * raw[j * width + i]);

                    if (i >= w)
                    {
                        rollColumn[i] = rollColumn[i - 1] - rollRow[i - w] + rollRow[i];
                        rollColumnSquared[i] = rollColumnSquared[i - 1] - rollRowSquared[i - w] + rollRowSquared[i];

                        speckleContrast[(j - wR) * width + i - wR] =
                            (float)(els * System.Math.Sqrt(iDoF * (els * rollColumnSquared[i] - (rollColumn[i] * rollColumn[i])))) / rollColumn[i];

                        speckleFlowIndex[(j - wR) * width + i - wR] =
                            1f / (2f * speckleContrast[(j - wR) * width + i - wR] * speckleContrast[(j - wR) * width + i - wR] * t);
                    }
                    else
                    {
                        rollColumn[w - 1] += rollRow[i];
                        rollColumnSquared[w - 1] += rollRowSquared[i];

                        if (i == (w - 1))
                        {
                            speckleContrast[(j - wR) * width + i - wR] =
                                (float)(els * System.Math.Sqrt(iDoF * (els * rollColumnSquared[w - 1] - (rollColumn[w - 1] * rollColumn[w - 1])))) / rollColumn[w - 1];

                            //speckleFlowIndex[(j - wR) * width + i - wR] = 1f / (2f * speckleContrast[(jwR) * width + i - wR] * speckleContrast[(j - wR) * width + i - wR] * t);
                            speckleFlowIndex[(j - wR) * width + i - wR] = 1f / (2f * speckleContrast[(j - wR) * width + i - wR] * speckleContrast[(j - wR) * width + i - wR] * t);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 'Roll' Algorithm, Custom Integration based off Tom, et al, curtesy of Owen Yang.
        /// Owen Yang, David Cuccia, and Bernard Choi, "Real-time blood flow imaging using the
        /// graphics processing unit," (paper in preparation)
        /// </summary>
        /// <param name="raw">raw image</param>
        /// <param name="width">image width</param>
        /// <param name="height">image height</param>
        /// <param name="radius">radius of moving window (full window size will be 2*radius+1)</param>
        /// <param name="processed">pre-allocated processed image destination</param>
        /// <param name="rollRow">pre-allocated row strip sum</param>
        /// <param name="rollColumn">pre-allocated column strip sum</param>
        public static void RollFilter(
            float[] raw, int width, int height, int radius,
            ref float[] processed, ref float[] rollRow, ref float[] rollColumn)
        {
            // input validation
            if (raw.Length != width * height ||
                raw.Length != processed.Length ||
                rollRow.Length != width ||
                rollColumn.Length != width)
            {
                throw new ArgumentException("Parameters not the correct lengths.");
            }

            int w = radius * 2 + 1; // full window size
            int nElements = w * w; // number of elements within sliding window
            float nElementsInverse = 1f / nElements; // inverse of nElements for faster computations 

            // Step 1) Calculate first accumulated sum to start
            // Perform analysis on first accumulated row
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < w; j++)
                {
                    rollRow[i] += raw[j * width + i];
                }

                if (i >= w) // place averaged value in the "middle" of the window
                {
                    rollColumn[i] = rollColumn[i - 1] - rollRow[i - w] + rollRow[i];
                    processed[radius * width + i - radius] = rollColumn[i] * nElementsInverse; 
                }
                else
                {
                    rollColumn[w - 1] += rollRow[i];
                    if (i == (w - 1))
                    {
                        processed[radius * width + i - radius] = rollColumn[i] * nElementsInverse;
                    }
                }
            }

            // Step 2) Perform optimized roll algorithm
            // Cumulate rows and columns simultaneously
            for (int j = w; j < height; j++)
            {
                rollColumn[w - 1] = 0;
                for (int i = 0; i < width; i++)
                {
                    rollRow[i] = rollRow[i] - raw[(j - w) * width + i] + raw[j * width + i];
                    if (i >= w)
                    {
                        rollColumn[i] = rollColumn[i - 1] - rollRow[i - w] + rollRow[i];
                        processed[(j - radius) * width + i - radius] = rollColumn[i] * nElementsInverse;
                    }
                    else
                    {
                        rollColumn[w - 1] += rollRow[i];
                        if (i == (w - 1))
                        {
                            processed[(j - radius) * width + i - radius] = rollColumn[i] * nElementsInverse;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 'Roll' Algorithm, Custom Integration based off Tom, et al, curtesy of Owen Yang.
        /// Owen Yang, David Cuccia, and Bernard Choi, "Real-time blood flow imaging using the
        /// graphics processing unit," (paper in preparation)
        /// </summary>
        /// <param name="raw">raw image</param>
        /// <param name="width">image width</param>
        /// <param name="height">image height</param>
        /// <param name="radius">radius of moving window (full window size will be 2*radius+1)</param>
        /// <param name="processed">pre-allocated processed image destination</param>
        /// <remarks>Allocates row and column strip sums and calls the most general overload</remarks>
        public static void RollFilter(
            float[] raw, int width, int height, int radius,
            ref float[] processed)
        {
            var rollRow = new float[width];
            var rollColumn = new float[width];

            RollFilter(raw, width, height, radius, ref processed, ref rollRow, ref rollColumn);
        }

        /// <summary>
        /// 'Roll' Algorithm, Custom Integration based off Tom, et al, curtesy of Owen Yang.
        /// Owen Yang, David Cuccia, and Bernard Choi, "Real-time blood flow imaging using the
        /// graphics processing unit," (paper in preparation)
        /// </summary>
        /// <param name="raw">raw image</param>
        /// <param name="width">image width</param>
        /// <param name="height">image height</param>
        /// <param name="radius">radius of moving window (full window size will be 2*radius+1)</param>
        /// <returns>processed image</returns>
        /// <remarks>Allocates processed image, as well as row and column strip sums and calls the most 
        /// general overload. Calling this version multiple times with large images will probably thrash the GC.</remarks>
        public static float[] RollFilter(
            float[] raw, int width, int height, int radius)
        {
            var rollRow = new float[width];
            var rollColumn = new float[width];
            var processed = new float[raw.Length];

            RollFilter(raw, width, height, radius, ref processed, ref rollRow, ref rollColumn);

            return processed;
        }
    }
}
