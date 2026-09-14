using FreeSql;
using FreeSql.Internal.Model;
using SimpleCollectionHub.Framework.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SimpleCollectionHub.Framework.Database;

public class EntityRepository<TEntity, TKey> : BaseRepository<TEntity, TKey>, INotifyPropertyChanged
	where TEntity : class, IEntity<TKey>
{
	public event PropertyChangedEventHandler PropertyChanged;

	public override ISelect<TEntity> Select => base.Select;

	public EntityRepository(IFreeSql sql) : base(sql)
	{
		if (sql is null)
		{
			throw new ArgumentNullException(nameof(sql));
		}
	}

	public virtual PagedResult<TEntity> GetPageModels(int page = 1, int pageSize = 20, string sort = "CreationTime")
	{
		BasePagingInfo pageInfo = new()
		{
			PageNumber = page,
			PageSize = pageSize,
		};
		List<TEntity> entities = base.Select.Page(pageInfo).OrderBy(sort).ToList();
		return new PagedResult<TEntity>(pageInfo.Count, entities);
	}

	protected virtual bool SetProperty<T>(ref T storage, T value, Action onChanged = null, [CallerMemberName] string propertyName = null)
	{
		storage = value;
		onChanged?.Invoke();
		RaisePropertyChanged(propertyName);
		return true;
	}

	protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
	{
		PropertyChanged?.Invoke(this, args);
	}

	protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
	{
		OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
	}
}