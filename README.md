# Muyan.Search
-----------------------------------------------
基于Lucen.NET封装的工具库，支持.NET Core，包含通用的索引操作、检索查询、聚合统计等功能。

## NuGet安装
```powershell
PM> Install-Package Muyan.Search
```
## .NET Core项目配置

### appsetting.json
```json
  //全文检索配置
  "Search": {
    "DefaultPath": "Lucene/data",
    "FacetPath": "Lucene/facet",
    "StopWords": "Resources/stopwords.txt"
  }
```
### Startup.cs
```csharp
  services.AddSearchManager(new SearchManagerConfig()
  {
      DefaultPath = _appConfiguration["Search:DefaultPath"],
      FacetPath = _appConfiguration["Search:FacetPath"],
      StopWords = _appConfiguration["Search:StopWords"]
  });
```
