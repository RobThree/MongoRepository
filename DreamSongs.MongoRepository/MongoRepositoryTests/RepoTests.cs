using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using DreamSongs.MongoRepository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using MongoRepositoryTests.Entities;

namespace MongoRepositoryTests
{
    //TODO: Tests here might need a little more logical grouping

    [TestClass]
    public class RepoTests
    {
        IRepository<Customer> _customerRepo;
        IRepository<Product> _productRepo;

        [TestInitialize]
        public void Setup()
        {
            // setup the db, tables and Repository 
            _customerRepo = new MongoRepository<Customer>();
            _productRepo = new MongoRepository<Product>();
        }

        [TestCleanup]
        public void Cleanup()
        {
            //new MongoRepositoryManager<Customer>().Drop();
            //new MongoRepositoryManager<Product>().Drop();

            var x = new MongoUrl(ConfigurationManager.ConnectionStrings["MongoServerSettings"].ConnectionString);
            var s = new MongoServer(x.ToServerSettings());
            var d = s.GetDatabase(x.DatabaseName);
            d.Drop();
        }

        [TestMethod]
        public void AddAndUpdateTest()
        {
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

            Assert.IsNotNull(customer.Id);

            // fetch it back 
            var alreadyAddedCustomer = _customerRepo.GetSingle(c => c.FirstName == "Bob");

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
            var theOrders = _customerRepo.All(c => c.Id == customer.Id).Select(c => c.Orders).ToList();
            var theOrderItems = theOrders[0].Select(o => o.Items);

            Assert.IsNotNull(theOrders);
            Assert.IsNotNull(theOrderItems);
        }


        [TestMethod]
        public void BatchTest()
        {
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

            foreach (Customer c in _customerRepo.All())
                Assert.AreEqual(c.FirstName, c.LastName);

            //Delete by criteria
            _customerRepo.Delete(f => f.FirstName.StartsWith("Client"));

            count = _customerRepo.Count();
            Assert.AreEqual(4, count);

            //Delete specific object
            _customerRepo.Delete(custlist[0]);

            //Test AsQueryable
            var selectedcustomers = from cust in _customerRepo.AsQueryable()
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

    }
}
