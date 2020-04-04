using System.Threading.Tasks;

namespace Trakx.Persistence.Initialisation
{
    public interface IDatabaseInitialiser
    {
        Task SeedDatabase();
    }
}