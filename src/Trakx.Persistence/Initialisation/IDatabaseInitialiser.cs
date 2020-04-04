using System.Threading.Tasks;

namespace Trakx.Data.Persistence.Initialisation
{
    public interface IDatabaseInitialiser
    {
        Task SeedDatabase();
    }
}