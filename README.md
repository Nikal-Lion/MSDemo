# MSDemo

基于Asp.Net Core ~~3.1~~ 5.0 WebApi搭建后端多层网站架构（框架已升级为5.0）

博客链接：[ASP.NET Core搭建多层网站架构·博客园](https://www.cnblogs.com/kasnti/p/12236586.html)，基于此框架做以下修改：


- 添加swagger API文档
- Docker支持

### Swagger：
- 无特殊用法

### Docker支持：

##### 镜像构建
1. 在本机将代码发布至文件夹
2. 把发布目录在Linux中打包成名为 'publish.tgz' 文件
3. 将打包好的 'publish.tgz' 文件以及Dockerfile 放到一个空白目录下
	原因： 因为构建镜像所在目录下所有文件都将会被构建程序传入，导致构建镜像时间太久；


    说明： 传入构建进程中的文件不包含.dockerignore中声明的文件

##### 镜像文件说明

```

FROM wolfife/aspnetcore-runtime:latest

RUN sed -i 's/TLSv1.2/TLSv1.0/g' /etc/ssl/openssl.cnf
WORKDIR /app
ADD publish.tgz .

ENV ASPNETCORE_URLS http://*:80
ENV ASPNETCORE_ENVIRONMENT Development

WORKDIR /app
ENTRYPOINT ["/app/MS.WebApi"]
```

基础镜像：wolfife/aspnetcore-runtime   

  1. 该镜像已包含ASP.NET Core的运行环境
  2. 构建镜像时只需要将已打包好的文件，通过ADD命令将文件解压到目录下
  3. 修改openssl.cnf使的数据库连接可用
  4. 设置环境变量
  5. 修改目录，并设置入口

### 数据库说明

- 本示例使用MySQL作为存储数据库
- 需要手动创建数据库 ``` create database msdb default character set utf8mb4 collate utf8mb4_unicode_ci; ```
- 程序启动时会生成种子数据到数据库的表中
- 实体映射数据库表存在项目 MS.DbContexts 的 Mappings目录下



原 [Github](https://github.com/kasnti/MSDemo) 链接