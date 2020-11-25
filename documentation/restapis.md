***
## Legenda

term| definition or values
|:---|:---|
|root|api/authorisations  
|type|account, organisation, product   
|id|GUID  
|Q|Query  
|C|Command
|verb| GET, POST, PUT, PATCH, DELETE, HEAD, OPTIONS, CONNECT, TRACE  |
</br>  
  
  ***  

## Idempotency

> A request method is considered "idempotent" if the intended effect on the server of multiple identical requests with that method is the same as the effect for a single such request

</br>  

*** 
## API definiition

|done|style|CQ|path | verb | success | Error | description|
|:---:|:---:|:---:|:---|:---:|:---:|:---:|---|
|<span style="color:green">Yes</span>|REST|Q|root|GET| 200 | 404| returns { status: "up" }|
|<span style="color:red">No</span>|REST|Q|root/request/{id}/status |GET| 200|404|  returns current status of request with {id}|
|<span style="color:red">No</span>|RPC|Q|root/request/{id}/{status} |GET|200|404|  returns true if status of request with {id}| is {status}
|<span style="color:green">Yes</span>|REST|Q|root/requests/under-consideration |GET| 200|404| return total number of requests being processed
|<span style="color:green">Yes</span>|REST|Q|root/requests/under-consideration/{type} |GET| 200 |404 | return total number of account requests of {type }being processed
|<span style="color:red">No</span>|REST|Q|root/requests/{status} |GET| 200|404|  returns all requests with status = {status}
|<span style="color:red">No</span>|REST|Q|root/requests/{type}/{status} |GET| 200|404|  returns all requests of {type} with status = {status}}
|<span style="color:red">No</span>|REST|Q|root/request/{id}|GET| 200|404|  return  account request info
|<span style="color:red">No</span>|REST|C|root/request/{type}/submit|POST| 202|404,405|  submit request where type is in {account, product, organisation}
|<span style="color:red">No</span>|REST|C|root/request/{id}/confirm|POST| 202|404,405|  confirm request where id = {id} 
|<span style="color:red">No</span>|REST|C|root/request/{id}/cancel|POST| 202|404,405|  cancel request where id = {id} 
|<span style="color:red">No</span>|REST|C|root/request/{id}/approve|POST| 202|404,405|  approve request where id = {id} 
|<span style="color:red">No</span>|REST|C|root/request/{id}/disapprove|POST| 202|404,405|  disapprove request where id = {id} 
|<span style="color:red">No</span>|REST|C|root/request/{id}/conclude|POST| 202|404,405|  conclude request where id = {id} 
|<span style="color:red">No</span>|REST|C|root/request/{id}/remove|DELETE|  204|404,405| remove request where id = {id} 

<br>  


***
# Valid transitions

 |Command: |new|submit|Confirm|Cancel|Approve|Disapprove|Conclude|Remove|
 |:---:|:---:|:---:|:---:|:---:|:---:|:---:|:---:|:---:|
 |State |||||||||
|New|<span style="color:yellow">Yes</span>|<span style="color:green">Yes</span>|<span style="color:red">No</span>|<span style="color:red">No</span>|<span style="color:red">No</span>|<span style="color:red">No</span>|<span style="color:red">No</span>|<span style="color:green">Yes</span>
|Submitted|<span style="color:red">No</span>|<span style="color:yellow">Yes</span>|<span style="color:green">Yes</span>|<span style="color:green">Yes</span>|<span style="color:red">No</span>|<span style="color:red">No</span>|<span style="color:red">No</span>|<span style="color:red">No</span>
|Confirmed|<span style="color:red">No</span>|<span style="color:red">No</span>|<span style="color:yellow">Yes</span>|<span style="color:red">No</span>|<span style="color:green">Yes</span>|<span style="color:green">Yes</span>|<span style="color:red">No</span>|<span style="color:red">No</span>
|Cancelled|<span style="color:red">No</span>|<span style="color:red">No</span>|<span style="color:red">No</span>|<span style="color:yellow">Yes</span>|<span style="color:red">No</span>|<span style="color:red">No</span>|<span style="color:green">Yes</span>|<span style="color:red">No</span>
|Approved|<span style="color:red">No</span>|<span style="color:red">No</span>|<span style="color:red">No</span>|<span style="color:red">No</span>|<span style="color:yellow">Yes</span>|<span style="color:red">No</span>|<span style="color:green">Yes</span>|<span style="color:red">No</span>
|Disapproved|<span style="color:red">No</span>|<span style="color:red">No</span>|<span style="color:red">No</span>|<span style="color:red">No</span>|<span style="color:red">No</span>|<span style="color:yellow">Yes</span>|<span style="color:green">Yes</span>|<span style="color:red">No</span>
|Concluded|<span style="color:red">No</span>|<span style="color:red">No</span>|<span style="color:red">No</span>|<span style="color:red">No</span>|<span style="color:red">No</span>|<span style="color:red">No</span>|<span style="color:yellow">Yes</span>|<span style="color:green">Yes</span>
|Removed|<span style="color:red">No</span>|<span style="color:red">No</span>|<span style="color:red">No</span>|<span style="color:red">No</span>|<span style="color:red">No</span>|<span style="color:red">No</span>|<span style="color:red">No</span>|<span style="color:yellow">Yes</span>

