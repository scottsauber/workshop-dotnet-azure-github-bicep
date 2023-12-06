# workshop-dotnet-azure-github-bicep

In this workshop we're going to cover .NET, Azure, GitHub, and Bicep.

This workshop will highlight the following:

- Health Checks
- Zero Downtime Deployments
- Infrastructure managed by code using Bicep
- Automated builds and deploys
- WhatIf on PRs for Infrastructure Changes
- Follows [Azure Naming Standards](https://learn.microsoft.com/en-us/azure/cloud-adoption-framework/ready/azure-best-practices/resource-abbreviations#compute-and-web) for naming resources
- Version Endpoint so you know what's deployed

## Workshop

### Module 1 - Review Azure Slides

1. What is a Subscription?
2. What is a Resource Group?
3. What is Azure App Service?
4. What is Azure App Service Plan?

### Module 2 - Live Demo of Running Azure App Service

1. Lay of the land
   1. Each of you have 2 Resource Groups for Dev + Prod where you are a Contributor
   2. Each of you have a Federated Credential behind each RG where it's Contributor that's authed to your repo
      1. This is the user Bicep will run under
   3. Each of you have Reader access to two already deployed Dev and Prod Resource Groups with App Services in them so you can follow along
2. View Subscription
   1. View Costs
   2. Set Cost Alerts
   3. Budgets
   4. Access Control (IAM)
   5. Resource Groups
3. View Resource Group
   1. See all resources at a glance
   2. Access Control (IAM)
   3. Deployments
   4. Costs
   5. Go to App Service Plan
4. App Service Plan
   1. Show CPU, Memory, Network
   2. Show RG
   3. Show Scale Up and Scale Out
   4. Show Apps
5. View Home Page of an App Service
   1. Home Page, Stop, Restart
   2. RG + Subscription
   3. Location
   4. Default Domain
   5. OS
   6. Health Check
   7. Show Configuration
   8. Show Custom Domains
   9. Show Certificates
   10. Log Stream
   11. Advanced Tools (Kudu)
       1. Bash Terminal
       2. File System
       3. Logs
   12. Deployment Slots
6. We have the same for each env - Dev and Prod
   1. How do we do that without clicking those same settings everywhere... enter Bicep

### Module 4 - Review Bicep Slides

### Module 5 - Bicep Demo

1. Show parameters with @allowed values
   1. Show creating parameters.bicepparam with wrong value

## Hands On Work
