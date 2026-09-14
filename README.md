# SimpleCollectionHub

给已经躺在磁盘上的游戏、音乐、视频目录贴一层封面、截图、文字和链接。原文件继续留在原地；元数据只存在本机库。

## 打开方式

用 Visual Studio 或 `dotnet run` 启动 `Source/SimpleCollectionHub/SimpleCollectionHub.csproj`（解决方案：`SimpleCollectionHub.slnx`）。

数据目录：

`%LocalAppData%\RainbowWolfer\SimpleCollectionHub\`

其中 `Data\AppData.db` 是 SQLite 收藏库，`Data\Media\` 是导入图片的副本。软件不会在你绑定的原目录里新建、修改或删除任何文件。

## 会做什么 / 不会做什么

- 会：嵌套分组、绑定已有路径、自定义字段（图片 / 文字 / 链接）、标签筛选、完整度筛选、组内排序
- 不会：启动游戏或用默认程序打开文件、扫描原目录当封面、路径重新指定、拖放、Steam / 网络同步

## 设计稿

`Files/ui-prototype-4/` 是界面设计稿，不是产品文档。HTML 原型里的演示数据不会写入空库。

## 依赖

RW.Common / RW.Common.WPF / RW.Base.WPF 从仓库根目录 `Nugets\` 还原（根目录 `nuget.config`）。升级时把新的 `.nupkg` 放进 `Nugets\`，再改 `Source\Directory.Build.props` 里的 `RW_Common_Version`。
