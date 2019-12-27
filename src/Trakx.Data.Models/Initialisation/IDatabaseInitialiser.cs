using System.Threading.Tasks;

namespace Trakx.Data.Models.Initialisation
{
    public interface IDatabaseInitialiser
    {
        Task SeedDatabase();
    }
}