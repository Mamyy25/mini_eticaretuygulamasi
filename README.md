\# 🛒 E-Commerce Web Application



A full-featured e-commerce platform built with \*\*ASP.NET Core MVC\*\* and \*\*Entity Framework Core\*\*.



\[🇹🇷 Türkçe README için tıklayın](README.tr.md)



---



\## 🚀 Features



\### 👥 User Management

\- User registration and authentication

\- Profile management

\- Admin and seller role support

\- Session-based authentication



\### 🛍️ Product Management

\- Product CRUD operations

\- Category management

\- Image upload support

\- Stock tracking

\- Active/inactive product status

\- Soft delete functionality



\### 🛒 Shopping Cart

\- Add/remove products

\- Quantity management

\- Real-time cart total calculation

\- Persistent cart storage



\### 📦 Order Management

\- Order creation and tracking

\- Order status management

\- Shipping information

\- Order history



\### 🔧 Admin Panel

\- Product management dashboard

\- Category management

\- Statistics and analytics

\- Deleted items recovery

\- Low stock alerts



\### 📚 API Documentation

\- RESTful API with Swagger/OpenAPI

\- Interactive API testing interface

\- Complete endpoint documentation



---



\## 🛠️ Technologies Used



\### Backend

\- \*\*Framework\*\*: ASP.NET Core 8.0 MVC

\- \*\*ORM\*\*: Entity Framework Core 8.0

\- \*\*Database\*\*: SQL Server

\- \*\*API Documentation\*\*: Swagger/Swashbuckle



\### Frontend

\- \*\*CSS Framework\*\*: Tailwind CSS

\- \*\*Icons\*\*: Font Awesome

\- \*\*Fonts\*\*: Google Fonts (Inter)



\### Architecture

\- \*\*Pattern\*\*: MVC (Model-View-Controller)

\- \*\*Data Layer\*\*: Repository Pattern

\- \*\*Business Logic\*\*: Service Layer

\- \*\*Authentication\*\*: Session-based



---



\## 📋 Prerequisites



\- \[.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

\- \[SQL Server](https://www.microsoft.com/sql-server/sql-server-downloads) (Express or LocalDB)

\- \[Visual Studio 2022](https://visualstudio.microsoft.com/) or \[VS Code](https://code.visualstudio.com/)

\- \[Git](https://git-scm.com/)



---



\## ⚙️ Installation



\### 1. Clone the Repository



```bash

git clone https://github.com/Mamyy25/mini\_eticaretuygulamasi.git

cd mini\_eticaretuygulamasi

```



\### 2. Configure Application Settings



Copy the example settings file and update with your configuration:



```bash

cd ECommerce.Web

copy appsettings.example.json appsettings.json

```



Then open `ECommerce.Web/appsettings.json` and update the connection string:



```json

{

&nbsp; "ConnectionStrings": {

&nbsp;   "DefaultConnection": "Server=YOUR\_SERVER;Database=ECommerceDB;Trusted\_Connection=True;TrustServerCertificate=True;"

&nbsp; }

}

```



\*\*⚠️ Important\*\*: Never commit `appsettings.json` with real connection strings to Git!



\### 3. Apply Database Migrations



```bash

cd ECommerce.Data

dotnet ef database update --startup-project ../ECommerce.Web

```



Or use Package Manager Console in Visual Studio:



```powershell

Update-Database

```



\### 4. Run the Application



```bash

cd ECommerce.Web

dotnet run

```



Or press \*\*F5\*\* in Visual Studio.



\### 5. Access the Application



\- \*\*Website\*\*: `http://localhost:5133`

\- \*\*Swagger API\*\*: `http://localhost:5133/swagger`



---



\## 📁 Project Structure



```

ECommerce/

├── ECommerce.Web/              # MVC Web Application

│   ├── Controllers/            # MVC Controllers

│   │   ├── API/               # API Controllers

│   │   ├── HomeController.cs

│   │   ├── ProductController.cs

│   │   └── ...

│   ├── Views/                 # Razor Views

│   ├── wwwroot/               # Static files

│   └── Program.cs             # Application entry point

│

├── ECommerce.Data/            # Data Access Layer

│   ├── ApplicationDbContext.cs

│   └── Migrations/

│

├── ECommerce.Models/          # Domain Models

│   ├── Product.cs

│   ├── Category.cs

│   ├── User.cs

│   └── ...

│

└── ECommerce.Business/        # Business Logic Layer

&nbsp;   └── Services/

```



---



\## 🔐 Default Admin Account



After running migrations, you can create an admin account manually in the database or through the registration page with admin privileges.



---



\## 🌐 API Endpoints



\### Products API



| Method | Endpoint | Description |

|--------|----------|-------------|

| GET | `/api/ProductsApi` | Get all products |

| GET | `/api/ProductsApi/{id}` | Get product by ID |

| GET | `/api/ProductsApi/category/{categoryId}` | Get products by category |

| GET | `/api/ProductsApi/search?searchTerm={term}` | Search products |

| POST | `/api/ProductsApi` | Create new product |

| PUT | `/api/ProductsApi/{id}` | Update product |

| DELETE | `/api/ProductsApi/{id}` | Delete product (soft delete) |

| GET | `/api/ProductsApi/{id}/stock` | Get stock information |



\### Categories API



| Method | Endpoint | Description |

|--------|----------|-------------|

| GET | `/api/CategoriesApi` | Get all categories |

| GET | `/api/CategoriesApi/{id}` | Get category by ID |

| POST | `/api/CategoriesApi` | Create new category |

| PUT | `/api/CategoriesApi/{id}` | Update category |

| DELETE | `/api/CategoriesApi/{id}` | Delete category (soft delete) |

| GET | `/api/CategoriesApi/{id}/product-count` | Get product count in category |



\*\*Full API documentation\*\*: `http://localhost:5133/swagger`



---



\## 🎨 Features Showcase



\### Admin Panel

\- Real-time statistics dashboard

\- Product and category management with dropdown navigation

\- Inventory tracking with stock alerts

\- Deleted items recovery

\- Bulk operations support



\### User Features

\- Responsive design with Tailwind CSS

\- Clean and modern minimalist UI

\- Shopping cart with real-time updates

\- Order tracking and history

\- Session-based cart persistence



\### API

\- RESTful design with proper HTTP methods

\- Interactive Swagger/OpenAPI documentation

\- JSON responses with circular reference handling

\- Comprehensive error handling

\- Bearer token authentication support (ready for implementation)



---



\## 🐛 Troubleshooting



\### Database Connection Issues



If you get connection errors:



1\. Check SQL Server is running

2\. Update connection string in `appsettings.json`

3\. Run migrations: `dotnet ef database update`



\### Port Already in Use



Change the port in `ECommerce.Web/Properties/launchSettings.json`



\### NuGet Package Errors



```bash

dotnet restore

dotnet clean

dotnet build

```



\### Circular Reference Errors in API



Already handled with `ReferenceHandler.IgnoreCycles` in Program.cs



---



\## 🤝 Contributing



1\. Fork the project

2\. Create your feature branch (`git checkout -b feature/AmazingFeature`)

3\. Commit your changes (`git commit -m 'Add some AmazingFeature'`)

4\. Push to the branch (`git push origin feature/AmazingFeature`)

5\. Open a Pull Request



---



\## 📝 License

**Copyright © 2025 Mamyy25. All rights reserved.**

This project is proprietary and confidential. Unauthorized copying, distribution, 
modification, or use of this software is strictly prohibited without explicit 
written permission from the owner.

For licensing inquiries, please contact: [mekkoseoglu@gmail.com]

See the [LICENSE](LICENSE) file for more details.



---



\## 👨‍💻 Author



\*\*Mamyy25\*\*

\- GitHub: \[@Mamyy25](https://github.com/Mamyy25)

\- Project Repository: \[mini\_eticaretuygulamasi](https://github.com/Mamyy25/mini\_eticaretuygulamasi)



---



\## 🙏 Acknowledgments



\- ASP.NET Core Team

\- Entity Framework Core Team

\- Tailwind CSS

\- Font Awesome

\- Swagger/OpenAPI

\- Google Fonts



---



\## 🔒 Security Notes



\- Connection strings are excluded from Git via `.gitignore`

\- Use `appsettings.example.json` as a template for configuration

\- Session-based authentication with configurable timeout

\- SQL injection protection through Entity Framework parameterization

\- XSS protection with Razor view encoding



---





