using FreeSql.DataAnnotations;
using System;

namespace SimpleCollectionHub.Framework.Database;

public interface IEntity<T>
{
	T Id { get; }
}

public interface IEntity
{
	Guid Id { get; }
}

public abstract class Entity<T> : IEntity<T>
{

	[Column(IsPrimary = true, IsIdentity = true)]
	public virtual T Id { get; set; }

	protected Entity()
	{
		Id = default;
	}

	protected Entity(T id)
	{
		Id = id;
	}
}

public abstract class Entity : Entity<Guid>
{
	protected Entity()
	{
		Id = Guid.NewGuid();
	}

	protected Entity(Guid id)
	{
		Id = id;
	}
}

