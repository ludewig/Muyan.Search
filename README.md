# Muyan.Search

基于Lucen.NET封装的工具库，支持.NET Core，包含通用的索引操作、检索查询、聚合统计等功能，默认使用JieBa分词器。

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
    "FacetPath": "Lucene/facet"
  }
```
### Startup.cs
```csharp
  services.AddSearchManager(new SearchManagerConfig()
  {
      DefaultPath = _appConfiguration["Search:DefaultPath"],
      FacetPath = _appConfiguration["Search:FacetPath"]
  });
```

## 实体属性
```csharp
public class DataEntity:IEntity<string>
{
  [Index(FieldName="",FieldType=FieldDataType.Text,IsStore=Field.Store.YES)]
  public string DataValue{get;set;}
}
```

## 依赖注入
```csharp
public class DataContentManager : DomainService, IDataContentManager
{
    private readonly ISearchManager _searchManager;
    public DataContentManager(ISearchManager searchManager)
    {
        _searchManager = searchManager;
    }
  }
```
