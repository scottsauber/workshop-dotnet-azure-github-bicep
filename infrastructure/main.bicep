@allowed(['dev', 'prod'])
param environment string

targetScope = 'resourceGroup'

module appService 'appservice.bicep' = {
  name: 'appService'
  params: {
    appName: 'workshop-dnazghbicep-scottsauber-${environment}'
    location: 'centralus'
    environment: environment
  }
}
