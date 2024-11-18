@allowed(['dev', 'prod'])
param environment string

targetScope = 'resourceGroup'

var location = 'centralus'

var myName = 'scottsauber'
var appNameWithEnvironment = 'workshop-dnazghbicep-${myName}-${environment}'

module appService 'appservice.bicep' = {
  name: 'appService'
  params: {
    appName: appNameWithEnvironment
    location: location
    environment: environment
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
