using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using MongoRepository;
using MongoRepositoryTests.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace MongoRepositoryTests
{
    //TODO: We REALLY need some decent tests and cleanup this mess.

    [TestClass]
    public class RepoTests
    {
        [TestInitialize]
        public void Setup()
        {
            this.DropDB();
        }

        [TestCleanup]
        public void Cleanup()
        {
            this.DropDB();
        }

        private void DropDB()
        {
            var url = new MongoUrl(ConfigurationManager.ConnectionStrings["MongoServerSettings"].ConnectionString);
            var client = new MongoClient(url);
            client.GetServer().DropDatabase(url.DatabaseName);
        }


        [TestMethod]
        public void AddAndUpdateTest()
        {
            IRepository<Customer> _customerRepo = new MongoRepository<Customer>();
            IRepositoryManager<Customer> _customerMan = new MongoRepositoryManager<Customer>();

            Assert.IsFalse(_customerMan.Exists);

            var customer = new Customer();
            customer.FirstName = "Bob";
            customer.LastName = "Dillon";
            customer.Phone = "0900999899";
            customer.Email = "Bob.dil@snailmail.com";
            customer.HomeAddress = new Address
            {
                Address1 = "North kingdom 15 west",
                Address2 = "1 north way",
                PostCode = "40990",
                City = "George Town",
                Country = "Alaska"
            };

            _customerRepo.Add(customer);

            Assert.IsTrue(_customerMan.Exists);

            Assert.IsNotNull(customer.Id);

            // fetch it back 
            var alreadyAddedCustomer = _customerRepo.Where(c => c.FirstName == "Bob").Single();

            Assert.IsNotNull(alreadyAddedCustomer);
            Assert.AreEqual(customer.FirstName, alreadyAddedCustomer.FirstName);
            Assert.AreEqual(customer.HomeAddress.Address1, alreadyAddedCustomer.HomeAddress.Address1);

            alreadyAddedCustomer.Phone = "10110111";
            alreadyAddedCustomer.Email = "dil.bob@fastmail.org";

            _customerRepo.Update(alreadyAddedCustomer);

            // fetch by id now 
            var updatedCustomer = _customerRepo.GetById(customer.Id);

            Assert.IsNotNull(updatedCustomer);
            Assert.AreEqual(alreadyAddedCustomer.Phone, updatedCustomer.Phone);
            Assert.AreEqual(alreadyAddedCustomer.Email, updatedCustomer.Email);

            Assert.IsTrue(_customerRepo.Exists(c => c.HomeAddress.Country == "Alaska"));
        }

        [TestMethod]
        public void ComplexEntityTest()
        {
            IRepository<Customer> _customerRepo = new MongoRepository<Customer>();
            IRepository<Product> _productRepo = new MongoRepository<Product>();

            var customer = new Customer();
            customer.FirstName = "Erik";
            customer.LastName = "Swaun";
            customer.Phone = "123 99 8767";
            customer.Email = "erick@mail.com";
            customer.HomeAddress = new Address
            {
                Address1 = "Main bulevard",
                Address2 = "1 west way",
                PostCode = "89560",
                City = "Tempare",
                Country = "Arizona"
            };

            var order = new Order();
            order.PurchaseDate = DateTime.Now.AddDays(-2);
            var orderItems = new List<OrderItem>();

            var shampoo = _productRepo.Add(new Product() { Name = "Palmolive Shampoo", Price = 5 });
            var paste = _productRepo.Add(new Product() { Name = "Mcleans Paste", Price = 4 });


            var item1 = new OrderItem { Product = shampoo, Quantity = 1 };
            var item2 = new OrderItem { Product = paste, Quantity = 2 };

            orderItems.Add(item1);
            orderItems.Add(item2);

            order.Items = orderItems;

            customer.Orders = new List<Order>
            {
                order
            };

            _customerRepo.Add(customer);

            Assert.IsNotNull(customer.Id);
            Assert.IsNotNull(customer.Orders[0].Items[0].Product.Id);

            // get the orders  
            var theOrders = _customerRepo.Where(c => c.Id == customer.Id).Select(c => c.Orders).ToList();
            var theOrderItems = theOrders[0].Select(o => o.Items);

            Assert.IsNotNull(theOrders);
            Assert.IsNotNull(theOrderItems);
        }


        [TestMethod]
        public void BatchTest()
        {
            IRepository<Customer> _customerRepo = new MongoRepository<Customer>();

            var custlist = new List<Customer>(new Customer[] {
                new Customer() { FirstName = "Customer A" },
                new Customer() { FirstName = "Client B" },
                new Customer() { FirstName = "Customer C" },
                new Customer() { FirstName = "Client D" },
                new Customer() { FirstName = "Customer E" },
                new Customer() { FirstName = "Client F" },
                new Customer() { FirstName = "Customer G" },
            });

            //Insert batch
            _customerRepo.Add(custlist);

            var count = _customerRepo.Count();
            Assert.AreEqual(7, count);
            foreach (Customer c in custlist)
                Assert.AreNotEqual(new string('0', 24), c.Id);

            //Update batch
            foreach (Customer c in custlist)
                c.LastName = c.FirstName;
            _customerRepo.Update(custlist);

            foreach (Customer c in _customerRepo)
                Assert.AreEqual(c.FirstName, c.LastName);

            //Delete by criteria
            _customerRepo.Delete(f => f.FirstName.StartsWith("Client"));

            count = _customerRepo.Count();
            Assert.AreEqual(4, count);

            //Delete specific object
            _customerRepo.Delete(custlist[0]);

            //Test AsQueryable
            var selectedcustomers = from cust in _customerRepo
                                    where cust.LastName.EndsWith("C") || cust.LastName.EndsWith("G")
                                    select cust;

            Assert.AreEqual(2, selectedcustomers.ToList().Count);

            count = _customerRepo.Count();
            Assert.AreEqual(3, count);

            //Drop entire repo
            new MongoRepositoryManager<Customer>().Drop();

            count = _customerRepo.Count();
            Assert.AreEqual(0, count);
        }

        [TestMethod]
        public void CollectionNamesTest()
        {
            var a = new MongoRepository<Animal>();
            var am = new MongoRepositoryManager<Animal>();
            var va = new Dog();
            Assert.IsFalse(am.Exists);
            a.Update(va);
            Assert.IsTrue(am.Exists);
            Assert.IsInstanceOfType(a.GetById(va.Id), typeof(Dog));
            Assert.AreEqual(am.Name, "AnimalsTest");
            Assert.AreEqual(a.CollectionName, "AnimalsTest");

            var cl = new MongoRepository<CatLike>();
            var clm = new MongoRepositoryManager<CatLike>();
            var vcl = new Lion();
            Assert.IsFalse(clm.Exists);
            cl.Update(vcl);
            Assert.IsTrue(clm.Exists);
            Assert.IsInstanceOfType(cl.GetById(vcl.Id), typeof(Lion));
            Assert.AreEqual(clm.Name, "Catlikes");
            Assert.AreEqual(cl.CollectionName, "Catlikes");

            var b = new MongoRepository<Bird>();
            var bm = new MongoRepositoryManager<Bird>();
            var vb = new Bird();
            Assert.IsFalse(bm.Exists);
            b.Update(vb);
            Assert.IsTrue(bm.Exists);
            Assert.IsInstanceOfType(b.GetById(vb.Id), typeof(Bird));
            Assert.AreEqual(bm.Name, "Birds");
            Assert.AreEqual(b.CollectionName, "Birds");

            var l = new MongoRepository<Lion>();
            var lm = new MongoRepositoryManager<Lion>();
            var vl = new Lion();
            //Assert.IsFalse(lm.Exists);   //Should already exist (created by cl)
            l.Update(vl);
            Assert.IsTrue(lm.Exists);
            Assert.IsInstanceOfType(l.GetById(vl.Id), typeof(Lion));
            Assert.AreEqual(lm.Name, "Catlikes");
            Assert.AreEqual(l.CollectionName, "Catlikes");

            var d = new MongoRepository<Dog>();
            var dm = new MongoRepositoryManager<Dog>();
            var vd = new Dog();
            //Assert.IsFalse(dm.Exists);
            d.Update(vd);
            Assert.IsTrue(dm.Exists);
            Assert.IsInstanceOfType(d.GetById(vd.Id), typeof(Dog));
            Assert.AreEqual(dm.Name, "AnimalsTest");
            Assert.AreEqual(d.CollectionName, "AnimalsTest");

            var m = new MongoRepository<Bird>();
            var mm = new MongoRepositoryManager<Bird>();
            var vm = new Macaw();
            //Assert.IsFalse(mm.Exists);
            m.Update(vm);
            Assert.IsTrue(mm.Exists);
            Assert.IsInstanceOfType(m.GetById(vm.Id), typeof(Macaw));
            Assert.AreEqual(mm.Name, "Birds");
            Assert.AreEqual(m.CollectionName, "Birds");

            var w = new MongoRepository<Whale>();
            var wm = new MongoRepositoryManager<Whale>();
            var vw = new Whale();
            Assert.IsFalse(wm.Exists);
            w.Update(vw);
            Assert.IsTrue(wm.Exists);
            Assert.IsInstanceOfType(w.GetById(vw.Id), typeof(Whale));
            Assert.AreEqual(wm.Name, "Whale");
            Assert.AreEqual(w.CollectionName, "Whale");
        }

        [TestMethod]
        public void CustomIDTest()
        {
            var x = new MongoRepository<CustomIDEntity>();
            var xm = new MongoRepositoryManager<CustomIDEntity>();

            x.Add(new CustomIDEntity() { Id = "aaa" });

            Assert.IsTrue(xm.Exists);
            Assert.IsInstanceOfType(x.GetById("aaa"), typeof(CustomIDEntity));

            Assert.AreEqual("aaa", x.GetById("aaa").Id);

            x.Delete("aaa");
            Assert.AreEqual(0, x.Count());

            var y = new MongoRepository<CustomIDEntityCustomCollection>();
            var ym = new MongoRepositoryManager<CustomIDEntityCustomCollection>();

            y.Add(new CustomIDEntityCustomCollection() { Id = "xyz" });

            Assert.IsTrue(ym.Exists);
            Assert.AreEqual(ym.Name, "MyTestCollection");
            Assert.AreEqual(y.CollectionName, "MyTestCollection");
            Assert.IsInstanceOfType(y.GetById("xyz"), typeof(CustomIDEntityCustomCollection));

            y.Delete("xyz");
            Assert.AreEqual(0, y.Count());
        }

        [TestMethod]
        public void CustomIDTypeTest()
        {
            var xint = new MongoRepository<IntCustomer, int>();
            xint.Add(new IntCustomer() { Id = 1, Name = "Test A" });
            xint.Add(new IntCustomer() { Id = 2, Name = "Test B" });

            var yint = xint.GetById(2);
            Assert.AreEqual(yint.Name, "Test B");

            xint.Delete(2);
            Assert.AreEqual(1, xint.Count());
        }

        [TestMethod]
        public void OverrideCollectionName()
        {
            IRepository<Customer> _customerRepo = new MongoRepository<Customer>("mongodb://localhost/MongoRepositoryTests", "TestCustomers123");
            _customerRepo.Add(new Customer() { FirstName = "Test" });
            Assert.IsTrue(_customerRepo.Single().FirstName.Equals("Test"));
            Assert.AreEqual("TestCustomers123", _customerRepo.Collection.Name);
            Assert.AreEqual("TestCustomers123", ((MongoRepository<Customer>)_customerRepo).CollectionName);

            IRepositoryManager<Customer> _curstomerRepoManager = new MongoRepositoryManager<Customer>("mongodb://localhost/MongoRepositoryTests", "TestCustomers123");
            Assert.AreEqual("TestCustomers123", _curstomerRepoManager.Name);
        }

        #region Reproduce issue: https://mongorepository.codeplex.com/discussions/433878
        public abstract class BaseItem : IEntity
        {
            public string Id { get; set; }
        }

        public abstract class BaseA : BaseItem
        { }

        public class SpecialA : BaseA
        { }

        [TestMethod]
        public void Discussion433878()
        {
            var specialRepository = new MongoRepository<SpecialA>();
        }
        #endregion

        #region Reproduce issue: https://mongorepository.codeplex.com/discussions/572382
        public abstract class ClassA : Entity
        {
            public string Prop1 { get; set; }
        }

        public class ClassB : ClassA
        {
            public string Prop2 { get; set; }
        }

        public class ClassC : ClassA
        {
            public string Prop3 { get; set; }
        }

        [TestMethod]
        public void Discussion572382()
        {
            var repo = new MongoRepository<ClassA>() { 
                new ClassB() { Prop1 = "A", Prop2 = "B" } ,
                new ClassC() { Prop1 = "A", Prop3 = "C" }
            };

            Assert.AreEqual(2, repo.Count());

            Assert.AreEqual(2, repo.OfType<ClassA>().Count());
            Assert.AreEqual(1, repo.OfType<ClassB>().Count());
            Assert.AreEqual(1, repo.OfType<ClassC>().Count());
        }
        #endregion

    }
}
