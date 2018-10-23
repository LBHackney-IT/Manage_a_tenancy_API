# Manage a tenancy API

# Accounts

# Get account information for housing residents
```
Get /v1/Accounts/AccountDetailsByPaymentorTagReference?referencenumber=123409789
```
 
The endpoint retrieves account and contact related information based on a tag reference or a paris payment reference. The account entity represents the tenancy agreement for a property and has a link to the contacts living in that property. The API retrieves data from two entities and returns a merged result.
# Parameters
- referencenumber (required) (could be either tag reference or paris reference)
# Response
A successful response should get a list of account information corresponding to the given Parisreferencenumber.
```
{
  "results": [
    {
      "accountid": "5c5dae75-e89d-e711-80ff-1111111",
      "tagReferenceNumber": "02345/11",
      "benefit": 78.21,
      "propertyReferenceNumber": "02345",
      "currentBalance": -77.65,
      "rent": 133.99,
      "housingReferenceNumber": "02345",
      "directdebit": null,
      "ListOfTenants": [
        {
          "personNumber": "1",
          "responsible": true,
          "title": "Mr",
          "forename": "John",
          "surname": "Test"
        },
        {
          "personNumber": "2",
          "responsible": true,
          "title": "Mrs",
          "forename": "Sofia",
          "surname": "Test"
        }
      ],
      "ListOfAddresses": [
        {
          "postCode": "E8 2LN",
          "shortAddress": "Hackney Service Center",
          "addressTypeCode": null
        }
      ]
    }
  ]
}

```

# Get account information by contact ID
```
Get /v1/Accounts/AccountDetailsByContactId?contactid=b111111-1234-e811-8126-70106faa6a31
```
The endpoint returns data from the account entity (the tenancy agreement) based on a contact ID. 
# Parameters
- contactid (required)
# Response
A successful response should get account information corresponding to the given contact ID.
```
{
  "results": [
    {
      "propertyReferenceNumber": "0001234",
      "benefit": "0",
      "tagReferenceNumber": "01234/01",
      "accountid": "412333-129f-1234-1234-70106faa4841",
      "currentBalance": "-553.95",
      "rent": "102.59",
      "housingReferenceNumber": "000123",
      "directdebit": null,
      "personNumber": ,
      "responsible": false,
      "title": null,
      "forename": null,
      "surname": null,
      "tenuretype": "Secure"
    }
  ]
}
```

# Area Patch

# Get all estate officers for an area
Returns a list of all estate officers for a given area.
```
Get/v1/areapatch/getallofficersperarea?areaId=1
```
The endpoint returns all the officers that belong to an area. Note: it only returns officers that are assigned to a patch. If an officer is currently unassigned to a patch, it means they are not working at the moment (e.g. they are on long leave).
# Parameters
- Area ID (required)
# Response
A successful response should return a list all officers for a given area.
```
{
  "results": [
  {
    "propertyAreaPatchId": "1234527c-b205-1231-811c-71234faa1234",
    "estateOfficerPropertyPatchId": "bo11oo1o44-b005-e811-811c-12126faa1212",
    "estateOfficerPropertyPatchName": "Test Officer Patch Name",
    "llpgReferenece": "12345678901",
    "patchId": "1234545f-b4f7-e711-1234-12345faa6a31",
    "patchName": "Test Patch",
    "propetyReference": "010203040",
    "wardName": "Test Ward Name",
    "wardId": 0,
    "areaName": "Test Area Name",
    "areaId": 0,
    "managerPropertyPatchId": "b2o2o2o44-b005-e811-811c-12126faa1212",
    "managerPropertyPatchName": "Test Manager Name",
    "areaManagerName": "Test Area Manager",
    "areaManagerId": "a1a1a1a1a-b4f7-e711-1234-12345faa6a31",
    "isaManager": false,
    "officerId": "a1c1c1c1c-b4f7-e711-1234-12345faa6a31",
    "officerName": "Test Officer Name"
  },
  {
   ...etc...
  }
 ]
}
```
# Get Area Patch
```
Get /v1/AreaPatch/GetAreaPatch?postcode=E8%202AN&UPRN=10008230000
```
The endpoint returns the officer patch information based on a postcode and UPRN. UPRN is used as sometimes there are two officers that are responsible for one postcode (e.g. a block of apartments has the same postcode but properties are split between two officers).
 
# Parameters
- postcode (required)
- UPRN (required)
# Response
A successful response should get the property patch information corresponding to a combination of a given postcode and UPRN.
```
{
  "result": {
    "hackneyAreaId": "3",
    "hackneyAreaName": "Homerton 1",
    "hackneyPropertyReference": "45650",
    "hackneyPostCode": "E8 2AN",
    "hackneyllpgReference": "10008230000",
    "hackneyManagerPropertyPatchId": "123456-9953-e811-8126-70106faaf8c1",
    "hackneyManagerPropertyPatchName": "Manager Smith",
    "hackneyWardId": "6",
    "hackneyWardName": "Hackney Central",
    "hackneyEstateofficerPropertyPatchId": "123456-8553-e811-8126-70106faaf8c1",
    "hackneyEstateofficerPropertyPatchName": "Test Smith",
    "hackneyEstateOfficerId": "123456-5254-e811-8126-70106faa6a31"
  }
}
```
  
# Update officer patch or area manager
```
PUT v1/AreaPatch/UpdateOfficerPatchOrAreaManager
``` 
 The endpoint updates an existing officer patch or area manager. It is used to dissociate an officer from a patch (or manager from an area) and possibly assign a new officer to that particular patch. The endpoint allows managers to change patch assignment, which is required if a piece of work for a patch needs to be completed, but the officer responsible for that patch is unexpectedly (or unexpectedly) not being able to work (e.g. maternity leave or sudden long sick leave). 
# Parameters
- officerId (required) – ID of the officer
- patchId (required if isUpdatingPatch is true) – ID of the patch being updated (if one)
- isUpdatingPatch (required) – used to determine if update is for a patch or area manager
- deleteExistingRelationship – true if the update is to disassociate an officer with a patch and leave patch without an officer assigned to it
- updatedByOfficer – ID of the officer updating the patch/manager
 
# Request
```
{ 
   "officerId":"12345-6354-e811-8120-e0071b7fe041",
   "patchId":"f2ccbfc1-8553-e811-12345-70106faaf8c1",
   "isUpdatingPatch":true,
   "deleteExistingRelationship":false,
   "updatedByOfficer":"0b4319a7-12345-e811-8118-70106faa6a31"
}
```
# Response
```
{ 
   "id":"f2caaaa1-8553-e811-8126-70106faaf8c1",
   "patchName":"HN7"
}
 
 ```
# Get all unassigned officers
```
Get /v1/AreaPatch/GetAllUnassignedOfficers
```
The endpoint returns all officers that are currently not assigned to a patch/area. It is used when officer reassignment is done. The API returns a list of all unassigned officers so the manager could pick who to assign to a patch.
# Parameters
None
# Response
A successful response should return a list of all officers that are not currently assigned to a patch.
```
{
  "results": [
    {
      "firstName": "Testing",
      "lastName": "TestingLastName",
      "fullName": "Testing TestingLastName",
      "officerId": "1b4d2249-123-e811-a96c-002248072cc3"
    },
    {
...etc...
    }
  ]
}
```

# Citizen Index Search

# Get citizen index search result
Returns a list of citizens corresponding to a search parameter.
```
Get/v1/citizenindexsearch?firstname=test&surname=test&addressline12=flat%201%20test&postcode=E82HH&isAdvanceSearch=false
```
The endpoint returns contact information. It has two functions - advanced and non-advanced search. If isAdvanceSearch is marked as true, the endpoint also queries CI to retrieve contact information and returns a merged result from CRM and CI. The endpoint performs search if any one of the parameters is provided.
# Parameters
- First name (optional)
- Surname (optional)
- Address line 1 (optional)
- Postcode (optional)
- isAdvanceSearch (optional) – if true, search is also conducted in CI
Note: At least one of the optional parameters must be present to execute a search.
#Response
A successful response should return a list all matching citizens.
```
{
  "results": [
  {
    "id": null,
    "hackneyhomesId": null,
    "title": "MR",
    "surname": "Test",
    "firstName": "Testing",
    "dateOfBirth": "1962-01-1",
    "address": null,
    "addressLine1": "1 THE HACKNEY SERVICE CENTRE HILLMAN STREET HACKNEY LONDON HACKNEY E8 1DY",
    "addressLine2": "HILLMAN STREET",
    "addressLine3": "HACKNEY",
    "addressCity": "LONDON",
    "addressCountry": "HACKNEY",
    "postCode": "E8 1DY",
    "systemName": "CitizenIndex",
    "larn": "LARN2TEST260",
    "uprn": "10012345678",
    "usn": "101010",
    "fullAddressSearch": "1THEHACKNEYSERVICECENTREHILLMANSTREETHACKNEYLONDONHACKNEYE81DY",
    "fullAddressDisplay": "1 THE HACKNEY SERVICE CENTRE HILLMAN STREET HACKNEY LONDON HACKNEY E8 1DY",
    "crMcontactId": "00000000-0000-0000-0000-000000000000",
    "fullName": "TESTING TEST",
    "cautionaryAlert": null,
     "propertyCautionaryAlert": null,
     "crmContactId": "00000000-0000-0000-0000-000000000000",
     "emailAddress": null,
     "telephone1": null,
     "telephone2": null,
     "telephone3": null,
     "fullName": "TEST TEST WATSON",
     "isActiveTenant": false,
     "householdId": null,
     "accounttype": null
  },
  {
   ...etc...
  }
 ]
}
```  

# Contacts

# Add a new citizen
Creates a record of a new citizen.
```
POST /v1/contacts
```
The endpoint creates a new CRM contact. 
```
Request
{
  "crMcontactId": null,
  "title": "MR",
  "dateOfBirth": "2018-01-01",
  "lastName": "Testing",
  "firstName": "Test",
  "email": "test@test.com",
  "address1": "MAURICE BISHOP HOUSE E8 1HH",
  "address2": "17 READING LANE",
  "address3": "HACKNEY",
  "city": "LONDON",
  "postCode": "E8 1HH",
  "telephone1": "0912345678",
  "telephone2": null,
  "telephone3": null,
  "larn": "LARN2TEST129",
  "housingId": "1234567",
  "usn": "121212",
  "createdByOfficer": "a1c1c1c1c-b4f7-e711-1234-12345faa6a31",
  "uprn": null,
  "fullAddressSearch": "MAURICEBISHOPHOUSEE81HH17READINGLANEHACKNEYLONDONE81HH",
  "fullAddressDisplay": "MAURICE BISHOP HOUSE E8 1HH 17 READING LANE HACKNEY LONDON E8 1HH",
  "fullName": "Test Testing"
}
```
# Parameters
- crMcontactId - used if the citizen already exists in CRM2011 so the new citizen can be created with the same contact ID in Dynamics 365 CRM
- title - citizen's title
- dateOfBirth - citizen's date of birth
- lastName - citizen's last name (mandatory)
- firstName - citizen's first name (mandatory)
- email - citizen's email
- address1 - citizen's address line 1
- address2 - citizen's address line 2
- address3 - citizen's address line 3
- city - citizen's city
- postCode - citizen's postcode
- telehpone1 - citizen's primary telephone
- telehpone2 - citizen's telephone 2
- telehpone3 - citizen's telephone 3
- larn - citizen's LARN (as returned from Citizen Index)
- housingId - citizen's housingId (as returned from Citizen Index)
- usn - citizen's usn
- createdByOfficer - the ID of the officer who is currently logged into the system and is creating the new citizen contact (mandatory)
- uprn - citizen's urpn
- fullAddressSearch - a concatenated string with no space used with Citizen Index search
- fullAddressDisplay - the full address of the citizen as to be displayed
- fullName - citizen's full name


# Response
A successful POST request should have the following response:
```
{
    "contactid": "8c12345a-fd0b-e811-811d-123456a6a11",
    "firstName": "Test",
    "lastName": "Testing",
    "fullName": "Test Testing",
    "dateOfBirth": "2018-01-01",
    "email": "test@test.com",
    "address1": "MAURICE BISHOP HOUSE E8 1HH",
    "address2": "17 READING LANE",
    "address3": "HACKNEY",
    "city": "LONDON",
    "postCode": "E8 1HH",
    "telephone1": "0912345678",
    "larn": "LARN2TEST129",
    "housingId": "1234567",
    "usn": "121212"
}   
```
# Update contact
Creates a record of a new citizen.
```
PUT /v1/contacts
```
The endpoint updates a CRM contact. 
# Parameters
- contactId (required)

The below parameters are sent as one object and at least one parameter needs to be sent for the update

- title - citizen's title
- dateOfBirth - citizen's date of birth
- lastName - citizen's last name
- firstName - citizen's first name
- email - citizen's email
- address1 - citizen's address line 1
- address2 - citizen's address line 2
- address3 - citizen's address line 3
- city - citizen's city
- postCode - citizen's postcode
- telehpone1 - citizen's primary telephone
- telehpone2 - citizen's telephone 2
- telehpone3 - citizen's telephone 3
- larn - citizen's LARN (as returned from Citizen Index)
- housingId - citizen's housingId (as returned from Citizen Index)
- usn - citizen's usn
- updatedByOfficer - the ID of the officer who is currently logged into the system and is creating the new citizen contact (required)
- uprn - citizen's urpn
- fullAddressSearch - a concatinated string with no space used with Citizen Index search
- fullAddressDisplay - the full address of the citizen as to be displayed
- fullName - citizen's full name

#Request
```
{ 
   "email":"testing@yahoo.co.uk",
   "telephone1":"0123546879",
   "telephone2":"",
   "telephone3":"12345678987",
   "updatedByOfficer":"123546-ce1b-e811-8118-70106faa6a31",
   "id":"ab5adefa-1f56-e811-812e-123546"
}
``` 

# Response
A successful PUT request should have the following response:
 ```
{ 
   "firstName":"Test Test ",
   "lastName":"Testore",
   "dateOfBirth":null,
   "email":" testing @yahoo.co.uk",
   "address1":"FLAT AB FELLOWS COURT",
   "address2":"THE TERRACE",
   "address3":"HACKNEY",
   "city":"LONDON",
   "postcode":"E2 2LN",
   "telephone1":"0123546879",
   "telephone2":null,
   "telephone3":"12345678987"
}
 ```
 
 
# Get cautionary alerts
Returns a list of citizens corresponding to a search parameter.
```
Get/ v1/Contacts/GetCautionaryAlerts?urpn=100000000001
```
The endpoint returns a list of cautionary alerts for a given property. Cautionary alerts are saved against contacts and have URPN as identifier so they can be retrieved for a property. 
# Parameters
- UPRN (optional)
- Response

A successful response should return a list of all cautionary alerts recorded against a UPRN.
```
{
  "results": [
    {
      "cautionaryAlertType": "[Temporary] Other type of incident",
      "cautionaryAlertId": "123456-b05a-e811-8131-70106faa6a11",
      "contactId": "aeef158a-e4f1-e111-855d-123456",
      "contactName": "Jo Testing",
      "uprn": "100000000001",
      "createdOn": "2018-05-18 15:30:53"
    },
    {
...etc...
    }
  ]
}
```

# Create cautionary alerts
```
POST v1/Contacts/CreateCautionaryAlerts
```
The endpoint creates a new cautionary alert. As cautionary alerts are linked with contacts, a contact ID is required. Multiple cautionary alerts can be created at once.  
 
# Parameters
- contactId (required)
- uprn (required)
- cautionaryAlertType (at least one required)
# Request
```
{  
   "contactId":"12356-1f56-e811-812e-70106faa6a11",
   "uprn":"100000000000",
   "cautionaryAlertType":[  
      "12"
   ]
}
``` 

# Response
```
{  
   "alertContactId":"12356-1f56-e811-812e-70106faa6a11",
   "alertUprn":"100000000000",
   "alertCautionaryAlertType":[  
      "12"
   ],
   "createdOn":"2018-10-03"
}
 
```

# Remove cautionary alerts
```
POST /v1/Contacts/RemoveCautionaryAlerts
``` 
The endpoint removes cautionary alert that was against a contact. It could remove multiple alerts at once. If all alerts are removed from a contact (when cautionaryAlertIsToBeRemoved), the endpoint also checks if anyone else in the property (by using UPRN) has cautionary alert. If no, it updates the property cautionary alert flag in the contact entity to false.
# Parameters
- cautionaryAlertsIds (at least one required)
- uprn (required)
- contactId (required)
- cautionaryAlertIsToBeRemove (required)
# Request
```
{  
   "cautionaryAlertIds":[  
      "123546-10c7-e811-a967-002248072abd",
      "123546-11c7-e811-a96a-00224807251a"
   ],
   "contactId":"123546-1f56-e811-812e-70106faa6a11",
   "uprn":"100000000000",
   "cautionaryAlertIsToBeRemoved":true
}
```
 
 
# Response
204 status code

 
# Get contact details by contact ID
```
Get / v1/Contacts/GetContactDetails?contactId=123546-e4f1-e111-855d-00505691098c
```
The endpoint retrieves contact details by a given contact ID.
 
# Parameters
- contactId (required)
# Response
A successful response should get contact information based on contact ID.
```
{
  "contactId": "123546-e4f1-e111-855d-00505691098c",
  "emailAddress": null,
  "uprn": "10000000000",
  "addressLine1": "FLAT 164 MAURICE BISHOP HOUSE",
  "addressLine2": "NEW HIGH STREET",
  "addressLine3": "HACKNEY",
  "firstName": "Test",
  "lastName": "Smith",
  "larn": "LARN10000800",
  "address1AddressId": "b6aff9fd-748d-4e06-123546-f9eaa492d95a",
  "address2AddressId": "d2eea18e-fd32-123546-b763-3a7ffbc57151",
  "address3AddressId": "0b515067-123546-457c-b2b9-9aab53d9fb55",
  "telephone1": "01234567899",
  "telephone2": "01234567899",
  "telephone3": "01234567899",
  "cautionaryAlert": true,
  "propertyCautionaryAlert": false,
  "houseRef": null,
  "title": "Mr",
  "fullAddressDisplay": "FLAT 164 MAURICE BISHOP HOUSE\r\NEW HIGH STREET\r\nHACKNEY\r\nLONDON E9 2LN",
  "fullAddressSearch": "flat164mauricebishophousenewhighstreet",
  "postCode": "E9 2LN ",
  "dateOfBirth": "1981-11-12",
  "hackneyHomesId": "11603",
  "houseHoldId": "7bcfde57-123546-e811-812e-70106faa6a11",
  "memberId": "55420000",
  "personno": "1",
  "accountId": "123546-039f-e711-8100-70106faa4841"
}
 ```
# Get contacts by UPRN
Returns a list of transactions
```
Get v1/Contacts/GetContactsByUprn?urpn=1000000000
```
The endpoint retrieves all contacts for a property and their information by using UPRN. 

# Parameters
- uprn (required)
# Response
A successful response should get a list of transactions based on the tag reference provided.
```
{
  "results": [
    {
      "contactId": "123456-1f56-e811-812e-70106faa6a11",
      "emailAddress": "testing@yahoo.co.uk",
      "uprn": "1000000000",
      "addressLine1": "FLAT 66 TEST COURT",
      "addressLine2": "THE TERRACE",
      "addressLine3": "HACKNEY",
      "firstName": "Test Testing",
      "lastName": "Testore",
      "fullName": " Test Testing Testore",
      "larn": "LARN1700027",
      "telephone1": "01234567891",
      "telephone2": null,
      "telephone3": "01234567891",
      "cautionaryAlert": false,
      "propertyCautionaryAlert": false,
      "houseRef": null,
      "title": "Miss",
      "fullAddressDisplay": " FLAT 66 TEST COURT \r\ THE TERRACE\r\nHACKNEY\r\nLONDON E2 2LN",
      "fullAddressSearch": "flat66testcourttheterrace",
      "postCode": "E2 2LN",
      "dateOfBirth": null,
      "hackneyHomesId": "211111",
      "disabled": false,
      "relationship": "Person 1",
      "extendedrelationship": null,
      "responsible": true,
      "age": "26"
    },	
    {
      ...etc...
    }
  ]
}
```

# Login
Authenticate users based on Username and Password
Returns the user details
```
Get/v1/login/authenticatenhoofficers?username=uaccount&password=hackney
```
The endpoint is used to authenticate estate officers trying to login into the Manage a tenancy system and returns they officer and patch information. During the authentication process, the endpoints encrypts the password typed by the user and then does comparison with the password saved in CRM (as passwords in CRM are saved encrypted)
# Parameters
- Username (required)
- Password (required)
# Response
A successful response should get the detail of Authenticated neighbourhood officers corresponding to the required parameters.
```
{
  "result": {
    "estateOfficerLoginId": "d1o1o1o1o1o-15dc-e711-8115-7be3faabb11",
    "officerId": "d1b1b1b1b1b-15dc-e711-8115-7be3faabb11",
    "firstName": "Shweta",
    "surName": "Test",
    "username": "stttya",
    "fullName": "Test Test",
    "isManager": false,
    "areaManagerId": null,
    "officerPatchId": "d1c1c1c1c-15dc-e711-8115-7be3faabb11"    
  }
}
```

# Tenancy management interactions
Get tenancy management interactions
```
Get /v1/tenancymanagementinteractions?contactid=12345b678-d901-e511-b5a2-12121298417b&personType=contact
``` 
Returns a list of tenancy management interactions. If the type is contact, it returns all interactions against a contact. If the type is officer/manager, it returns all interactions assigned to them.
# Parameters
- Contact ID (required)
- Person Type (required)
  - contact
  - officer
  - manager
# Response
A successful response should return a list of all tenancy management interactions that are assigned to the provided contact id.
```
{
 "results": [
    {
      "incidentId": "1c2c3b4d-ef0f-e811-8114-1c2bfaaf8c1",
      "isTransferred": false,
      "ticketNumber": "CAS-31234-W1F7S2",
      "stateCode": 1,
      “processStage” :null,
      "nccOfficersId": "9912345-da01-e234-8112-71236faaf8c1",
      "nccEstateOfficer": "Bhavesh Test",
      "createdon": "2018-02-12T12:22:26Z",
      "nccOfficerUpdatedById": "12345567-da01-e811-1234-34567faaf8c1",
      "nccOfficerUpdatedByName": "Bhavesh Test",
      "natureOfEnquiryId": 3,
      "natureOfEnquiry": "Estate Managment",
      "enquirySubjectId": 100000005,
      "enquirysubject": "Joint tenancy application",
      "interactionId": "11c2b3d-ef0f-e811-123d-70106faa6a11",
      "areaManagerId": "1b2b3bc4d-b005-e811-811c-71236faa6a11",
      "areaManagerName": "Mirela Estate Manager Test",
      "officerPatchId": "1b2b3b4bcb005-e811-811c-70106faa6a11",
      "officerPatchName": "Bhavesh Patch",
      "areaName": "Central Panel",
      "handledBy": "Estate Officer",
      "requestCallBack": true,
      "contactId": "1b2bcb3d4f-ed0f-e123-811d-70106faa6a11",
      "contactName": "Will Test",
      "contactPostcode": "E8 2LN",
      "contactAddressLine1": "11, TEST ROAD, HACKNEY, LONDON, E8 2LN",
      "contactAddressLine2": null,
      "contactAddressLine3": null,
      "contactAddressCity": null,
      "contactBirthDate": null,
      "contactTelephone": "1114567890",
      "contactEmailAddress": null,
      "contactLarn": null,
      "householdID": "7111111-2556-e811-812e-11111116a11",
      "AnnotationList": [
        {
          "noteText": "Test logged on  12/02/2018 12:38:21 by Bhavesh Test",
          "annotationId": "ee1bb2vv-f10f-123vc-8111-e0071b7fe041",
          "noteCreatedOn": "12/02/2018T12:38Z"
        },
	{
         ...etc...
        }
      ]
    },
    {
     ...etc...
    }
  ]
}
``` 

   

# Get tenancy management interactions by area
Returns a list of tenancy management interactions based on a selected area.
```
Get /v1/tenancymanagementinteractions/getareatrayinteractions?officeid=1
``` 
The endpoint returns a list of all tenancy interactions that belong to a specific area.
# Parameters
- Office ID (required)
# Response
A successful response should return a list of all tenancy management interactions that are corresponding to the selected area.
```
{
 "results": [
    {
      "incidentId": "1c2c3b4d-ef0f-e811-8114-1c2bfaaf8c1",
      "isTransferred": false,
      "ticketNumber": "CAS-31234-W1F7S2",
      "stateCode": 1,
      “processStage” :null,
      "nccOfficersId": "9912345-da01-e234-8112-71236faaf8c1",
      "nccEstateOfficer": "Bhavesh Test",
      "createdon": "2018-02-12T12:22:26Z",
      "nccOfficerUpdatedById": "12345567-da01-e811-1234-34567faaf8c1",
      "nccOfficerUpdatedByName": "Bhavesh Test",
      "natureOfEnquiryId": 3,
      "natureOfEnquiry": "Estate Managment",
      "enquirySubjectId": 100000005,
      "enquirysubject": "Joint tenancy application",
      "interactionId": "11c2b3d-ef0f-e811-123d-70106faa6a11",
      "areaManagerId": "1b2b3bc4d-b005-e811-811c-71236faa6a11",
      "areaManagerName": "Mirela Estate Manager Test",
      "officerPatchId": "1b2b3b4bcb005-e811-811c-70106faa6a11",
      "officerPatchName": "Bhavesh Patch",
      "areaName": "Central Panel",
      "handledBy": "Estate Officer",
      "requestCallBack": true,
      "contactId": "1b2bcb3d4f-ed0f-e123-811d-70106faa6a11",
      "contactName": "Will Test",
      "contactPostcode": "E8 2LN",
      "contactAddressLine1": "47, ABERSHAM ROAD, HACKNEY, LONDON, E8 2LN",
      "contactAddressLine2": null,
      "contactAddressLine3": null,
      "contactAddressCity": null,
      "contactBirthDate": null,
      "contactTelephone": "1114567890",
      "contactEmailAddress": null,
      "contactLarn": null,
      "householdID": "7111111-2556-e811-812e-11111116a11",
      "AnnotationList": [
        {
          "noteText": "Test logged on  12/02/2018 12:38:21 by Bhavesh Test",
          "annotationId": "ee1bb2vv-f10f-123vc-8111-e0071b7fe041",
          "noteCreatedOn": "12/02/2018T12:38Z"
        },
	{
         ...etc...
        }
      ]
    },
    {
     ...etc...
    }
  ]
}
```   
# Create tenancy management interaction
```
POST /v1/tenancymanagementinteractions/createtenancymanagementinteraction
``` 
The endpoint creates a new tenancy interaction. 
# Parameters
- contactId (required)
- enquirySubject (required)  (value corresponds to a CRM option set)
- estateOfficerId (required)
- subject (required) (persistent value – changes only based on environment)
- source (required) (used to allow functionality to determine if officer or call centre worker has created the interaction)
- natureOfEnquiry (required) (value corresponds to a CRM option set)
- serviceRequest (required)
- estateOfficerName (required)
- OfficerPatchId (required)
- areaName (required)
- processType (required) (0 – interaction, 1 process, 2 post-visit action)
# Request
```
{ 
   "contactId":"ab5adefa-1f56-1234-1234-70106faa6a11",
   "enquirySubject":"100000100",
   "estateOfficerId":"1f1bb727-1234-1234-8118-70106faa6a31",
   "subject":"c1f72d01-28dc-e711-1234-70106faa6a11",
   "source":"1",
   "natureofEnquiry":"8",
   "serviceRequest":{ 
      "title":"Tenancy Management",
      "description":"ttest",
      "contactId":"ab5adefa-1234-e811-812e-70106faa6a11",
      "subject":"c1f72d01-28dc-e711-1234-70106faa6a11",
      "requestCallback":false
   },
   "estateOfficerName":"M Test",
   "officerPatchId":"8e958a37-1234-1234-8126-70106faaf8c1",
   "areaName":"5",
   "processType":"0"
}
``` 
 
# Response
```
{
  "interactionId": "1234-b0c7-e811-1234-00224807251a",
  "contactId": "ab5adefa-1f56-1234-812e-70106faa6a11",
  "enquirySubject": "100000100",
  "estateOfficerId": "1f1bb727-1234-e811-8118-70106faa6a31",
  "subject": "c1f72d01-28dc-1234-8115-70106faa6a11",
  "adviceGiven": null,
  "estateOffice": null,
  "source": "1",
  "natureofEnquiry": "8",
  "estateOfficerName": null,
  "officerPatchId": "8e958a37-1234-e811-8126-70106faaf8c1",
  "areaName": "5",
  "managerId": null,
  "assignedToPatch": false,
  "assignedToManager": false,
  "transferred": false,
  "serviceRequest": {
    "id": "4c782e8a-b0c7-e811-1234-00224807251a",
    "title": "Tenancy Management",
    "description": "ttest",
    "contactId": "ab5adefa-1234-e811-812e-70106faa6a11",
    "parentCaseId": null,
    "subject": "c1f72d01-1234-e711-8115-70106faa6a11",
    "createdDate": null,
    "enquiryType": null,
    "ticketNumber": "CAS-1234-K3V7F5",
    "requestCallback": false,
    "transferred": false,
    "createdBy": null,
    "childRequests": null
  },
  "status": 0,
  "parentInteractionId": null,
  "processType": "0",
  "householdId": null,
  "processStage": 0,
  "reasonForStartingProcess": null
}

```

# Update tenancy management interaction
```
PATCH /v1/TenancyManagementInteractions
```
 
Used for updating an existing interaction or closing an interaction. If closing, “status” should be set to 0.
# Parameters
- interactionId (required)
- estateOfficerId (required)
- status (required)
- serviceRequest (required)
- estateOfficerName (required)
# Request
```
{
   "interactionId":"1234-b0c7-e811-1234-00224807251a",
   "estateOfficerName":"M Test",
   "ServiceRequest":{ 
      "description":"test test",
 	 "requestCallback":false,
      "Id":"1234-aec7-e811-a967-002248072abd"
   },
   "status":1,
   "estateOfficerId":"1f1bb727-ce1b-1234-1234-70106faa6a31"
}
``` 
 
# Response
```
{
   "result":{ 
      "annotationId":"bc002da6-b0c7-1234-a96b-002248072825",
      "serviceRequestId":"c1cb69e6-1234-1234-1234-002248072abd",
      "interactionId":"c8cb69e6-aec7-1234-1234-002248072abd",
      "description":"test test",
      "status":"InProgress",
      "requestCallBack":false,
      "processStage":0
   }
}
```

# Transfer tenancy management interaction
```
PUT /v1/TenancyManagementInteractions/TransferCall
```
The endpoint updates a tenancy interaction by “transferring” it to another officer. This is achieved by updating the officer patch/area manager value with the new officer patch/area manager ID that should be assigned to the interaction.
# Parameters
- interactionId (required)
- estateOfficerId (required)
- officerPatchId (required)
- managerId (required if sending to manager)
- areaName (required)
- serviceRequest (required)
- assignedToPatch (required)
- estateOfficerName (required)
# Request
```
{ 
   "interactionId":"c8cb69e6-1234-1234-1234-002248072abd",
   "estateOfficerId":"1f1bb727-1234-1234-1234-70106faa6a31",
   "officerPatchId":"1f1bb727-1234-1234-1234-70106faa6a31",
   "managerId":"5512c473-9953-1234-1234-70106faaf8c1",
   "areaName":"6",
   "serviceRequest":{ 
      "description":"Transferred from: M Test\r\nsending to manager for review",
      "requestCallback":false,
      "Id":"c1cb69e6-aec7-1234-1234 -002248072abd"
   },
   "assignedToPatch":"True",
   "estateOfficerName":"M Test"
}
 
``` 

# Response
```
{ 
   "interactionId":"c8cb69e6-1234-1234-1234-002248072abd",
   "contactId":null,
   "enquirySubject":null,
   "estateOfficerId":"1f1bb727-1234-1234-1234-70106faa6a31",
   "subject":null,
   "adviceGiven":null,
   "estateOffice":null,
   "source":null,
   "natureofEnquiry":null,
   "estateOfficerName":"M Test",
   "officerPatchId":null,
   "areaName":"6",
   "managerId":"5512c473-1234-1234-1234-70106faaf8c1",
   "assignedToPatch":false,
   "assignedToManager":false,
   "transferred":false,
   "ServiceRequest":{ 
      "id":"c1cb69e6-1234-1234-1234-002248072abd",
      "title":null,
      "description":"Transferred from: M Test\r\nsending to manager for review",
      "contactId":null,
      "parentCaseId":null,
      "subject":null,
      "createdDate":null,
      "enquiryType":null,
      "ticketNumber":null,
      "requestCallback":false,
      "transferred":false,
      "createdBy":null,
      "childRequests":null
   },
   "status":0,
   "parentInteractionId":null,
   "processType":null,
   "householdId":null,
   "processStage":0,
   "reasonForStartingProcess":null,
   "annotationid":"c179e5b3-1234-1234-1234-002248072cc3"
}

```

# Officer accounts
 
Create officer account
```
POST v1/OfficerAccounts/CreateOfficerAccount
```

The endpoint creates a new officer account that is used for logging into the system.  
 
# Parameters
- hackney_firstname (required)
- hackney_lastname (required)
- hackney_emailaddress (required)
- hackney_username (required)
- hackney_password (required)
# Request
```
{ 
   "officeraccount":{ 
      "hackney_firstname":"justtest",
      "hackney_lastname":"testjust",
      "hackney_emailaddress":"testtesttest"
   },
   "officerloginaccount":{ 
      "hackney_username":"justesting",
      "hackney_password":"test123"
   }
}
``` 
 
# Response
```
{ 
   "result":{ 
      "EstateOfficerid":"d63d976c-123-e811-a967-002248072abd",
      "Name":"justtest testjust",
      "FirstName":"justtest",
      "LastName":"testjust",
      "EmailAddress":"testtesttest",
      "OfficerAccountStatus":"Active",
      "EstateOfficerLoginId":"db3d976c-123-e811-a967-002248072abd",
      "UserName":"justesting",
      "LoginAccountStatus":"Active"
   }
}
```
# Transactions
 
# Get housing transactions for residents
```
Get /v1/transactions?tagReference=123456/01
``` 
Returns a list of housing transactions for the last one year.

# Parameters
- Tag Reference (required)

# Response
A successful response should get a list of transactions based on the tag reference provided.
```
{
  "results": [
    {
      "tagReference": "123456/11",
      "propertyReference": "01234678",
      "transactionSid": null,
      "houseReference": "123456",
      "transactionType": "RPO",
      "postDate": "2017-11-28T00:00:00",
      "realValue": -10,
      "transactionID": "49ct627-e9d4-e711-8109-zzzzzz041",
      "debDesc": "PayPoint/Post Office"
    },
    {
      "tagReference": "123456/11",
      "propertyReference": "01234678",
      "transactionSid": null,
      "houseReference": "123456",
      "transactionType": "RTB",
      "postDate": "2017-11-27T00:00:00",
      "realValue": -110.95,
      "transactionID": "c4396c29-87o3-e711-8109-zzzzzzfe041",
      "debDesc": "Housing Benefit"
    },
    {
      ...etc...
    }
  ]
}
```


 


 
 

