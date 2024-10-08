
# Trading app

This is Web Api application that manages buy / sell transactions and displays some reports.

This app is developed using C# .NET Core framework.
I have used CODE FIRST approach for the database with Entity Framework.
The database used is Microsoft SQL.



## Steps to set the app on your local machine

1. **Install .Net Framework Core & SQL Express database Server instance** on your machine
Check if appsettings.json has the right connection string and adjust accordingly

2. Open the solution in **Visual Studio** and Run '**update-database**' command into 'Package Manager Console' with **Ferovinum.Services** project selected to install the database schema ( apply migration ) into your installed local SQL express database

3. Optional: To browse directly through the database you may want to install **Sql Server Management Studio**

4. Run your app locally on IIS Express or any other .NET web server

The Swagger start up page will help in calling the Transaction and Author RESTFul API.

Start by creating a new transaction, by hitting POST on the /order API Endpoint with the following JSON or something similar:

  ```sh
{ 
    "clientId": "C-1", 
    "productId": "P-1", 
    "orderType": "buy",
    "quantity": 1000,
    "timestamp": "2020-01-01T10:00:00"
}
  ```

This will create a buy transaction, the price of the transaction will be fetched from the Price sql table.

You can also make a sell transaction posting this to the order endpoint

  ```sh
{ 
    "clientId": "C-1", 
    "productId": "P-1", 
    "ordertype": "sell",
    "quantity": 500,
    "timestamp": "2020-02-01T10:00:00"
}
  ```
  
This will create a sell transaction, the price of the transaction will be calculated based on the Price from sql table, 
the fee of the client and the time from the transaction that this stock is being fetched.

Hitting GET on the following endpoints will return the created transactions with the set unit price
/transactions/product/P-1
/transactions/product/P-1?fromDate=2020-01-01&toDate=2020-05-02

  ```sh
[
    {
        "clientId": "C-1",
        "productId": "P-1",
        "quantity": 1000,
        "price": 47.9,
        "orderType": "buy",
        "timestamp": "2020-01-01T10:00:00"
    },
    {
        "clientId": "C-2",
        "productId": "P-1",
        "quantity": 1000,
        "price": 47.9,
        "orderType": "buy",
        "timestamp": "2020-01-01T10:00:00"
    },
    {
        "clientId": "C-1",
        "productId": "P-1",
        "quantity": 600,
        "price": 49.46,
        "orderType": "sell",
        "timestamp": "2020-01-01T10:00:00"
    },
    {
        "clientId": "C-1",
        "productId": "P-1",
        "quantity": 400,
        "price": 49.46,
        "orderType": "sell",
        "timestamp": "2020-01-01T10:00:00"
    },
    {
        "clientId": "C-1",
        "productId": "P-1",
        "quantity": 1000,
        "price": 47.9,
        "orderType": "buy",
        "timestamp": "2020-01-01T10:00:00"
    },
    {
        "clientId": "C-1",
        "productId": "P-1",
        "quantity": 1000,
        "price": 47.9,
        "orderType": "buy",
        "timestamp": "2020-05-01T10:00:00"
    }
]
  ```

## Main services of the app

**Ferovinum.Services\PortfolioService.cs** - calculates metrics

**Ferovinum.Services\TransactionsService.cs** - creates a new buy / sell transaction & gets list of transactions based on productId and clientId

**Ferovinum.Services\BalanceService.cs** - gets balances on productId and clientId

## Validations are in place for API requests

With the help of FluentValidation library, JSON validations are in place in the **Ferovinum.Validators** namespace

## Automapper library used for mapping DTOs to Models

The MappingProfiles in Ferovinum.Services takes care of the mappings setup between DTOs and Models in namespace namespace **Ferovinum.Services.Mappings**.

## Entity Framework Core as ORM

The database is being accessed using EF Core
Code First approach is in place with migrations located in **Ferovinum.Services**.

## Data Models

* **Client** - the clients fees
* **Product** - the products prices
* **Transaction** contains ClientId, ProductId, Quantity, Price, OrderType, Timestamp and 2 additional columns: 
  **StockLeft** (used only for buy transactions)- for storing the stock available after sells, 
  **ParentBuyTransactionId** (only used for sell transactions)- to link the sell transaction to the buy transaction which is drawing the items from 

## Projects

* **Ferovinum** - the web api project
* **Ferovinum.Domain** - the data models
* **Ferovinum.Services** - the transaction service, the repositories wrappers for accessing the database
* **Ferovinum.Services.Tests** - the services NUnit tests

In this project, I wrote the model classes first and EF Core will create the database. 
This is called Code First Approach.

## Database optimisation
There are 3 indexes on the Transactions SQL Table that help much improving the query time:

* **1 - ProductId** - single index - improved performance when searching by ProductId
* **2 - ClientId** - single index - improved performance when searching by ClientId
* **3 - ProductId with ClientId** - composite index - that helps when searching on both field.

This enlarges the sql table ( making it bigger in dimension/ data stored) and also makes the adding process take longer but much improves the query time when large datasets in place.

## Possible improvements to the app / Limitations / Trade-offs
At the moment you can only make a sell transaction that finishes or takes part of the first in order available buy transaction stock.
So, for example if there are 3 buy transactions of 1000 items at different times by Ferovinum from the Client, the Client can, at the moment, make a 'sell' order from Ferovinum only less or the quantity left from the first transaction.

As the way of taking the items is FIFO, the app does not allow The Client to make a first transaction of 1100 items,
as the price would have a more complex calculation ( 1000 items at the interest rate of the first order with different timestamp, and 100 items of the second transaction of the 1000 items )

This can be achieved with a more complex algorithm in place.


Right now, the app is not intended for high frequency transactions. The app is not optimized for high frequency transactions.
When a lot of transactions will happen at the same time this could cause the transaction mechanism to fail, to enter a long period of waiting.

There are multiple ways to scale this to high transactional system:
* Implement a distributed transaction system with a load balancer in place
* Asynchronous transactions can be implemented as well.
* Optimise database EF Core queries or use / call T-SQL Queries / Stored Procedures.
* Make use of SignalR or Pure Websockets to have a faster communication.
* Indexing the appropiate columns in database.
* Caching can be added as well.

Also rollback strategy can be added in case an exception occurs in the buy/sell transaction, if other entities are in place.

To improve the database performance I could have created 2 tables: one for buys and one for sells.
Now, for example the sells will never store data in StockLeft column and buys will never store information in ParentBuyTransactionId column, 
but for the simplicity I used only one transactions sql table.
Also there are columns that can be indexed for faster browsing through the database, but in our case is not necessary.

Regarding unit tests & e2e tests:
There are a lot of other unit tests to be created, for different cases.

An authentication and authorization ( OpenID Connect & OAuth2 ) system like Azure B2C with MSAL .NET would be necessary here to secure these RESTFUL APIs.

I have not managed to successfully come up with the weightedAverageRealisedAnnualisedYieldSum formula and to match what is the example returning.
The formula that I know for "Annualised yield of fees
weighted on notional amount of sold stock" is  
sum of all sold transactions ( notional amount ) of Quantity sold * unit product Price * ((1 + client.Fee / 12 ) ^ 12 - 1)
divided by sum of all sold notional amount ( for that client)

Also, for me the “lifeToDateProductNotional”: is returning 65990 ( with the test exercise example) ,not 659900 like mentioned in the exercise example.
