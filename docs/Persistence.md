# Persistence

## Creating Entities

We created our entities in another project named Domain. We have BaseEntity and other models for entities derived from it.

### BaseEntity.cs
```
public abstract class BaseEntity
{
    public Guid Id { get; set; }

    public DateTime CreateDate { get; set; }
}
```
Example for an entity class:
```
public class User: BaseEntity
{
    public string FirstName { get; set; }

    public string LastName { get; set; }
    
}
```
## Entity Configuration
We have a general BaseEntityConfiguration, other EntityConfiguration files derived from it.

### BaseEntityConfiguration.cs
```
public abstract class BaseEntityConfiguration<T> : IEntityTypeConfiguration<T> where T : BaseEntity
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Id).ValueGeneratedOnAdd();
        builder.Property(i => i.CreateDate).ValueGeneratedOnAdd();
    }
}
```
Example for any other EntityConfiguration file derived from BaseEntityConfiguration:
This class is for configuring the User entity for build a table called "dbo.User"
```
public class UserEntityConfiguration : BaseEntityConfiguration<User>
{
    public override void Configure(EntityTypeBuilder<User> builder)
    {
        base.Configure(builder);

        builder.ToTable("user", "dbo");
    }
}
```

## Creating Repositories for Entities

* GenericRepository
The main part of this process, we need an interface for general purpose IGenericRepository and we need a class called GenericRepository inherites IGenericRepository.
GenericRepository contains methods for general purpose entity functionality.
```
public interface IGenericRepository<TEntity> where TEntity: BaseEntity
{
    Task<int> AddAsync(TEntity entity);
    int Add(TEntity entity);
    int Add(IEnumerable<TEntity> entities);
    int Update(TEntity entity);
    .
    .
}

public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : BaseEntity
{
    private readonly DbContext dbContext;
    protected DbSet<TEntity> entity => dbContext.Set<TEntity>();

    public GenericRepository(DbContext dbContext)
    {
        this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public virtual async Task<int> AddAsync(TEntity entity)
    {
        await this.entity.AddAsync(entity);
        return await dbContext.SaveChangesAsync();
    }

    public virtual int Add(TEntity entity)
    {
        this.entity.Add(entity);
        return dbContext.SaveChanges();
    }

    public Task<int> SaveChangesAsync()
    {
        return dbContext.SaveChangesAsync();
    }

    public int SaveChanges()
    {
        return dbContext.SaveChanges();
    }

}
```
* Created Repositories
Now we can create repositories based on the entity. It should be derived from GenericRepository. It can contain special function based on the entity only.
Example Repository:
```
public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(BlazorSozlukContext dbContext) : base(dbContext)
    {
    }
}
```

## Creating The Context

### BlazorSozlukContext.cs : DbContext
This class is inherited from DbContext class.

* Has two constructors

```
public BlazorSozlukContext()
{
}

public BlazorSozlukContext(DbContextOptions options) : base(options)
{
}
```

* Has DbSet properties

Example:
```
public DbSet<User> Users { get; set; }
public DbSet<Entry> Entries { get; set; }
```

* Has Overrides 

In initial state if its not configured, use this SqlServer and configure an option for enabling retry mechanism:
```
 protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var connectionString = "exampleConnectionString";
            optionsBuilder.UseSqlServer(connStr, opt =>
            {
                opt.EnableRetryOnFailure();
            });
        }
    }
```
On model creating, apply assembly configurations from current assembly.
```
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
```
These are methods for the last time editing mechanism for entities that we want to add. 
```
    private void OnBeforeSave()
    {
        var addedEntites = ChangeTracker.Entries()
                                .Where(i => i.State == EntityState.Added)
                                .Select(i => (BaseEntity)i.Entity);

        PrepareAddedEntities(addedEntites);
    }
    
    private void PrepareAddedEntities(IEnumerable<BaseEntity> entities)
    {
        foreach (var entity in entities)
        {
            if (entity.CreateDate == DateTime.MinValue)
                entity.CreateDate = DateTime.Now;
        }
    }
```


Save changes methods, they can be changed into Unit of Work design pattern
```
    public override int SaveChanges()
    {
        OnBeforeSave();
        return base.SaveChanges();
    }
    
    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        OnBeforeSave();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }
    
    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        OnBeforeSave();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }
    
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        OnBeforeSave();
        return base.SaveChangesAsync(cancellationToken);
    }
```

## Seeding Data
We will create SeedData class for seeding the data with Bogus Nuget Package for creating fake data.
We determine rules for each entities for creating fake datas and bulk insert to them in database.

```
internal class SeedData
{
    private static List<User> GetUsers()
    {
        var result = new Faker<User>("tr")
                .RuleFor(i => i.Id, i => Guid.NewGuid())
                .RuleFor(i => i.CreateDate,
                        i => i.Date.Between(DateTime.Now.AddDays(-100), DateTime.Now))
                .RuleFor(i => i.FirstName, i => i.Person.FirstName)
                .RuleFor(i => i.LastName, i => i.Person.LastName)
                .RuleFor(i => i.EmailAddress, i => i.Internet.Email())
                .RuleFor(i => i.UserName, i => i.Internet.UserName())
                .RuleFor(i => i.Password, i => PasswordEncryptor.Encrpt(i.Internet.Password()))
                .RuleFor(i => i.EmailConfirmed, i => i.PickRandom(true, false))
            .Generate(500);

        return result;
    }

    public async Task SeedAsync(IConfiguration configuration)
    {
        var dbContextBuilder = new DbContextOptionsBuilder();
        dbContextBuilder.UseSqlServer(configuration["BlazorSozlukDbConnectionString"]);

        var context = new BlazorSozlukContext(dbContextBuilder.Options);

        if (context.Users.Any())
        {
            await Task.CompletedTask;
            return;
        }

        var users = GetUsers();
        var userIds = users.Select(i => i.Id);

        await context.Users.AddRangeAsync(users);

        var guids = Enumerable.Range(0, 150).Select(i => Guid.NewGuid()).ToList();
        int counter = 0;

        var entries = new Faker<Entry>("tr")
                .RuleFor(i => i.Id, i => guids[counter++])
                .RuleFor(i => i.CreateDate,
                            i => i.Date.Between(DateTime.Now.AddDays(-100), DateTime.Now))
                .RuleFor(i => i.Subject, i => i.Lorem.Sentence(5, 5))
                .RuleFor(i => i.Content, i => i.Lorem.Paragraph(2))
                .RuleFor(i => i.CreatedById, i => i.PickRandom(userIds))
            .Generate(150);

        await context.Entries.AddRangeAsync(entries);

        var comments = new Faker<EntryComment>("tr")
                .RuleFor(i => i.Id, i => Guid.NewGuid())
                .RuleFor(i => i.CreateDate,
                            i => i.Date.Between(DateTime.Now.AddDays(-100), DateTime.Now))
                .RuleFor(i => i.Content, i => i.Lorem.Paragraph(2))
                .RuleFor(i => i.CreatedById, i => i.PickRandom(userIds))
                .RuleFor(i => i.EntryId, i => i.PickRandom(guids))
            .Generate(1000);

        await context.EntryComments.AddRangeAsync(comments);
        await context.SaveChangesAsync();
    }
}

```

## Adding Migration and Updating Database
At this step; we can open PackageManagerConsole, choose the current Project and create an migration by "Add-migration migrationName".
This will create Migration folder to our project and get the current migration files for us automatically.
Then we can execute "Update Database" command on PackageManagerConsole to applying those migration files to our database.

## Creating Registiration File
This is an optional final step. We could skip this an add files to current project's Program file. But we want to use this Project and Entities on another Project. So we will use this Persistence project in another class, we should create registiration methods.

We will AddDbContext  to our services with using DbContext that we created earlier.
Also we should add our Repositories scoped to use them with DependencyInjection.
We also could seed data optionally. If we have data in our db we dont need to seedData everytime, we need it initially in empty database only.
```
public static class Registration
{
    public static IServiceCollection AddInfrastructureRegistration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<BlazorSozlukContext>(conf => 
        {
            var connStr = configuration["BlazorSozlukDbConnectionString"].ToString();
            conf.UseSqlServer(connStr, opt => 
            {
                opt.EnableRetryOnFailure();
            });
        });

        // SEEDING DATA INITIAL FOR TESTING
        // UNCOMMENT THIS LINES FOR SEED DATA FIRST TIME

        //var seedData = new SeedData();
        //seedData.SeedAsync(configuration).GetAwaiter().GetResult();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IEmailConfirmationRepository, EmailConfirmationRepository>();
        services.AddScoped<IEntryRepository, EntryRepository>();
        services.AddScoped<IEntryCommentRepository, EntryCommentRepository>();

        return services;
    }
}
```
