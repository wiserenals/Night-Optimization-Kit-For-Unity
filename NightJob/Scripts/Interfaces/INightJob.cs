using System.Collections;
using System.Threading.Tasks;

namespace NightJob
{
    public interface INightJob
    {
        public bool JobCompleted { get; set; }
        public IEnumerator OnMainThread();
        public Task OnSecondaryThread();
    }
}
