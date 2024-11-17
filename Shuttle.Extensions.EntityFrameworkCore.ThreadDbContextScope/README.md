# Shuttle.Extensions.EntityFrameworkCore.ThreadDbContextScope

```
PM> Install-Package Shuttle.Extensions.EntityFrameworkCore.ThreadDbContextScope
```

Creates a new `DbContext` for each thread that is created.  This is useful when you have a multi-threaded application that requires a `DbContext` for each thread.

## Configuration

```c#
services.AddThreadDbContextScope();
```
