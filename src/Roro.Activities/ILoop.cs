using System.Collections;

namespace Roro.Activities
{
    public interface ILoop : Activity
    {
        IEnumerator GetEnumerator();

        void Execute(object current);
    }
}