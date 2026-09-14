using System.Collections.Generic;

namespace SimpleCollectionHub.Framework.Models;

public class PagedResult<T>(long totalCount, IReadOnlyList<T> items)
{
	public static PagedResult<T> Empty { get; } = new(0, null);

	public long TotalCount { get; } = totalCount;

	public IReadOnlyList<T> Items { get; } = items ?? [];
}
