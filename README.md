# 🛒 E-Commerce REST API

A backend REST API for an E-Commerce application built using **ASP.NET Core Web API, Entity Framework Core, and SQL Server**.

The project provides APIs for authentication, products, categories, cart management, checkout, coupons, orders, payments, and an admin dashboard.

---

## 🚀 Tech Stack

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

## ✨ Features

### 🔐 Authentication & Authorization

- User registration
- User login
- Password hashing
- JWT-based authentication
- Role-based authorization
- Admin and User roles

### 📦 Product Management

- Get all products
- Search products
- Filter by category
- Sort by price
- Pagination
- Create product
- Update product
- Delete product

### 🛍️ Shopping Cart

- Add product to cart
- Update quantity
- Remove cart item
- Clear cart
- Stock validation
- Automatically merge duplicate products in cart

### 🎟️ Coupon Management

- Create coupons
- Update coupons
- Activate/deactivate coupons
- Percentage discount
- Fixed amount discount
- Minimum order amount
- Expiry date validation
- Discount cannot exceed order total

### 🧾 Checkout

- Checkout from cart
- Create order
- Create order items
- Calculate total amount
- Apply coupon
- Validate product stock
- Reduce product stock
- Clear cart after successful checkout

### 📋 Order Management

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
