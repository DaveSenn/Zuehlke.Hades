# Zuehlke.Hades (A SDK for access control policies)
Currently it is possible to use it in memory and with a SQL database (T-SQL) other persistence can be achieved through implementing the IAclManager interface. It supports (custom) conditions to check additional information and custom matching to improve the performance in certain scenarios.

## Overview
This repository consists of one Visual Studio solution, which hosts two projects:
- Zuehlke.Hades (class library that can be embedded into a .NET project)
- Zuehlke.Hades.Test (unit tests for the class library)

## Usage
### 1. Initializing a new instance of the AclService and using it
The AclService handles the access control checks. By default it uses an in memory policy manager and searches for exact matches:  
```csharp
var aclservice = new AclService();
var result = await aclservice.CheckAccessAsync(new AccessRequest(){
    Subject = "user:2",
    Action = "write",
    Resource = "qwertz"
});
if(result == AccessRequestResult.Granted){
    //allow
}else{
    //deny
}
```
To use another policy manager (SQL server or custom) an instance of the manager can be passed to the AclService constructor or by setting its Manager property. A policy manager might allow setting the matcher that is used to identify patterns (eg. through the constructor). So initializing an instance of an AclService for a SQL server database with pattern matching looks like this:
```csharp
var aclservice = new AclService(new SqlServerManager("{your-connection-string-here}"));
```
### 2. AccessRequests
AccessRequests need to have a Subject (string), Action (string) and Resource (string) and they can have an optional Context (Dictionary<string, string>) that is used for conditions.

### 3. PolicyCreationRequests & Policies
To create a policy a PolicyCreationRequest is needed. It consists of a list of subjects, list of actions, list of resources, an effect (allow/deny) and optional a description and list of conditions:
```csharp
new PolicyCreationRequest()
{
    Subjects = new List<string>() { "user:1", },
    Actions = new List<string>() { "read" },
    Conditions = new List<ICondition>()
    {
        new StringEqualsCondition("key", "value"),
        new CidrCondition("192.168.0.1/16")
    },
    Resources = new List<string>() { "abc" },
    Effect = RequestEffect.Allow
}
```
A Policy is a PolicyCreationRequest with an id that was assigned to it after the creation.

### 4. Policy Managers (IAclManager)
Policy managers handle the policies that access requests are checked against and must implement the IAclManager interface. They expose the following methods:
```csharp
Task<Policy> AddPolicyAsync(PolicyCreationRequest policyCreationRequest);
Task<Policy> UpdatePolicyAsync(Policy policy);
Task<Policy> GetPolicyByIdAsync(string id);
Task<bool> DeletePolicyAsync(string id);
Task<List<Policy>> GetAllPoliciesAsync();
Task<List<Policy>> GetRequestCandidatesAsync(AccessRequest request);
```