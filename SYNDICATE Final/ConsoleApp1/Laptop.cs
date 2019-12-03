using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json;
using WebApplication1;
using System.IO;


namespace ConsoleApp1
{
    public class LaptopRead
    {
        int price = 0;
        int qty = 0;
        int localPrice = 0;
        public void lapDisplay(dynamic lapVariants)                                                 //fn to display laptop variants
        {
            Console.WriteLine();
            Console.WriteLine("---------> Welcome to CRAZY MACHINES - The Laptop Specialist <----------");
            Console.WriteLine();
            Console.WriteLine("-----------------------Available Laptop Variants------------------------");
            dynamic availableLapVariants = JsonConvert.DeserializeObject(lapVariants);
            foreach (var i in availableLapVariants)
            {
                String id = i.Id;
                String brandname = i.Brand;
                String price_detail = i.Price;
                String model_detail = i.Model;
                String Stock = i.Stock;
                Console.WriteLine("Id: {0}", id);
                Console.WriteLine("Brand: {0}", brandname);
                Console.WriteLine("Model: {0}", model_detail);
                Console.WriteLine("Price: Rs. {0}", price_detail);
                Console.WriteLine("Stock:{0}", Stock);
                Console.WriteLine("-----------------------------------------------------------------------");
            }
            Console.WriteLine();
            Console.Write("Please Enter the Item Id you wish to buy -");

            lapSelection(availableLapVariants);
        }

        public void lapSelection(dynamic lapSelect)                                            //fn to display laptop variant selected by the user
        {
            var user_id = Console.ReadLine();
            int stock = 0;
            var flag = 0;
            String user_choice;
            Program pur = new Program();
            LaptopRead lap = new LaptopRead();
            foreach (var i in lapSelect)
            {
                if (user_id == i.Id.ToString())
                {
                    Console.WriteLine();
                    Console.WriteLine("---------------------------Your Selection-----------------------------");
                    Console.WriteLine();
                    String id = i.Id;
                    String brandname = i.Brand;
                    String price_detail = i.Price;
                    String model_detail = i.Model;
                    String Stock = i.Stock;
                    lap.price = Convert.ToInt32(price_detail);
                    stock = Convert.ToInt32(Stock);
                    Program.brandCart.Add(brandname);
                    Program.priceCart.Add(price_detail);
                    Program.modelCart.Add(model_detail);
                    Console.WriteLine("Id: {0}", id);
                    Console.WriteLine("Brand: {0}", brandname);
                    Console.WriteLine("Model: {0}", model_detail);
                    Console.WriteLine("Price: Rs. {0}", price_detail);
                    Console.WriteLine("----------------------------------------------------------------------");
                    flag = 1;
                }
            }
            if (flag == 1)
            {
                calculation(lapSelect);
            }
            else
            {
                Console.WriteLine("Please Enter Correct Item Id:");
                lapSelection(lapSelect);
            }
      
            void calculation(dynamic calc)
            {
                do
                {
                    Console.WriteLine();
                    Console.Write("Enter the Quantity Required:");
                    lap.qty = Convert.ToInt32(Console.ReadLine());
                    lap.localPrice = lap.qty * lap.price;
                    Console.WriteLine("Total Price: Rs. {0}", lap.localPrice);

                    Console.WriteLine();
                    Console.WriteLine("Do you want to Change quantity? (y/n)");
                    user_choice = Console.ReadLine();
                    Console.WriteLine();

                    while ((user_choice != "y") && (user_choice != "n"))
                    {
                        Console.WriteLine();
                        Console.WriteLine("Invalid Option. Please Choose 'y' or 'n'");
                        user_choice = Console.ReadLine();
                    }
                    if (user_choice == "n")
                    {
                        stock = stock - lap.qty;
           
                        foreach (var i in calc)
                        {
                            if (user_id == i.Id.ToString())
                            {
                                i.Stock = stock;
                            }
                        }
                        string output = JsonConvert.SerializeObject(calc, Formatting.Indented);
                        File.WriteAllText(@"C:\Users\dhinesh.ks\Desktop\WebService\Laptop.json", output);
                        selection();
                    }
                } while (user_choice == "y");
            }

            void selection()
            {
                Program.quantity.Add(lap.qty);
                Program.totalPrice.Add(lap.localPrice);
                Program.purchasePrice += lap.localPrice;
                Program.laptopShopPrice += lap.localPrice;
                Console.WriteLine("Do you want to purchase another item...(Y/N)");
                string choice = Console.ReadLine();
                if (choice == "y" || choice == "Y")
                {
                    pur.purchase();
                }
                else if (choice == "N" || choice == "n")
                {
                    pur.display();
                }
                else
                {
                    Console.WriteLine("Please Enter Y or N...");
                    selection();
                }
            }
        }
        public void updateDetail()
        {
            try
            {
                string output;
                var flag = 0;
                var choiceupd = 0;
                string json = File.ReadAllText(@"C:\Users\dhinesh.ks\Desktop\WebService\Laptop.json");
                dynamic jObject = JsonConvert.DeserializeObject(json);
                Console.WriteLine(json);
                Console.WriteLine("1.Press 1 to Update - PRICE");
                Console.WriteLine("2.Press 2 to Update - STOCK");
                choiceupd = Convert.ToInt32(Console.ReadLine());
                if (choiceupd == 1)
                {
                    Console.Write("Enter the Item ID for which price to be Updated - ");
                    string Id = Console.ReadLine();

                    foreach (var i in jObject)
                    {
                        if (Id == i.Id.ToString())
                        {
                            Console.Write("Enter new price : ");
                            dynamic price = Convert.ToInt32(Console.ReadLine());
                            i.Price = price;
                            flag = 1;
                        }
                    }
                    if (flag == 1)
                    {
                        output = JsonConvert.SerializeObject(jObject, Formatting.Indented);
                        File.WriteAllText(@"C:\Users\dhinesh.ks\Desktop\WebService\Laptop.json", output);
                        Console.WriteLine("Price Updated");
                        Console.ReadLine();
                    }
                    else
                    {
                        Console.WriteLine("InValid Id");
                        Console.ReadLine();
                        updateDetail();
                    }
                }
                else if (choiceupd == 2)
                {
                    Console.Write("Enter the Item ID for which stock to be Updated - ");
                    string Id = Console.ReadLine();
                    foreach (var i in jObject)
                    {
                        if (Id == i.Id.ToString())
                        {
                            Console.WriteLine("Enter the new Stock");
                            dynamic stocknew = Convert.ToInt32(Console.ReadLine());
                            i.Stock = stocknew;
                            flag = 1;
                        }
                    }
                    if (flag == 1)
                    {
                        output = JsonConvert.SerializeObject(jObject, Formatting.Indented);
                        File.WriteAllText(@"C:\Users\dhinesh.ks\Desktop\WebService\Laptop.json", output);
                        Console.WriteLine("Stock Updated");
                        Console.ReadLine();
                    }
                    else
                    {
                        Console.WriteLine("InValid Id");
                        Console.ReadLine();
                        updateDetail();
                    }

                }
                else
                {
                    Console.WriteLine("Enter the valid choice from list !");
                    updateDetail();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Update Error : " + ex.Message.ToString());
            }
        }
        public void deleteDetail()
        {
            var json = File.ReadAllText(@"C:\Users\dhinesh.ks\Desktop\WebService\Laptop.json");
            Console.WriteLine(json);
            try
            {
                var flag = 0;
                dynamic jObject = JsonConvert.DeserializeObject(json);
                Console.Write("Enter Item ID to be deleted from the catalogue  : ");
                string Id = Console.ReadLine();

                foreach (var i in jObject)
                {
                    if (Id == i.Id.ToString())
                    {
                        i.Remove();
                        string output = JsonConvert.SerializeObject(jObject, Formatting.Indented);
                        File.WriteAllText(@"C:\Users\dhinesh.ks\Desktop\WebService\Laptop.json", output);
                        Console.WriteLine("Deletion Done");
                        flag = 1;
                    }
                }
                if (flag == 0)
                {
                    Console.WriteLine("Invalid ID");
                    deleteDetail();

                }
            }
            catch (Exception)
            {
                Console.WriteLine();

            }
        }
        public void addDetail()
        {
            Console.WriteLine("Instructions :");
            Console.WriteLine("'Brand','Price' - Add the Details within single Quotes('_')");
            Console.WriteLine();
            Console.WriteLine("Enter the new laptop details you wish add to the existing Catalogue");
            Console.Write("ID :");
            var Idadd = (Console.ReadLine());
            Console.Write("Brand :");
            string Brandadd = (Console.ReadLine());
            Console.Write("Model :");
            string Modeladd = (Console.ReadLine());
            Console.Write("Price :");
            var PriceAdd = Convert.ToInt32(Console.ReadLine());
            Console.Write("Stock :");
            var StockAdd = Convert.ToInt32(Console.ReadLine());
            var newdevice = "{'Id':" + Idadd.ToString() + ",'Brand':" + Brandadd.ToString() + ",'Model':"
                            + Modeladd.ToString() + ",'Price':" + PriceAdd +",'Stock':" + StockAdd + "}";

            try
            {
                var json = File.ReadAllText(@"C:\Users\dhinesh.ks\Desktop\WebService\Laptop.json");
                dynamic jObject = JsonConvert.DeserializeObject(json);
                dynamic newitem = JsonConvert.DeserializeObject(newdevice);
                jObject.Add((newitem));
                dynamic newJsonResult = JsonConvert.SerializeObject(jObject, Formatting.Indented);
                File.WriteAllText(@"C:\Users\dhinesh.ks\Desktop\WebService\Laptop.json", newJsonResult);
                Console.WriteLine("Item Successfully Added...");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Add Error : " + ex.Message.ToString());
            }
        }
    }
}