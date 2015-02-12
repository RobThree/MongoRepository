# ![Logo](https://raw.githubusercontent.com/RobThree/MongoRepository/master/mongorepositorylogo.png) Project Description

An easy to use library to use MongoDB with .NET. It implements a Repository pattern on top of Official MongoDB C# driver. This project is now available as a [NuGet](https://www.nuget.org) package for your convenience. If you're new to NuGet, [check it out](http://docs.nuget.org/); it's painless, easy and fast. You can find this project by [searching for MongoRepository](https://www.nuget.org/packages?q=mongorepository) in NuGet (or [simply clicking here](http://nuget.org/packages/MongoRepository)).

Check the [documentation](https://github.com/RobThree/MongoRepository/wiki/Documentation) for a step-by-step example and more advanced usage.

## Example:

```c#
// The Entity base-class is provided by MongoRepository
// for all entities you want to use in MongoDb
public class Customer : Entity 
{
        public string FirstName { get; set; }
        public string LastName { get; set; }
}

public class CustomerRepoTest
{
        public void Test()
        {
            var repo = new MongoRepository<Customer>();

            // adding new entity
            var newCustomer = new Customer {
                FirstName = "Steve",
                LastName = "Cornell"
            };

            repo.Add(newCustomer);

            // searching
            var result = repo.Where(c => c.FirstName == "Steve");

            // updating 
            newCustomer.LastName = "Castle";
            repo.Update(newCustomer);
        }
}
```
[<img src="http://i.imgur.com/2yf60gf.png" alt="Productivity Visual Studio add-in for C#, VB.NET, XML, XAML, ASP.NET and more">](http://www.jetbrains.com/resharper/features/index.html)
