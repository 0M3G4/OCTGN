using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Octgn.Play.Gui
{
    public class DragAdorner : Adorner
    {
        public DragAdorner(UIElement adorned)
            : base(adorned)
        {
            var brush = new VisualBrush(adorned) {Stretch = Stretch.None, AlignmentX = AlignmentX.Left};
            // HACK: this makes the markers work properly, 
            // even after adding 4+ markers and then removing to 3-,
            // and then shift-dragging (in which case the size of the adorner is correct, 
            // but the VisualBrush tries to render the now invisible number.
            child = new Rectangle();
            child.BeginInit();
            child.Width = adorned.RenderSize.Width;
            child.Height = adorned.RenderSize.Height;
            child.Fill = brush;
            child.IsHitTestVisible = false;
            child.EndInit();

            var animation = new DoubleAnimation(0.6, 0.85, new Duration(TimeSpan.FromMilliseconds(500)))
                                {AutoReverse = true, RepeatBehavior = RepeatBehavior.Forever};
            animation.Freeze();

            brush.BeginAnimation(Brush.OpacityProperty, animation);
        }

        #region Content Management

        private readonly Rectangle child;

        protected override int VisualChildrenCount
        {
            get { return 1; }
        }

        protected override Size MeasureOverride(Size constraint)
        {
            child.Measure(constraint);
            return child.DesiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            child.Arrange(new Rect(finalSize));
            return finalSize;
        }

        protected override Visual GetVisualChild(int index)
        {
            return child;
        }

        #endregion

        #region Position

        private double leftOffset, topOffset;

        public double LeftOffset
        {
            get { return leftOffset; }
            set
            {
                leftOffset = value;
                UpdatePosition();
            }
        }

        public double TopOffset
        {
            get { return topOffset; }
            set
            {
                topOffset = value;
                UpdatePosition();
            }
        }

        private void UpdatePosition()
        {
            var layer = (AdornerLayer) Parent;
            if (layer != null) layer.Update(AdornedElement);
        }

        public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
        {
            var result = new GeneralTransformGroup();
            result.Children.Add(new TranslateTransform(leftOffset, topOffset));
            result.Children.Add(base.GetDesiredTransform(transform));
            return result;
        }

        #endregion
    }
}