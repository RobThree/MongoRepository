using System;
using System.Collections.Generic;
using DreamSongs.MongoRepository;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoRepositoryTests.Entities
{
    /// <summary>
    /// Business Entity for Customer
    /// </summary>
   public class Customer : Entity
    {
       public Customer() : base("Customer")
       {           
       }

       [BsonElement("fname")]
       public string FirstName { get; set; }

       [BsonElement("lname")]
       public string LastName { get; set; }

       public string Email { get; set; }

       public string Phone { get; set; }

       public Address HomeAddress { get; set; }

       public IList<Order> Orders { get; set; }
    }

   public class Order
   {
       public DateTime PurchaseDate { get; set; }

       public IList<OrderItem> Items;
   }

   public class OrderItem
   {
       public Product Product
       {
           get;
           set;
       }

       public int Quantity { get; set; }
   }

    public class Address
    {
        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string City { get; set; }

        public string PostCode { get; set; }

        [BsonIgnoreIfNull]
        public string Country { get; set; }
    }
}
