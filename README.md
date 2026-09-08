E-Commerce REST API

A backend REST API for an E-Commerce application built using **ASP.NET Core Web API, Entity Framework Core, and SQL Server**.

The project provides APIs for authentication, products, categories, cart management, checkout, coupons, orders, payments, and an admin dashboard.

---

Tech Stack

- **ASP.NET Core Web API**
- **.NET 10**
- **C#**
- **Entity Framework Core**
- **SQL Server**
- **JWT Authentication**
- **Swagger / OpenAPI**
- **Repository Pattern**
- **Service Layer Architecture**

---

Features

Authentication & Authorization

- User registration
- User login
- Password hashing
- JWT-based authentication
- Role-based authorization
- Admin and User roles

Product Management

- Get all products
- Search products
- Filter by category
- Sort by price
- Pagination
- Create product
- Update product
- Delete product

Shopping Cart

- Add product to cart
- Update quantity
- Remove cart item
- Clear cart
- Stock validation
- Automatically merge duplicate products in cart

Coupon Management

- Create coupons
- Update coupons
- Activate/deactivate coupons
- Percentage discount
- Fixed amount discount
- Minimum order amount
- Expiry date validation
- Discount cannot exceed order total

Checkout

- Checkout from cart
- Create order
- Create order items
- Calculate total amount
- Apply coupon
- Validate product stock
- Reduce product stock
- Clear cart after successful checkout

Order Management

- Get orders
- Get order by ID
- Create orders
- Update order status
- Delete orders

Order status flow:

```text
Pending
   ├──→ Confirmed → Shipped → Delivered
   │
   └──→ Cancelled

Payment Management
UPI payment
Card payment
Cash payment
Payment success/failure simulation
Transaction ID generation for successful payments
Prevent duplicate payment for the same order
Automatically confirm order after successful payment
Failed payment keeps the order in Pending state

Admin Dashboard

Admin can view:

Total products
Total customers
Total orders
Total revenue
Pending orders
Confirmed orders
Shipped orders
Delivered orders
Cancelled orders
Successful payments
Failed payments

Project Architecture

The project follows a layered architecture:

Client
   │
   ▼
Controllers
   │
   ▼
Services
   │
   ▼
Repositories
   │
   ▼
Entity Framework Core
   │
   ▼
SQL Server

Project Structure

ECommerceAPI/
│
├── Controllers/
├── Data/
├── DTOs/
├── Middleware/
├── Models/
├── Repositories/
├── Services/
├── Migrations/
│
├── Program.cs
├── ECommerceAPI.csproj
└── appsettings.example.json

Authentication & Authorization

The API uses JWT Bearer Authentication.

Authentication Flow

Register
   ↓
Login
   ↓
JWT Token
   ↓
Authorize
   ↓
Access Protected APIs

Role-based authorization is implemented for Admin operations.

Database

The application uses SQL Server with Entity Framework Core.

Main Entities
Users
Customers
Categories
Products
Carts
CartItems
Orders
OrderItems
Coupons
Payments

Entity relationships are configured using Entity Framework Core.

Database migrations are included in the project.

Main API Endpoints

Authentication
POST /api/Auth/register
POST /api/Auth/login

Products
GET    /api/Products
GET    /api/Products/{id}
POST   /api/Products
PUT    /api/Products/{id}
DELETE /api/Products/{id}

Cart
GET    /api/Cart
POST   /api/Cart/items
PUT    /api/Cart/items/{id}
DELETE /api/Cart/items/{id}
DELETE /api/Cart

Checkout
POST /api/Checkout

Orders
GET    /api/Orders
GET    /api/Orders/{id}
POST   /api/Orders
PUT    /api/Orders/{id}/status
DELETE /api/Orders/{id}

Coupons
GET    /api/Coupons
GET    /api/Coupons/{id}
POST   /api/Coupons
PUT    /api/Coupons/{id}
DELETE /api/Coupons/{id}

Payments
GET  /api/Payments
GET  /api/Payments/{id}
GET  /api/Payments/order/{orderId}
POST /api/Payments

Admin Dashboard
GET /api/AdminDashboard/summary

How to Run
1. Clone the repository
git clone https://github.com/subodhkrsoni/ECommerceAPI.git

cd ECommerceAPI
2. Configure the database

Create your local appsettings.json using:

appsettings.example.json

Configure your SQL Server connection string and JWT settings.

3. Apply database migrations
dotnet ef database update
4. Run the application
dotnet run --project .\ECommerceAPI\ECommerceAPI.csproj
5. Open Swagger
http://localhost:5199/swagger

Security
Passwords are stored using password hashing.
JWT authentication is used for protected APIs.
Role-based authorization is used for Admin APIs.
Sensitive appsettings.json configuration is excluded from Git.
appsettings.example.json is provided as a configuration template.

API Testing

The API has been tested using Swagger UI.

Tested functionality includes:

Authentication
Product CRUD
Product search, filtering and sorting
Pagination
Cart operations
Checkout
Coupon validation
Order status transitions
Payment success/failure

Admin dashboard
Future Enhancements
Frontend application
Product image upload

Wishlist
Email notifications
Real payment gateway integration
Docker containerization
CI/CD pipeline
Cloud deployment

Author

Subodh Kumar Soni

GitHub:
https://github.com/subodhkrsoni

