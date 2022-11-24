# EF Core vs Dapper Query Performance Testing
A sample .NET Core web api to compare EF Core and Dapper query performance from a Postgres database. 

**In general, the results show that Dapper offers much faster performance compared to EF Core. All the test analysis validate that, but with Dapper you have to write more code and worry about the database, table and column creation.**  

[Entity Framework (EF) Core](https://learn.microsoft.com/en-us/ef/core) is a lightweight, extensible, open source and cross-platform version of the popular Entity Framework data access technology.

[Dapper](https://www.learndapper.com) is a simple object mapper for the Microsoft .NET platform. It is a framework for mapping an object-oriented domain model to a traditional relational database.

The data model is represented in the next image:
<p align="center">
  <img src="./docs/img/database-diagram-1.png" alt="Database diagram" width="500">
</p>

### Technologies in this repo:
* DotNet Core 6
* Entity Framework 6
* Dapper 2
* k6 (performance testing)
* Postgres (Docker Container)
* xUnit (Integration Tests)

#### Setup Database
Create the database container (you need to have Docker installed on your system):

```sh
docker run -d --name my-postgres -p 5432:5432 -e POSTGRES_PASSWORD=My@Passw0rd postgres
```

Stop and remove the container when needed:

```sh
docker stop my-postgres && docker rm my-postgres
```

#### Create Database

```sh
dotnet ef database update --project EntityFrameworkDapperApp.Repository.EntityFramework --startup-project EntityFrameworkDapperApp.Web
```

## Performance Testing Analysis
The idea is to create one analysis for every CRUD item. The duration of every performance test is 30 seconds, except for the DELETE request, where we will create 500 iterations.

In general, we want to see how many requests we can do in those 30 seconds and analyze all the benchmarking results.

#### **CREATE (post)**

Entity Framework
```sh
k6 run -e API_VERSION=v1 --duration 30s --vus 1 books-create-test.js
```
<p align="center">
  <img src="./docs/img/k6-books-v1-create-test.png" alt="Performance test" width="500">
</p>

Dapper
```sh
k6 run -e API_VERSION=v2 --duration 30s --vus 1 books-create-test.js
```
<p align="center">
  <img src="./docs/img/k6-books-v2-create-test.png" alt="Performance test" width="500">
</p>

Results:

| ORM       | avg | min | med | max | p(90) | p(95) | iterations |
|-----------| --- | --- | --- | --- | --- | --- |------------|
| `EF Core` | 2.78ms | 1.31ms | 1.98ms | 1.38s | 4.22ms | 6.03ms | 10.168     |
| `Dapper`  | 1.16ms | 680µs | 876µs | 110.37ms | 1.97ms | 2.46ms | 23.361     |