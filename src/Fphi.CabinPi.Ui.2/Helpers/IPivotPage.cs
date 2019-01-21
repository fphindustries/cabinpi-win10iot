using System.Threading.Tasks;

namespace Fphi.CabinPi.Ui.Helpers
{
    public interface IPivotPage
    {
        Task OnPivotSelectedAsync();

        Task OnPivotUnselectedAsync();
    }
}
