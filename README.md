# Full Banking App, API and Admin Portal  

## Lachlan Ward and Dushyant Kinnigoli Mallya

All of our three projects are contained within the zip file. Each folder is labeled with its program. 	The Customer Website can be run individually. The API website can be run individually. However the Admin Portal Website requires connection to the API thus the API must be simultaneously run. Connections to local hosts are different across these two projects for this reason.

To run each project open the repo and build the solution and hit run in visual studio. 

## EF Core Code First

We used a code first approach to build our models and database context. Appropriate models can be found in the Models Folder. We primarily used similar models to our assignment one. With the addition of data validation and BillPays and Payee Models. We also utilized some ViewModels for particular pages such as transaction view. These models specially for views can be found in the ViewModels folder. On occasion we added enums such as “Blocked” “B” for Billpay or Authorization “A” or “U” for functions of blocking bill pays and login authorization used later on in the admin portal and API. Migrations can be found in the migration folder. For our site we used Dushyant's Azure database s3842605, abc123.

## Pre Loading

The function in program called ``Seedata.ClearData()`` is a function that clears the connected data in the database and we used for testing and debugging. Uncomment it to be used on startup.  

* First the Program checks if the Database contains customers, and if it does, skips the preloading process.  
* If the Database does not contain customers, it contacts the provided web-service. First the data from the web-service is deserialized by the ``Newtonsoft`` deserializer method, and the necessary objects to populate the database are created.  Then, our hardcoded payees are created.
* Subsequently, the database is populated with these objects, which already contain some form of validation in the form of Annotations in the ``Model``. There is also some conversion of data before the database is populated as there are some extra fields in the database as well as some mismatched field names.  
* After Preloading is complete, the ``BillPay`` objects are checked asynchronously to see if any outstanding bills are due payment, pays any due ``BillPay``s and updates the balance, as well as creates a new ``BillPay`` if it was a monthly recurring payment and updates the database with this ``BillPay``. This goes on without blocking the execution of the program.
* Right after Preloading and while the ``BillPay`` objects are being checked, the home page is loaded ready for the customer to continue using the program.

 ## Customer Functions

Our login functions are primarily pulled from Day 6 Lectorials Login. Same validation, controller and views are used with minor modifications. Once a user is logged in we created a partial view that displays user account options such as deposit and withdrawal. This partial view can be found in the shared folder under “_MiniNavBar”. We found this navigation was the most user friendly and thus this partial view is rendered on every page the user is logged in as. Reference to this can be found in shared “_Layout”.

This nave bar displays all required functions. 

Deposit allows drop down selection of user owned accounts, input amount and comment. Appropriate data validation is implemented.


Withdrawal allows drop down selection of user owned accounts, input amount and comment. Appropriate data validation is implemented with additional validation in ensuring non-negative balance and adding service fees for over two free transactions. 

Transfer allows one drop down selection for user owned account and int input for account number for any account in the database, Validation is similar as above and ensures a user cant transfer to and from the same account as well as non-negative balance. 

All three types of these transactions after input are sent to the ReviewTransaction controller. This controller returns a detailed view of the transaction and allows the user to confirm it. Once confirmed this sends the transaction to an AddTransaction controller that adds the appropriate transaction to the account depending on its type as well as adding service fees as appropriate. 

MyStatements allows the user to select a specific account to view. This sends the list of transactions to the MyStatementView with a starting int index. If there are more than four transactions the user can navigate next and previous as appropriate to these transactions. The list is ordered by most recent and displayed in local time. Only four are displayed at one time. Logic behind naviating list can be found in the view and MyStatement and MyStatementView controllers as well as a special ViewModel. 

MyProfile displays the user's customer details, pre populated in the fields and allows the user to edit them and submit them back to the database. There is also a password change button that takes the user to the password change page. This page requires input of current LoginId and Old password and new password. LoginID and oldPassword are checked and must be correct/match before the new password is hashed and updated in the database.

All format constraints are validated in the models with the addition of html validation and controller validation in some cases.m 


## BillPay Functions

On the navigation bar the user can select BillPay. The user can select the account they wish to view their current bill and also add a billpay right there. The BillPay statement view displays a table of all bills paid, their status, secluded times, period ect. The user can add a billpay and schedule a time, amount, period, ect. Once added this appears in the BillPay view for that account and is pending until the time arrives for it to be paid out. During the program a list of billpays is checked for their scheduled time. If it arrives the bill is converted into a transaction and added to the account. If the amount results in a negative balance the bill fails. On startup this method is immediately called and pays out any bills with appropriate validation before the website runs. The user can also edit and delete bills in the table view of BillPay. We also added the add payee function that adds a payee to the database. We also hardcoded some payees already for testing. HTML AND CSS was modified in the views to display Failed and Pending/Paid for bills that have been successfully paid. For monthly bills that are paid, a new bill is created and added to the list for exactly one month in the future thus the cycle repeats. One Off bills are only paid once and thus not added again.

## Admin Portal
Along the lines of the main program, the Admin Portal contains much of the same logic for the creation of the program and many of the pages.  
But, here authentication is not checked and a simple ``admin`` credential entry will grant access to the Admin Portal.  
 
Once in the Admin portal, we have implemented all the functions mentioned in the spec which use the Admin API endpoints for all the necessary validation and data retrieval.
 
* Transaction History: The admin is able to choose which ``Account``'s transaction history he wants to view, and view it a few tranasctions at a time sorted by date, as well as page through the different pages if there are a large number of transactions.
* Customer Profile: The admin can choose to view and edit any customer's information, and any changes are saved by passing it to the Admin API Endpoint in the back end of the program.
* Lock/Unlock Login: The admin can choose an ``Account`` to toggle lock/unlock, and this ``Account`` number is passed onto the API endpoint through the backend, then the API sets the ``Account`` to the desired authorization status.
* Block/Unblock ``BillPay``s: The admin can choose a ``BillPay`` to block/unblock and using the Admin API the database is updated. Once a ``BillPay`` is blocked, it is overlooked in all checks and never runs till it is manually unblocked by an Admin through the Admin Portal.
 
## Admin API
The Admin API contains all the logic, most of the validation and the DataSets while being in close contact with the database. It implements the Repository design pattern with the IRepository Interface which involves generics, as well as all the Repository classes that implement this interface.
The interface contains the commonly used methods such as Get, Put, Post and Update which are used to update the database and retrieve data as necessary in order to facilitate the smooth functioning of the Admin Portal.

## Unit Tests

(**The database needs to be populated before the tests are run as the tests contact the database**)

The unit tests we created covers certain crucial Admin API Endpoints. Testing these endpoints ensures their functionality and the smooth operation of the Admin API. These in turn also test the connection between the repository and the controllers, further strengthening and validating the program.
* ``AccountGetTest`` tests the Get method of the Accounts repository and checks for wrong values as well.
* ``CustomerGetTest`` tests the Get method of the Customer repository and checks for wrong values as well.
* ``CustomerPutTest`` tests the Put method of the Customer repository and checks for other state values.
* ``LoginLockTest`` tests the Locking method of the ``Login`` objects, taking into account the toggle, hence the test method can be run without any intervention.


## GitHub
We used 8 branches during the dev process. All these branches were merged back into main at the end of the process as they implemented various different features such as BillPay, Admin API and Portal and Login functions ect.

Git Hub Branches
![Alt text](Trello/Trello/branches.png?raw=true "Trello")

A Example of our commits can be seen here as well. 

Git Hub Commit Example
![Alt text](Trello/Trello/gitcommits.png?raw=true "Trello")


## Trello
We used trello for tracking all relevant dev processes and tasks. We planned each task/function we needed to create and implement and assinged it to either dushyant or myself. When completed we labled it with a green label or added a finished comment. Below are examples of our dev process over time using Trello. 

15/1
![Alt text](Trello/Trello/15_1_22.png?raw=true "Trello")

17/1
![Alt text](Trello/Trello/17_1_22.png?raw=true "Trello")

19/1
![Alt text](Trello/Trello/19_1_22.png?raw=true "Trello")

21/1
![Alt text](Trello/Trello/21_1_22.png?raw=true "Trello")

25/1
![Alt text](Trello/Trello/25_1_22.png?raw=true "Trello")

27/1
![Alt text](Trello/Trello/27_1_22.png?raw=true "Trello")

29/1
![Alt text](Trello/Trello/29_1_22.png?raw=true "Trello")

31/1
![Alt text](Trello/Trello/31_1_22.png?raw=true "Trello")