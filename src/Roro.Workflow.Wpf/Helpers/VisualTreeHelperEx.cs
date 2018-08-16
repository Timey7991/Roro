using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Roro.Workflow.Wpf
{
    public static class VisualTreeHelperEx
    {
        public static T GetAncestor<T>(DependencyObject child) where T : DependencyObject
        {
            return GetAncestor<T>(child, x => true);
        }

        public static T GetAncestor<T>(DependencyObject child, Func<T, bool> predicate) where T : DependencyObject
        {
            var candidate = child;
            while (candidate != null)
            {
                candidate = VisualTreeHelper.GetParent(candidate);
                if (candidate is T result && predicate.Invoke(result))
                {
                    return result;
                }
            }

            return null;
        }

        public static T GetDescendant<T>(DependencyObject parent) where T : DependencyObject
        {
            return GetDescendant<T>(parent, x => true);
        }

        public static T GetDescendant<T>(DependencyObject parent, Func<T, bool> predicate) where T : DependencyObject
        {
            var queue = new Queue<DependencyObject>();
            queue.Enqueue(parent);

            while (queue.Count > 0)
            {
                var candidate = queue.Dequeue();
                if (candidate is T result && predicate.Invoke(result) && candidate != parent)
                {
                    return result;
                }

                var count = VisualTreeHelper.GetChildrenCount(candidate);
                for (var index = 0; index < count; index++)
                {
                    queue.Enqueue(VisualTreeHelper.GetChild(candidate, index));
                }
            }

            return null;
        }
    }
}
