using hazinDNS_v2.Models;

namespace hazinDNS_v2.Data
{
    public static class DbInitializer
    {
        public static void ResetAdminBalance(ApplicationDbContext context)
        {
            var admin = context.Users.FirstOrDefault(u => u.Username == "admin");
            if (admin != null)
            {
                admin.Balance = 0;
                context.SaveChanges();
            }
        }

        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();
            
            if (!context.Users.Any())
            {
                var users = new User[]
                {
                    new User
                    {
                        Username = "admin",
                        Password = "python",
                        Email = "admin@hazindns.com",
                        Role = "Admin"
                    },
                    new User
                    {
                        Username = "user",
                        Password = "password",
                        Email = "user@hazindns.com",
                        Role = "User"
                    }
                };
                context.Users.AddRange(users);
                context.SaveChanges();
            }

            if (!context.Products.Any())
            {
                var products = new Product[]
                {
                    new Product 
                    { 
                        Name = "Холодильник Samsung RF50",
                        Price = 49999.99M,
                        Description = "Двухкамерный холодильник с системой NoFrost, объем 500л",
                        ImageUrl = "/images/samsung-fridge.jpg",
                        Category = "Холодильники",
                        InStock = true
                    },
                    new Product 
                    { 
                        Name = "Стиральная машина LG F2J3NS0W",
                        Price = 35999.99M,
                        Description = "Стиральная машина с функцией пара, загрузка 6.5 кг",
                        ImageUrl = "/images/lg-washer.jpg",
                        Category = "Стиральные машины",
                        InStock = true
                    },
                    new Product 
                    { 
                        Name = "Телевизор Sony XR-65A80L",
                        Price = 199999.99M,
                        Description = "OLED телевизор 65 дюймов, 4K, Smart TV",
                        ImageUrl = "/images/sony-tv.jpg",
                        Category = "Телевизоры",
                        InStock = true
                    },
                    new Product 
                    { 
                        Name = "Микроволновая печь Panasonic NN-GT261W",
                        Price = 8999.99M,
                        Description = "Микроволновая печь с грилем, 20л",
                        ImageUrl = "/images/panasonic-microwave.jpg",
                        Category = "Микроволновые печи",
                        InStock = true
                    },
                    new Product 
                    { 
                        Name = "Пылесос Dyson V15",
                        Price = 49999.99M,
                        Description = "Беспроводной пылесос с лазером для обнаружения пыли",
                        ImageUrl = "/images/dyson-vacuum.jpg",
                        Category = "Пылесосы",
                        InStock = true
                    },
                    new Product 
                    { 
                        Name = "Кофемашина DeLonghi ECAM 370.95.T",
                        Price = 89999.99M,
                        Description = "Автоматическая кофемашина с капучинатором",
                        ImageUrl = "/images/delonghi-coffee.jpg",
                        Category = "Кофемашины",
                        InStock = true
                    },
                    new Product 
                    { 
                        Name = "Посудомоечная машина Bosch SMS44DI01T",
                        Price = 42999.99M,
                        Description = "Отдельностоящая посудомоечная машина, 12 комплектов",
                        ImageUrl = "/images/bosch-dishwasher.jpg",
                        Category = "Посудомоечные машины",
                        InStock = true
                    },
                    new Product 
                    { 
                        Name = "Духовой шкаф Electrolux EOE7P31X",
                        Price = 39999.99M,
                        Description = "Электрический духовой шкаф с паром",
                        ImageUrl = "/images/electrolux-oven.jpg",
                        Category = "Духовые шкафы",
                        InStock = true
                    },
                    new Product 
                    { 
                        Name = "MSI Raider GE78 HX",
                        Description = "Игровой ноутбук с RTX 4090, Intel Core i9-13980HX",
                        Price = 399999M,
                        ImageUrl = "/images/msi-raider-ge78.png",
                        Category = "Ноутбуки",
                        InStock = true
                    },
                    new Product 
                    { 
                        Name = "ASUS ROG Strix G16",
                        Description = "Игровой ноутбук с RTX 4080, Intel Core i9-13980H",
                        Price = 289999M,
                        ImageUrl = "/images/asus-rog-strix-g16.png",
                        Category = "Ноутбуки",
                        InStock = true
                    },
                    new Product 
                    { 
                        Name = "Alienware Aurora R15",
                        Description = "Игровой компьютер с RTX 4090, Intel Core i9-13900K",
                        Price = 459999M,
                        ImageUrl = "/images/alienware-aurora-r15.png",
                        Category = "Компьютеры",
                        InStock = true
                    },
                    new Product 
                    { 
                        Name = "Bosch MFW3X10B",
                        Description = "Мясорубка с производительностью 2 кг/мин",
                        Price = 7999M,
                        ImageUrl = "/images/bosch-grinder.png",
                        Category = "Бытовая техника",
                        InStock = true
                    },
                    new Product 
                    { 
                        Name = "MSI MEG Aegis Ti5",
                        Description = "Игровой компьютер с RTX 4080, Intel Core i7-13700K",
                        Price = 349999M,
                        ImageUrl = "/images/msi-meg-aegis-ti5.png",
                        Category = "Компьютеры",
                        InStock = true
                    },
                    new Product 
                    { 
                        Name = "ASUS ROG Swift PG32UQX",
                        Description = "4K Gaming Monitor, 32 дюйма, 144Hz, Mini LED",
                        Price = 199999M,
                        ImageUrl = "/images/asus-rog-swift-pg32uqx.png",
                        Category = "Мониторы",
                        InStock = true
                    },
                    new Product 
                    { 
                        Name = "Samsung Odyssey Neo G9",
                        Description = "49 дюймов, 5120x1440, 240Hz, Mini LED",
                        Price = 249999M,
                        ImageUrl = "/images/samsung-odyssey-neo-g9.png",
                        Category = "Мониторы",
                        InStock = true
                    },
                    new Product 
                    { 
                        Name = "JBL PartyBox 710",
                        Description = "Мощная портативная колонка с световыми эффектами",
                        Price = 69999M,
                        ImageUrl = "/images/jbl-partybox-710.png",
                        Category = "Аудиотехника",
                        InStock = true
                    },
                    new Product 
                    { 
                        Name = "JBL Charge 5",
                        Description = "Портативная водонепроницаемая колонка",
                        Price = 12999M,
                        ImageUrl = "/images/jbl-charge-5.png",
                        Category = "Аудиотехника",
                        InStock = true
                    },
                    new Product 
                    { 
                        Name = "Samsung Galaxy Z Flip 5",
                        Description = "Складной смартфон с 6.7\" Dynamic AMOLED 2X дисплеем, 8GB RAM, 256GB",
                        Price = 89999M,
                        ImageUrl = "/images/samsung-zflip5.png",
                        Category = "Смартфоны",
                        InStock = true
                    },
                    new Product 
                    { 
                        Name = "Polaris PWK 1725CGLD",
                        Description = "Электрический чайник с LED подсветкой, 1.7л",
                        Price = 2999M,
                        ImageUrl = "/images/polaris-kettle.png",
                        Category = "Бытовая техника",
                        InStock = true
                    },
                    new Product 
                    { 
                        Name = "Tefal OptiGrill+ GC712",
                        Description = "Умный гриль с автоматическими программами",
                        Price = 12999M,
                        ImageUrl = "/images/tefal-optigrill.png",
                        Category = "Бытовая техника",
                        InStock = true
                    },
                    new Product 
                    { 
                        Name = "Redmond RMC-M92",
                        Description = "Мультиварка с 45 программами приготовления",
                        Price = 5999M,
                        ImageUrl = "/images/redmond-multicooker.png",
                        Category = "Бытовая техника",
                        InStock = true
                    },
                    new Product 
                    { 
                        Name = "Philips EP2231/40",
                        Description = "Автоматическая кофемашина с капучинатором",
                        Price = 39999M,
                        ImageUrl = "/images/philips-coffee.png",
                        Category = "Бытовая техника",
                        InStock = true
                    }
                };

                context.Products.AddRange(products);
                context.SaveChanges();
            }
        }

        public static void UpdateProducts(ApplicationDbContext context)
        {
            // Очищаем существующие продукты
            context.Products.RemoveRange(context.Products);
            context.SaveChanges();
            
            // Вызываем Initialize для добавления всех продуктов заново
            Initialize(context);
        }
    }
} 