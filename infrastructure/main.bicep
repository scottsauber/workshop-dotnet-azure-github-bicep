@allowed(['dev', 'prod'])
param environment string

var location = 'centralus'
var myName = 'scottsauber'
var appNameWithEnvironment = 'workshop-dnazghbicep-${myName}-${environment}'

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
    appName: '${myName}-${environment}' // key vault has 24 char max so just doing your name, usually would do appname-env but that'll conflict for everyone
  }
}
