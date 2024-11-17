param location string 
param appName string
param keyVaultName string

resource logWs 'Microsoft.OperationalInsights/workspaces@2022-10-01' = {
  name: 'law-${appName}'
  location: location
}

resource applicationInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: 'ai-${appName}'
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    WorkspaceResourceId: logWs.id
  }
}

resource keyVault 'Microsoft.KeyVault/vaults@2022-07-01' existing = {
  name: keyVaultName
}

resource aiSecret 'Microsoft.KeyVault/vaults/secrets@2023-02-01' = {
  name: 'ConnectionStrings--ApplicationInsights'
  parent: keyVault
  properties: {
    value: applicationInsights.properties.ConnectionString
  }
}

resource logAnalyticsSecret 'Microsoft.KeyVault/vaults/secrets@2023-02-01' = {
  name: 'ConnectionStrings--LogAnalytics'
  parent: keyVault
  properties: {
    value: logWs.listKeys().primarySharedKey
  }
}
