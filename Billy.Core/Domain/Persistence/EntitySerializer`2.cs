﻿using Billy.Domain.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Threading;
using System.Threading.Tasks;

namespace Billy.Domain.Persistence
{
    // TODO: save to a copy, if save failes, reload data to undo changes to in memory entities
    // TODO: guard from concurrent Deserialize/Serialize, ecc.
    public abstract class EntitySerializer<TEntity, TIdentity> : IDisposable
        where TEntity : Entity<TIdentity> // TODO: in order to provide rollback to handle updates' failures without reloading in memory cached data to undo changes, add constraint: where TEntity : IEditableObject
        where TIdentity : IEquatable<TIdentity>
    {
        #region constants & fields

        protected readonly string _dbFilePath;
        protected ConcurrentDictionary<TIdentity, TEntity> _entities;

        #endregion

        #region ctor

        public EntitySerializer(string dbFilePath)
        {
            this._dbFilePath = dbFilePath;

            this._serializationSemaphore = new SemaphoreSlim(1, 1).DisposeWith(this._disposables);
        }

        #endregion

        #region methods

        public abstract Task<TIdentity> GetNewIdentity();

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            TEntity result = null;

            await this.Serialize(() =>
            {
                if (!this._entities.ContainsKey(entity.Id))
                {
                    if (!this._entities.TryAdd(entity.Id, entity))
                    {
                        throw new Exception();
                    }

                    result = entity;
                }
            });

            return result;
        }

        public async Task<IReadOnlyCollection<TEntity>> AddAsync(IEnumerable<TEntity> entities)
        {
            IReadOnlyCollection<TEntity> result = null;

            await this.Serialize(() =>
            {
                // ensure NONE of the entities are already in the DB
                if (!entities.Any(t => this._entities.ContainsKey(t.Id)))
                {
                    foreach (var entity in entities)
                    {
                        if (!this._entities.TryAdd(entity.Id, entity))
                        {
                            throw new Exception("feawfkpawkefawkefpawke");
                        }

                    }

                    result = entities.ToArray();
                    // TODO: handle when list of things to add is null or empty
                    //this._addedSubject.OnNext(result);
                }
            });

            return result;
        }

        public async Task<IReadOnlyCollection<TEntity>> GetAllAsync()
        {
            await this.EnsureDeserialized();

            return this._entities.Values.ToArray();
        }

        public async Task<TEntity> RemoveAsync(TIdentity identity)
        {
            TEntity removedEntity = null;

            await this.Serialize(() =>
            {
                if (!this._entities.TryRemove(identity, out removedEntity))
                {
                    throw new Exception();
                }
            });

            return removedEntity;
        }

        public async Task<IReadOnlyCollection<TEntity>> RemoveAsync(IEnumerable<TIdentity> identities)
        {
            List<TEntity> removedEntities = new List<TEntity>(identities.Count());

            await this.Serialize(() =>
            {
                foreach (var identity in identities)
                {
                    if (!this._entities.ContainsKey(identity))
                    {
                        throw new Exception();
                    }
                }

                foreach (var identity in identities)
                {
                    if (!this._entities.TryRemove(identity, out var entity))
                    {
                        throw new Exception();
                    }

                    removedEntities.Add(entity);
                }
            });

            return removedEntities;
        }

        #region serialization

        private bool _isDeserialized = false;
        private readonly SemaphoreSlim _serializationSemaphore;

        protected async Task EnsureDeserialized()
        {
            await this._serializationSemaphore.WaitAsync();
            if (!this._isDeserialized)
            {
                // TODO: handle exceptions
                await this.DeserializeCore();
                this._isDeserialized = true;
            }
            this._serializationSemaphore.Release();
        }

        private async Task Serialize(Task alterDbAction)
        {
            await this.EnsureDeserialized();

            await this._serializationSemaphore.WaitAsync();

            // TODO: handle exceptions
            await alterDbAction;
            // TODO: handle exceptions
            await this.SerializeCore();

            this._serializationSemaphore.Release();
        }

        private Task Serialize(Action alterDbAction)
        {
            return this.Serialize(Task.Run(() => alterDbAction()));
        }

        protected abstract Task DeserializeCore();
        protected abstract Task SerializeCore();

        #endregion

        #endregion

        //#region events

        //private readonly ISubject<IReadOnlyList<TEntity>> _addedSubject = new Subject<IReadOnlyList<TEntity>>();
        //public IObservable<IReadOnlyList<TEntity>> Addeded => this._addedSubject;

        //#endregion

        #region IDisposable

        // https://docs.microsoft.com/en-us/dotnet/api/system.idisposable?view=netframework-4.8
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private bool _isDisposed = false;

        // use this in derived class
        // protected override void Dispose(bool isDisposing)
        // use this in non-derived class
        protected virtual void Dispose(bool isDisposing)
        {
            if (this._isDisposed)
                return;

            if (isDisposing)
            {
                // free managed resources here
                this._disposables.Dispose();
            }

            // free unmanaged resources (unmanaged objects) and override a finalizer below.
            // set large fields to null.

            this._isDisposed = true;
        }

        // remove if in derived class
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool isDisposing) above.
            this.Dispose(true);
        }

        #endregion
    }
}