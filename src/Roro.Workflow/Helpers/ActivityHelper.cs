using Roro.Activities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Roro.Workflow
{
    public static class ActivityHelper
    {
        private static IEnumerable<TypeWrapper> _activityTypes;

        public static IEnumerable<TypeWrapper> ActivityTypes
        {
            get
            {
                if (_activityTypes == null)
                {
                    Directory.GetFiles(Environment.CurrentDirectory, typeof(Activity).Namespace + "?*.dll").ToList()
                        .ForEach(x => Assembly.LoadFile(x));

                    _activityTypes = AppDomain.CurrentDomain.GetAssemblies()
                        .Where(x => x.FullName.StartsWith(typeof(Activity).Namespace))
                        .SelectMany(x => x.GetTypes())
                        .Where(x => !x.IsAbstract)
                        .Where(x => !x.IsInterface)
                        .Where(x => !x.IsGenericType)
                        .Where(x => typeof(Activity).IsAssignableFrom(x))
                        .Select(x => new TypeWrapper(x));
                }
                return _activityTypes;
            }
        }

        public static IEnumerable<TypeWrapper> ActionTypes => ActivityTypes.Where(x => typeof(IAction).IsAssignableFrom(x.WrappedType));

        public static IEnumerable<TypeWrapper> DecisionTypes => ActivityTypes.Where(x => typeof(IDecision).IsAssignableFrom(x.WrappedType));

        public static IEnumerable<TypeWrapper> LoopTypes => ActivityTypes.Where(x => typeof(ILoop).IsAssignableFrom(x.WrappedType));

    }
}
