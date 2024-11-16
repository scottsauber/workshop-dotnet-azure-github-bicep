@allowed(['dev', 'prod'])
param environment string

targetScope = 'resourceGroup'

module app './appservice.bicep' = {
  name: 'appservice'
  params: {
    appName: 'workshop-dnazghbicep-scottsauber'
    environment: environment
    location: 'centralus'
  }
}
