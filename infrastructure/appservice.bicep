param appName string
@allowed(['dev', 'prod'])
param environment string
param location string

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


resource appServicePlan 'Microsoft.Web/serverfarms@2022-09-01' = {
  name: 'asp-${appName}-${environment}'
  location: location
  sku: {
    name: 'S1'
  }
  kind: 'linux'
  properties: {
    reserved: true
  }
}

resource appService 'Microsoft.Web/sites@2022-09-01' = {
  name: 'app-${appName}-${environment}'
  location: location
  identity: {
    type: 'SystemAssigned'
  }
  properties: appServiceProperties
}

resource appSettings 'Microsoft.Web/sites/config@2022-09-01' = {
  name: 'appsettings'
  kind: 'string'
  parent: appService
  properties: {
    ASPNETCORE_ENVIRONMENT: environment
  }
}

resource appServiceSlot 'Microsoft.Web/sites/slots@2022-09-01' = {
  location: location
  parent: appService
  name: 'slot'
  identity: {
    type: 'SystemAssigned'
  }
  properties: appServiceProperties
}

resource appServiceSlotSetting 'Microsoft.Web/sites/slots/config@2022-09-01' = {
  name: 'appsettings'
  kind: 'string'
  parent: appServiceSlot
  properties: {
    ASPNETCORE_ENVIRONMENT: environment
  }
}
