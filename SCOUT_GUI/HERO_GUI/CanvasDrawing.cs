using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;

namespace HERO_GUI
{
    public partial class MainWindow : Window
    {
        private void DrawPizzaSlice(double centerX, double centerY, double radius, double startAngle, double sweepAngle, Brush fill)
        {
            // Convert angles to radians
            double startAngleRad = startAngle * Math.PI / 180;
            double endAngleRad = (startAngle + sweepAngle) * Math.PI / 180;

            // Calculate start and end points on the circle
            Point startPoint = new Point(
                centerX + radius * Math.Cos(startAngleRad),
                centerY + radius * Math.Sin(startAngleRad) // Y is positive downward in WPF
            );

            Point endPoint = new Point(
                centerX + radius * Math.Cos(endAngleRad),
                centerY + radius * Math.Sin(endAngleRad)
            );

            // Determine if the arc is greater than 180°
            bool isLargeArc = sweepAngle > 180;

            // Create the arc segment
            ArcSegment arcSegment = new ArcSegment
            {
                Point = endPoint,
                Size = new Size(radius, radius),
                IsLargeArc = isLargeArc,
                SweepDirection = SweepDirection.Clockwise // Ensure it moves correctly
            };

            // Create the path figure
            PathFigure pathFigure = new PathFigure
            {
                StartPoint = new Point(centerX, centerY), // Start at the center
                Segments = new PathSegmentCollection
        {
            new LineSegment(startPoint, true), // Line from center to start point
            arcSegment,                       // Arc to end point
            new LineSegment(new Point(centerX, centerY), true) // Line back to center
        },
                IsClosed = true
            };

            // Create the path geometry
            PathGeometry pathGeometry = new PathGeometry();
            pathGeometry.Figures.Add(pathFigure);

            // Create the path
            Path path = new Path
            {
                Data = pathGeometry,
                Fill = fill, // Fill the pizza slice
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };

            // Add the path to the canvas
            distance_cnv.Children.Add(path);
        }

        private void DrawRingSlice(double centerX, double centerY, double innerRadius, double outerRadius, double startAngle, double sweepAngle, Brush fill)
        {
            // Convert angles to radians
            double startAngleRad = startAngle * Math.PI / 180;
            double endAngleRad = (startAngle + sweepAngle) * Math.PI / 180;

            // Outer arc start and end points
            Point outerStart = new Point(
                centerX + outerRadius * Math.Cos(startAngleRad),
                centerY + outerRadius * Math.Sin(startAngleRad)
            );

            Point outerEnd = new Point(
                centerX + outerRadius * Math.Cos(endAngleRad),
                centerY + outerRadius * Math.Sin(endAngleRad)
            );

            // Inner arc start and end points (opposite direction)
            Point innerStart = new Point(
                centerX + innerRadius * Math.Cos(endAngleRad),
                centerY + innerRadius * Math.Sin(endAngleRad)
            );

            Point innerEnd = new Point(
                centerX + innerRadius * Math.Cos(startAngleRad),
                centerY + innerRadius * Math.Sin(startAngleRad)
            );

            // Determine if arcs should be large (for >180° slices)
            bool isLargeArc = sweepAngle > 180;

            // Create outer arc
            ArcSegment outerArc = new ArcSegment
            {
                Point = outerEnd,
                Size = new Size(outerRadius, outerRadius),
                IsLargeArc = isLargeArc,
                SweepDirection = SweepDirection.Clockwise
            };

            // Create inner arc (opposite direction)
            ArcSegment innerArc = new ArcSegment
            {
                Point = innerEnd,
                Size = new Size(innerRadius, innerRadius),
                IsLargeArc = isLargeArc,
                SweepDirection = SweepDirection.Counterclockwise
            };

            // Create the path figure (ring slice)
            PathFigure pathFigure = new PathFigure
            {
                StartPoint = outerStart, // Start at outer arc start
                Segments = new PathSegmentCollection
        {
            outerArc,                       // Draw outer arc
            new LineSegment(innerStart, true), // Line to inner arc start
            innerArc,                       // Draw inner arc
            new LineSegment(outerStart, true)  // Close shape by connecting back to outer start
        },
                IsClosed = true
            };

            // Create the path geometry
            PathGeometry pathGeometry = new PathGeometry();
            pathGeometry.Figures.Add(pathFigure);

            // Create the path
            Path path = new Path
            {
                Data = pathGeometry,
                Fill = fill, // Fill color
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };

            // Add the path to the canvas
            distance_cnv.Children.Add(path);
        }

    }
}
