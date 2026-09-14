using RW.Base.WPF.DependencyInjections;
using RW.Common.Helpers;
using System.IO;

namespace SimpleCollectionHub.Framework.Services;

/// <summary>只读探测用户绑定路径是否还在。禁止在该路径创建或修改文件。</summary>
public interface IPathProbeService
{
	/// <summary>文件或目录存在则为 true。</summary>
	bool Exists(string path);
}

/// <summary>用 File / Directory 探测，不做任何写入。</summary>
internal class PathProbeService : IPathProbeService, ISingletonDependency
{
	public bool Exists(string path)
	{
		if (path.IsBlank())
		{
			return false;
		}

		return File.Exists(path) || Directory.Exists(path);
	}
}
