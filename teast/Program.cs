using System;
using System.Collections.Generic;

namespace PromotionEngineCodingExercise
{
    public class Product //entity for a product
    {
        public char Id { get; set; }
        public decimal Price { get; set; }
    }

    public abstract class Promotion //using abstract interface class to force all types of promotions to adhere to the correct framework i.e their version of applying the promotion. leaves room for new promotions such as x% off  and BOGOF
    {
        public abstract decimal ApplyPromotion(Dictionary<char, int> cart, List<Product> products);
    }

    public class QuantityDiscountPromotion : Promotion // discounts on quantity of the same product such as buy 3 pay 130
    {
        private readonly char productId;
        private readonly int quantity;
        private readonly decimal price;

        public QuantityDiscountPromotion(char productId, int quantity, decimal price)
        {
            this.productId = productId;
            this.quantity = quantity;
            this.price = price;
        }

        public override decimal ApplyPromotion(Dictionary<char, int> cart, List<Product> products)
        {
            if (cart.ContainsKey(productId))
            {
                int count = cart[productId];
                int sets = count / quantity;
                int remainder = count % quantity;
                decimal appliedPromotion = (sets * price) + (remainder * products.Find(p => p.Id == productId).Price);
                if (appliedPromotion < price)
                {
                    return 0;
                }
                else
                {
                    return appliedPromotion;
                }

            }
            return 0;
        }
    }

    public class ComboDiscountPromotion : Promotion // discounts on buying 2 different products such as buy x and y and pay 30
    {
        private readonly char productId1;
        private readonly char productId2;
        private readonly decimal price;

        public ComboDiscountPromotion(char productId1, char productId2, decimal price)
        {
            this.productId1 = productId1;
            this.productId2 = productId2;
            this.price = price;
        }

        public override decimal ApplyPromotion(Dictionary<char, int> cart, List<Product> products)
        {
            if (cart.ContainsKey(productId1) && cart.ContainsKey(productId2))
            {
                int count1 = cart[productId1];
                int count2 = cart[productId2];
                int minCount = Math.Min(count1, count2);
                return minCount * price;
            }
            return 0;
        }
    }

    public class Checkout //instance of a specific checkout. simplified. would have had this sliced up into multiple layers if given a larger time frame
    {
        private Dictionary<char, int> cart = new Dictionary<char, int>();
        private List<Promotion> promotions = new List<Promotion>();

        public void Scan(char productId, int quantity = 1) //scan the item into the cart like you're actually at a till!
        {
            if (cart.ContainsKey(productId))
            {
                cart[productId] += quantity;
            }
            else
            {
                cart[productId] = quantity;
            }
        }

        public void AddPromotion(Promotion promotion) //adds a specific promotion to this specific instance checkout
        {
            promotions.Add(promotion);
        }

        public decimal CalculateTotal(List<Product> products) //calculates the total of the cart. ran into an issue where it would calculate twice after promotions or not calculate if no promotions were involved. i needed to add the products without promotions onto the total at the end
        {
            decimal total = 0;

            foreach (var promotion in promotions)
            {
                total += promotion.ApplyPromotion(cart, products);
            }

            //add the items which were not involved in the promotions totaling
            //foreach (var item in cart)
            //{

            //    total += item.Value * products.Find(p => p.Id == item.Key).Price;
            //}

            return total;
        }
    }

    public class Program
    {
        public static List<Product> products = new List<Product>() //uses char for clarity in context, a char as simple as this wouldnt be used as a unique identifier
            {
                new Product { Id = 'A', Price = 50 },
                new Product { Id = 'B', Price = 30 },
                new Product { Id = 'C', Price = 20 },
                new Product { Id = 'D', Price = 15 }
            };

        public static void Main(string[] args) //test cases, uses cmd, no gui. 
        {
            Checkout checkout = new Checkout();
            checkout.AddPromotion(new QuantityDiscountPromotion('A', 3, 130));
            checkout.AddPromotion(new QuantityDiscountPromotion('B', 2, 45));
            checkout.AddPromotion(new ComboDiscountPromotion('C', 'D', 30));
            checkout.Scan('A');
            checkout.Scan('B');
            checkout.Scan('C');
            Console.WriteLine("Scenario A Total: " + checkout.CalculateTotal(products)); // Expected: 100

            checkout = new Checkout();
            checkout.AddPromotion(new QuantityDiscountPromotion('A', 3, 130));
            checkout.AddPromotion(new QuantityDiscountPromotion('B', 2, 45));
            checkout.AddPromotion(new ComboDiscountPromotion('C', 'D', 30));
            checkout.Scan('A', 5);
            checkout.Scan('B', 5);
            checkout.Scan('C');
            Console.WriteLine("Scenario B Total: " + checkout.CalculateTotal(products)); // Expected: 370

            checkout = new Checkout();
            checkout.AddPromotion(new QuantityDiscountPromotion('A', 3, 130));
            checkout.AddPromotion(new QuantityDiscountPromotion('B', 2, 45));
            checkout.AddPromotion(new ComboDiscountPromotion('C', 'D', 30));
            checkout.Scan('A', 3);
            checkout.Scan('B', 5);
            checkout.Scan('D');
            Console.WriteLine("Scenario C Total: " + checkout.CalculateTotal(products)); // Expected: 280
            Console.ReadLine();
        }
    }
}




