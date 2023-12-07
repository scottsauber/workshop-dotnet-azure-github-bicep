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

Pre-requisites:

1. [.NET 8](https://dotnet.microsoft.com/en-us/download)
1. Git
1. GitHub Account
1. Tell Scott the following:

- Email you will use for Azure
- GitHub account
- GitHub repo name
  - Should be the same as this one: workshop-dotnet-azure-github-bicep

1. Recommended to use VS Code with the [Bicep extension](https://marketplace.visualstudio.com/items?itemName=ms-azuretools.vscode-bicep) for editing Bicep
1. Fork this repo

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
7. Show the Version Endpoint
8. Make some change and deploy it, watch it go through the AzurePing service

### Module 3 - Review Bicep Slides

### Module 4 - Bicep Hands On

1. Delete your `.github` folder and your `infrastructure` folder and commit and push that code. This history is here for reference in case you get stuck.
1. Create a new folder called `infrastructure`
1. Create a `appservice.bicep` file
1. Create a Linux App Service Plan resource

   ```bicep
      resource appServicePlan 'Microsoft.Web/serverfarms@2022-09-01' = {
         name: 'asp-dnazghbicep-'PUTYOURUSERNAMEHERE'-dev'
         location: 'centralus'
         sku: {
           name: 'S1'
         }
         kind: 'linux'
         properties: {
           reserved: true
         }
       }
   ```

1. Next create an App Service resource, by referencing the App Service Plan's ID (note replace the ???? with the reference's ID)

   ```bicep
    resource appService 'Microsoft.Web/sites@2022-09-01' = {
       name: 'app-dnazghbicep-'PUTYOURUSERNAMEHERE'-dev'
       location: 'centralus'
       identity: {
        type: 'SystemAssigned'
       }
       properties: {
          serverFarmId: ????
          httpsOnly: true
          siteConfig: {
            http20Enabled: true
            linuxFxVersion: 'DOTNETCORE|8.0'
            alwaysOn: true
            ftpsState: 'Disabled'
            minTlsVersion: '1.2'
            webSocketsEnabled: true
            healthCheckPath: '/api/healthz'
            requestTracingEnabled: true
            detailedErrorLoggingEnabled: true
            httpLoggingEnabled: true
          }
        }
    }
   ```

1. Add the environment variables for the app service, by referencing the app service object itself (note replace the ???? with the reference)

   ```bicep
       resource appSettings 'Microsoft.Web/sites/config@2022-09-01' = {
         name: 'appsettings'
         kind: 'string'
         parent: appService
         properties: {
             ASPNETCORE_ENVIRONMENT: 'dev'
         }
       }
   ```

1. Add the App Service Slot. Note: there's a lot of duplication in the properties... maybe we should do something about that?

   ```bicep
      resource appServiceSlot 'Microsoft.Web/sites/slots@2022-09-01' = {
         location: 'centralus'
         parent: appService
         name: 'slot'
         identity: {
             type: 'SystemAssigned'
         }
         properties: {
             serverFarmId: appServicePlan.id
             httpsOnly: true
             siteConfig: {
                http20Enabled: true
                linuxFxVersion: 'DOTNETCORE|8.0'
                alwaysOn: true
                ftpsState: 'Disabled'
                minTlsVersion: '1.2'
                webSocketsEnabled: true
                healthCheckPath: '/api/healthz'
                requestTracingEnabled: true
                detailedErrorLoggingEnabled: true
                httpLoggingEnabled: true
             }
         }
      }
   ```

1. Add the environment variables for the app service slot, by referencing the app service object itself (note replace the ???? with the reference)

   ```bicep
    resource appServiceSlotSetting 'Microsoft.Web/sites/slots/config@2022-09-01' = {
      name: 'appsettings'
      kind: 'string'
      parent: appServiceSlot
      properties: {
        ASPNETCORE_ENVIRONMENT: 'dev'
      }
    }
   ```

1. Review the code and identify duplication

1. If you're in VS Code, you're getting a warning for the location not being parameterized. Add a parameter below and then replace all the 'centralus' with a reference to that parameter:

   ```bicep
    param location string

    // replace centralus with location everywhere
   ```

1. If you look closely for duplication you'll see we have "dev" repeated a lot, let's parameterize that too, because we'll want to swap that out for the word "prod" later. Also restrict the values to only allow 'dev' and 'prod'

   ```bicep
     @allowed(['dev', 'prod'])
     param environment string

     // replace dev with environment everywhere. Note: ${variableName} is how you do concatenation
   ```

1. If you look closely for more duplication, you'll see we have the app name repeated in both the appService and appServicePlan. Extract that out to a parameter

   ```bicep
    param appName string

    // down in the appServicePlan replace the name so it looks like this:
    resource appServicePlan 'Microsoft.Web/serverfarms@2022-09-01' = {
      name: 'asp-${appName}-${environment}'
      // the rest
    }

    // down in the appService replace the name so it looks like this:
    resource appService 'Microsoft.Web/sites@2022-09-01' = {
      name: 'app-${appName}-${environment}'
      // the rest
    }
   ```

1. Finally, if you look closely you'll see one last bit of duplication. The properties between the appService and the appServiceSlot. Let's extract out those properties to a variable and assign it in one place

   ```bicep
      // This is reused between the App Service and the Slot
      var appServiceProperties = {
          serverFarmId: appServicePlan.id
          httpsOnly: true
          siteConfig: {
            http20Enabled: true
            linuxFxVersion: 'DOTNETCORE|8.0'
            alwaysOn: true
            ftpsState: 'Disabled'
            minTlsVersion: '1.2'
            webSocketsEnabled: true
            healthCheckPath: '/api/healthz'
            requestTracingEnabled: true
            detailedErrorLoggingEnabled: true
            httpLoggingEnabled: true
          }
      }

      // Assign it in the appService and the appServiceSlot
      resource appService 'Microsoft.Web/sites@2022-09-01' = {
         // the rest
         properties: appServiceProperties
      }

      resource appServiceSlot 'Microsoft.Web/sites/slots@2022-09-01' = {
         // the rest
         properties: appServiceProperties
      }
   ```

1. Awesome! We now have a reusable Module that also enforces our naming standards that follow the [Azure Guidelines](https://learn.microsoft.com/en-us/azure/cloud-adoption-framework/ready/azure-best-practices/resource-abbreviations#compute-and-web) with the `asp-` prefix for App Service plans and `app-` for App Services.

1. Now we need to pass some values to those parameters. So let's create a `main.bicep` file

1. Provide a `targetScope` of 'resourceGroup' for the `main.bicep` module

   ```bicep
    targetScope = 'resourceGroup'
   ```

1. Now reference the `appservice.bicep` module you just created and pass the parameters to it

   ```bicep
    module app './appservice.bicep' = {
      name: 'appservice'
      params: {
        appName: 'workshop-dnazghbicep-'YOURUSERNAMEHERE''
        environment: ???
        location: 'centralus'
      }
    }
   ```

1. Crap - where does the environment come from? I want to pass that in dynamically depending on dev or prod. Enter `.bicepparam` files (these will be passed in dynamically via the CLI)

1. Add a parameter for environment and only allow `'dev'` and `'prod'` as values then reference that environment below in the `app` module

   ```bicep
    @allowed(['dev', 'prod'])
    param environment string

    targetScope = 'resourceGroup'

    module app './appservice.bicep' = {
      name: 'appservice'
      params: {
        appName: 'workshop-dnazghbicep-'YOURUSERNAMEHERE''
        environment: environment
        location: 'centralus'
      }
    }

   ```

1. Under the `infrastructure` folder, create a folder called `environments`.

1. Create a `dev.bicepparam` file with the following contents

   ```bicep
     using '../main.bicep'

     param environment = 'dev'
   ```

1. The `using` tells the `bicepparam` file what parameters are required. Try taking away the `param environment = 'dev'` and see what happens if you're in VS Code with the Bicep Extension. You will receive an error.

   ![Bicep Params](./docs/images/missing-environment.png)

1. Now add back `param environment ='dev'` but switch `'dev'` to `'qa'` or some other invalid value. You will see an error like this if you use VS Code.

   ![Bicep Params](./docs/images/wrong-environment.png)

1. That's it! We now have Bicep ready to be configured... but crap... how do we deploy it?? 👇👇

### Module 5 - GitHub Actions Review Slides

### Module 6 - GitHub Actions Review Slides

1. If it doesn't already exist, create a folder called `.github` and then `workflows` under that folder.
1. Create 6 new secrets based on the email you received at the beginning of this workshop. These secrets will be used to authenticate to Azure. If this wasn't in place, you wouldn't be able to talk to your Azure account, because Azure doesn't just let anyone in.
   1. Go to your Repo out in GitHub
   1. Click on Settings
   1. Click on Secrets and Variables
   1. Click on Actions
   1. Click New Repository Secret
   1. For secret name type `DEV_AZURE_CLIENT_ID`
   1. For the value paste in the value from the Dev value in the email
   1. Hit Add Secret
   1. Repeat for `DEV_AZURE_SUBSCRIPTION_ID` and `DEV_AZURE_TENANT_ID`
   1. Repeat for the Prod values for `PROD_AZURE_CLIENT_ID`, `PROD_AZURE_SUBSCRIPTION_ID`, and `PROD_AZURE_TENANT_ID`
   1. Note - the Subscription ID and the Tenant ID are the same. That is just for Workshop demo purposes. In a real world scenario the Subscription ID should be different.
1. Create a `ci.yml` file that looks like this. Note to replace the "<YOURUSERNAMEHERE>" with your GH username

   ```yml
   name: CI - Deploy App and Bicep

   on:
     push:
       branches: [main]
     workflow_dispatch:

   permissions:
     id-token: write
     pull-requests: write
     contents: read

   jobs:
     build_and_test:
       runs-on: ubuntu-latest
       name: Build, Test, Upload Artifact

       steps:
         - name: Checkout repo
           uses: actions/checkout@v1

         - name: Run dotnet test
           run: |
             dotnet test -c Release

         - name: Run dotnet publish
           run: |
             dotnet publish ./src/WorkshopDemo/WorkshopDemo.csproj -c Release -o ./publish

         - name: Update version file with GHA run number and git short SHA
           run: echo $(date +'%Y%m%d.%H%M').${{github.run_number}}-$(git rev-parse --short HEAD) > publish/version.txt

         - name: Upload artifact
           uses: actions/upload-artifact@v3
           with:
             name: dotnet-artifact
             path: publish/

     dev:
       needs: build_and_test
       uses: ./.github/workflows/step-deploy.yml
       with:
         env: dev
         artifact_name: dotnet-artifact
         resource_group_name: rg-workshop-dnazghbicep-<YOURUSERNAMEHERE>-dev
         app_service_name: app-workshop-dnazghbicep-<YOURUSERNAMEHERE>-dev
         app_service_slot_name: slot
       # Note: use GH Environment Secrets if using a Pro/Enterprise version of GH
       secrets:
         azure_client_id: ${{ secrets.DEV_AZURE_CLIENT_ID }}
         azure_subscription_id: ${{ secrets.DEV_AZURE_SUBSCRIPTION_ID }}
         azure_tenant_id: ${{ secrets.DEV_AZURE_TENANT_ID }}

     prod:
       needs: dev
       uses: ./.github/workflows/step-deploy.yml
       with:
         env: prod
         artifact_name: dotnet-artifact
         resource_group_name: rg-workshop-dnazghbicep-<YOURUSERNAMEHERE>-prod
         app_service_name: app-workshop-dnazghbicep-<YOURUSERNAMEHERE>-prod
         app_service_slot_name: slot
       # Note: use GH Environment Secrets if using a Pro/Enterprise version of GH
       secrets:
         azure_client_id: ${{ secrets.PROD_AZURE_CLIENT_ID }}
         azure_subscription_id: ${{ secrets.PROD_AZURE_SUBSCRIPTION_ID }}
         azure_tenant_id: ${{ secrets.PROD_AZURE_TENANT_ID }}
   ```

1. Note the `needs` in this workflow where the `dev` job needs `build_and_test` and the `prod` job needs `dev`. That indicates the jobs should be ran sequentially and not in parallel. Specifically in the order of `build_and_test` => `dev` => `prod`
1. We now need to create the reusable step in the `step-deploy.yml` to reduce duplication. Create a `step-deploy.yml` under the `.github/workflows` folder and enter in this data:

   ```yml
   name: "Step - Deploy"

   on:
     workflow_call:
       inputs:
         env:
           required: true
           type: string
         artifact_name:
           required: true
           type: string
         resource_group_name:
           required: true
           type: string
         app_service_name:
           required: true
           type: string
         app_service_slot_name:
           required: true
           type: string
       secrets:
         azure_client_id:
           required: true
           description: "Client ID for Azure Service Principal"
         azure_subscription_id:
           required: true
           description: "Azure Subscription ID for the targeted Resource Group"
         azure_tenant_id:
           required: true
           description: "Azure Tenant ID for the targeted Resource Group"

   jobs:
     deploy:
       name: Deploy to Azure App Service
       runs-on: ubuntu-latest
       steps:
         - uses: actions/checkout@v3

         - name: Log in to Azure
           uses: azure/login@v1
           with:
             client-id: ${{ secrets.azure_client_id }}
             tenant-id: ${{ secrets.azure_tenant_id }}
             subscription-id: ${{ secrets.azure_subscription_id }}

         - name: Run Bicep
           run: |
             az deployment group create \
               --name ${{ inputs.env }}-deployment-${{ github.run_number }} \
               --template-file infrastructure/main.bicep \
               --parameters infrastructure/environments/${{ inputs.env }}.bicepparam \
               --resource-group ${{ inputs.resource_group_name }} \
               --verbose

         - uses: actions/download-artifact@v3
           with:
             name: ${{ inputs.artifact_name }}
             path: publish

         - name: Get publish profile
           id: publishprofile
           run: |
             profile=$(az webapp deployment list-publishing-profiles --resource-group ${{ inputs.resource_group_name }} --name ${{ inputs.app_service_name }} --slot ${{ inputs.app_service_slot_name }} --xml)
             echo "PUBLISH_PROFILE=$profile" >> $GITHUB_OUTPUT

         - name: Deploy to Slot
           uses: azure/webapps-deploy@v2
           with:
             app-name: ${{ inputs.app_service_name }}
             slot-name: ${{ inputs.app_service_slot_name }}
             publish-profile: ${{ steps.publishprofile.outputs.PUBLISH_PROFILE }}
             package: publish/

         - name: Swap slots
           run: |
             az webapp deployment slot swap -g ${{ inputs.resource_group_name }} -n ${{ inputs.app_service_name }} --slot ${{ inputs.app_service_slot_name }} --target-slot production --verbose
   ```

1. When you commit and push this code with both the action and the pipeline, your Action will trigger immediately. Go to the Actions tab in GitHub and follow its progress from Dev all the way to Production

1. Go to your Dev App Service Plan and note that the SKU is an S1. Let's change that to an S2 and commit and push that.

1. Go to your `WeatherForecastController` and get rid of all the `summaries` except `Freezing`. Then commit and push and watch it deploy.

1. Go to your Dev App Service Slot /api/WeatherForecast URL and note that it is still changing the summaries. Whereas the main App Service /api/WeatherForecast URL is ha

1. Take note of the /api/version endpoint and then correlate that back to the Git SHA back in GitHub. Note how in `ci.yml` we are setting that version and putting it in `version.txt` file that gets read by the application.
