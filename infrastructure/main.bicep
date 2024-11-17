@allowed(['dev', 'prod'])
param environment string

var location = 'centralus'
var appNameWithEnvironment = 'workshop-dnazghbicep-scottsauber-${environment}'

targetScope = 'resourceGroup'

module appService './appservice.bicep' = {
  name: 'appservice'
  params: {
    appName: appNameWithEnvironment
    environment: environment
    location: location
  }
}

module keyvault './keyvault.bicep' = {
  name: 'keyvault'
  params: {
    appId: appService.outputs.appServiceInfo.appId
    slotId: appService.outputs.appServiceInfo.slotId
    location: location
    appName: appNameWithEnvironment
  }
}
