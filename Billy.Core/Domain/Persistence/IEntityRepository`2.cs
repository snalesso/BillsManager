using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Billy.Domain.Models;

namespace Billy.Domain.Persistence
{
    public interface IEntityRepository<TEntity, TIdentity>
        where TEntity : Entity<TIdentity>
        where TIdentity : IEquatable<TIdentity>
    {
        /* internal entity ctor prevents instantiating entity outside of its assembly -> ctor must be public
         * track can be created outside of repository -> repository can only validate Id
         * even if tracks could only be instantiated in repository, tracks would keep existing after being removed, so handling a removed track is already out of "safety"
         */
        //Task<TEntity> GetAllAsync<TEntity>();
        Task<TEntity> AddAsync(TEntity entity);
        Task<IReadOnlyCollection<TEntity>> AddAsync(IEnumerable<TEntity> entities);
    }
}