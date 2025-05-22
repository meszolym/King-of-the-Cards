using System.Drawing;
using System;
using System.Collections.Generic;
using System.Windows.Forms.VisualStyles;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using BorderType = Emgu.CV.CvEnum.BorderType;

namespace KC.Detector.Logic;

public static class ImageUtils
{
    public static List<Rectangle> GetTopCards(Mat img)
    {
        // Get image dimensions
        int height = img.Rows;
        int width = img.Cols;

        // Crop the image to the top 4/5 of the height
        Rectangle roi = new Rectangle(0, 0, width, height * 4 / 5);
        Mat croppedImg = new Mat(img, roi);

        // Convert to grayscale
        Mat gray = new Mat();
        CvInvoke.CvtColor(croppedImg, gray, ColorConversion.Bgr2Gray);

        // Apply Canny edge detection
        Mat canny = new Mat();
        CvInvoke.Canny(gray, canny, 200, 200);

        // Apply dilation
        Mat dilated = new Mat();
        Mat structuringElement = CvInvoke.GetStructuringElement(ElementShape.Ellipse, new Size(5, 5), new Point(-1, -1));
        CvInvoke.Dilate(canny, dilated, structuringElement, new Point(-1, -1), 2, BorderType.Default, new MCvScalar(0));

        // Find contours
        using VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
        Mat hierarchy = new Mat();
        CvInvoke.FindContours(dilated, contours, hierarchy, RetrType.List, ChainApproxMethod.ChainApproxSimple);

        // Filter and approximate contours
        List<Rectangle> rectangles = new List<Rectangle>();
        for (int i = 0; i < contours.Size; i++)
        {
            double area = CvInvoke.ContourArea(contours[i]);
            if (area > 20000 && area < 50000)
            {
                double epsilon = 0.1 * CvInvoke.ArcLength(contours[i], true);
                using VectorOfPoint approx = new VectorOfPoint();
                CvInvoke.ApproxPolyDP(contours[i], approx, epsilon, true);

                Rectangle boundingBox = CvInvoke.BoundingRectangle(contours[i]);
                rectangles.Add(boundingBox);
            }
        }

        return rectangles;
    }
}