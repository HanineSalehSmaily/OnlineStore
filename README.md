# 🛒 Online Store Project

A simple online store system built with basic e-commerce functionalities.

## 🚀 Features

- 🔐 User Authentication (Login)
- 🛍️ Add products to cart
- 🧾 Checkout system
- 🗄️ Database integration

## 🗃️ Database Structure

The project includes the following tables:

- **Cart** → stores selected items
- **Orders** → temporary order data
- **Checkout** → finalized orders
- **CheckoutDetails** → stores user address and order details

## 🔄 Order Flow

1. User logs in
2. Adds items to cart
3. Proceeds to checkout
4. Order is transferred from **Orders** to **Checkout**
5. User address is saved in **CheckoutDetails**

## 💻 Technologies Used

- (Add your language here: C#, Java, etc.)
- Database (SQL Server / MySQL / etc.)

## 📌 Notes

This project demonstrates the core logic of an e-commerce system including cart management and order processing.
