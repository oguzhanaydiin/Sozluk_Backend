# Domain

This project contains entities. It also have BaseEntity to derive others.

This is our BaseEntity. It can specialized if you choose to but thats enough for now.
```
public abstract class BaseEntity
{
    public Guid Id { get; set; }
    public DateTime CreateDate { get; set; }
}
```

Example entity derived from BaseEntity:
```
public class User: BaseEntity
{
    public string FirstName { get; set; }

    public string LastName { get; set; }


    public string EmailAddress { get; set; }

    public string UserName { get; set; }

    public string Password { get; set; }

    public bool EmailConfirmed { get; set; }


    public virtual ICollection<Entry> Entries { get; set; }
    public virtual ICollection<EntryFavorite> EntryFavorites { get; set; }


    public virtual ICollection<EntryComment> EntryComments { get; set; }
    public virtual ICollection<EntryCommentFavorite> EntryCommentFavorites { get; set; }
    
}

```
As we see; it has Entries, EntryFavorites, EntryComments, EntryCommentFavorites. 
Because every user have Entries, EntryFavorites. 
Users can comment on Entries or not so Users have EntryComments too.
Users can also like other people's entry comments if they want to.

User has this properties. But our relations with this properties will configured on Persistence project within EntityConfigurations.
