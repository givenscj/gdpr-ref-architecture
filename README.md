# gdpr-ref-architecture
## GDPR Reference Architecture

This is a reference archirecture that demostrates a message and command pattern style of development to support the [Data Subject Action Pattern](http://blogs.architectingconnectedsystems.com/2017/11/16/compliance-manager-gdpr-data-subject-action-pattern/) in applicaton "stubs".  This is the follow-up to my blog post on [GDPR and Microsoft Technologies](http://blogs.architectingconnectedsystems.com/2017/12/01/gdpr-microsoft-sample-presentation-slides/).  

This reference architecture includes the following items:

- Azure ARM Template to setup resources
- Supporting GDPR master database
- A GDPR Data Subject Portal
- A PowerApps Form and supporting custom connector
- An event processor console
- A windows service for on-premises integration

## Setup Steps

- Deploy the ARM template to an azure instance
- Deploy the two dacpac database files to your new Azure SQL server resource
- Deploy the WebApp.GDPRHubProxy (take note of its azure endpoint)
- Deploy the PowerApps application
- Update the custom PowerApp connector OpenAPI config file
- Deploy the custom PowerApp connector
- Configure the PowerApps GRPR form button to use the custom connector
- Update all the app.config and web.config files to point to your Azure Key Vault connection strings (Event Hub, Database)
- Start the event hub processor console project
- Start the Console.Test project to fire some messages to the Event Hub

## GDPR Data Subject Action Pattern

In order for an application to be GDPR compliant, it must be able to handle the following requests:

- Record Create (Out)
- Record Search (All, Single entity)
- Record Delete (In/Out)
- Record Hold
- Record Update (In/Out)
- GetChanges (Created and updated records)
- Export Data
