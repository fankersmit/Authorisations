root =  api/authorisations  
type =  account, organisation, product   
id   =  GUID

|done|CQ|path | verb | success | Error | description|
|:---:|:---:|:---|:---:|:---:|:---:|---|
|<span style="color:green">Yes</span>|Q|root|GET| 201 | 404| returns { status: "up" }|
|<span style="color:red">No</span>|Q|root/request/{id}/status |GET| 200|404|  returns current status of request with {id}|
|<span style="color:red">No</span>|Q|root/request/{id}/{status} |GET|200|404|  returns true if status of request with {id}| is {status}
|<span style="color:green">Yes</span>|Q|root/requests/under-consideration |GET| 200|404| return total number of requests being processed
|<span style="color:red">No</span>|Q|root/requests/{type}/under-consideration |GET| 200 |404 | return total number of account requests of {type }being processed
|<span style="color:red">No</span>|Q|root/requests/{status} |GET| 200|404|  returns all requests with status = {status}
|<span style="color:red">No</span>|Q|root/requests/{type}/{status} |GET| 200|404|  returns all requests of {type} with status = {status}}
|<span style="color:red">No</span>|Q|root/request/{id}|GET| 200|404|  return  account request info
|<span style="color:red">No</span>|C|root/request/{type}/submit|POST| 202|404,405|  submit request where type is in {account, product, organisation}
|<span style="color:red">No</span>|C|root/request/{id}/confirm|POST| 202|404,405|  confirm request where id = {id} 
|<span style="color:red">No</span>|C|root/request/{id}/cancel|POST| 202|404,405|  cancel request where id = {id} 
|<span style="color:red">No</span>|C|root/request/{id}/approve|POST| 202|404,405|  approve request where id = {id} 
|<span style="color:red">No</span>|C|root/request/{id}/disapprove|POST| 202|404,405|  disapprove request where id = {id} 
|<span style="color:red">No</span>|C|root/request/{id}/conclude|POST| 202|404,405|  conclude request where id = {id} 
|<span style="color:red">No</span>|C|root/request/{id}/remove|POST|  204|404,405| remove request where id = {id} 