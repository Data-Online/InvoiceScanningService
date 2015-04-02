<?xml version="1.0" encoding="utf-8"?>
<serviceModel xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" name="InvoiceScanningService" generation="1" functional="0" release="0" Id="573f4dfc-b486-4d88-aeda-b3a8b2449d83" dslVersion="1.2.0.0" xmlns="http://schemas.microsoft.com/dsltools/RDSM">
  <groups>
    <group name="InvoiceScanningServiceGroup" generation="1" functional="0" release="0">
      <componentports>
        <inPort name="WebInterfaceRole:Endpoint1" protocol="http">
          <inToChannel>
            <lBChannelMoniker name="/InvoiceScanningService/InvoiceScanningServiceGroup/LB:WebInterfaceRole:Endpoint1" />
          </inToChannel>
        </inPort>
      </componentports>
      <settings>
        <aCS name="WebInterfaceRole:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/InvoiceScanningService/InvoiceScanningServiceGroup/MapWebInterfaceRole:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </maps>
        </aCS>
        <aCS name="WebInterfaceRoleInstances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/InvoiceScanningService/InvoiceScanningServiceGroup/MapWebInterfaceRoleInstances" />
          </maps>
        </aCS>
        <aCS name="WorkerRole:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/InvoiceScanningService/InvoiceScanningServiceGroup/MapWorkerRole:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </maps>
        </aCS>
        <aCS name="WorkerRoleInstances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/InvoiceScanningService/InvoiceScanningServiceGroup/MapWorkerRoleInstances" />
          </maps>
        </aCS>
      </settings>
      <channels>
        <lBChannel name="LB:WebInterfaceRole:Endpoint1">
          <toPorts>
            <inPortMoniker name="/InvoiceScanningService/InvoiceScanningServiceGroup/WebInterfaceRole/Endpoint1" />
          </toPorts>
        </lBChannel>
      </channels>
      <maps>
        <map name="MapWebInterfaceRole:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/InvoiceScanningService/InvoiceScanningServiceGroup/WebInterfaceRole/Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </setting>
        </map>
        <map name="MapWebInterfaceRoleInstances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/InvoiceScanningService/InvoiceScanningServiceGroup/WebInterfaceRoleInstances" />
          </setting>
        </map>
        <map name="MapWorkerRole:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/InvoiceScanningService/InvoiceScanningServiceGroup/WorkerRole/Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </setting>
        </map>
        <map name="MapWorkerRoleInstances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/InvoiceScanningService/InvoiceScanningServiceGroup/WorkerRoleInstances" />
          </setting>
        </map>
      </maps>
      <components>
        <groupHascomponents>
          <role name="WebInterfaceRole" generation="1" functional="0" release="0" software="P:\Projects\Cloud_Development\GitHub\InvoiceScanningService\InvoiceScanningService\csx\Debug\roles\WebInterfaceRole" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaIISHost.exe " memIndex="-1" hostingEnvironment="frontendadmin" hostingEnvironmentVersion="2">
            <componentports>
              <inPort name="Endpoint1" protocol="http" portRanges="80" />
            </componentports>
            <settings>
              <aCS name="Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;WebInterfaceRole&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;WebInterfaceRole&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;/r&gt;&lt;r name=&quot;WorkerRole&quot; /&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/InvoiceScanningService/InvoiceScanningServiceGroup/WebInterfaceRoleInstances" />
            <sCSPolicyUpdateDomainMoniker name="/InvoiceScanningService/InvoiceScanningServiceGroup/WebInterfaceRoleUpgradeDomains" />
            <sCSPolicyFaultDomainMoniker name="/InvoiceScanningService/InvoiceScanningServiceGroup/WebInterfaceRoleFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
        <groupHascomponents>
          <role name="WorkerRole" generation="1" functional="0" release="0" software="P:\Projects\Cloud_Development\GitHub\InvoiceScanningService\InvoiceScanningService\csx\Debug\roles\WorkerRole" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaWorkerHost.exe " memIndex="-1" hostingEnvironment="consoleroleadmin" hostingEnvironmentVersion="2">
            <settings>
              <aCS name="Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;WorkerRole&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;WebInterfaceRole&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;/r&gt;&lt;r name=&quot;WorkerRole&quot; /&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/InvoiceScanningService/InvoiceScanningServiceGroup/WorkerRoleInstances" />
            <sCSPolicyUpdateDomainMoniker name="/InvoiceScanningService/InvoiceScanningServiceGroup/WorkerRoleUpgradeDomains" />
            <sCSPolicyFaultDomainMoniker name="/InvoiceScanningService/InvoiceScanningServiceGroup/WorkerRoleFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
      </components>
      <sCSPolicy>
        <sCSPolicyUpdateDomain name="WebInterfaceRoleUpgradeDomains" defaultPolicy="[5,5,5]" />
        <sCSPolicyUpdateDomain name="WorkerRoleUpgradeDomains" defaultPolicy="[5,5,5]" />
        <sCSPolicyFaultDomain name="WebInterfaceRoleFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyFaultDomain name="WorkerRoleFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyID name="WebInterfaceRoleInstances" defaultPolicy="[1,1,1]" />
        <sCSPolicyID name="WorkerRoleInstances" defaultPolicy="[1,1,1]" />
      </sCSPolicy>
    </group>
  </groups>
  <implements>
    <implementation Id="dec33262-3c5d-432b-b400-5e9a1b393d46" ref="Microsoft.RedDog.Contract\ServiceContract\InvoiceScanningServiceContract@ServiceDefinition">
      <interfacereferences>
        <interfaceReference Id="fb816551-0867-4e3f-ae7f-16d4cd31c53b" ref="Microsoft.RedDog.Contract\Interface\WebInterfaceRole:Endpoint1@ServiceDefinition">
          <inPort>
            <inPortMoniker name="/InvoiceScanningService/InvoiceScanningServiceGroup/WebInterfaceRole:Endpoint1" />
          </inPort>
        </interfaceReference>
      </interfacereferences>
    </implementation>
  </implements>
</serviceModel>