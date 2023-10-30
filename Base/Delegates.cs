using System.Collections.Generic;
using System.Threading.Tasks;

namespace Karayel
{
    public delegate Task AsyncDelegate(List<object> container);
    public delegate void DeltaTimeDelegate(float deltaTime);
}
