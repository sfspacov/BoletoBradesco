using System;
using System.Data.Entity;

namespace Boleto.Domain
{
    public partial class Entities : IEntities
    {
        void IEntities.SaveChanges()
        {
            SaveChanges();
        }
    }
}
