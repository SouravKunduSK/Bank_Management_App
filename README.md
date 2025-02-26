
# Bank Management App 

## Table of Contents
- [Overview](#overview)
- [Task Accomplished](#task-accomplished)
    - [Backend](#backend)
    - [Database](#database)
- [Key Features](#key-features)
- [Necessary Nuget Packages / Libraries](#necessary-nuget-packages--libraries)
 - [Project Structure](#project-structure)
 - [API Endpoints](#api-endpoints)
 - [FrontEnd](#frontend)
 - [Technology Stack](#technology-stack)
 - [How to run](#how-to-run)
 - [Limitations](#limitations)
 - [Conclusions](#conclusions)
## Overview
A Bank Management App simulates core banking functionalities, allowing users to manage 
accounts, conduct transactions, and view reports. This project would involve both a backend API 
and frontend interface, where clients (customers, bank staff, and admins) interact with the 
system.
### Task Accomplished
#### Backend
- [x] Write backend services using C# and ASP .NET Core 9.
- [x] Route management
- [x] Entity Framework Core (ORM) used
- [x] JWT based authentication
#### Database
- [x] MS SQL database integration
### Key Features 
- User Authentication and Authorization 
  - Roles: Define roles for customers, bank employees, and administrators. 
  - JWT/OAuth2 Authentication: Secure API access using JWT tokens or OAuth2 protocol for user authentication and authorization. 
  - Two-Factor Authentication: Optional feature for increased security. 

- Account Management 
  - Account Types: Support for checking, savings, and loan accounts. 
  - CRUD Operations: Allow creation, viewing, updating, and deletion of accounts. 
  - Account Details: Display balance, account history, and customer details. 
  - Multi-Currency Support: Optionally add support for different currencies if needed.
- Transactions and Fund Transfers 
    - Deposit and Withdrawals: Enable deposits, withdrawals, and transfers between accounts. 
   - Fund Transfers: Allow internal and external fund transfers (between different banks). 
    - Transaction Limits and Fees: Set daily transaction limits and apply fees for certain types of accounts. 
    - Transaction History: Store and retrieve transaction history for each account.

- Loan Management 
  - Loan Application: Enable customers to apply for loans (with fields for loan type, amount, etc.). 
  - Loan Approval Workflow: Admins or managers review and approve/reject loan applications. 
  - Repayment Schedules: Calculate and display loan repayment schedules, including interest. 
  - Penalty Calculation: Add functionality to handle penalties for overdue repayments.
- Interest Calculation and Savings Plans 
  - Interest Calculation Engine: Calculate monthly or yearly interest based on account types. 
  - Savings Plans: Offer fixed deposit or recurring deposit plans with interest calculations. 
  - Automated Interest Credit: Monthly crediting of interest to accounts with a scheduler. 
- Notifications and Alerts 
  - Email/SMS Alerts: Notify customers on significant transactions, low balance warnings, and due dates for loans. 
  - Real-Time Notifications: Implement real-time notifications for users, using SignalR or similar technologies to deliver instant updates (e.g., account activity, transaction status, security alerts).

### Necessary Nuget Packages / Libraries
Go to 'Manage NuGet Packages' or Open Package manager console in Visual Studio, then install below packages:
```bash
1.  Install-Package Microsoft.EntityFramework.Core
```
```bash
2.  Install-Package Microsoft.EntityFramework.Core.Tools
```
```bash
3.  Install-Package Microsoft.EntityFramework.Core.SQLServer
```
```bash
4.  Install-Package Extensions.Identity.Stores
```
```bash
5.  Install-Package AspNetCore.SwaggerUI
```
```bash
6.  Install-Package Microsoft.AspNetCore.Builder
```
```bash
7.  Install-Package Microsoft.EntityFramework.Core.Authentication.JWT Bearer
```
```bash
8.  Install-Package Newtonsoft.Json
```
### Project Structure
- `Bank_Management_Api` for implenting the api.
   - `Controllers` for creating api controllers.
   - `Helpers` for creating common problem solution.
   - `Models` for creating requsest and response models.
   - `Services` for creating Interfaces and Service.
   - `appsetting.json` for using sensetive data.
   - `Program.cs` for Dependency Injection, Middleware etc.
- `Bank_Management_Data` for accesing database.
   - `Data` for creating static data and DbContext.
   - `Migrations` for performing migration.
   - `Models` for creating classes and business logic.
- `Bank_Management_UI` for consuming api and presenting UI.
   - `Controllers` for consuming api.
   - `Views` for presenting the UI.

### API Endpoints
#### Authentication (Auth) (`https://localhost:7077/api/Auth`)
- `POST /api/Auth/create new user` - Register a new user.
- `POST /api/Auth/login` - User login.
- `GET /api/Auth/user-data` - Get logged-in user data.
- `POST /api/Auth/refresh-token` - Refresh authentication token.
- `POST /api/Auth/revoke-token` - Revoke authentication token.
- `POST /api/Auth/logout` - Logout the user.

#### Account (`https://localhost:7077/api/Account`)
- `POST /api/Account/create` - Create a new account.
- `GET /api/Account/details/{accountNumber}` - View account details.
- `GET /api/Account/all-accounts` - List all accounts.
- `POST /api/Account/approve/{accountNumber}` - Approve an account.
- `POST /api/Account/freeze/{accountNumber}` - Freeze an account.
- `POST /api/Account/close/{accountNumber}` - Close an account.
- `DELETE /api/Account/{accountNumber}` - Delete an account.

#### Account Type (`https://localhost:7077/api/AccountType`)
- `GET /api/AccountType/all-account-types` - Get all account types.
- `GET /api/AccountType/detail/{typeName}` - View account type details.
- `POST /api/AccountType/create` - Create a new account type.
- `PUT /api/AccountType/edit/{id}` - Edit an account type.
- `DELETE /api/AccountType/delete/{id}` - Delete an account type.

#### Transaction (`https://localhost:7077/api/Transaction`)
- POST /api/Transaction/deposit - Deposit funds.
- POST /api/Transaction/withdraw - Withdraw funds.
- POST /api/Transaction/transfer - Transfer funds between accounts.
- GET /api/Transaction/history/{accountNumber} - Get transaction history.

#### Loan (`https://localhost:7077/api/Loan`)
- POST /api/Loan/apply - Apply for a loan.
- POST /api/Loan/approve/{loanId} - Approve a loan.
- POST /api/Loan/reject/{loanId} - Reject a loan.
- POST /api/Loan/repay - Repay a loan.
- GET /api/Loan/{loanId} - Get loan details.
- GET /api/Loan/all-loans - Get all loans.

#### LoanRepayment (`https://localhost:7077/api/LoanRepayment`)
- POST/api/LoanRepayment/RepayLoan/{loanId} - Repay a loan.
- GET/api/LoanRepayment/CalculatePenalty/{loanId} - Calculate penalty for overdue loans.
- GET/api/LoanRepayment/GetRepaymentHistory/{loanId} - Get loan repayment history.
- GET/api/LoanRepayment/GetRepaymentsByLoanId/{loanId} - Get loan repayment history by id.

##### LoanType (`https://localhost:7077/api/LoanType`)
- GET/api/LoanType/all-loan-types - Get all types for loan.

#### Interest (`https://localhost:7077/api/Interest`)
- POST/api/Interest/CalculateAndApplyInterest - Calculate and apply interest on loan account.
## FrontEnd
Simple UI for consuming api
### Login Page
![Login](https://github.com/user-attachments/assets/9aa1404d-8f92-46cd-9664-712d8e157769)

### Create Account
![Create_Account](https://github.com/user-attachments/assets/bb85a41d-3c9f-4e04-b534-963b38a6891e)

### All Account List
![AccountList](https://github.com/user-attachments/assets/aaadff10-a8fa-4966-ae6f-b4168719bbc7)

### Deposit
![B_Deposit](https://github.com/user-attachments/assets/67ae5155-03af-419f-a439-f4c0cdce38ec)


### Transaction
![Transaction History](https://github.com/user-attachments/assets/364712b9-19ff-484a-98c9-d46c6a9094cc)


## Technology Stack

**Backend:** C#

**FrontEnd:** Razor Views 

**Framework:** ASP .NET Core MVC, WebAPI 9

**Database:** MS SQL Server

**Authentication:** JWT Token

**ORM:** Entity Framework Core

**Notification/Email:** SMTP 


## How to Run
    1. Clone the repository using (https://github.com/SouravKunduSK/Bank_Management_App)
    2. Download necessary packages
    3. Connect to Database
    4. Create New Profile for multiple project run at a time and Run.

## Limitations
`x ` Not implemented full functional frontend.

`x ` Not able to transfer fund to other bank.

`x ` Penalty Calculation is not implemented.

`x ` Automated Interest Credit is not implemented.

`x `Reporting, Auto-BackUp and Realtime notification are not implemented.(Only Email notification is implemented).

`x ` Not unit-test is done.

## Conclusions
This is the first time I have worked with Bnaking Management System. I have faced problem for creating database at first, then I have faced some problems while consuming api to frontend using controller.
Thanks for givinig me this opportunity, your time and consideration.

