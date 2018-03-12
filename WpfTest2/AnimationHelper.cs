using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace WpfTest2
{
    public struct ActiveAnimationInfo
    {
        public UIElement element;
        public DependencyProperty dp;
        public DoubleAnimation animation;
    }

    public class AnimationHelper
    {

        Dictionary<AnimationClock, ActiveAnimationInfo> _activeAnimations = new Dictionary<AnimationClock, ActiveAnimationInfo>();


        void AnimationCompleted(object sender, EventArgs e)
        {
            //Debug.WriteLine("AnimationCompleted.");

            AnimationClock clock = (AnimationClock)sender;
            ActiveAnimationInfo info = _activeAnimations[clock];

            object value = info.element.GetValue(info.dp);
            info.element.BeginAnimation(info.dp, null);
            info.element.SetValue(info.dp, value);
            info.element.ApplyAnimationClock(info.dp, null);

            _activeAnimations.Remove(clock);

            //Debug.WriteLine("  _activeAnimations.Count: " + _activeAnimations.Count);
        }

        public bool HasActiveClock()
        {
            return _activeAnimations.Count > 0;
        }


        public void BeginAnimation(UIElement element, DependencyProperty dp, double to, double accelerationRatio, double decelerationRatio)
        {
            DoubleAnimation animation = new DoubleAnimation();

            animation.To = to;
            animation.Duration = new Duration(TimeSpan.FromSeconds(0.4));
            animation.AccelerationRatio = accelerationRatio;
            animation.DecelerationRatio = decelerationRatio;
            animation.Completed += AnimationCompleted;

            AnimationClock clock = animation.CreateClock();

            element.ApplyAnimationClock(dp, clock);

            _activeAnimations.Add(clock, new ActiveAnimationInfo { element = element, dp = dp, animation = animation });

            //Debug.WriteLine("  _activeAnimations.Count: " + _activeAnimations.Count);
        }

        public void RemoveAll()
        {
            if (_activeAnimations.Count == 0)
            {
                return;
            }

            foreach (var item in _activeAnimations.Keys)
            {
                ActiveAnimationInfo info = _activeAnimations[item];

                object value = info.element.GetValue(info.dp);
                info.element.BeginAnimation(info.dp, null);
                info.element.SetValue(info.dp, value);
                info.element.ApplyAnimationClock(info.dp, null);

                //item.Controller.Remove();
                item.Controller.Stop();
            }

            _activeAnimations.Clear();
        }

    }

}
