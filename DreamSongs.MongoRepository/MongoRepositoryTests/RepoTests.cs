using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DreamSongs.MongoRepository;
using MongoRepositoryTests.Entities;
using FluentMongo;
using MongoDB.Driver;


namespace MongoRepositoryTests
{
    [TestClass]
    public class RepoTests
    {
        IRepository<Customer> _customerRepo;
        
        [TestInitialize]   
        public void Setup()
        {
            // setup the db, tables and Repository 
            _customerRepo = new MongoRepository<Customer>();            
        }

        [TestCleanup]
        public void Cleanup()
        {
            _customerRepo.DB.Drop();
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
            var updatedCustomer = _customerRepo.GetById(customer.Id.ToString());

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

            var item1 =  new OrderItem{ Product = new Product(){ Name = "Palmolive Shampoo", Price = 5 }, Quantity = 1};
            var item2 = new OrderItem { Product = new Product() { Name = "Mcleans Paste", Price = 4 }, Quantity = 2 };
            
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
            var theOrders = _customerRepo.GetAll(c => c.Id == customer.Id).Select(c => c.Orders).ToList();
            var theOrderItems = theOrders[0].Select(o => o.Items);

            Assert.IsNotNull(theOrders);
            Assert.IsNotNull(theOrderItems);            
        }
    }
}
