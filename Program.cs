using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ProductCategoryPairs
{
    /// <summary>
    /// не совсем умею работать с Python, поэтому прочел пару статей и сделал что-то похоже с С# и LINQ, был б ыне против научиться Python
    /// но знания на базовом уровне и особо не понимаю что и как в нем
    /// поэтому выполнил задание на C#
    /// </summary>
   


    // не стал разделять на классы по отдельным файлам 

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }


    public class ProductCategory
    {
        public int ProductId { get; set; }
        public int CategoryId { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Пример данных
            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Продукт A" },
                new Product { Id = 2, Name = "Продукт B" },
                new Product { Id = 3, Name = "Продукт C" },
                new Product { Id = 4, Name = "Продукт D" } // test no category
            };

            var categories = new List<Category>
            {
                new Category { Id = 100, Name = "Категория X" },
                new Category { Id = 200, Name = "Категория Y" }
            };

            var productCategories = new List<ProductCategory>
            {
                new ProductCategory { ProductId = 1, CategoryId = 100 }, 
                new ProductCategory { ProductId = 2, CategoryId = 100 }, 
                new ProductCategory { ProductId = 3, CategoryId = 200 }  
            };

            
            DataTable resultTable = GetProductCategoryPairs(products, categories, productCategories);

           
            Console.WriteLine("Результаты объединения:");
            foreach (DataRow row in resultTable.Rows)
            {
                
                string category = row["CategoryName"] != DBNull.Value ? row["CategoryName"].ToString() : "Нет категории";
                Console.WriteLine($"{row["ProductName"]} - {category}");
            }

            Console.ReadLine();
        }


        public static DataTable GetProductCategoryPairs(
            List<Product> products,
            List<Category> categories,
            List<ProductCategory> productCategories)
        {
        
            DataTable result = new DataTable();
            result.Columns.Add("ProductName", typeof(string));
            result.Columns.Add("CategoryName", typeof(string));

         
            var assignedPairs = from pc in productCategories
                                join p in products on pc.ProductId equals p.Id
                                join c in categories on pc.CategoryId equals c.Id
                                select new { p.Name, CategoryName = c.Name };

            foreach (var pair in assignedPairs)
            {
                DataRow row = result.NewRow();
                row["ProductName"] = pair.Name;
                row["CategoryName"] = pair.CategoryName;
                result.Rows.Add(row);
            }

            var productIdsWithCategories = productCategories.Select(pc => pc.ProductId).Distinct();
            var productsWithoutCategories = from p in products
                                            where !productIdsWithCategories.Contains(p.Id)
                                            select p;

            foreach (var product in productsWithoutCategories)
            {
                DataRow row = result.NewRow();
                row["ProductName"] = product.Name;
                row["CategoryName"] = DBNull.Value; //Nullable values
                result.Rows.Add(row);
            }

            return result;
        }
    }
}
