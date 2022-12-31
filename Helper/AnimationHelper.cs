using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace MenschAergerDichNicht.Helper
{
    class AnimationHelper
    {

        private static double Lerp(double firstFloat, double secondFloat, double by)
        {
            return firstFloat * (1 - by) + secondFloat * by;
        }

        public static void AnimateXY(UIElement el, double x, double y, double dur = 1)
        {
            double by = 0;
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(dur);
            timer.Tick += (sender, e) => by = _AnimateXY(timer, el, x, y, by);
            timer.Start();
        }


        public static double AnimateRotation(UIElement element, double duration = 500, double from = 0, double to = 360)
        {
            element.RenderTransform = new RotateTransform(0);
            element.RenderTransformOrigin = new Point(0.5, 0.5);
            Storyboard myStoryboard = createStoryboard(element, from, to, TimeSpan.FromMilliseconds(duration), new PropertyPath("(UIElement.RenderTransform).(RotateTransform.Angle)"));

            myStoryboard.Begin();
            return duration;
        }


        public static Storyboard[] HighlightAnimation(UIElement element, double duration)
        {
            Storyboard[] sbs = new Storyboard[3];

            element.RenderTransform = new ScaleTransform(1, 1);
            element.RenderTransformOrigin = new Point(0.5, 0.5);

            sbs[0] = createStoryboard(element, 1, 0.5, TimeSpan.FromMilliseconds(duration), new PropertyPath(UIElement.OpacityProperty), true, true);
            sbs[1] = createStoryboard(element, 1, 1.15, TimeSpan.FromMilliseconds(duration), new PropertyPath("(UIElement.RenderTransform).(ScaleTransform.ScaleX)"), true, true);
            sbs[2] = createStoryboard(element, 1, 1.15, TimeSpan.FromMilliseconds(duration), new PropertyPath("(UIElement.RenderTransform).(ScaleTransform.ScaleY)"), true, true);

            foreach (var sb in sbs)
            {
                sb.Begin();
            }

            return sbs;
        }

        private static Storyboard createStoryboard(UIElement element, double from, double to, TimeSpan duration, PropertyPath propertyPath, bool loop = false, bool autoReverse = false)
        {
            Storyboard sb = new Storyboard();
            DoubleAnimation myDoubleAnimation = new DoubleAnimation();
            myDoubleAnimation.From = from;
            myDoubleAnimation.To = to;
            myDoubleAnimation.Duration = new Duration(duration);
            if (loop)
            {
                myDoubleAnimation.RepeatBehavior = RepeatBehavior.Forever;
                myDoubleAnimation.AutoReverse = autoReverse;
            }

            sb.Children.Add(myDoubleAnimation);
            Storyboard.SetTarget(myDoubleAnimation, element);
            Storyboard.SetTargetProperty(myDoubleAnimation, propertyPath);

            return sb;
        }



        private static double _AnimateXY(DispatcherTimer timer, UIElement el, double x, double y, double by)
        {
            if (by <= 1)
            {
                double oldX = Canvas.GetLeft(el);
                double oldY = Canvas.GetTop(el);
                Canvas.SetLeft(el, Lerp(oldX, x, by));
                Canvas.SetTop(el, Lerp(oldY, y, by));
            }
            if (by == 1)
            {
                timer.Stop();
            }
            return by + 0.05;
        }

    }
}
