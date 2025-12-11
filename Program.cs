// Holly, Anthony, Moises, Matthew
// Date: 12/10/2025
// C#2: (CIT-111) Final Proj.
// Professor Crouch

using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using TeamHAMM.Models;

class Program
{
    static void Main(string[] args)
    {
        // connect to DB
        using var db = new TeamHammDbContext();

        bool running = true;

        while (running)
        {
            // gui menu list (full CRUD capabilities)
            Console.WriteLine("\n*** Welcome to Team HAMM's Database ***");
            Console.WriteLine("1. View Customers");
            Console.WriteLine("2. View Products");
            Console.WriteLine("3. View Orders");
            Console.WriteLine("4. View Everything");
            Console.WriteLine("5. Add Customer");
            Console.WriteLine("6. Add Order");
            Console.WriteLine("7. Update Customer");
            Console.WriteLine("8. Update Order");
            Console.WriteLine("9. Delete Customer");
            Console.WriteLine("0. Exit");
            Console.Write("Choose option: ");

            string choice = Console.ReadLine();

            // Main menu: user choices (CRUD)
            switch (choice)
            {
                case "1": ViewCustomers(db); break;
                case "2": ViewProducts(db); break;
                case "3": ViewOrders(db); break;
                case "4": ViewAll(db); break;
                case "5": AddCustomer(db); break;
                case "6": AddOrder(db); break;
                case "7": UpdateCustomer(db); break;
                case "8": UpdateOrder(db); break;
                case "9": DeleteCustomer(db); break;
                case "0": running = false; break;
                default: Console.WriteLine("Invalid option."); break;
            }
        }
    }

    // helpers (replaces repeated manual loops)
    static Customers FindCustomer(TeamHammDbContext db, int id) =>
        db.Customers.FirstOrDefault(c => c.CustomerId == id);

    static Products FindProduct(TeamHammDbContext db, int id) =>
        db.Products.FirstOrDefault(p => p.ProductId == id);

    static Orders FindOrder(TeamHammDbContext db, int id) =>
        db.Orders.FirstOrDefault(o => o.OrderId == id);


    // display ALL customers
    static void ViewCustomers(TeamHammDbContext db)
    {
        Console.WriteLine("\nCustomers:");
        var list = db.Customers.ToList(); // pulls ALL Customers

        if (!list.Any())
        {
            Console.WriteLine("No customers found.");
            return;
        }

        foreach (var c in list)
            Console.WriteLine($"{c.CustomerId}: {c.FirstName} {c.LastName} @{c.Email}");
    }

    // display ALL products
    static void ViewProducts(TeamHammDbContext db)
    {
        Console.WriteLine("\nProducts:");
        var list = db.Products.ToList(); // pulls ALL Products

        if (!list.Any())
        {
            Console.WriteLine("No products found.");
            return;
        }

        foreach (var p in list)
            Console.WriteLine($"{p.ProductId}: {p.ProductName} - ${p.UnitPrice}");
    }

    // display ALL orders -> [shows customer & product]
    static void ViewOrders(TeamHammDbContext db)
    {
        Console.WriteLine("\nOrders:");
        var orders = db.Orders.ToList(); // pulls ALL Orders
        var customers = db.Customers.ToList();
        var products = db.Products.ToList();

        if (!orders.Any())
        {
            Console.WriteLine("No orders found.");
            return;
        }

        foreach (var o in orders)
        {
            var customer = customers.FirstOrDefault(c => c.CustomerId == o.CustomerId);
            var product = products.FirstOrDefault(p => p.ProductId == o.ProductId);

            string name = customer?.LastName ?? "(no customer)";
            string item = product?.ProductName ?? "(no product)";

            Console.WriteLine($"{o.OrderId}: {name} ordered {item} x{o.Quantity} @ {o.UnitPrice} on {o.OrderDate:d}");
        }
    }

    // display ALL tables at once
    static void ViewAll(TeamHammDbContext db)
    {
        Console.WriteLine("\nCUSTOMER LIST");
        foreach (var c in db.Customers)
            Console.WriteLine($"{c.CustomerId}: {c.FirstName} {c.LastName}");

        Console.WriteLine("\nPRODUCT LIST");
        foreach (var p in db.Products)
            Console.WriteLine($"{p.ProductId}: {p.ProductName} - ${p.UnitPrice}");

        Console.WriteLine("\nORDER LIST");
        foreach (var o in db.Orders)
            Console.WriteLine($"{o.OrderId}: Customer {o.CustomerId} ordered Product {o.ProductId} x{o.Quantity}");
    }

    // add new customer
    static void AddCustomer(TeamHammDbContext db)
    {
        Console.Write("Enter first name: ");
        string first = Console.ReadLine();

        Console.Write("Enter last name: ");
        string last = Console.ReadLine();

        Console.Write("Enter email: ");
        string email = Console.ReadLine();

        var c = new Customers
        {
            FirstName = first,
            LastName = last,
            Email = email
        };

        db.Customers.Add(c);      // final push to db
        db.SaveChanges();

        Console.WriteLine("Customer successfully added.");
    }

    // add new order
    static void AddOrder(TeamHammDbContext db)
    {
        Console.Write("Enter Customer ID: ");
        Console.Write("Enter Customer ID: ");
        string cInput = Console.ReadLine();

        Console.Write("Enter Product ID: ");
        string pInput = Console.ReadLine();

        Console.Write("Enter Quantity: ");
        string qInput = Console.ReadLine();

        // convert user input: strings -> ints
        if (!int.TryParse(cInput, out int custId) ||
            !int.TryParse(pInput, out int prodId) ||
            !int.TryParse(qInput, out int qty))
        {
            Console.WriteLine("Invalid input. Please enter a number.");
            return;
        }

        var cust = FindCustomer(db, custId);
        var prod = FindProduct(db, prodId);

        if (cust == null || prod == null)
        {
            Console.WriteLine("Customer / Product not found.");
            return;
        }

        var order = new Orders
        {
            CustomerId = custId,
            ProductId = prodId,
            Quantity = qty,
            UnitPrice = prod.UnitPrice,
            OrderDate = DateTime.Now
        };

        db.Orders.Add(order); // final push to db
        db.SaveChanges();

        Console.WriteLine("Order successfully created.");
    }

    // update customer
    static void UpdateCustomer(TeamHammDbContext db)
    {
        Console.Write("Enter Customer ID to update: ");
        string input = Console.ReadLine();

        if (!int.TryParse(input, out int id))
        {
            Console.WriteLine("Invalid number.");
            return;
        }

        var cust = FindCustomer(db, id);

        if (cust == null)
        {
            Console.WriteLine("Customer not found.");
            return;
        }

        Console.Write("New First Name (leave blank to keep): ");
        string first = Console.ReadLine();

        Console.Write("New Last Name (leave blank to keep): ");
        string last = Console.ReadLine();

        Console.Write("New Email (leave blank to keep): ");
        string email = Console.ReadLine();

        if (!string.IsNullOrWhiteSpace(first)) cust.FirstName = first;
        if (!string.IsNullOrWhiteSpace(last)) cust.LastName = last;
        if (!string.IsNullOrWhiteSpace(email)) cust.Email = email;

        db.SaveChanges();
        Console.WriteLine("Customer updated successfully.");
    }

    // update order
    static void UpdateOrder(TeamHammDbContext db)
    {
        Console.Write("Enter Order ID to update: ");
        string input = Console.ReadLine();

        if (!int.TryParse(input, out int id))
        {
            Console.WriteLine("Invalid Order ID.");
            return;
        }

        var order = FindOrder(db, id);

        if (order == null)
        {
            Console.WriteLine("Order not found.");
            return;
        }

        Console.Write("New Quantity (leave blank to keep): ");
        string q = Console.ReadLine();

        if (!string.IsNullOrWhiteSpace(q) && int.TryParse(q, out int newQty))
            order.Quantity = newQty;

        db.SaveChanges();
        Console.WriteLine("Order updated successfully.");
    }

    // delete customer
    static void DeleteCustomer(TeamHammDbContext db)
    {
        Console.Write("Please enter Customer ID: ");
        string input = Console.ReadLine();

        if (!int.TryParse(input, out int id))
        {
            Console.WriteLine("Invalid number.");
            return;
        }

        var cust = FindCustomer(db, id);

        if (cust == null)
        {
            Console.WriteLine("Customer not found.");
            return;
        }

        // check if customer has order(s)
        bool hasOrders = db.Orders.Any(o => o.CustomerId == id);

        if (hasOrders)
        {
            Console.WriteLine("Can't delete!\nReason: This customer has existing orders.");
            return;
        }

        // final push to db
        db.Customers.Remove(cust); 
        db.SaveChanges();

        Console.WriteLine("Customer successfully deleted.");
    }
}