using System.Data.Entity;

namespace Boleto.Domain
{
    public interface IEntities
    {
        IDbSet<BradescoIntegration> BradescoIntegration { get; set; }
        void SaveChanges();
    }
}
