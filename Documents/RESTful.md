<!-- TOC -->

- [RESTful 级别划分](#restful-级别划分)
    - [0 级 -- The swarmp of POX(Plain old XML) - POX 沼泽](#0-级----the-swarmp-of-poxplain-old-xml---pox-沼泽)
    - [1 级 -- Resource - 资源](#1-级----resource---资源)
    - [2 级 -- HTTP Verbs - 动词](#2-级----http-verbs---动词)
    - [3 级 -- Hypermedia Controls - 超媒体](#3-级----hypermedia-controls---超媒体)
- [HTTP 动词概述](#http-动词概述)
    - [GET(获取资源)](#get获取资源)
    - [DELETE(删除资源)](#delete删除资源)
    - [POST(创建资源)](#post创建资源)
    - [PUT(整体更新)](#put整体更新)
    - [PATCH(局部更新)](#patch局部更新)
    - [HEAD 和 OPTIONS 比较少用--略](#head-和-options-比较少用--略)
- [HTTP 请求的安全性和幂等性](#http-请求的安全性和幂等性)
- [HTTP 动词细节](#http-动词细节)
    - [GET](#get)
    - [POST](#post)
    - [DELETE](#delete)
    - [PUT](#put)
    - [PATCH](#patch)
- [HTTP 状态码详解](#http-状态码详解)
    - [200 级别](#200-级别)
    - [400 级别](#400-级别)
    - [500 级别](#500-级别)
- [Media Type](#media-type)

<!-- /TOC -->

### RESTful 级别划分

#### 0 级 -- The swarmp of POX(Plain old XML) - POX 沼泽

这里`HTTP`协议只是被用来进行远程交互，协议的其余部分都用错来，都是`RPC`风格的实现(例如`SOAP`，尤其是使用`WCF`的时候)

#### 1 级 -- Resource - 资源

每个资源都映射在一个`URI`上，但是`HTTP`方法没有被正确使用，结果的复杂度不算太高

#### 2 级 -- HTTP Verbs - 动词

正确使用`HTTP`动词，状态码也正确使用了，同时也去掉了不必要的变种

#### 3 级 -- Hypermedia Controls - 超媒体

API 支持超媒体作为应用状态的引擎`HATEOAS(Hypertext As The Engine Of Application State)`，引入了可发现性(即服务端的响应要封装"下一步如何做")

### HTTP 动词概述

#### GET(获取资源)

- `GET api/orders`，返回`200`，集合数据；找不到数据返回`404`
- `GET api/orders/{id}`，返回`200`，单个数据，找不到数据返回`404`

#### DELETE(删除资源)

- `DELETE api/orders/{id}`，成功返回`204`；没找到资源返回`404`
- `DELETE api/orders`，很少用，成功返回`204`；没找到资源返回`404`

#### POST(创建资源)

- `POST api/orders`，成功返回`201`和单个数据；如果资源没有创建则返回`404`
- `POST api/orders/{id}`，肯定不会成功，返回`404`或`409`
- `POST api/ordercollections`，成功返回`201`和集合数据；没有创建资源则返回`404`

#### PUT(整体更新)

- `PUT api/orders/{id}`，成功返回`200`或`204`；没找到资源返回`404`
- `PUT api/orders`，集合操作很少见，成功返回`200`或`204`；没找到资源返回`404`

#### PATCH(局部更新)

- `PATCH api/orders/{id}`，成功返回`200`和单个数据，或`204`不带数据；没找到资源返回`404`
- `PATCH api/orders`，集合操作很少见，成功返回`200`或`204`；没找到资源返回`404`

#### HEAD 和 OPTIONS 比较少用--略

### HTTP 请求的安全性和幂等性

    - 安全性：方法执行后会不会改变资源的表述
    - 幂等性：方法无论执行多少次都会得到同样的结果

| HTTP 方法 | 安全? | 幂等? |
| :-------- | :---- | :---- |
| GET       | 是    | 是    |
| OPTIONS   | 是    | 是    |
| HEAD      | 是    | 是    |
| POST      | 不    | 不    |
| DELETE    | 不    | 是    |
| PUT       | 不    | 是    |
| PATCH     | 不    | 不    |

### HTTP 动词细节

#### GET

- 资源应该使用名词
  - ~~`api/getusers`~~ 错误
  - `api/users` 正确
  - `api/users/{id}` 正确
- 资源有层级关系
  - 例如 `api/orders/{orderId}/products`，这表示了订单和商品之间是有主从关系
  - `api/orders/{orderId}/products/{productId}`，表示指定订单下的指定商品
- 过滤和排序不是资源，应该作为参数
  - 例如`api/orders?orderby=createdon`
- 资源的`URI`应该永远都是一样的
  - 推荐`GUID`应该作为主键来使用
  - 自增`int`类型的 Id，在迁移到新数据库的时候需要特殊设置，保证 Id 值不变
- 分页查询使用抽象父类`QueryParameter`，常见参数：`PageIndex`,`PageSize`,`OrderBy`
- 如果将数据和翻页元素一起返回：
  - 相应的 body 不再符合`Accept Header`了(不再是资源的`application/json`)，这是一种新的`media type`
  - 违反`REST`约束，API 消费者不知道如何通过`application/json`这个类型来解析相应的数据
- 翻页数据不是资源表述的一部分，应使用自定义的`Header(X-Pagination)`
- 存在翻页的数据类：`PaginatedList<T>`继承于`List<T>`
- 过滤：对集合资源附加一些条件，筛选出结果
  - api/orders?orderNo=12345
  - 条件应用于`Resource Model`
  - 过滤条件可以放在`QueryParameter`的子类中
- 搜索：使用关键字对集合资源进行模糊搜索
- 翻页(较复杂 _todo:完善代码后补充文档_)
  - 需要排序
  - 让资源按资源的某个或多个属性进行正向或反向的排序
  - `Resource Model`的一个属性可能会映射到`Entity Model`的多个属性上
  - `Resource Model`上的正序可能在`Entity Model`上是倒序的
  - 需要支持多属性的排序
  - 代码服用
- 如果资源的属性较多，而且 API 消费者只需要一部分属性，则可以考虑资源塑形
  - 集合资源
  - 单个资源
  - 异常处理

#### POST

- 接受参数`[FromBody]`
- 成功返回 201 Created
  - `CreatedAtRoute()`：这个方法允许在响应里带着`Location Header`，在这个`Location Header`里包含着一个`URI`,通过这个`URI`就可以`GET`到刚刚创建好的资源
- 一次性添加集合资源
  - 把整个集合看作一个资源
  - 参数`[FromBody]IEnumerable<T>`
  - 返回 201，`CreatedAtRoute()`，带着 Id 的集合
  - `GET`方法参数为 Id，用于查询创建的资源(`ArrayModelBinder:IModelBinder`)
- Model 验证
  - 定义验证规则
  - 检查验证规则
  - 把验证错误信息发送给 API 消费者
    - 返回 422 Unprocessable Entity
    - `return BadRequest(ModelState);`
    - 验证错误信息在响应 body 里面带回去
  - 内置验证
    - `DataAnnotation`
    - `ValidationAttribute`
    - `IValidatebleObject`
  - 第三方验证
    - `FluentValidation`

#### DELETE

- 参数：Id
- 返回 204 No Content

#### PUT

- 参数：Id，`[FromBody]`(fromBody 不需要 Id)
- 需要单独的`Resource Model`
- 返回 200 或 204
- 整体更新容易引起问题(将未赋值的属性更新为`null`)

#### PATCH

- 参数：Id，`[FromBody] (JsonPathDocument<T>)`
- `patchDoc.ApplyTo()`
- 返回 200 或 204
- 遵照`RFC 6902(Json Patch)`
- 使用的`media type`为`application/json-patch+json`
- 参数示例
  ```
  [
      {
          "op":"replace",
          "path":"/status",
          "value":"new status"
      },
      {
          "op":"remove",
          "path":"/mobile"
      }
  ]
  ```
- op 操作类型
  - 添加：`{"op":"add","path":"/xxx","value":"xxx"}`，如果属性不存在，那么就添加属性，如果属性已存在，就更新属性的值，这个对静态类型不适用
  - 删除：`{"op":"remove","path":"/xxx"}`，删除某个属性，或把它的值设为默认值(例如空值)
  - 替换：`{"op":"replace","path":"/xxx","value":"xxx"}`，改变属性的值，可以理解为先删除后添加
  - 复制：`{"op":"copy","from":"/xxx","value":"/yyy"}`，把某个属性的值赋值给目标属性
  - 移动：`{"op":"move","from":"/xxx","value":"/yyy"}`，把某个属性的值赋值给目标属性，并把源属性删除或设为默认值
  - 测试：`{"op":"test","path":"/xxx","value":"xxx"}`，测试目标属性的值和指定的值是否一致
- path：资源的属性名，可以有层级结构
- value：更新的值

### HTTP 状态码详解

#### 200 级别

- 200 - OK(响应可包含内容)
- 201 - Created - 资源创建成功
- 204 - No content - 成功执行，但响应不返回任何东西

#### 400 级别

- 400 - Bad request - 表示 API 的消费者发送到服务器的请求是错误的
- 401 - Unauthorized - 表示没有权限
- 403 - Forbidden - 表示用户验证成功，但是该用户仍然无法访问该资源
- 404 - Not found - 表示请求的资源不存在
- 405 - Method not allowed - API 消费者尝试发送请求给某个资源时，使用的 HTTP 方法确实不允许的，例如使用`POST api/countries`，而该资源只实现量`GET`，所以`POST`不被允许
- 406 - Not acceptable - 这里涉及到`media type`，例如 API 消费者请求的是`application/xml`格式的`media type`，而 API 只支持`application/json`
- 409 - Conflict - 表示该请求无法完成，因为请求与当前资源的状态有冲突，例如 API 消费者编辑某个资源数据以后，该资源又被其他人更新了，这是 API 消费者再`PUT`数据就会出现`409`，有时也用在尝试创建资源时该资源已存在的情况
- 415 - Unsupported media type - 这个和`406`相反，API 消费者提交的数据`media type`是`xml`的，而服务器只支持`json`，则会返回`415`
- 422 - Unprocessable entity - 表示请求的格式没问题，但语义有错误，例如实体验证错误

#### 500 级别

- 500 - Internal server error - 表示服务器发生错误

### Media Type

如果资源支持多种展现格式，那么 API 消费者可以选择它想要的格式

- 在请求的`Accept Header`注定`Media Type`
  - `appliaction/json`，`application/xml`
  - 若未指定，则默认为`application/json`
- 请求的`media type`不可用时，并且消费者不支持默认格式，则返回`406`
- `ASP.NET Core`支持输出和输入两种格式化器
  - 用于输出的`media type`放在`Accept Header`里，表示客户端接受这种格式的输出
  - 用于输入的`media type`放在`Content-Type Header`里，表示客户端传进来的数据是这种格式
  - 在service.AddMvc()的时候将`ReturnHttpNotAcceptable`设为`true`，就会返回`406`
