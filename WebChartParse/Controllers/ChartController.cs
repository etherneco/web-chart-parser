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

        public ActionResult Draw(string id, double? scale, double? step)
        {
            string expression = DecodeExpression(id);

            const int Width = 1920;
            const int Height = 1080;
            double axisLimit = scale.HasValue && scale.Value > 0 ? scale.Value : 10d;
            double axisStep = step.HasValue && step.Value > 0 ? step.Value : 1d;
            double plotStep = Math.Max(axisStep / 10d, 0.01d);

            using (Image image = new Bitmap(Width, Height))
            using (Graphics graphics = Graphics.FromImage(image))
            using (Pen axisPen = new Pen(Color.Black, 5))
            using (Pen gridPen = new Pen(Color.Gray, 1))
            using (Pen graphPen = new Pen(Color.Red, 3))
            {
                double rangeChartX = axisLimit * 2;
                double centerX = rangeChartX / 2d;
                double scaleX = Width / rangeChartX;

                double rangeChartY = axisLimit * 2;
                double centerY = rangeChartY / 2d;
                double scaleY = Height / rangeChartY;

                for (double i = -axisLimit; i <= axisLimit + axisStep; i += axisStep)
                {
                    Pen penToUse = Math.Abs(i) < 0.000001d ? axisPen : gridPen;
                    float xRes = (float)((i + centerX) * scaleX);
                    graphics.DrawLine(penToUse, xRes, 0, xRes, Height);
                }

                for (double i = -axisLimit; i <= axisLimit + axisStep; i += axisStep)
                {
                    Pen penToUse = Math.Abs(i) < 0.000001d ? axisPen : gridPen;
                    float yRes = (float)((i + centerY) * scaleY);
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

                    double xStart = -axisLimit;
                    string xStartExpression = loweredExpression.Replace(
                        "x",
                        "(" + xStart.ToString(CultureInfo.InvariantCulture) + ")");
                    double yStart = parser.Parse(xStartExpression);

                    float moveLineX = (float)((xStart + centerX) * scaleX);
                    float moveLineY = (float)((centerY - yStart) * scaleY);

                    double stepsPerUnit = 1d / plotStep;
                    for (double i = -axisLimit * stepsPerUnit; i <= (axisLimit + axisStep) * stepsPerUnit; i += 1d)
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
