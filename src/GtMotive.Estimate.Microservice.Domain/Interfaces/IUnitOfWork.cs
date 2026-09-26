using System.Threading.Tasks;

namespace GtMotive.Estimate.Microservice.Domain.Interfaces {
    public interface IUnitOfWork {
        Task<int> Save();
    }
}
