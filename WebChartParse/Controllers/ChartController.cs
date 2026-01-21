using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web.Mvc;
using WebChartParse.Models;

namespace WebChartParse.Controllers
{
    public class ChartController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Draw(string id)
        {
            string expression = DecodeExpression(id);

            const int Width = 1920;
            const int Height = 1080;
            const int AxisMax = 10;
            const int AxisMin = -10;
            const int AxisStep = 1;
            const double XStep = 0.1;

            using (Image image = new Bitmap(Width, Height))
            using (Graphics graphics = Graphics.FromImage(image))
            using (Pen axisPen = new Pen(Color.Black, 5))
            using (Pen gridPen = new Pen(Color.Gray, 1))
            using (Pen graphPen = new Pen(Color.Red, 3))
            {
                int rangeChartX = AxisMax - AxisMin;
                float centerX = rangeChartX / 2f;
                float scaleX = (float)Width / rangeChartX;

                int rangeChartY = AxisMax - AxisMin;
                float centerY = rangeChartY / 2f;
                float scaleY = (float)Height / rangeChartY;

                for (int i = AxisMin; i <= AxisMax + AxisStep; i += AxisStep)
                {
                    Pen penToUse = i == 0 ? axisPen : gridPen;
                    float xRes = (i + centerX) * scaleX;
                    graphics.DrawLine(penToUse, xRes, 0, xRes, Height);
                }

                for (int i = AxisMin; i <= AxisMax + AxisStep; i += AxisStep)
                {
                    Pen penToUse = i == 0 ? axisPen : gridPen;
                    float yRes = (i + centerY) * scaleY;
                    graphics.DrawLine(penToUse, 0, yRes, Width, yRes);
                }

                if (!string.IsNullOrWhiteSpace(expression))
                {
                    using (Font labelFont = new Font(FontFamily.GenericSansSerif, 16, FontStyle.Bold))
                    using (Brush labelBrush = new SolidBrush(Color.Green))
                    {
                        graphics.DrawString(expression, labelFont, labelBrush, 5, 5);
                    }

                    string loweredExpression = expression.ToLowerInvariant();
                    Parser parser = new Parser();

                    double xStart = AxisMin;
                    string xStartExpression = loweredExpression.Replace(
                        "x",
                        "(" + xStart.ToString(CultureInfo.InvariantCulture) + ")");
                    double yStart = parser.Parse(xStartExpression);

                    float moveLineX = (float)((xStart + centerX) * scaleX);
                    float moveLineY = (float)((centerY - yStart) * scaleY);

                    double stepsPerUnit = 1d / XStep;
                    for (double i = AxisMin * stepsPerUnit; i <= (AxisMax + AxisStep) * stepsPerUnit; i += 1)
                    {
                        double x = i / stepsPerUnit;
                        string pointExpression = loweredExpression.Replace(
                            "x",
                            "(" + x.ToString(CultureInfo.InvariantCulture) + ")");
                        double y = parser.Parse(pointExpression);
                        float lineToX = (float)((x + centerX) * scaleX);
                        float lineToY = (float)((centerY - y) * scaleY);
                        graphics.DrawLine(graphPen, moveLineX, moveLineY, lineToX, lineToY);
                        moveLineX = lineToX;
                        moveLineY = lineToY;
                    }
                }

                using (MemoryStream ms = new MemoryStream())
                {
                    image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    return File(ms.ToArray(), "image/png");
                }
            }
        }

        private static string DecodeExpression(string encoded)
        {
            if (string.IsNullOrWhiteSpace(encoded))
            {
                return string.Empty;
            }

            try
            {
                byte[] data = Convert.FromBase64String(encoded);
                return Encoding.ASCII.GetString(data);
            }
            catch (FormatException)
            {
                return encoded;
            }
        }
    }
}
